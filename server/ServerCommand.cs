using System;
using System.Linq;

using Sandbox;

using charleroi.server.DAL;
using charleroi.client;
using System.Threading.Tasks;

namespace charleroi.server
{
	class ServerCommand
	{

		[ServerCmd( "rp_health" )]
		public static void Cmd_Health()
		{
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			if ( client == null ) return;

			client.Health = Rand.Float( 1.0f, 100.0f );
		}

		[ServerCmd( "rp_createdb" )]
		public static void Cmd_CreateDB()
		{
			var uow = new UnitofWork();
			_ = uow.Seed();
		}
		[ServerCmd( "rp_loaddb" )]
		public static void Cmd_LoadDB()
		{
			var uow = new UnitofWork();
			CCraft c = uow.SCraft.Get( 10 ).Result as CCraft;
			Log.Info( c.Item.Name );
		}

		[ServerCmd( "rp_delme" )]
		public static void Cmd_DelMe()
		{
			var uow = new UnitofWork();
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			client.SteamID = (ulong)client.Client.PlayerId;

			bool success = uow.SPlayer.Delete( client ).Result;

			if ( success ) {
				Log.Info( "Player data deleted" );
			}
			else {
				Log.Info( "Player not deleted" );
			}
		}

		[ServerCmd( "rp_saveme" )]
		public static void Cmd_SaveMe()
		{
			var uow = new UnitofWork();
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			client.SteamID = (ulong)client.Client.PlayerId;

			bool success = uow.SPlayer.Update( client ).Result;

			if ( success ) {
				Log.Info( "Player data saved" );
			}
			else {
				Log.Info( "Player not saved" );
			}
		}

		[ServerCmd( "rp_spawn2" )]
		public static void Cmd_spawn2()
		{
			string model = "models/tsx/kitchen.vmdl";

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
				description = "CECI EST UNE TABLE",
				model = CEntityCrafttable.Type.none,
				color = new Color( 0xF2, 0xBE, 0x5C )
			};

			ent.Rotation = Rotation.From( new Angles( 0, client.EyeRot.Angles().yaw, 0 ) ) * Rotation.FromAxis( Vector3.Up, 180 );
			ent.Position = tr.EndPos - Vector3.Up * ent.CollisionBounds.Mins.z;

			ent.TurnOff();
		}
	}
}
