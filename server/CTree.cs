using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace charleroi.server
{
	public partial class CTree : Entity
	{
		[Net] public IList<CTreePart> logs { get; set; }
		[Net] public float Ratio { get; set; } = 1.0f;
		[Net] public int Slice { get; set; } = 8;
		[Net] public float Delta { get; set; } = 8.0f;
		[Net] public Vector3 Size { get; set; }
		[Net] public int Fork { get; set; } = 1;

		public CTree() {
		}

		[Input]
		public void Build() {
			Host.AssertServer();
			logs?.ToList().ForEach( i => {
				i.Delete();
			});

			var builder = new CTreeBuilder( this );
			logs = builder.Build();
			logs?.ToList().ForEach( i => i.Build());
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

		public List<CTreePart> Build() {
			var logs = new List<CTreePart>();

			for ( int i = start; i < stop; i++ ) {
				if ( fork > 0 && i >= config.Slice / 2 ) { // TODO: mid entre start et stop
					var f = Fork( i );
					logs.AddRange( f.Build() );

					var d = f.firstDirection;
					d.x = -d.x;
					d.y = -d.y;
					d.z = 0f;
					d = d.Normal * config.Delta * 2f;
					d.z = Rand.Float( -config.Delta, config.Delta );
					logs.AddRange( AddLayer( i, d ) );
				}
				else {
					logs.AddRange( AddLayer( i ) );
				}
			}

			return logs;
		}

		private CTreeBuilder Fork( int position ) {
			var builder = new CTreeBuilder( config );
			builder.lastPos = lastPos;
			builder.lastDirection = lastDirection;
			builder.fork = (--fork) / 2;
			builder.start = position;
			builder.stop = stop--;
			return builder;
		}

		private List<CTreePart> AddLayer( int i, Vector3? direction = null ) {
			var ret = new List<CTreePart>();

			float a = MathX.LerpTo( 1, config.Ratio, (float)(i + 0) / config.Slice );
			float b = MathX.LerpTo( 1, config.Ratio, (float)(i + 1) / config.Slice );

			if ( direction.HasValue == false ) {
				var d = new Vector3( Rand.Float( -1, 1 ), Rand.Float( -1, 1 ), 0 ).Normal;
				d *= config.Delta;
				d.x = (lastDirection.x + d.x) / 2.0f;
				d.y = (lastDirection.y + d.y) / 2.0f;
				// Vector3.Reflect
				d.z = Rand.Float( -config.Delta, config.Delta );
				direction = d;
			}

			if ( i == start )
				firstDirection = direction.Value;

			var log = new CTreeLog() {
				SrcSize = new Vector2( config.Size.x * a, config.Size.y * a ),
				DstSize = new Vector2( config.Size.x * b, config.Size.y * b ),
				Height = sliceHeight,
				Position = lastPos + lastDirection + Vector3.Up * sliceHeight,
				Slice = 12,
				Direction = direction.Value,
			//	RenderColor = Color.Random,
				Parent = config
			};

			if ( i == stop - 1 )
				log.TopCap = true;
			if ( i == 0 )
				log.BotCap = true;

			ret.Add( log );

			if ( i > config.Slice / 2 ) {
				var stick = new CTreeStick {
					Position = log.Position,
					Direction = Vector3.Random,
					Iteration = stop-i+2,
					Height = sliceHeight/2,
					Fork = 2,
					Parent = log
				};
				ret.Add( stick );

				var leaf = new CTreeLeaf {
					Position = log.Position,
					Parent = log
				};
				ret.Add( leaf );
			}


			lastPos = log.Position;
			lastDirection = direction.Value;

			return ret;
		}
	}

	public partial class CTreePart : ModelEntity
	{
		[Net, Change( nameof( CreateMesh ) )] public int Slice { get; set; } = 6;
		[Net, Change( nameof( CreateMesh ) )] public float Height { get; set; } = 32.0f;
		[Net, Change( nameof( CreateMesh ) )] public Vector3 Direction { get; set; } = Vector3.Up;

		public override void Spawn() {
			CreateMesh();
		}

		public override void ClientSpawn() {
			CreateMesh();
		}

		[Input]
		public void Build()
		{
			CreateMesh();
		}

		protected virtual bool HasValidProperties() {
			if ( Height == 0 || Slice <= 2 || Slice > 32 || Direction.LengthSquared == 0 )
				return false;
			return true;
		}

		protected virtual void CreateMesh() {
			if ( !HasValidProperties() )
				return;

			var mesh = BuildMesh();
			var collider = BuildCollider();

			var builder = Model.Builder.AddMesh( mesh );
			if ( collider != null )
				builder.AddCollisionHull( collider );
			var model = builder.Create();

			SetModel( model );
			if ( collider != null )
				SetupPhysicsFromModel( PhysicsMotionType.Static );
			EnableAllCollisions = true;
		}
		protected virtual Mesh BuildMesh() { return null; }
		protected virtual Vector3[] BuildCollider() { return null; }

		protected static Vector3 GetCircleNormal( int i, int tessellation) {
			var angle = i * (float)Math.PI * 2 / tessellation;

			var dx = (float)Math.Cos( angle );
			var dy = (float)Math.Sin( angle );

			var v = new Vector3( dx, dy, 0 );
			return v.Normal;
		}
		protected static Vector2 Planar( Vector3 pos, Vector3 uAxis, Vector3 vAxis ) {
			return new Vector2()
			{
				x = Vector3.Dot( uAxis, pos ),
				y = Vector3.Dot( vAxis, pos )
			};
		}
		protected static void GetTangentBinormal( Vector3 normal, out Vector3 tangent, out Vector3 binormal ) {
			var t1 = Vector3.Cross( normal, Vector3.Forward );
			var t2 = Vector3.Cross( normal, Vector3.Up );
			if ( t1.Length > t2.Length )
			{
				tangent = t1;
			}
			else
			{
				tangent = t2;
			}
			binormal = Vector3.Cross( normal, tangent ).Normal;
		}
	}

	public partial class CTreeLeaf : CTreePart
	{
		private List<List<Vector2>> Tiles() {
			List<Vector2> Tile_1() {
				var pos = new List<Vector2>();
				pos.Add( new Vector2( 700, 512 ) );
				pos.Add( new Vector2( 950, 446 ) );
				pos.Add( new Vector2( 1520, 0 ) );
				pos.Add( new Vector2( 2048, 0 ) );
				pos.Add( new Vector2( 2048, 950 ) );
				pos.Add( new Vector2( 1520, 950 ) );
				pos.Add( new Vector2( 950, 632 ) );
				pos.Add( new Vector2( 700, 632 ) );
				return pos;
			}
			List<Vector2> Tile_2() {
				var pos = new List<Vector2>();
				pos.Add( new Vector2( 1214, 64 ) );
				pos.Add( new Vector2( 960, 8 ) );
				pos.Add( new Vector2( 0, 8 ) );
				pos.Add( new Vector2( 0, 700 ) );

				pos.Add( new Vector2( 440, 640 ) );
				pos.Add( new Vector2( 700, 512 ) );
				pos.Add( new Vector2( 950, 446 ) );

				pos.Add( new Vector2( 1214, 128 ) );
				return pos;
			}
			List<Vector2> Tile_3() {
				var pos = new List<Vector2>();
				pos.Add( new Vector2( 380, 2048 ) );
				pos.Add( new Vector2( 185, 1470 ) );
				pos.Add( new Vector2( 0, 1470 ) );
				pos.Add( new Vector2( 0, 1920 ) );
				pos.Add( new Vector2( 256, 2048 ) );

				return pos;
			}
			List<Vector2> Tile_4() {
				var pos = new List<Vector2>();
				pos.Add( new Vector2( 356, 1985 ) );
				pos.Add( new Vector2( 192, 1466 ) );
				pos.Add( new Vector2( 0, 1280 ) );
				pos.Add( new Vector2( 0, 700 ) );
				pos.Add( new Vector2( 440, 640 ) );
				pos.Add( new Vector2( 760, 640 ) );
				pos.Add( new Vector2( 700, 1024 ) );
				pos.Add( new Vector2( 570, 1466 ) );
				pos.Add( new Vector2( 448, 1985 ) );

				return pos;
			}
			List<Vector2> Tile_5() {
				var pos = new List<Vector2>();
				pos.Add( new Vector2( 570, 1466 ) );
				pos.Add( new Vector2( 700, 1024 ) );
				pos.Add( new Vector2( 760, 640 ) );
				pos.Add( new Vector2( 950, 632 ) );
				pos.Add( new Vector2( 1452, 900 ) );
				pos.Add( new Vector2( 700, 1466 ) );

				return pos;
			}
			List<Vector2> Tile_6() {
				var pos = new List<Vector2>();
				pos.Add( new Vector2( 650, 1600 ) );
				pos.Add( new Vector2( 1215, 1850 ) );
				pos.Add( new Vector2( 1215, 2048 ) );
				pos.Add( new Vector2( 570, 2048 ) );
				pos.Add( new Vector2( 570, 1661 ) );

				return pos;
			}
			List<Vector2> Tile_7() {
				var pos = new List<Vector2>();
				pos.Add( new Vector2( 700, 1466 ) );
				pos.Add( new Vector2( 1470, 950 ) );
				pos.Add( new Vector2( 2048, 950 ) );
				pos.Add( new Vector2( 2048, 2048 ) );
				pos.Add( new Vector2( 1215, 2048 ) );
				pos.Add( new Vector2( 1215, 1850 ) );
				pos.Add( new Vector2( 700, 1600 ) );

				return pos;
			}

			var list = new List<List<Vector2>>();
			list.Add( Tile_1() );
			list.Add( Tile_2() );
			list.Add( Tile_3() );
			list.Add( Tile_4() );
			list.Add( Tile_5() );
			list.Add( Tile_6() );
			list.Add( Tile_7() );
			return list;
		}

		private void Create( ref List<SimpleVertex> verts, ref List<int> indices )
		{
			GetTangentBinormal( Vector3.Left, out Vector3 u, out Vector3 v );

			var tiles = Tiles();
			var tile = tiles[Rand.Int( 0, tiles.Count - 1 )];

			float scale = 32.0f;
			Vector2 size = new Vector2( 2048, 2048 );

			Vector2 min = tile[0];
			Vector2 attach = Vector2.Lerp( tile[0], tile[tile.Count-1], 0.5f );
			Vector2 direction = Vector2.Lerp( tile[tile.Count / 2], tile[tile.Count / 2 + 1], 0.5f ) - attach;

			foreach ( var t in tile ) {
				if ( t.x < min.x )
					min.x = t.x;
				if ( t.y < min.y )
					min.y = t.y;
			}

			var rot = Rotation.LookAt( new Vector3( direction.x, 0, direction.y ).Normal );
			var pivot = new Vector3( (attach.x - min.x) / scale, 0, -(attach.y - min.y) / scale );

			foreach (var p in tile) {
				var pos = new Vector3( (p.x - min.x) / scale, 0, -(p.y - min.y) / scale );
				pos = (pos-pivot) * rot;

				verts.Add( new SimpleVertex() {
					normal = Vector3.Left,
					position = pos,
					tangent = u,
					texcoord = new Vector2( p.x / size.x, p.y / size.y )
				} );
			}

			for(int i=0; i<=verts.Count/3; i++) {
				indices.Add( i+0 );
				indices.Add( i+1 );
				indices.Add( verts.Count-2-i );

				indices.Add( verts.Count-2-i );
				indices.Add( verts.Count-1-i );
				indices.Add( i+0 );
			}
		}

		protected override Mesh BuildMesh()
		{
			var mesh = new Mesh( Material.Load( "models/sbox_props/trees/oak/oak_branch.vmat" ) );
			//var mesh = new Mesh( Material.Load( "models/sbox_props/trees/oak/oak_branch_dry.vmat" ) );
			//var mesh = new Mesh( Material.Load( "models/sbox_props/trees/oak/oak_branch_dead.vmat" ) );

			var verts = new List<SimpleVertex>();
			var indices = new List<int>();

			Create(ref verts, ref indices);

			mesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
			mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );

			return mesh;
		}
	}

	public partial class CTreeStick : CTreePart
	{
		[Net, Change( nameof( CreateMesh ) )] public int Iteration { get; set; } = 1;
		[Net, Change( nameof( CreateMesh ) )] public int Fork { get; set; } = 1;

		protected override bool HasValidProperties()
		{
			if ( !base.HasValidProperties() )
				return false;
			if ( Iteration <= 0 || Fork <= 0 )
				return false;
			return true;
		}

		void Create( ref List<SimpleVertex> verts, ref List<int> indices, Vector2 srcSize, Vector2 dstSize, Vector3 position, List<Vector3> directions, int dirIndex)
		{
			int s = verts.Count;

			for ( int i = 0; i <= Slice; i++ ) {
				var normal = GetCircleNormal( i, Slice );
				var texCoord = new Vector2( (float)i / Slice, 0.0f );

				var pos = normal;
				pos.x *= dstSize.x / 2;
				pos.y *= dstSize.y / 2;
				//pos *= Rotation.LookAt( directions[dirIndex + 1] );
				pos += Vector3.Up * Height;
				pos *= Rotation.LookAt( directions[dirIndex + 0] );
				pos += position;

				GetTangentBinormal( normal, out Vector3 u, out Vector3 v );

				verts.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = texCoord
				} );

				pos = normal;
				pos.x *= srcSize.x / 2;
				pos.y *= srcSize.y / 2;
				//pos *= Rotation.LookAt( directions[dirIndex + 0] );
				pos += Vector3.Up * 0;
				pos *= Rotation.LookAt( directions[dirIndex - 1] );
				pos += position;

				texCoord.y = 1.0f;
				verts.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = v, // u ?
					texcoord = texCoord
				} );
			}

			for ( int i = 0; i < Slice; i++ ) {
				indices.Add( s + i * 2 );
				indices.Add( s + i * 2 + 1 );
				indices.Add( s + i * 2 + 2 );

				indices.Add( s + i * 2 + 1 );
				indices.Add( s + i * 2 + 3 );
				indices.Add( s + i * 2 + 2 );
			}
		}

		protected void Build( ref List<SimpleVertex> verts, ref List<int> indices, Vector3 startPosition, Vector2 Size, int length, int fork) {
			if ( length <= 0 )
				return;
			if ( Size.Length <= 1f )
				return;

			for ( int j = 0; j < fork; j++ ) {
				var pos = startPosition;
				var dir = new List<Vector3>();
				dir.Add( Vector3.Random.Normal );
				for ( int i = 0; i < length + 1; i++ )
					dir.Add( Vector3.Lerp( dir[i], Vector3.Random.Normal, 0.25f ).Normal );


				for ( int i = 1; i <= length; i++ ) {
					float a = MathX.LerpTo( 1, 0.0f, (float)(i - 1) / length );
					float b = MathX.LerpTo( 1, 0.0f, (float)(i + 0) / length );

					Create( ref verts, ref indices, Size*a, Size*b, pos, dir, i );
					if( i >= length/2 )
						Build( ref verts, ref indices, pos, Size*(a+b)/2, length-1, 1);

					pos += (Vector3.Up * Height) * Rotation.LookAt( dir[i] );
				}
			}
		}

		protected override Mesh BuildMesh() {
			var mesh = new Mesh( Material.Load( "models/sbox_props/trees/oak/oak_bark.vmat" ) );
			//var mesh = new Mesh( Material.Load( "materials/dev/reflectivity_30.vmat" ) );

			var verts = new List<SimpleVertex>();
			var indices = new List<int>();
			var Size = new Vector2( 4, 4 );

			Build( ref verts, ref indices, Vector3.Zero, Size, Iteration, Fork);

			mesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
			mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );

			return mesh;
		}
	}

	public partial class CTreeLog : CTreePart {
		[Net, Change( nameof( CreateMesh ) )] public Vector2 SrcSize { get; set; }
		[Net, Change( nameof( CreateMesh ) )] public Vector2 DstSize { get; set; }
		[Net, Change( nameof( CreateMesh ) )] public bool TopCap { get; set; } = false;
		[Net, Change( nameof( CreateMesh ) )] public bool BotCap { get; set; } = false;

		public CTreeLog() {
			
		}

		protected override bool HasValidProperties() {
			if ( !base.HasValidProperties() )
				return false;
			if ( SrcSize.x + SrcSize.y == 0 || DstSize.x + DstSize.y == 0 )
				return false;
			return true;
		}

		protected override Mesh BuildMesh( ) {
			var mesh = new Mesh( Material.Load( "models/sbox_props/trees/oak/oak_bark.vmat" ) );
			//var mesh = new Mesh( Material.Load( "materials/dev/reflectivity_30.vmat" ) );

			var verts = new List<SimpleVertex>();
			var indices = new List<int>();

			for ( int i = 0; i <= Slice; i++ ) {
				var normal = GetCircleNormal( i, Slice);
				var texCoord = new Vector2( (float)i / Slice, 0.0f );

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

			for ( int i = 0; i < Slice; i++ ) {
				indices.Add( i * 2 );
				indices.Add( i * 2 + 1 );
				indices.Add( i * 2 + 2 );

				indices.Add( i * 2 + 1 );
				indices.Add( i * 2 + 3 );
				indices.Add( i * 2 + 2 );
			}

			if ( TopCap ) 
				CreateCap( Slice, Height, Vector3.Up, indices, verts, DstSize, Direction );
			if( BotCap )
				CreateCap( Slice, Height, Vector3.Zero, indices, verts, SrcSize, Vector3.Zero );

			mesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
			mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );

			return mesh;
		}
		protected override Vector3[] BuildCollider() {
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
				var pos = GetCircleNormal( i, tesselation ) + normal * height;
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
	}
}
