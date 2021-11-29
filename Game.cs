using Sandbox;

using charleroi.client;
using charleroi.server.DAL;
using System.Threading.Tasks;

namespace charleroi
{
	public partial class Game : Sandbox.Game {
		HUD hud;

		public Game() {
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

			CItem.Dictionnary.Clear();
			var items = await uow.SItem.GetAll();
			foreach ( var item in items )
				CItem.Dictionnary.Add( item.Id, item as CItem );
			Log.Info( "CItem.Dictionnary initialized with " + CItem.Dictionnary.Count );

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

		public async override void ClientJoined( Client client ) {
			base.ClientJoined( client );

			var player = new CPlayer();
			client.Pawn = player;
			player.SteamID = (ulong)client.PlayerId;
			player.Job = "Chomeur";

			var uow = new UnitofWork();
			SPlayer data = await uow.SPlayer.Get( player.SteamID );
			if ( data != null )
				player.Load( data );
			else
				await uow.SPlayer.Update( player );

			player.Respawn();
		}
	}

}
