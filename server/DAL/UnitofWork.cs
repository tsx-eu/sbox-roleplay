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
		private readonly IRepository<SJob> _SJob;
#pragma warning restore 0649

		public UnitofWork() {
			Host.AssertServer();
		}

		public async Task<bool> Seed()
		{
			await CRUDTools.GetInstance().WipeDB();
			await Seed_Items();
			await Seed_Jobs();

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
		private async Task<bool> Seed_Jobs()
		{
			Log.Info( "Creating jobs" );


			await SJob.Insert( new CJob {Id=1, Name = "Chimiste", Description = "Vend des truc" });
			await SJob.Insert( new CJob {Id=2, Name = "Vendeur", Description = "Vend des truc" });

			Log.Info( "Creating jobs complete!" );
			return true;
		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new SPlayerRepository();
		public IRepository<SItem> SItem => _SItem ?? new SItemRepository();
		public IRepository<SJob> SJob => _SJob ?? new SJobRepository();
	}
}
