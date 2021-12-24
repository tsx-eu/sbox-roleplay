using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace charleroi.server
{
	public partial class CTree : Entity
	{
		public List<CTreeLog> logs { get; set; }
		[Net] public float Ratio { get; set; } = 1.0f;
		[Net] public int Slice { get; set; } = 8;
		[Net] public float Delta { get; set; } = 8.0f;
		[Net] public Vector3 Size { get; set; }

		public CTree() {
			logs = new List<CTreeLog>();
		}


		public override void Spawn() {
			CreateMesh();
		}

		private void CreateMesh() {
			Host.AssertServer();

			logs?.ForEach( i => i.Delete() );
			logs = new List<CTreeLog>();

			float sliceHeight = Size.z / Slice;
			var lastPos = Position - Vector3.Up * sliceHeight;
			var lastDelta = Vector3.Zero;

			for (int i=0; i< Slice; i++)
			{
				float a = MathX.LerpTo( 1, Ratio, (float)(i + 0) / Slice );
				float b = MathX.LerpTo( 1, Ratio, (float)(i + 1) / Slice );
				var delta = new Vector3( Rand.Float(-Delta, Delta), Rand.Float( -Delta, Delta ), 0.0f );

				var log = new CTreeLog();
				log.SrcSize = new Vector2( Size.x * a, Size.y * a );
				log.DstSize = new Vector2( Size.x * b, Size.y * b );
				log.Height = sliceHeight;
				log.Position = lastPos + lastDelta + Vector3.Up * sliceHeight;
				log.Slice = 12;
				log.Direction = delta;

				if ( i == Slice-1 )
					log.TopCap = true;
				if ( i == 0 )
					log.BotCap = true;

				log.Spawn();
				logs.Add( log );

				lastPos = log.Position;
				lastDelta = delta;
			}
		}
	}

	public partial class CTreeLog : ModelEntity
	{
		[Net, Change( nameof( CreateMesh ) )] public bool TopCap { get; set; } = false;
		[Net, Change( nameof( CreateMesh ) )] public bool BotCap { get; set; } = false;
		[Net, Change( nameof( CreateMesh ) )] public int Slice { get; set; } = 12;
		[Net, Change( nameof( CreateMesh ) )] public float Height { get; set; } = 32.0f;

		[Net, Change( nameof( CreateMesh ) )] public Vector2 SrcSize { get; set; }
		[Net, Change( nameof( CreateMesh ) )] public Vector2 DstSize { get; set; }

		[Net, Change( nameof( CreateMesh ) )] public Vector3 Direction { get; set; } = Vector3.Up;

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
				.Create();

			SetModel( model );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			EnableAllCollisions = true;
		}

		private void BuildMesh( Mesh mesh ) {
			int tesselation = Slice;
			if ( tesselation <= 0 || tesselation > 32 )
				tesselation = 12;

			var verts = new List<SimpleVertex>();
			var indices = new List<int>();

			for ( int i = 0; i <= tesselation; i++ )
			{
				var normal = GetCircleVector( i, tesselation );
				var texCoord = new Vector2( (float)i / tesselation, 0.0f );

				var pos = normal + Vector3.Up * Height;
				pos.x *= DstSize.x / 2;
				pos.y *= DstSize.y / 2;
				pos += Direction;
				GetTangentBinormal( normal, out Vector3 u, out Vector3 v );

				verts.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = texCoord
				} );

				pos = normal + Vector3.Zero * Height; // Vector3.Down
				pos.x *= SrcSize.x / 2;
				pos.y *= SrcSize.y / 2;
				texCoord.y = 1.0f;
				verts.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = v, // u ?
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

			if ( TopCap ) 
				CreateCap( tesselation, Height, Vector3.Up, indices, verts, DstSize, Direction );
			if( BotCap )
				CreateCap( tesselation, Height, Vector3.Zero, indices, verts, SrcSize, Vector3.Zero );

			mesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
			mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );


		}
		private void CreateCap( int tesselation, float height, Vector3 normal, List<int> indices, List<SimpleVertex> verts, Vector2 size, Vector3 direction ) {
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
				pos.x *= size.x / 2;
				pos.y *= size.y / 2;
				pos += direction;
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
