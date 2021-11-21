using Sandbox;

using charleroi.server.DAL;
using charleroi.client;
using charleroi.client.WorldUI;
using System.Linq;

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

		[ServerCmd( "rp_spawn_old" )]
		public static void Cmd_spawnold()
		{
			string model = "models/tsx/table_craft3.vmdl";
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

			var lightPosition = ent.GetAttachment( "light" ); // recup de la position de l'attache sur le props
			if ( lightPosition.HasValue )
			{
				var light = new SpotLightEntity();                      // création de la lumiere
				light.Transform = lightPosition.Value;                  // encapsulation de la position
				light.SetParent( ent );                                 // attache lumiere avec l'attache du props
				light.SetLightBrightness( 0.8f );                       // réglage luminosité
				light.SetLightColor( new Color( 0xF2, 0xBE, 0x5C ) );   // réglage couleur
				light.Range = 128.0f;
			}
		}

		[ServerCmd( "rp_spawn" )]
		public static void Cmd_spawn()
		{
			Entity.All.Where( i => i.Tags.Has( "show" ) ).ToList().ForEach( i => i.Delete() );

			var client = ConsoleSystem.Caller?.Pawn;
			var tr = Trace.Ray( client.EyePos, client.EyePos + client.EyeRot.Forward * 100 )
				.UseHitboxes()
				.Ignore( client )
				.Run();

			var ent = new CEntityCrafttable {
				description = "CECI EST UNE TABLE"
			};

			ent.Rotation = Rotation.From( new Angles( 0, client.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
			ent.Position = tr.EndPos - Vector3.Up * ent.CollisionBounds.Mins.z;

			ent.TurnOff();
		}
	}
}
