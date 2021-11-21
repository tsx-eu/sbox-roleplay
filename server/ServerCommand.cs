using Sandbox;

using charleroi.server.DAL;
using charleroi.client;
using charleroi.client.WorldUI;

namespace charleroi.server
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
			var steamid = (ulong)client.Client.PlayerId;
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
			var steamid = (ulong)client.Client.PlayerId;

			bool success = uow.SPlayer.Delete( new SPlayer
			{
				SteamID = steamid
			} );

			if ( success )
			{
				Log.Info( "Player data deleted" );
			}
			else
			{
				Log.Info( "Player not deleted" );
			}
		}
		
		[ServerCmd( "rp_spawn" )]
		public static void Cmd_spawn()
		{
			string model = "models/tsx/table_craft.vmdl";
			string tagname = "Table de craft";

			var client = ConsoleSystem.Caller?.Pawn;

			var tr = Trace.Ray( client.EyePos, client.EyePos + client.EyeRot.Forward * 100 )
				.UseHitboxes()
				.Ignore( client )
				.Run();

			var ent = new Prop();
			ent.Position = tr.EndPos;
			ent.Rotation = Rotation.From( new Angles( 0, client.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
			ent.SetModel( model );
			ent.Position = tr.EndPos - Vector3.Up * ent.CollisionBounds.Mins.z;
			ent.Name = tagname;
			ent.Tags.Add( "show" );
		}
	}
}
