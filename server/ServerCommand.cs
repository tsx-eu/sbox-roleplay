using charleroi.server.DAL;
using charleroi.server.POCO;
using Sandbox;

namespace charleroi
{
	class ServerCommand
	{

		[ServerCmd( "rp_health" )]
		public static void Cmd_Health()
		{
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			if ( client == null ) return;

			client.Health = Rand.Float( 1.0f, client.MaxHealth );
		}

		[ServerCmd( "rp_loadme" )]
		public static void Cmd_LoadMe()
		{
			var uow = new UnitofWork();
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			var steamid = client.Client.PlayerId;
			SPlayer SPly = uow.SPlayer.Get( steamid );
			if ( SPly == null )
			{
				SPly = new SPlayer()
				{
					Thirst = 50
				};
				uow.SPlayer.Insert( SPly );
				Log.Info( "Player data created" );
			}
			else
			{
				Log.Info( "Player data loaded" );
			}
		}

		[ServerCmd( "rp_delme" )]
		public static void Cmd_DelMe()
		{
			var uow = new UnitofWork();
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			var steamid = client.Client.PlayerId;

			bool success =  uow.SPlayer.Delete( uow.SPlayer.Get( steamid ) );

			if ( success )
			{
				Log.Info( "Player data deleted" );
			}
			else
			{
				Log.Info( "Player not deleted" );
			}
		}

	}
}
