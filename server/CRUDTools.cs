using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Sandbox;

namespace Charleroi
{

	class CRUDRequest
	{
		private static ulong incr = 1;
		private static ulong ReqIncr
		{
			get
			{
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
		public ulong ID { get; set; }

		[JsonPropertyName( "data" )]
		public IDictionary<string, object> Data { get; set; }


		public CRUDRequest( string operation, string datatype, ulong id, IDictionary<string, object> data )
		{
			ReqID = ReqIncr;
			ReqType = operation;
			DataType = datatype;
			ID = id;
			Data = data;
		}

	}
	class CRUDResponse
	{
		[JsonPropertyName( "reqid" )]
		public ulong ReqID { get; set; }

		[JsonPropertyName( "error" )]
		public string Error { get; }

		[JsonPropertyName( "success" )]
		public bool Success { get; }

		[JsonPropertyName( "lastinsertid" )]
		public ulong LastInsertID { get; }

		[JsonPropertyName( "data" )]
		public IDictionary<string, object> Data { get; }

		public override string ToString()
		{
			return string.Format( "{0} - {1} - {2} - {3} - {4} - {5}", ReqID, Error, Success, LastInsertID, Data );
		}
	}

	class CRUDGetAllData
	{
		public int count { get; set; }
		public IDictionary<ulong, IDictionary<string, object>> Data { get; set; }
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
			IncludeFields = true
		};

		private WebSocket WS;

		private Dictionary<ulong, CRUDRequest> Processing = new();

		private CRUDTools()
		{
			InitWs();
		}


		private void InitWs()
		{
			WS = new();
			Log.Info( "Connecting to websocket." );
			WS.Connect( $"wss://wsstore.stone.leethium.fr/ws" );

			WS.OnDisconnected += ( status, reason ) =>
			{
				Log.Error( string.Format( "WS Disconnected with: %d %s", status, reason ) );
				// Empty or resend processing, relog ?
			};

			WS.OnMessageReceived += WsResponseHandler;

			Log.Info( "Sending login to websocket." );

			Dictionary<string, string> LoginData = new();
			LoginData["user"] = "api";
			LoginData["pass"] = "api";
			// LoginData["debug"] = "full";
			WS.Send( JsonSerializer.Serialize( LoginData, JSONOpt ) );
		}

		private void WsResponseHandler( string message )
		{
			Log.Info( message );
			CRUDResponse resp = JsonSerializer.Deserialize<CRUDResponse>( message );
			if ( Processing.ContainsKey( resp.ReqID ) )
			{
				Processing[resp.ReqID].ResponsePromise.TrySetResult( resp );
				Processing.Remove( resp.ReqID );
			}
		}

		public async Task<CRUDResponse> Get( string type, ulong id )
		{
			CRUDRequest req = new CRUDRequest( "GET", type, id, null );
			Log.Info( req.ToString() );
			Processing[req.ReqID] = req;
			var reqJson = JsonSerializer.Serialize( req, JSONOpt );
			await WS.Send( reqJson );
			return await req.ResponsePromise.Task;
		}

		public async Task<CRUDResponse> Set( string type, ulong id, object data )
		{
			CRUDRequest req = new CRUDRequest( "SET", type, id, data.AsDictionary() );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}

		public async Task<CRUDResponse> Del( string type, ulong id )
		{
			CRUDRequest req = new CRUDRequest( "DEL", type, id, null );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}

		public async Task<CRUDResponse> Add( string type, object data )
		{
			CRUDRequest req = new CRUDRequest( "ADD", type, 0, data.AsDictionary() );
			Processing[req.ReqID] = req;
			await WS.Send( JsonSerializer.Serialize( req, JSONOpt ) );
			return await req.ResponsePromise.Task;
		}


		public async Task<CRUDResponse> GetAll( string type )
		{
			CRUDRequest req = new CRUDRequest( "GETALL", type, 0, null );
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
}
