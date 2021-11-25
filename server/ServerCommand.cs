using System;
using System.Linq;

using Sandbox;

using charleroi.server.DAL;
using charleroi.client;

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

		[ServerCmd( "rp_loadme" )]
		public static void Cmd_LoadMe()
		{
			var uow = new UnitofWork();
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;

			var steamid = (ulong)client.Client.PlayerId;
			Log.Info( "Your StemID:" + steamid );

			SPlayer SPly = uow.SPlayer.Get( steamid );

			if ( SPly == null ) {
				client.SteamID = (ulong)client.Client.PlayerId;
				uow.SPlayer.Insert( client );
				Log.Info( "Player data created" );
			}
			else {
				client.Load( SPly );
				Log.Info( "Player data loaded" );
			}


		}

		[ServerCmd( "rp_delme" )]
		public static void Cmd_DelMe()
		{
			var uow = new UnitofWork();
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			bool success = uow.SPlayer.Delete( client );

			if ( success )
			{
				Log.Info( "Player data deleted" );
			}
			else
			{
				Log.Info( "Player not deleted" );
			}
		}

		[ServerCmd( "rp_saveme" )]
		public static void Cmd_SaveMe()
		{
			var uow = new UnitofWork();
			CPlayer client = ConsoleSystem.Caller.Pawn as CPlayer;
			bool success = uow.SPlayer.Update( client );

			if ( success )
			{
				Log.Info( "Player data saved" );
			}
			else
			{
				Log.Info( "Player not saved" );
			}
		}

		[ServerCmd( "rp_spawn2" )]
		public static void Cmd_spawn2()
		{
			string model = "models/tsx/dab2.vmdl";

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
