using charleroi;
using Sandbox;

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
