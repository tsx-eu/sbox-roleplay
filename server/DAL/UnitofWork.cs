using charleroi.server.DAL.Repository;
using Sandbox;

namespace charleroi.server.DAL
{
	public class UnitofWork : IUnitOfWork
	{
#pragma warning disable 0649
		private readonly IRepository<SPlayer> _SPlayer;
		private readonly IRepository<SItem> _SItem;
#pragma warning restore 0649

		public UnitofWork() {
			Host.AssertServer();
		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new SPlayerRepository();
		public IRepository<SItem> SItem => _SItem ?? new SItemRepository();
	}
}
