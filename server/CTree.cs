using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace charleroi.server
{
	public partial class CTree : Entity
	{
		public IList<CTreeLog> logs { get; set; }
		[Net] public float Ratio { get; set; } = 1.0f;
		[Net] public int slice { get; set; } = 8;
		[Net] public Vector3 Size { get; set; }

		public CTree() {
			logs = new List<CTreeLog>();
		}


		public override void Spawn() {
			CreateMesh();
		}

		private void CreateMesh() {
			Host.AssertServer();

			if ( logs != null ) {
				foreach ( var l in logs )
					l.Delete();
				logs.Clear();
			}
			else {
				logs = new List<CTreeLog>();
			}

			float sliceHeight = Size.z / slice;
			float sliceRatio = MathX.LerpTo( 1, Ratio, (float)1 / slice );

			for (int i=0; i<slice;i++)
			{
				float r = MathX.LerpTo( 1, Ratio, (float)(i + 0) / slice );

				var log = new CTreeLog();
				log.Size = new Vector3( Size.x*r, Size.y*r, sliceHeight );
				log.Ratio = sliceRatio;
				log.Position = new Vector3(Position.x, Position.y, Position.z + i * sliceHeight );
				log.Cap = false;
				log.Slice = 12;

				if ( i + 1 == slice || i == 0 )
					log.Cap = true;
				log.Spawn();

				logs.Add(log);
			}
		}
	}

	public partial class CTreeLog : ModelEntity
	{
		[Net, Change( nameof( CreateMesh ) )] public float Ratio { get; set; } = 1.0f;
		[Net, Change( nameof( CreateMesh ) )] public bool Cap { get; set; } = false;
		[Net, Change( nameof( CreateMesh ) )] public int Slice { get; set; } = 12;
		[Net, Change( nameof( CreateMesh ) )] public Vector3 Size { get; set; }

		public CTreeLog() {
			
		}

		public override void Spawn() {
			CreateMesh();
		}

		public override void ClientSpawn() {
			CreateMesh();
		}

		private void CreateMesh() {
			var mesh = new Mesh( Material.Load( "materials/dev/reflectivity_30.vmat" ) );
			BuildMesh( mesh );

			var model = Model.Builder
				.AddMesh( mesh )
				.AddCollisionCapsule( Vector3.Zero, Vector3.Up * Size.z, Size.x/2)
				.Create();

			SetModel( model );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			EnableAllCollisions = true;
		}

		private void BuildMesh( Mesh mesh ) {
			var height = Size.z; // /2
			int tesselation = Slice;
			if ( tesselation <= 0 || tesselation > 32 )
				tesselation = 12;

			var verts = new List<SimpleVertex>();
			var indices = new List<int>();

			for ( int i = 0; i <= tesselation; i++ )
			{
				var normal = GetCircleVector( i, tesselation );
				var texCoord = new Vector2( (float)i / (float)tesselation, 0.0f );

				var pos = normal + Vector3.Up * height;
				pos.x *= (Size.x / 2 * Ratio);
				pos.y *= (Size.y / 2 * Ratio);
				GetTangentBinormal( normal, out Vector3 u, out Vector3 v );

				verts.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = texCoord
				} );

				pos = normal + Vector3.Zero * height; // Vector3.Down
				pos.x *= Size.x / 2;
				pos.y *= Size.y / 2;
				texCoord.y = 1.0f;
				verts.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = texCoord
				} );
			}

			for ( int i = 0; i < tesselation; i++ ) {
				indices.Add( i * 2 );
				indices.Add( i * 2 + 1 );
				indices.Add( i * 2 + 2 );

				indices.Add( i * 2 + 1 );
				indices.Add( i * 2 + 3 );
				indices.Add( i * 2 + 2 );
			}

			if ( Cap ) {
				CreateCap( tesselation, height, Vector3.Up, indices, verts, Ratio );
				CreateCap( tesselation, height, Vector3.Zero, indices, verts, 1.0f );
			}

			mesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
			mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );


		}
		private void CreateCap( int tesselation, float height, Vector3 normal, List<int> indices, List<SimpleVertex> verts, float ratio ) {
			for ( int i = 0; i < tesselation - 2; i++ ) {
				if ( normal.z > 0 ) {
					indices.Add( verts.Count );
					indices.Add( verts.Count + (i + 1) % tesselation );
					indices.Add( verts.Count + (i + 2) % tesselation );
				}
				else {
					indices.Add( verts.Count );
					indices.Add( verts.Count + (i + 2) % tesselation );
					indices.Add( verts.Count + (i + 1) % tesselation );
				}
			}

			// Create cap vertices.
			for ( int i = 0; i < tesselation; i++ ) {
				var pos = GetCircleVector( i, tesselation ) + normal * height;
				pos.x *= Size.x / 2 * ratio;
				pos.y *= Size.y / 2 * ratio;
				GetTangentBinormal( normal, out Vector3 u, out Vector3 v );

				verts.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = Planar( (Position + pos) / 32, u, v )
				} );
			}
		}

		private static Vector3 GetCircleVector( int i, int tessellation ) {
			var angle = i * (float)Math.PI * 2 / tessellation;

			var dx = (float)Math.Cos( angle );
			var dy = (float)Math.Sin( angle );

			var v = new Vector3( dx, dy, 0 );
			return v.Normal;
		}
		private static Vector2 Planar( Vector3 pos, Vector3 uAxis, Vector3 vAxis ) {
			return new Vector2() {
				x = Vector3.Dot( uAxis, pos ),
				y = Vector3.Dot( vAxis, pos )
			};
		}
		private static void GetTangentBinormal( Vector3 normal, out Vector3 tangent, out Vector3 binormal ) {
			var t1 = Vector3.Cross( normal, Vector3.Forward );
			var t2 = Vector3.Cross( normal, Vector3.Up );
			if ( t1.Length > t2.Length ) {
				tangent = t1;
			}
			else {
				tangent = t2;
			}
			binormal = Vector3.Cross( normal, tangent ).Normal;
		}
	}
}
