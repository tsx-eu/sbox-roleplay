using Sandbox;

using charleroi.client;
using charleroi.server.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace charleroi
{
	public partial class Game : Sandbox.Game {
		HUD hud;

		public static Game Instance {
			get => Current as Game;
		}

		[Net] public IList<CItem> Items { get; set; }
		public IDictionary<ulong, CItem> DItems { get; protected set; }

		[Net] public IList<CJob> Jobs { get; set; }
		public IDictionary<ulong, CJob> DJobs { get; protected set; }

		[Net] public IList<CSkill> Skills { get; set; }
		public IDictionary<ulong, CSkill> DSkills { get; protected set; }

		[Net] public IList<CCraft> Crafts { get; set; }
		public IDictionary<ulong, CCraft> DCraft { get; protected set; }

		public Game()
		{
			Transmit = TransmitType.Always;

			if ( IsServer ) {
				hud = new HUD();
				_ = InitializeDB();
			}

			if ( IsClient ) {
				// ...
			}

		}

		public async Task<bool> InitializeDB() {
			var uow = new UnitofWork();

			Instance.DItems = new Dictionary<ulong, CItem>();
			var items = await uow.SItem.GetAll();
			foreach ( var item in items )
				Instance.DItems.Add( item.Id, item as CItem );
			Instance.Items = Instance.DItems.Values.ToList();
			Log.Info( "CItem.Dictionnary initialized with " + Instance.Items.Count );


			Instance.DJobs = new Dictionary<ulong, CJob>();
			var jobs = await uow.SJob.GetAll();
			foreach ( var job in jobs )
				Instance.DJobs.Add( job.Id, job as CJob );
			Instance.Jobs = Instance.DJobs.Values.ToList();
			Log.Info( "CJob.Dictionnary initialized with " + Instance.Jobs.Count );


			Instance.DSkills = new Dictionary<ulong, CSkill>();
			var skills = await uow.SSkill.GetAll();
			foreach ( var skill in skills )
				Instance.DSkills.Add(skill.Id, skill as CSkill );
			Instance.Skills = Instance.DSkills.Values.ToList();
			Log.Info( "CSkill.Dictionnary initialized with " + Instance.Skills.Count );


			Instance.DCraft = new Dictionary<ulong, CCraft>();
			var crafts = await uow.SCraft.GetAll();
			foreach ( var craft in crafts )
				Instance.DCraft.Add( craft.Id, craft as CCraft );
			Instance.Crafts = Instance.DCraft.Values.ToList();
			Log.Info( "CCraft.Dictionnary initialized with " + Instance.Crafts.Count );

			return true;
		}

		[Event.Hotload]
		public void ReloadUI() {
			if ( IsServer ) {
				hud.Delete();
				hud = new HUD();

				_ = InitializeDB();
			}
		}

		private async Task<bool> LoadPlayerData( CPlayer player )
		{
			var uow = new UnitofWork();

			SPlayer data = await uow.SPlayer.Get( player.SteamID );
			if ( data != null )
				player.Load( data );
			else
				await uow.SPlayer.Update( player );

			player.Respawn();

			return true;
		}
		private async Task<bool> SavePlayerData( CPlayer player )
		{
			var uow = new UnitofWork();
			await uow.SPlayer.Update( player );

			return true;
		}

		public override void ClientJoined( Client client ) {
			base.ClientJoined( client );

			var player = new CPlayer();
			client.Pawn = player;
			player.SteamID = (ulong)client.PlayerId;
			player.Job = "Chomeur";

			player.Respawn();

			_ = LoadPlayerData( player);
		}

		public override void ClientDisconnect( Client client, NetworkDisconnectionReason reason )
		{
			var player = client.Pawn as CPlayer;
			_ = SavePlayerData( player );

			base.ClientDisconnect( client, reason );
		}
	}

}
