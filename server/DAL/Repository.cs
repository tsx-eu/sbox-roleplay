using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Sandbox;

namespace charleroi.server.DAL
{
	public class Repository<S> : IRepository<S>
		where S : class, IStorable
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

			var resPlayer = await CRUDSerializer.Deserialize( res.Data, typeof(S).Name );
			return resPlayer as S;
		}

		public async Task<IList<S>> GetAll()
		{
			var res = await CRUDTools.GetInstance().GetAll( typeof( S ).Name );
			var resMap = JsonSerializer.Deserialize<CRUDGetAllData>( res.Data );

			var SPlyList = Library.Create<IList<S>>( "ListOf" + typeof( S ).Name );

			foreach ( var elem in resMap.Data )
			{
				var resPlayer = await CRUDSerializer.Deserialize( elem.Value, typeof( S ).Name );
				SPlyList.Add( resPlayer as S);
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
