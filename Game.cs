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

		public override void ClientJoined( Client client ) {
			base.ClientJoined( client );

			var player = new CPlayer();
			client.Pawn = player;

			/*
			if ( IsServer ) {
				var steamid = (ulong)client.PlayerId;

				var uow = new UnitofWork();
				SPlayer SPly = uow.SPlayer.Get( steamid );
				if ( SPly == null )
					uow.SPlayer.Insert( player );
				else
					player.Load( SPly );
			}*/

			player.Respawn();
		}
	}

}
