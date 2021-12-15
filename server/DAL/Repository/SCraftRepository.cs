using charleroi.client;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace charleroi.server.DAL.Repository
{
	class SCraftRepository : IRepository<SCraft>
	{
		public SCraftRepository()
		{
			Host.AssertServer();
		}

		public async Task<bool> Delete( SCraft entity )
		{
			var res = await CRUDTools.GetInstance().Del( typeof( SCraft ).Name, entity.Id.ToString() );
			if(res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public async Task<SCraft> Get( object id )
		{
			var req = await CRUDTools.GetInstance().Get( typeof( SCraft ).Name, id.ToString() );

			var res = req;
			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resPlayer = await CRUDSerializer.Deserialize<CCraft>( res.Data );
			return resPlayer;
		}

		public async Task<IList<SCraft>> GetAll()
		{
			var res = await CRUDTools.GetInstance().GetAll( typeof(SCraft).Name );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resMap = await CRUDSerializer.Deserialize<CRUDGetAllData>( res.Data );

			var SPlyList = new List<SCraft>();
			foreach (var elem in resMap.Data ) {
				var SPly = await CRUDSerializer.Deserialize<CCraft>( elem.Value );
				SPlyList.Add( SPly );
			}

			return SPlyList;
		}

		public async Task<bool> Insert( SCraft entity )
		{
			JsonDocument toast = CRUDSerializer.SerializeToDocument<SCraft>( entity );
			var res = await CRUDTools.GetInstance().Set( typeof( SCraft ).Name, entity.Id.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}

		public async Task<bool> Update( SCraft entity )
		{
			JsonDocument toast = CRUDSerializer.SerializeToDocument<SCraft>( entity );
			var res = await CRUDTools.GetInstance().Set( typeof( SCraft ).Name, entity.Id.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}
	}
}
