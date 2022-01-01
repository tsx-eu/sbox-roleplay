using charleroi.client.UI;
using charleroi.client.WorldUI;
using Sandbox;
using System;
using System.Collections.Generic;

namespace charleroi.client
{
	public partial class CPlayer : Player, SPlayer
	{
		public ulong SteamID { get { return Id; } set { Id = value; } }
		public ulong Id { get; set; }
		[Net, Local]	public float Thirst { get; set; }
		[Net, Local]	public float Hunger { get; set; }
		[Net, Local]	public string Job { get; set; }
		[Net, Local]	public string Clothes { get; set; }
		[Net, Local]    public float CurrentXP { get; set; }
		[Net, Local]    public float CurrentItemCount { get; set; }

		//New version of Items in Bag
		// [Net]
		// public IList<CItemQuantity> ItemsBag { get; set; } = new List<CItemQuantity>();

		public Clothing.Container Clothing = new();

		public CPlayer() {

		}

		public void Load(SPlayer data) {
			foreach ( var childProp in typeof( SPlayer ).GetProperties() ) {
				var parentProp = typeof( CPlayer ).GetProperty( childProp.Name );
				var childValue = childProp.GetValue( data, null );

				Log.Info( String.Format( "{0}={1}", childProp.Name, childValue ) );

				parentProp.SetValue( this, childValue, null );
			}
		}

		public override void Respawn()
		{
			Clothing.LoadFromClient( base.Client );

			SetModel( "models/citizen/citizen.vmdl" );
			Clothing.DressEntity( this );
			Clothes = Clothing.Serialize();

			Controller = new WalkController();
			Animator = new PlayerAnimator();
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;


			//New way of Items in Bag
			/*
			ItemsBag = new List<CItemQuantity>() {
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Pomme rouge", ShortDescription = "Une pink lady" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Diamant", ShortDescription = "Vaut son pesant d'or" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Redbull", ShortDescription = "te donne des ailes" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Caillou", ShortDescription = "Est une forme de lythothérapie, si lancé très fort sur la tête de quelqu'un" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Cuivre", ShortDescription = "On s'en sert principalement pour faire des cables" } },
			};*/


			Health = 100;

			base.Respawn();
		}

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );
			SimulateActiveChild( cl, ActiveChild );

			if ( Input.Pressed( InputButton.View ) )
			{
				if ( Camera is not FirstPersonCamera )
					Camera = new FirstPersonCamera();
				else
					Camera = new ThirdPersonCamera();
			}

			if ( Input.Pressed( InputButton.Menu ) )
			{
				CPlayerMenu.Show( );
			}

			if ( IsServer ) {
				CurrentXP = (Noise.Turbulence( 1, Time.Now ) + 1.0f) / 2.0f;
				CurrentItemCount = (Noise.Turbulence( 1, Time.Now ) + 1.0f) / 2.0f;  // ItemsBag.Count / 40.0f;



				if ( Input.Down( InputButton.Attack1 ) )
				{
					var tr = Trace.Ray( cl.Pawn.EyePos, cl.Pawn.EyePos + cl.Pawn.EyeRot.Forward * 1024 )
						.UseHitboxes()
						.Ignore( cl.Pawn )
						.Run();

					if ( tr.Hit ) {

						var ent = tr.Entity;
						while ( ent != null ) {
							if ( ent is IAttackable i )
								i.OnAttack( cl.Pawn, tr.EndPos );

							ent = ent.Parent;
						}
					}
				}
			}

			TickPlayerUse();
		}

		protected override void UseFail() {
			// DO NOTHING
		}
	}

}
