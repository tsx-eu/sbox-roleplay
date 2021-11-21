using charleroi.client;
using charleroi.client.WorldUI;
using Sandbox;
using System;
using System.Collections.Generic;

namespace charleroi
{
	[Library( "tsx_crafttable" )]
	partial class CEntityCrafttable : Prop
	{
		public static class CEntityCrafttableType {
			public static string none = "models/tsx/table_craft3.vmdl";
		}
		private IList<Entity> lightings { get; set; } = new List<Entity>();

		[Net]
		public string description { get; set; } = "Table de craft V3";
		private Nametag tag = null;

		public CEntityCrafttable()  {
			
		}

		public override void Spawn() {
			SetModel( CEntityCrafttableType.none );
			Tags.Add( "show" );

			for(int i=0; i<8; i++) {
				Transform? pos;
				pos = GetAttachment( "spotlight" + (i > 0 ? i : "") );
				if ( pos.HasValue ) {
					var light = new SpotLightEntity();
					light.Transform = pos.Value;
					light.SetParent( this );
					light.SetLightBrightness( 0.8f );
					light.SetLightColor( new Color( 255, 255, 64 ) );
					light.Range = (Math.Abs( CollisionBounds.Mins.z ) + Math.Abs( CollisionBounds.Maxs.z )) * 2;
					lightings.Add( light );
				}

				pos = GetAttachment( "light" + (i > 0 ? i : "") );
				if ( pos.HasValue ) {
					var light = new PointLightEntity();
					light.Transform = pos.Value;
					light.SetParent( this );
					light.SetLightBrightness( 0.8f );
					light.SetLightColor( new Color( 255, 255, 64 ) );
					light.Range = (Math.Abs( CollisionBounds.Mins.z ) + Math.Abs( CollisionBounds.Maxs.z )) * 2;
					lightings.Add( light );
				}
			}

			base.Spawn();
		}

		public override void ClientSpawn() {
			base.ClientSpawn();
		}

		[Input]
		public void TurnOn() {
			foreach(var ent in lightings) {
				if( ent is SpotLightEntity spot )
					spot.TurnOn();
				if ( ent is PointLightEntity point )
					point.TurnOn();
			}
		}
		[Input]
		public void TurnOff() {
			foreach ( var ent in lightings ) {
				if ( ent is SpotLightEntity spot )
					spot.TurnOff();
				if ( ent is PointLightEntity point )
					point.TurnOff();
			}
		}

		[Event.Tick.Client]
		private void ClientTick() {
			Client cl = Local.Client;
			float dist = 128.0f;

			if ( tag == null && cl.Pawn.Transform.Position.Distance( Position ) <= dist ) {
				tag = new Nametag( this, description );
			}
			if ( tag != null && cl.Pawn.Transform.Position.Distance( Position ) > dist ) {
				tag.Delete();
				tag = null;
			}
		}
	}
}
