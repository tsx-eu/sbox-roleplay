using charleroi.client.WorldUI;
using Sandbox;
using System;
using System.Collections.Generic;

namespace charleroi.client
{
	partial class CPlayer : Player, SPlayer
	{
		public ulong SteamID { get; set; }
		[Net, Local, Predicted]
		public float Thirst { get; set; }
		[Net, Local, Predicted]
		public float Hunger { get; set; }
		[Net, Local]
		public string Job { get; set; }
		[Net, Local]
		public string Clothes { get; set; }

		//New version of Items in Bag
		[Net]
		public IList<CItemQuantity> ItemsBag { get; set; } = new List<CItemQuantity>();

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
			ItemsBag = new List<CItemQuantity>() {
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Pomme rouge", ShortDescription = "Une pink lady" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Diamant", ShortDescription = "Vaut son pesant d'or" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Redbull", ShortDescription = "te donne des ailes" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Caillou", ShortDescription = "Est une forme de lythothérapie, si lancé très fort sur la tête de quelqu'un" } },
				new CItemQuantity { Quantity = 6, Item = new CItem() { Name = "Cuivre", ShortDescription = "On s'en sert principalement pour faire des cables" } },
			};


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

			TickPlayerUse();
		}
	}

}
