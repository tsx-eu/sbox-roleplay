using charleroi.client;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace charleroi.server.DAL.Repository
{
	class SPlayerRepository : IRepository<SPlayer>
	{
		public SPlayerRepository()
		{
			Host.AssertServer();
		}

		public async Task<bool> Delete( SPlayer entity )
		{
			if( entity.SteamID == 0 ) 
				throw new Exception( "SteamID is unknown" );

			var res = await CRUDTools.GetInstance().Del( "player", entity.SteamID.ToString() );

			if(res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}

		public async Task<SPlayer> Get( object id )
		{
			if ( (ulong)id == 0 )
				throw new Exception( "SteamID is unknown" );

			var res = await CRUDTools.GetInstance().Get( "player", id.ToString() );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resPlayer = JsonSerializer.Deserialize<CPlayer>( res.Data );
			return resPlayer;
		}

		public async Task<IList<SPlayer>> GetAll()
		{
			var res = await CRUDTools.GetInstance().GetAll( "player" );
			
			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resMap = JsonSerializer.Deserialize<CRUDGetAllData>( res.Data );
			var SPlyList = new List<SPlayer>();
			foreach (var elem in resMap.Data ) {
				var SPly = JsonSerializer.Deserialize<CPlayer>( elem.Value );
				SPlyList.Add( SPly );
			}

			return SPlyList;
		}

		public async Task<bool> Insert( SPlayer entity )
		{
			if ( entity.SteamID == 0 )
				throw new Exception( "SteamID is unknown" );

			JsonDocument toast = CRUDSerializer.SerializeToDocument<SPlayer>( entity );
			var res = await CRUDTools.GetInstance().Set( "player", entity.SteamID.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}

		public async Task<bool> Update( SPlayer entity )
		{
			if ( entity.SteamID == 0 )
				throw new Exception( "SteamID is unknown" );

			JsonDocument toast = CRUDSerializer.SerializeToDocument<SPlayer>( entity );

			var res = await CRUDTools.GetInstance().Set( "player", entity.SteamID.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}
	}
}
