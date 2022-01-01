using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
		private TimeSince lastAttack;
		private HashSet<string> dig;

		[Net] public VoxelVolume Voxels { get; private set; }

		public override void Spawn() {
			base.Spawn();

			mins = CollisionBounds.Mins;
			maxs = CollisionBounds.Maxs;

			if ( Voxels == null ) {
				shape = new BBoxSdf(Vector3.One * -size/2, Vector3.One * size / 2, size );
				Voxels = new VoxelVolume( new Vector3( 32_768f, 32_768f, 32_768f ), 256f, 4, NormalStyle.Smooth );
				Voxels.SetParent( this );
			}
			dig = new HashSet<string>();

			Transmit = TransmitType.Always;

			Build();
		}

		private Vector3 AlignPositionToGrid(Vector3 pos) {
			return pos.SnapToGrid( size*2 );
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
					var pos = AlignPositionToGrid( Position + new Vector3( x, y, maxs.z ) );
					var hash = pos.ToString();

					if ( dig.Contains( hash ) )
						continue;

					Voxels.Add( shape, Matrix.CreateTranslation(pos), 0 );
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

			if ( lastAttack <= 0.1f )
				return;
			lastAttack = 0f;

			var scale = new Vector3( size, size, size );
			hitpos.z -= size;


			var digPos = AlignPositionToGrid( hitpos + Vector3.Zero - (scale / 2) );
			dig.Add( digPos.ToString() );

			Voxels.Subtract( shape, Matrix.CreateTranslation( digPos ), 0 );

			for ( int x = -2; x <= 2; x++ ) {
				for ( int y = -2; y <= 2; y++ ) {
					for ( int z = -2; z <= 2; z++ ) {
						var vec = new Vector3( x, y, z ) * scale;

						var pos = AlignPositionToGrid(hitpos + vec - (scale/2));

						if ( x == 0 && y == 0 && z == 0 )
							continue;

						if ( pos.z >= Position.z + maxs.z )
							continue;

						if ( dig.Contains( pos.ToString() ) )
							continue;

						Voxels.Add( shape, Matrix.CreateTranslation( pos ), 0 );
					}
				}
			}

			Voxels.Subtract( shape, Matrix.CreateTranslation( digPos ), 0 );
		}
	}
}
