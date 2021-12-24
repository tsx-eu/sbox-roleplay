using System.Threading.Tasks;
using charleroi.client;
using Sandbox;

namespace charleroi.server.DAL
{
	public class UnitofWork : IUnitOfWork
	{
#pragma warning disable 0649
		private readonly IRepository<SPlayer> _SPlayer;
		private readonly IRepository<SItem> _SItem;
		private readonly IRepository<SJob> _SJob;
		private readonly IRepository<SSkill> _SSkill;
		private readonly IRepository<SCraft> _SCraft;
#pragma warning restore 0649

		public UnitofWork() {
			Host.AssertServer();
		}

		public async Task<bool> Seed()
		{
			await CRUDTools.GetInstance().WipeDB();
			await Seed_Jobs();
			await Seed_Crafts();
			await Seed_Skills();
			await Game.Instance.InitializeDB();

			Log.Info( "Task complete!" );
			return true;
		}
		private async Task<bool> Seed_Jobs()
		{
			Log.Info( "Creating jobs" );

			ulong id = 1;

			await SJob.Insert( new CJob { Id = id++, Name = "Raffineur", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Électronicien", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Forgeron", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Mécanicien", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Ingénieur", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Menuisier", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Armurier", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Chimiste", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Dealer", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Menier", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Boulanger", Description = "TBD" } );

			await SJob.Insert( new CJob { Id = id++, Name = "Boucher", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Cuisinier", Description = "TBD" } );
			await SJob.Insert( new CJob { Id = id++, Name = "Couturier", Description = "TBD" } );

			Log.Info( "Creating jobs complete!" );
			return true;
		}
		private async Task<bool> Seed_Skills()
		{
			Log.Info( "Creating skills" );

			ulong id = 1;

			await SSkill.Insert( new CSkill { Id = id++, Name = "Mineur", Description = "TBD" } );
			await SSkill.Insert( new CSkill { Id = id++, Name = "Bucheron", Description = "TBD" } );
			await SSkill.Insert( new CSkill { Id = id++, Name = "Agrigulteur", Description = "TBD" } );
			await SSkill.Insert( new CSkill { Id = id++, Name = "Pêcheur", Description = "TBD" } );
			await SSkill.Insert( new CSkill { Id = id++, Name = "Éleveur", Description = "TBD" } );

			Log.Info( "Creating skills complete!" );
			return true;
		}
		private async Task<bool> Seed_Crafts()
		{
			Log.Info( "Creating crafts" );

			ulong id = 1;

			var Bois = new CItem { Id = id++, Name = "Bois", ImageURL = "https://cdn-icons-png.flaticon.com/512/1965/1965194.png" };
			var Planche = new CItem { Id = id++, Name = "Planche", ImageURL= "https://cdn-icons-png.flaticon.com/512/683/683456.png" };
			var Baton = new CItem { Id = id++, Name = "Baton", ImageURL = "https://www.pikpng.com/pngl/m/240-2406829_wood-stick-png-toothpick-clipart.png" };
			var Chaise = new CItem { Id = id++, Name = "Chaise", ImageURL = "https://cdn-icons-png.flaticon.com/512/3649/3649237.png" };
			var Table = new CItem { Id = id++, Name = "Table", ImageURL = "https://i.pinimg.com/originals/21/53/06/215306a0c2a47bb0ede347cb36c5ddb6.png" };
			var Lit = new CItem { Id = id++, Name = "Lit", ImageURL = "https://cdn-icons-png.flaticon.com/512/3313/3313611.png" };

			await SItem.Insert( Bois );
			await SItem.Insert( Planche );
			await SItem.Insert( Baton );
			await SItem.Insert( Chaise );
			await SItem.Insert( Table );
			await SItem.Insert( Lit );

			await SCraft.Insert( new CCraft { Id = id++, Item = Planche, Ingredients = { Bois } } );
			await SCraft.Insert( new CCraft { Id = id++, Item = Baton, Ingredients = { Bois } } );
			await SCraft.Insert( new CCraft { Id = id++, Item = Chaise, Ingredients = { Planche, Baton } } );
			await SCraft.Insert( new CCraft { Id = id++, Item = Table, Ingredients = { Planche, Baton, Chaise } } );
			await SCraft.Insert( new CCraft { Id = id++, Item = Lit, Ingredients = { Planche, Bois } } );

			Log.Info( "Creating craft complete!" );
			return true;
		}

		public IRepository<SPlayer> SPlayer => _SPlayer ?? new Repository<SPlayer>();
		public IRepository<SItem> SItem => _SItem ?? new Repository<SItem>();
		public IRepository<SJob> SJob => _SJob ?? new Repository<SJob>();
		public IRepository<SSkill> SSkill => _SSkill ?? new Repository<SSkill>();
		public IRepository<SCraft> SCraft => _SCraft ?? new Repository<SCraft>();
	}
}
