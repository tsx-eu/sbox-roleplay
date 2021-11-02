using charleroi.server.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Charleroi;
using Sandbox;

namespace charleroi.server.DAL.Repository
{
	class SPlayerRepository : IRepository<SPlayer>
	{
		public SPlayerRepository()
		{

		}

		public bool Delete( SPlayer entity )
		{
			var res = CRUDTools.GetInstance().Del( "player", entity.SteamID ).Result;
			if(res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public SPlayer Get( ulong id )
		{
			var res = CRUDTools.GetInstance().Get( "player", id ).Result;
			Log.Info( res );
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return null;
			}
			Log.Info( res.Data.ToObject<SPlayer>() );
			return res.Data.ToObject<SPlayer>();
		}

		public IList<SPlayer> GetAll()
		{
			throw new NotImplementedException();
		}

		public bool Insert( SPlayer entity )
		{
			var res = CRUDTools.GetInstance().Set( "player", entity.SteamID, entity ).Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}

		public void Save()
		{
		}

		public bool Update( SPlayer entity )
		{
			var res = CRUDTools.GetInstance().Set( "player", entity.SteamID, entity ).Result;
			if ( res.Error != "" )
			{
				Log.Error( res.Error );
				return false;
			}
			return true;
		}
	}
}
