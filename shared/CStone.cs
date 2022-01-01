using System.Collections.Generic;
using System.Threading.Tasks;
using charleroi.client;
using charleroi.shared;
using Sandbox;
using Voxels;

namespace charleroi.server
{
	[Library( "tsx_stone" )]
	[Hammer.Solid]
	[Hammer.AutoApplyMaterial()]
	public partial class CStone : ModelEntity, IAttackable
	{
		private Vector3 mins, maxs;
		private ISignedDistanceField shape;
		private float size = 32f;

		[Net] public VoxelVolume Voxels { get; private set; }

		public override void Spawn() {
			base.Spawn();

			mins = CollisionBounds.Mins;
			maxs = CollisionBounds.Maxs;

			if ( Voxels == null ) {
				shape = new BBoxSdf( new Vector3( -size, -size, -size ) / 4, new Vector3( size, size, size ) / 4, size );
				Voxels = new VoxelVolume( new Vector3( 32_768f, 32_768f, 32_768f ), 256f, 4, NormalStyle.Smooth ); // NormalStyle.Smooth
				Voxels.SetParent( this );
			}

			Build();
		}

		[Input]
		public void Build() {
			Host.AssertServer();
			Voxels.Clear();
			_ = BuildAsync();
		}
		public async Task<bool> BuildAsync() {
			int cpt = 0;

			for ( float x = mins.x; x <= maxs.x; x += size ) {
				for ( float y = mins.y; y <= maxs.y; y += size ) {
					Voxels.Add( shape, Matrix.CreateTranslation( Position + new Vector3( x, y, maxs.z ) ), 0 );
					cpt++;

					if ( cpt >= 32 ) {
						cpt = 0;
						await Task.Delay( 1 );
					}
				}
			}
			return true;
		}

		public void OnAttack( Entity user, Vector3 hitpos ) {
			Host.AssertServer();

			for ( int x = -1; x <= 1; x++ )
			{
				for ( int y = -1; y <= 1; y++ )
				{
					for ( int z = -1; z <= 1; z++ )
					{
						var pos = hitpos + new Vector3( size * x, size * y, size * z - size/2 );

						if ( x == 0 && y == 0 && z == 0 ) {
							Voxels.Subtract( shape, Matrix.CreateTranslation( pos ), 0 );
							continue;
						}

						if ( pos.z >= Position.z + maxs.z )
							continue; 

						//Voxels.Add( shape, Matrix.CreateTranslation( pos ), 0 );
					}
				}
			}
		}
	}
}
