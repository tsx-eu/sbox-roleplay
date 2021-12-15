using charleroi.client;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace charleroi.server.DAL.Repository
{
	class SItemRepository : IRepository<SItem>
	{
		public SItemRepository()
		{
			Host.AssertServer();
		}

		public async Task<bool> Delete( SItem entity )
		{
			var res = await CRUDTools.GetInstance().Del( typeof( SItem ).Name, entity.Id.ToString() );
			if(res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public async Task<SItem> Get( object id )
		{
			var req = await CRUDTools.GetInstance().Get( typeof( SItem ).Name, id.ToString() );

			var res = req;
			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resPlayer = CRUDSerializer.Deserialize<CItem>( res.Data );
			return resPlayer;
		}

		public async Task<IList<SItem>> GetAll()
		{
			var res = await CRUDTools.GetInstance().GetAll( typeof( SItem ).Name );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return null;
			}

			var resMap = CRUDSerializer.Deserialize<CRUDGetAllData>( res.Data );

			var SPlyList = new List<SItem>();
			foreach (var elem in resMap.Data ) {
				var SPly = CRUDSerializer.Deserialize<CItem>( elem.Value );
				SPlyList.Add( SPly );
			}

			return SPlyList;
		}

		public async Task<bool> Insert( SItem entity )
		{
			JsonDocument toast = CRUDSerializer.SerializeToDocument<SItem>( entity );
			var res = await CRUDTools.GetInstance().Set( typeof( SItem ).Name, entity.Id.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}

		public async Task<bool> Update( SItem entity )
		{
			JsonDocument toast = CRUDSerializer.SerializeToDocument<SItem>( entity );
			var res = await CRUDTools.GetInstance().Set( typeof( SItem ).Name, entity.Id.ToString(), toast );

			if ( res.Error != "" ) {
				Log.Error( res.Error );
				return false;
			}

			return true;
		}
	}
}
