using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace charleroi.server.DAL
{
	public class Repository<S, C> : IRepository<S>
		where S : class, IStorable
		where C : S, new()
	{
		public async Task<bool> Delete( S entity )
		{
			var res = await CRUDTools.GetInstance().Del( typeof( S ).Name, entity.Id.ToString() );
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public async Task<S> Get( object id )
		{
			var req = await CRUDTools.GetInstance().Get( typeof( S ).Name, id.ToString() );

			var res = req;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return default;
			}

			var resPlayer = await CRUDSerializer.Deserialize<C>( res.Data );
			return resPlayer;
		}

		public async Task<IList<S>> GetAll()
		{
			var res = await CRUDTools.GetInstance().GetAll( typeof( S ).Name );

			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return default;
			}

			var resMap = await CRUDSerializer.Deserialize<CRUDGetAllData>( res.Data );

			var SPlyList = new List<S>();
			foreach ( var elem in resMap.Data )
			{
				var SPly = await CRUDSerializer.Deserialize<C>( elem.Value );
				SPlyList.Add( SPly );
			}

			return SPlyList;
		}

		public async Task<bool> Insert( S entity )
		{
			JsonDocument toast = CRUDSerializer.SerializeToDocument<S>( entity );
			var res = await CRUDTools.GetInstance().Set( typeof( S ).Name, entity.Id.ToString(), toast );

			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}

			return true;
		}

		public async Task<bool> Update( S entity )
		{
			JsonDocument toast = CRUDSerializer.SerializeToDocument<S>( entity );
			var res = await CRUDTools.GetInstance().Set( typeof( S ).Name, entity.Id.ToString(), toast );

			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}

			return true;
		}
	}
}
