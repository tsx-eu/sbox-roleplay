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
		[Net] public int Fork { get; set; } = 1;


		public CTree() {
			logs = new List<CTreeLog>();
		}


		public override void Spawn() {
			CreateMesh();
		}

		private void CreateMesh() {
			Host.AssertServer();

			logs?.ForEach( i => i.Delete() );


			var builder = new CTreeBuilder( this );
			logs = builder.Build();
			logs.ForEach( i => i.Spawn() );
		}
	}

	class CTreeBuilder {
		private readonly CTree config;
		private readonly float sliceHeight;
		private Vector3 lastPos;
		private Vector3 firstDirection;
		private Vector3 lastDirection;
		private int fork;
		private int start;
		private int stop;

		public CTreeBuilder( CTree root ) {
			config = root;
			sliceHeight = root.Size.z / root.Slice;
			fork = root.Fork;
			start = 0;
			stop = root.Slice;
			lastPos = root.Position - Vector3.Up * sliceHeight;
			firstDirection = Vector3.Zero;
			lastDirection = Vector3.Zero;
		}

		public List<CTreeLog> Build() {
			var logs = new List<CTreeLog>();


			for ( int i = start; i < stop; i++ ) {

				if ( fork > 0 && i >= config.Slice / 2 )
				{
					var f = Fork( i );
					logs.AddRange( f.Build() );

					var d = f.firstDirection;
					d.x = -d.x;
					d.y = -d.y;
					d.z = 0f;
					d = d / d.Length * config.Delta * 2f;
					d.z = Rand.Float( -config.Delta, config.Delta );
					logs.Add( AddLayer( i, d ) );
				}
				else
				{
					logs.Add( AddLayer( i ) );
				}

			}

			return logs;
		}

		private CTreeBuilder Fork(int position) {
			var builder = new CTreeBuilder( config );
			builder.lastPos = lastPos;
			builder.lastDirection = lastDirection;
			builder.fork = (--fork)/2;
			builder.start = position;
			builder.stop = stop--;
			return builder;
		}

		private CTreeLog AddLayer( int i, Vector3? direction = null ) {
			float a = MathX.LerpTo( 1, config.Ratio, (float)(i + 0) / config.Slice );
			float b = MathX.LerpTo( 1, config.Ratio, (float)(i + 1) / config.Slice );

			if ( direction.HasValue == false ) {
				var d = new Vector3( Rand.Float( -1, 1), Rand.Float( -1, 1), 0 );
				d = d / d.Length * config.Delta;
				d.x = (lastDirection.x + d.x) / 2.0f;
				d.y = (lastDirection.y + d.y) / 2.0f;
				d.z = Rand.Float( -config.Delta, config.Delta );
				direction = d;
			}

			if ( i == start )
				firstDirection = direction.Value;

			var log = new CTreeLog();
			log.SrcSize = new Vector2( config.Size.x * a, config.Size.y * a );
			log.DstSize = new Vector2( config.Size.x * b, config.Size.y * b );
			log.Height = sliceHeight;
			log.Position = lastPos + lastDirection + Vector3.Up * sliceHeight;
			log.Slice = 12;
			log.Direction = direction.Value;

			if ( i == config.Slice - 1 )
				log.TopCap = true;
			if ( i == 0 )
				log.BotCap = true;

			lastPos = log.Position;
			lastDirection = direction.Value;

			return log;
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
			if ( Height == 0 || SrcSize.x + SrcSize.y == 0 || DstSize.x + DstSize.y == 0 || Slice <= 3 || Direction.LengthSquared == 0 )
				return;

			var mesh = BuildMesh();
			var collider = BuildCollider();

			var model = Model.Builder
				.AddMesh( mesh )
				.AddCollisionHull( collider )
				.Create();

			SetModel( model );
			SetupPhysicsFromModel( PhysicsMotionType.Static );
			EnableAllCollisions = true;
		}

		private Mesh BuildMesh( ) {
			var mesh = new Mesh( Material.Load( "materials/dev/reflectivity_30.vmat" ) );

			int tesselation = Slice;
			if ( tesselation <= 0 || tesselation > 32 )
				tesselation = 12;

			var verts = new List<SimpleVertex>();
			var indices = new List<int>();

			for ( int i = 0; i <= tesselation; i++ ) {
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

			return mesh;
		}
		private Vector3[] BuildCollider() {
			var vert = new List<Vector3>();

			vert.Add( new Vector3( -SrcSize.x / 2, -SrcSize.y / 2, 0 ) );
			vert.Add( new Vector3( +SrcSize.x / 2, -SrcSize.y / 2, 0 ) );
			vert.Add( new Vector3( +SrcSize.x / 2, +SrcSize.y / 2, 0 ) );
			vert.Add( new Vector3( -SrcSize.x / 2, +SrcSize.y / 2, 0 ) );

			vert.Add( new Vector3( -DstSize.x / 2 + Direction.x, -DstSize.y / 2 + Direction.y, Height + Direction.z ) );
			vert.Add( new Vector3( +DstSize.x / 2 + Direction.x, -DstSize.y / 2 + Direction.y, Height + Direction.z ) );
			vert.Add( new Vector3( +DstSize.x / 2 + Direction.x, +DstSize.y / 2 + Direction.y, Height + Direction.z ) );
			vert.Add( new Vector3( -DstSize.x / 2 + Direction.x, +DstSize.y / 2 + Direction.y, Height + Direction.z ) );

			return vert.ToArray();
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
