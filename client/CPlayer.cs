using charleroi;
using charleroi.server.POCO;
using Sandbox;
using System;
using System.Collections.Generic;

namespace charleroi
{
	partial class CPlayer : Player
	{
		[Net]
		public float MaxHealth { get; set; } = 100.0f;
		[Net]
		public float MaxThirst { get; set; } = 100.0f;
		[Net]
		public float MaxHunger { get; set; } = 100.0f;
		[Net]
		public float Thirst { get; set; } = 100.0f;
		[Net]
		public float Hunger { get; set; } = 100.0f;

		public string Job { get; set; } = "FC Chomage";

		//New version of Items in Bag
		public IList<TupleQuantitySItem> ItemsBag { get; set; } = new List<TupleQuantitySItem>();

		public Clothing.Container Clothing = new();

		public CPlayer()
		{



		}

		public override void Respawn()
		{
			Clothing.LoadFromClient( base.Client );
			Log.Info( "respawn: " + Clothing.Clothing.Count );

			SetModel( "models/citizen/citizen.vmdl" );
			Clothing.DressEntity( this );

			Controller = new WalkController();
			Animator = new PlayerAnimator();
			Camera = new FirstPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = false;



			//New way of Items in Bag
			ItemsBag = new List<TupleQuantitySItem>()
			{
				new TupleQuantitySItem(6,new SItem(){Id = new Guid(), Name = "Pomme rouge", ShortDescription = "Une pink lady" } ),
				new TupleQuantitySItem(6,new SItem(){Id = new Guid(), Name = "Diamant", ShortDescription = "Vaut son pesant d'or" } ),
				new TupleQuantitySItem(6,new SItem(){Id = new Guid(), Name = "Redbull", ShortDescription = "te donne des ailes" } ),
				new TupleQuantitySItem(6,new SItem(){Id = new Guid(), Name = "Caillou", ShortDescription = "Est une forme de lythothérapie, si lancé très fort sur la tête de quelqu'un" } ),
				new TupleQuantitySItem(6,new SItem(){Id = new Guid(), Name = "Cuivre", ShortDescription = "On s'en sert principalement pour faire des cables" } )
			};


			Health = MaxHealth;

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

		}
	}
	public class TupleQuantitySItem
	{
		// TODO pour John : le rendre [Net] compliant
		public int Quantity { get; set; }
		public SItem Item { get; set; }

		public TupleQuantitySItem( int quantity, SItem item )
		{
			Quantity = quantity;
			Item = item;
		}
	}

}
