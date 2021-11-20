using charleroi.server.DAL.Repository;

namespace charleroi.server.DAL
{
	public interface IUnitOfWork
	{
		public IRepository<SPlayer> SPlayer { get; }
		public IRepository<SItem> SItem { get; }
	}
}
