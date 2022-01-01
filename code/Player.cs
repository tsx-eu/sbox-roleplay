using Sandbox;
using Voxels;

namespace VoxelTest
{
	partial class Player : Sandbox.Player
	{
		public override void Respawn()
		{
			SetModel( "models/citizen/citizen.vmdl" );

			Controller = new WalkController();
			Animator = new StandardPlayerAnimator();
			Camera = new ThirdPersonCamera();

			EnableAllCollisions = true;
			EnableDrawing = true;
			EnableHideInFirstPerson = true;
			EnableShadowInFirstPerson = true;

			base.Respawn();
		}

		public override void OnKilled()
		{
			base.OnKilled();

			Controller = null;
			Animator = null;
			Camera = null;

			EnableAllCollisions = false;
			EnableDrawing = false;
		}

		public TimeSince LastEdit { get; private set; }

		public override void Simulate( Client cl )
		{
			base.Simulate( cl );

			if ( !IsServer )
				return;

			if ( LastEdit > 1f / 60f && (Input.Down( InputButton.Attack1 ) || Input.Down( InputButton.Attack2 )) )
			{
				var voxels = Game.Current.GetOrCreateVoxelVolume();
				var pos = EyePos + EyeRot.Forward * 128f;
				var transform = Matrix.CreateTranslation( pos );

				if ( Input.Down( InputButton.Attack1 ) )
				{
					var shape = new SphereSdf( Vector3.Zero, 8f, 32f );
					voxels.Add( shape, transform, 0 );
				}
				else
				{
					var shape = new SphereSdf( Vector3.Zero, 8f, 64f );
					voxels.Subtract( shape, transform, 0 );
				}
			}

			LastEdit = 0f;

			if ( Input.Pressed( InputButton.Flashlight ) )
			{
				var r = Input.Rotation;
				var ent = new Prop
				{
					Position = EyePos + r.Forward * 50,
					Rotation = r
				};

				ent.SetModel( "models/citizen_props/crate01.vmdl" );
				ent.Velocity = r.Forward * 1000;
			}
		}
	}
}
