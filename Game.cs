using Sandbox;
using System.Threading;

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

		public override void ClientJoined( Client client ) {
			base.ClientJoined( client );

			var player = new CPlayer();
			client.Pawn = player;
			player.Respawn();
		}
	}

}
