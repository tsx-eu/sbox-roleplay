using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sandbox;

namespace charleroi.server
{

	[Serializable]
	class CRUDRequest
	{
		private static ulong incr = 1;
		private static ulong ReqIncr {
			get {
				return incr++;
			}
		}

		[JsonIgnore]
		public TaskCompletionSource<CRUDResponse> ResponsePromise = new();

		[JsonPropertyName( "reqid" )]
		public ulong ReqID { get; set; }

		[JsonPropertyName( "reqtype" )]
		public string ReqType { get; set; }

		[JsonPropertyName( "datatype" )]
		public string DataType { get; set; }

		[JsonPropertyName( "id" )]
		public string ID { get; set; }

		[JsonPropertyName( "data" )]
		public JsonDocument Data { get; set; }


		public CRUDRequest( string operation, string datatype, string id, JsonDocument data )
		{
			ReqID = ReqIncr;
			ReqType = operation;
			DataType = datatype;
			ID = id;
			Data = data;
		}

	}

	[Serializable]
	class CRUDResponse
	{
		[JsonPropertyName( "reqid" )]
		public ulong ReqID { get; set; }

		[JsonPropertyName( "error" )]
		public string Error { get; set; }

		[JsonPropertyName( "success" )]
		public bool Success { get; set; }

		[JsonPropertyName( "lastinsertid" )]
		public ulong LastInsertID { get; set; }

		[JsonPropertyName( "data" )]
		public JsonElement Data { get; set; }

		public override string ToString()
		{
			return string.Format( "RID: {0} - Err: {1} - Success: {2} - LID: {3} - Data: {4}", ReqID, Error, Success, LastInsertID, Data );
		}
	}

	class CRUDGetAllData
	{

		[JsonPropertyName( "count" )]
		public int Count { get; set; }

		[JsonPropertyName( "list" )]
		public IDictionary<string, JsonElement> Data { get; set; }
		public CRUDGetAllData()
		{
		}
	}
	class CRUDTools
	{
		static CRUDTools instance = null;
		private static object objLock = new();

		private JsonSerializerOptions JSONOpt = new()
		{
			IncludeFields = true,
			IgnoreReadOnlyFields = true
		};


		private WebSocket WS;

		private Dictionary<ulong, CRUDRequest> Processing = new();

		private CRUDTools()
		{

		}

			
		private async Task InitWs() {
			if(WS != null && WS.IsConnected ) {
				return;
			}

			WS = new();
			Log.Info( "Connecting to websocket." );
			await WS.Connect( ServerConfig.WSUrl );

			WS.OnDisconnected += async ( status, reason ) => {
				Log.Error( string.Format( "WS Disconnected with: %d %s", status, reason ) );
				await Task.Delay( 1000 );
				await InitWs();
				// Empty or resend processing, relog ?
			};

			WS.OnMessageReceived += WsResponseHandler;

			Log.Info( "Sending login to websocket." );

			Dictionary<string, string> LoginData = new();
			LoginData["user"] = ServerConfig.WSUser;
			LoginData["pass"] = ServerConfig.WSPass;
			//LoginData["debug"] = "full";
			await WS.Send( JsonSerializer.Serialize( LoginData, JSONOpt ) );
		}

		private void WsResponseHandler( string message )
		{
			CRUDResponse resp = JsonSerializer.Deserialize<CRUDResponse>( message, JSONOpt );
			if ( Processing.ContainsKey( resp.ReqID ) )
			{
				Processing[resp.ReqID].ResponsePromise.TrySetResult( resp );
				Processing.Remove( resp.ReqID );
			}
		}

		public async Task<CRUDResponse> Get( string type, string id )
		{
			await InitWs();
			CRUDRequest req = new CRUDRequest( "GET", type, id, null );
			Processing[req.ReqID] = req;
			var reqJson = JsonSerializer.Serialize( req, JSONOpt );
			await WS.Send( reqJson );
			return await req.ResponsePromise.Task;
		}
		public async Task<CRUDResponse> Set( string type, string id, JsonDocument data )
		{
			await InitWs();
			CRUDRequest req = new CRUDRequest( "SET", type, id, data );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}
		public async Task<CRUDResponse> Del( string type, string id )
		{
			await InitWs();
			CRUDRequest req = new CRUDRequest( "DEL", type, id, null );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}
		public async Task<CRUDResponse> DelType( string type )
		{
			await InitWs();
			CRUDRequest req = new CRUDRequest( "DELTYPE", type, "", null );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}
		public async Task<CRUDResponse> WipeDB()
		{
			await InitWs();
			CRUDRequest req = new CRUDRequest( "DELALL", "", "", null );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}
		public async Task<CRUDResponse> Add( string type, JsonDocument data )
		{
			await InitWs();
			CRUDRequest req = new CRUDRequest( "ADD", type, "", data );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}
		public async Task<CRUDResponse> GetAll( string type )
		{
			await InitWs();
			CRUDRequest req = new CRUDRequest( "GETALL", type, "", null );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}


		public static CRUDTools GetInstance()
		{
			lock ( objLock )
			{
				if ( instance == null )
					instance = new CRUDTools();
				return instance;
			}
		}
	}

	class ForeignReference
	{
		[JsonPropertyName( "__foreign_id" )]
		public string Id { get; set; }

		[JsonPropertyName( "__foreign_type" )]
		public string TypeName { get
			{
				return Type.Name;
			}
		}

		[JsonIgnore]
		public Type Type { get; set; }
	}

	class CRUDSerializer
	{
		public static JsonDocument SerializeToDocument<T>(T baseObj )
		{
			var props = typeof( T ).GetProperties( BindingFlags.Instance | BindingFlags.Public );

			Dictionary<string, object> dict = new Dictionary<string, object>();

			foreach ( var childProp in props ) {
				var childName = childProp.Name;
				var childType = childProp.PropertyType;
				var childValue = childProp.GetValue( baseObj, null );

				if ( (childType.IsClass || childType.IsInterface) && childType != typeof(string) ) {
					Log.Error( "prop: " + childName + " value: " + childValue + " type: " + childType );
					continue;
				}


				dict.Add( childName, childValue );
				Log.Info( "prop: " + childName + " value: " + childValue + " type: " + childType);
			}

			return JsonSerializer.SerializeToDocument( dict );
		}

	}

	
}
