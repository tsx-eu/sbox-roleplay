using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace charleroi.server.DAL.Repository
{
	class SPlayerRepository : IRepository<SPlayer>
	{
		public SPlayerRepository()
		{
			Host.AssertServer();
		}

		public bool Delete( SPlayer entity )
		{
			if( entity.SteamID == 0 ) 
				throw new Exception( "SteamID is unknown" );

			var res = CRUDTools.GetInstance().Del( "player", entity.SteamID.ToString() ).Result;
			if(res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public SPlayer Get( object id )
		{
			if ( (ulong)id == 0 )
				throw new Exception( "SteamID is unknown" );

			var req = CRUDTools.GetInstance().Get( "player", id.ToString() );

			var res = req.Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return null;
			}
			var resPlayer = JsonSerializer.Deserialize<SPlayer>( res.Data.GetRawText() );
			return resPlayer;
		}

		public IList<SPlayer> GetAll()
		{
			var req = CRUDTools.GetInstance().GetAll( "player" );
			var res = req.Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return null;
			}
			var resMap = JsonSerializer.Deserialize<CRUDGetAllData>( res.Data.GetRawText() );
			var SPlyList = new List<SPlayer>();
			foreach (var elem in resMap.Data )
			{
				var SPly = JsonSerializer.Deserialize<SPlayer>( elem.Value.GetRawText() );
				SPlyList.Add( SPly );
			}
			return SPlyList;
		}

		public bool Insert( SPlayer entity )
		{
			if ( entity.SteamID == 0 )
				throw new Exception( "SteamID is unknown" );

			var res = CRUDTools.GetInstance().Set( "player", entity.SteamID.ToString(), entity ).Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public void Save()
		{
			throw new NotImplementedException();
		}

		public bool Update( SPlayer entity )
		{
			if ( entity.SteamID == 0 )
				throw new Exception( "SteamID is unknown" );

			var toast = JsonSerializer.Serialize<SPlayer>( entity );
			Log.Info( toast );
			var res = CRUDTools.GetInstance().Set( "player", entity.SteamID.ToString(), entity ).Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}
	}
}
