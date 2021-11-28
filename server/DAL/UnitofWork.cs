using System.Threading.Tasks;
using charleroi.client;
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

		public async Task<bool> Seed()
		{
			await CRUDTools.GetInstance().WipeDB();
			await Seed_Items();

			Log.Info( "Task complete!" );
			return true;
		}
		private async Task<bool> Seed_Items()
		{
			Log.Info( "Creating items" );

			await SItem.Insert( new CItem { Id=1, Name = "Pomme", ShortDescription = "Ceci est une pomme", MaxCarry=16 });
			await SItem.Insert( new CItem { Id=2, Name = "Poire", ShortDescription = "Ceci est une poire", MaxCarry=16 });

			Log.Info( "Creating items complete!" );
			return true;
		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new SPlayerRepository();
		public IRepository<SItem> SItem => _SItem ?? new SItemRepository();
	}
}
