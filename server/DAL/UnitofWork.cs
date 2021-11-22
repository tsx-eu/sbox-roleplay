using charleroi.server.DAL.Repository;
using Sandbox;

namespace charleroi.server.DAL
{
	public class UnitofWork : IUnitOfWork
	{
		private IRepository<SPlayer> _SPlayer;
		private IRepository<SItem> _SItem;

		public UnitofWork() {
			Host.AssertServer();
		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new SPlayerRepository();
		public IRepository<SItem> SItem => _SItem ?? new SItemRepository();
	}
}
