using charleroi.client;
using charleroi.client.UI;
using charleroi.client.WorldUI;
using Sandbox;
using System;
using System.Collections.Generic;

namespace charleroi
{
	[Library( "tsx_crafttable" )]
	partial class CEntityCrafttable : Prop, IUse
	{
		public static class Type {
			public static string none = "models/tsx/table_craft3.vmdl";
		}
		private IList<Entity> lightings { get; set; } = new List<Entity>();

		[Net]
		public string description { get; set; } = "Table de craft V3";
		public string model = Type.none;
		public Color color = new Color( 255, 200, 32 );
		public float brightness = 0.8f;
		private Nametag tag = null;

		public CEntityCrafttable()  {
			
		}

		public override void Spawn() {
			SetModel( model );
			Tags.Add( "show" );

			for(int i=0; i<8; i++) {
				Transform? pos;
				pos = GetAttachment( "spotlight" + (i > 0 ? i : "") );
				if ( pos.HasValue ) {
					var light = new SpotLightEntity();
					light.Transform = pos.Value;
					light.SetParent( this );
					light.SetLightBrightness( brightness );
					light.SetLightColor( color );
					light.Range = (Math.Abs( CollisionBounds.Mins.z ) + Math.Abs( CollisionBounds.Maxs.z )) * 2;
					lightings.Add( light );
				}

				pos = GetAttachment( "light" + (i > 0 ? i : "") );
				if ( pos.HasValue ) {
					var light = new PointLightEntity();
					light.Transform = pos.Value;
					light.SetParent( this );
					light.SetLightBrightness( brightness );
					light.SetLightColor( color );
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
		public virtual bool OnUse( Entity user ) {
			Host.AssertServer();
			var player = user as CPlayer;

			CPlayerMenuCraft.Show( To.Single(user), this );
			return false;
		}
		public virtual bool IsUsable( Entity user ) {
			return user is Player;
		}

		[Event.Tick.Client]
		private void ClientTick() {
			Client cl = Local.Client;
			float dist = 128.0f;

			if ( tag == null && cl.Pawn.Transform.Position.Distance( Position ) <= dist ) {
				tag = new CraftableTAG( this, description );

				foreach ( var item in CItem.Dictionnary ) {
					Log.Info( item.Value.Name );
				}


				Log.Info( "CItem.Dictionnary has " + CItem.Dictionnary.Count );
			}
			if ( tag != null && cl.Pawn.Transform.Position.Distance( Position ) > dist ) {
				tag.Delete();
				tag = null;
			}
		}
	}
}
