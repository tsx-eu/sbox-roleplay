using System;
using System.Collections.Generic;
using System.Linq;
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

		public override void Spawn()
		{
			base.Spawn();

			mins = CollisionBounds.Mins;
			maxs = CollisionBounds.Maxs;

			if ( Voxels == null )
			{
				shape = new BBoxSdf( Vector3.One * -size / 2, Vector3.One * size / 2, size );
				//				shape = new SphereSdf( Vector3.Zero, size, size );
				Voxels = new OurVoxelVolume( new Vector3( 32_768f, 32_768f, 32_768f ), 256f, 4, NormalStyle.Smooth );
				Voxels.SetParent( this );
			}
			dig = new HashSet<string>();

			Transmit = TransmitType.Always;

			Build();
		}

		private Vector3 AlignPositionToGrid( Vector3 pos )
		{
			var tmp = pos.SnapToGrid( size*2 );

			if ( MathF.Abs( tmp.x ) < 0.001f )
				tmp.x = 0;
			if ( MathF.Abs( tmp.y ) < 0.001f )
				tmp.y = 0;
			if ( MathF.Abs( tmp.z ) < 0.001f )
				tmp.z = 0;

			return tmp;
		}

		[Input]
		public void Build()
		{
			Host.AssertServer();
			Voxels.Clear();
			_ = BuildAsync();
		}
		public async Task<bool> BuildAsync()
		{
			int cpt = 0;

			for ( float x = mins.x; x <= maxs.x; x += size )
			{
				for ( float y = mins.y; y <= maxs.y; y += size )
				{
					var pos = AlignPositionToGrid( Position + new Vector3( x, y, maxs.z ) );
					var hash = pos.ToString();

					if ( dig.Contains( hash ) )
						continue;

					Voxels.Add( shape, Matrix.CreateTranslation( pos ), 0 );
					cpt++;

					if ( cpt >= 32 )
					{
						cpt = 0;
						await Task.Delay( 1 );
					}
				}
			}
			return true;
		}

		public void OnAttack( Entity user, Vector3 hitpos, Vector3 normal )
		{
			Host.AssertServer();

			if ( lastAttack <= 0.1f )
				return;
			lastAttack = 0f;

			var dst = AlignPositionToGrid( hitpos - normal * size);

			var scale = new Vector3( size, size, size );

			dig.Add( dst.ToString() );
			Voxels.Subtract( shape, Matrix.CreateTranslation( dst ), 0 );

			for ( int x = -2; x <= 2; x++ )
			{
				for ( int y = -2; y <= 2; y++ )
				{
					for ( int z = -2; z <= 2; z++ )
					{
						var pos = AlignPositionToGrid( dst + new Vector3( x * size, y * size, z * size ) );

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

			Voxels.Subtract( shape, Matrix.CreateTranslation( dst ), 0 );

		}
	}

	public partial class OurVoxelVolume : VoxelVolume
	{
		public OurVoxelVolume() : base() { }

		public OurVoxelVolume( Vector3 size, float chunkSize, int chunkSubdivisions = 4, NormalStyle normalStyle = NormalStyle.Smooth )
			: base ( size, chunkSize, chunkSubdivisions, normalStyle )
		{
		}

		protected override Voxels.VoxelChunk GetOrCreateChunk( Vector3i index3 )
		{
			if ( _chunks.TryGetValue( index3, out var chunk ) ) return chunk;

			_chunks.Add( index3, chunk = new OurVoxelChunk( new ArrayVoxelData( ChunkSubdivisions, NormalStyle ), ChunkSize ) );

			chunk.Name = $"Chunk {index3.x} {index3.y} {index3.z}";

			chunk.SetParent( this );
			chunk.LocalPosition = _chunkOffset + (Vector3)index3 * ChunkSize;

			return chunk;
		}
	}
	public partial class OurVoxelChunk : Voxels.VoxelChunk
	{
		public OurVoxelChunk() : base() { }

		public OurVoxelChunk( ArrayVoxelData data, float size )
			: base( data, size )
		{
		}

		public override void UpdateMesh( bool render, bool collision )
		{
			var writer = MarchingCubesMeshWriter.Rent();

			writer.Scale = Size;

			try
			{
				Data.UpdateMesh( writer, 0, render, collision );

				if ( render )
				{
					if ( writer.Vertices.Count == 0 )
					{
						EnableDrawing = false;
						EnableShadowCasting = false;

						SetModel( "" );
					}
					else
					{
						if ( _mesh == null )
						{
							var material = Material.Load( "materials/dev/reflectivity_30.vmat" );

							_mesh = new Mesh( material )
							{
								Bounds = new BBox( 0f, Size )
							};
						}

						if ( _mesh.HasVertexBuffer )
						{
							_mesh.SetVertexBufferSize( writer.Vertices.Count );
							_mesh.SetVertexBufferData( writer.Vertices );
						}
						else
						{
							_mesh.CreateVertexBuffer( writer.Vertices.Count, VoxelVertex.Layout, writer.Vertices );
						}

						_mesh.SetVertexRange( 0, writer.Vertices.Count );

						if ( _model == null )
						{
							var modelBuilder = new ModelBuilder();

							modelBuilder.AddMesh( _mesh );

							_model = modelBuilder.Create();
						}

						EnableDrawing = true;
						EnableShadowCasting = true;

						SetModel( _model );
					}
				}

				if ( collision )
				{
					if ( writer.CollisionVertices.Count == 0 )
					{
						if ( PhysicsBody != null && PhysicsBody.IsValid() && PhysicsBody.ShapeCount > 0 )
						{
							PhysicsBody.RemoveShape( PhysicsBody.Shapes.First(), false );
						}
					}
					else
					{
						if ( PhysicsBody == null || !PhysicsBody.IsValid() )
						{
							// Just to initialize PhysicsBody
							SetupPhysicsFromAABB( PhysicsMotionType.Static, 0f, Size );
						}

						if ( PhysicsBody.ShapeCount > 0 )
						{
							PhysicsBody.RemoveShape( PhysicsBody.Shapes.First(), false );
						}

						PhysicsBody.AddMeshShape( writer.CollisionVertices.ToArray(), writer.CollisionIndices.ToArray() );
					}
				}
			}
			finally
			{
				writer.Return();
			}
		}
	}
}
