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
		private readonly IRepository<SCraft> _SCraft;
#pragma warning restore 0649

		public UnitofWork() {
			Host.AssertServer();
		}

		public async Task<bool> Seed()
		{
			await CRUDTools.GetInstance().WipeDB();
			await Seed_Items();
			await Seed_Jobs();
			await Seed_Crafts();
			await Game.Instance.InitializeDB();

			Log.Info( "Task complete!" );
			return true;
		}
		private async Task<bool> Seed_Items()
		{
			Log.Info( "Creating items" );

			ulong id = 1;

			await SItem.Insert( new CItem { Id=id++, Name = "Pomme", ShortDescription = "Ceci est une pomme", MaxCarry=16 });
			await SItem.Insert( new CItem { Id=id++, Name = "Poire", ShortDescription = "Ceci est une poire", MaxCarry=16 });

			Log.Info( "Creating items complete!" );
			return true;
		}
		private async Task<bool> Seed_Jobs()
		{
			Log.Info( "Creating jobs" );

			ulong id = 1;

			await SJob.Insert( new CJob { Id=id++, Name = "Chimiste", Description = "Vend des truc" });
			await SJob.Insert( new CJob { Id=id++, Name = "Vendeur", Description = "Vend des truc" });

			Log.Info( "Creating jobs complete!" );
			return true;
		}
		private async Task<bool> Seed_Crafts()
		{
			Log.Info( "Creating crafts" );

			ulong id = 1;

			await SCraft.Insert( new CCraft { Id=id++, Name = "Acide Citrique", Description = "L'acide citrique est un acide tricarboxylique α-hydroxylé présent en abondance dans le citron, d'où son nom." } );
			await SCraft.Insert( new CCraft { Id=id++, Name = "Canne à pêche", Description = "Si tu ne sais pas à quoi ça sert, c'est que c'est pas pour toi ..." } );
			await SCraft.Insert( new CCraft { Id=id++, Name = "Super Canne", Description = "Si tu ne sais pas à quoi ça sert, c'est que c'est pas pour toi ...", Level = 1 } );
			await SCraft.Insert( new CCraft { Id=id++, Name = "Méga canne", Description = "Si tu ne sais pas à quoi ça sert, c'est que c'est pas pour toi ...", Level = 2} );

			Log.Info( "Creating craft complete!" );
			return true;
		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new SPlayerRepository();
		public IRepository<SItem> SItem => _SItem ?? new SItemRepository();
		public IRepository<SJob> SJob => _SJob ?? new SJobRepository();
		public IRepository<SCraft> SCraft => _SCraft ?? new SCraftRepository();
	}
}
