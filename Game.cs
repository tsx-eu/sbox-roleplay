using Sandbox;

using charleroi.client;
using charleroi.server.DAL;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace charleroi
{
	public partial class Game : Sandbox.Game {
		HUD hud;

		public static Game Instance {
			get => Current as Game;
		}

		[Net] public IList<CItem> Items { get; set; }
		[Net] public IList<CJob> Jobs { get; set; }
		[Net] public IList<CSkill> Skills { get; set; }
		[Net] public IList<CCraft> Crafts { get; set; }

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

			Instance.Items.Clear();
			var items = await uow.SItem.GetAll();
			foreach ( var item in items )
				Instance.Items.Add( item as CItem );
			Log.Info( "CItem.Dictionnary initialized with " + Instance.Items.Count );

			Instance.Jobs.Clear();
			var jobs = await uow.SJob.GetAll();
			foreach ( var job in jobs )
				Instance.Jobs.Add( job as CJob );
			Log.Info( "CJob.Dictionnary initialized with " + Instance.Jobs.Count );

			Instance.Skills.Clear();
			var skills = await uow.SSkill.GetAll();
			foreach ( var skill in skills )
				Instance.Skills.Add( skill as CSkill );
			Log.Info( "CSkill.Dictionnary initialized with " + Instance.Skills.Count );

			Instance.Crafts.Clear();
			var crafts = await uow.SCraft.GetAll();
			foreach ( var craft in crafts )
				Instance.Crafts.Add( craft as CCraft );
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
