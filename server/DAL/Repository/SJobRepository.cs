using charleroi.client;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace charleroi.server.DAL.Repository
{
	class SJobRepository : IRepository<SJob>
	{
		public SJobRepository()
		{
			Host.AssertServer();
		}

		public async Task<bool> Delete( SJob entity )
		{
			var res = await CRUDTools.GetInstance().Del( "job", entity.Id.ToString() );
			if(res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public async Task<SJob> Get( object id )
		{
			var req = await CRUDTools.GetInstance().Get( "job", id.ToString() );

			var res = req;
			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resPlayer = JsonSerializer.Deserialize<CJob>( res.Data );
			return resPlayer;
		}

		public async Task<IList<SJob>> GetAll()
		{
			var res = await CRUDTools.GetInstance().GetAll( "job" );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resMap = JsonSerializer.Deserialize<CRUDGetAllData>( res.Data );

			var SPlyList = new List<SJob>();
			foreach (var elem in resMap.Data ) {
				var SPly = JsonSerializer.Deserialize<CJob>( elem.Value );
				SPlyList.Add( SPly );
			}

			return SPlyList;
		}

		public async Task<bool> Insert( SJob entity )
		{
			JsonDocument toast = JsonSerializer.SerializeToDocument<SJob>( entity );
			var res = await CRUDTools.GetInstance().Set( "job", entity.Id.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}

		public async Task<bool> Update( SJob entity )
		{
			JsonDocument toast = JsonSerializer.SerializeToDocument<SJob>( entity );
			var res = await CRUDTools.GetInstance().Set( "job", entity.Id.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}
	}
}
