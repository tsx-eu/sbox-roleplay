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

			await SItem.Insert( new CItem { Id=id++, Name = "Bûche", ShortDescription = "Du bois", MaxCarry=16 });

			Log.Info( "Creating items complete!" );
			return true;
		}
		private async Task<bool> Seed_Jobs()
		{
			Log.Info( "Creating jobs" );

			ulong id = 1;

			await SJob.Insert( new CJob { Id = id++, Name = "Mineur", Description = "TBD" });
			await SJob.Insert( new CJob { Id = id++, Name = "Raffineur", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Électronicien", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Forgeron", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Mécanicien", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Ingénieur", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Bucheron", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Menuisier", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Armurier", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Chimiste", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Dealer", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Agrigulteur", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Menier", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Boulanger", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Pêcheur", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Éleveur", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Boucher", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Cuisinier", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Couturier", Description = "TBD" } );

			Log.Info( "Creating jobs complete!" );
			return true;
		}
		private async Task<bool> Seed_Crafts()
		{
			Log.Info( "Creating crafts" );

			ulong id = 1;

			await SCraft.Insert( new CCraft { Id = id++, Name = "Planche", Description = "TBD" } );
			await SCraft.Insert( new CCraft { Id = id++, Name = "Baton", Description = "TBD" } );
			await SCraft.Insert( new CCraft { Id = id++, Name = "Chaise", Description = "TBD", Level = 1 } );
			await SCraft.Insert( new CCraft { Id = id++, Name = "Table", Description = "TBD", Level = 1} );
			await SCraft.Insert( new CCraft { Id = id++, Name = "Lit", Description = "TBD", Level = 2 } );

			Log.Info( "Creating craft complete!" );
			return true;
		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new SPlayerRepository();
		public IRepository<SItem> SItem => _SItem ?? new SItemRepository();
		public IRepository<SJob> SJob => _SJob ?? new SJobRepository();
		public IRepository<SCraft> SCraft => _SCraft ?? new SCraftRepository();
	}
}
