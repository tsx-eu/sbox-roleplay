using charleroi;
using Sandbox;
using System.Collections.Generic;

namespace Charleroi
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

		public struct ItemsInBag {
			string item;
			int quantity;

			public ItemsInBag(string aItem, int aQuantity) {
				item = aItem;
				quantity = aQuantity;
			}
		};

		public List<ItemsInBag> Items = new List<ItemsInBag>();

		public Clothing.Container Clothing = new();

		public CPlayer() {
		}

		public override void Respawn() {
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

			Items.Clear();
			Items.Add( new ItemsInBag( "Pomme", 42 ) );
			Items.Add( new ItemsInBag( "Diamant", 1 ) );
			Items.Add( new ItemsInBag( "Fruit", 9999 ) );
			Items.Add( new ItemsInBag( "Fer", 10 ) );
			if ( Rand.Int(0, 1) == 1 )
				Items.Add( new ItemsInBag( "Poire", 1 ) );
			if ( Rand.Int( 0, 1 ) == 1 )
				Items.Add( new ItemsInBag( "Pèche", 2 ) );
			if ( Rand.Int( 0, 1 ) == 1 )
				Items.Add( new ItemsInBag( "Pierre", 5 ) );
			if ( Rand.Int( 0, 1 ) == 1 )
				Items.Add( new ItemsInBag( "Cuivre", 3 ) );


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

}
