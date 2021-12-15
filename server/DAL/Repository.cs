﻿using System.Collections.Generic;
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

			var resPlayer = await CRUDSerializer.Deserialize( res.Data, typeof(C).Name );
			return (S)resPlayer;
		}

		public async Task<IList<S>> GetAll()
		{
			throw new System.Exception();
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
