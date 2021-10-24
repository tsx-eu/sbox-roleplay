using charleroi.server.DAL.Repository;
using charleroi.server.POCO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi.server.DAL
{
	public interface IUnitOfWork
	{
		public IRepository<SPlayer> SPlayer { get; }
	}
}
