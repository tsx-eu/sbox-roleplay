using charleroi.server.DAL.Repository;
using charleroi.server.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi.server.DAL
{
	public class UnitofWork : IUnitOfWork
	{
		private IRepository<SPlayer> _SPlayer;

		public UnitofWork()
		{

		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new SPlayerRepository();
	}
}
