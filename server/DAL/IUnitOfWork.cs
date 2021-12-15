using System.Threading.Tasks;

namespace charleroi.server.DAL
{
	public interface IUnitOfWork
	{
		public IRepository<SPlayer> SPlayer { get; }
		public IRepository<SItem> SItem { get; }
		public IRepository<SJob> SJob { get; }
		public IRepository<SCraft> SCraft { get; }
	}
}
