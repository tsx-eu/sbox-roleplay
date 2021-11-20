using System;
using System.Collections.Generic;
using System.Text.Json;

namespace charleroi.server.DAL.Repository
{
	class SItemRepository : IRepository<SItem>
	{
		public SItemRepository()
		{

		}

		public bool Delete( SItem entity )
		{
			var res = CRUDTools.GetInstance().Del( "item", entity.Id.ToString() ).Result;
			if(res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public SItem Get( object id )
		{
			var req = CRUDTools.GetInstance().Get( "item", id.ToString() );

			var res = req.Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return null;
			}
			var resPlayer = JsonSerializer.Deserialize<SItem>( res.Data.GetRawText() );
			return resPlayer;
		}

		public IList<SItem> GetAll()
		{
			var req = CRUDTools.GetInstance().GetAll( "item" );
			var res = req.Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return null;
			}
			var resMap = JsonSerializer.Deserialize<CRUDGetAllData>( res.Data.GetRawText() );
			var SPlyList = new List<SItem>();
			foreach (var elem in resMap.Data )
			{
				var SPly = JsonSerializer.Deserialize<SItem>( elem.Value.GetRawText() );
				SPlyList.Add( SPly );
			}
			return SPlyList;
		}

		public bool Insert( SItem entity )
		{
			var res = CRUDTools.GetInstance().Set( "item", entity.Id.ToString(), entity ).Result;
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

		public bool Update( SItem entity )
		{
			var res = CRUDTools.GetInstance().Set( "item", entity.Id.ToString(), entity ).Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}
	}
}
