using Sandbox;
using System;

namespace Charleroi
{
	class ServerCommand {

		[ServerCmd( "rp_health" )]
		public static void Cmd_Health() {
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			if ( client == null ) return;

			client.Health = Rand.Float( 1.0f, client.MaxHealth );
		}
	}
}
