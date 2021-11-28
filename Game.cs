using Sandbox;

using charleroi.client;
using charleroi.server.DAL;

namespace charleroi
{
	public partial class Game : Sandbox.Game {
		HUD hud;

		public Game() {
			if ( IsServer )
				hud = new HUD();

			if ( IsClient ) {
				// ...
			}
		}

		[Event.Hotload]
		public void ReloadUI() {
			if ( IsServer ) {
				hud.Delete();
				hud = new HUD();
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
