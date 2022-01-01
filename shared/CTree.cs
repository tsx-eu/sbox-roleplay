using System;
using System.Collections.Generic;
using System.Linq;
using charleroi.client;
using charleroi.shared;
using Sandbox;

namespace charleroi.server
{
	[Library( "tsx_tree" )]
	public partial class CTree : ModelEntity, IUse
	{
		[Net] public IList<CTreePart> logs { get; set; }
		private TimeSince startMoving;
		private float lastZ;

		[Net] public float Ratio { get; set; } = 0.25f;
		[Net] public int Slice { get; set; } = 8;
		[Net] public float Delta { get; set; } = 8.0f;
		[Net, Property] public Vector3 Size { get; set; }
		[Net, Property] public int Fork { get; set; } = 1;

		[Input]
		public void Build() {
			Host.AssertServer();
			logs?.ToList().ForEach( i => i.Delete() );


			List<List<Vector3>> colliders = new ();

			var builder = new CTreeBuilder( this );
			logs = builder.Build();
			logs?.ToList().ForEach( i => {
				i.Build();
				if ( i.lastCollider != null && i.lastCollider.Length > 0 ) {
					List<Vector3> collider = new List<Vector3>();
					foreach ( var c in i.lastCollider )
						collider.Add( c + i.Position - Position );
					colliders.Add( collider );
				}
			} );

			PhysicsClear();
			PhysicsEnabled = false;
			if ( colliders.Count > 0 ) {
				var model = Model.Builder;
				foreach(var collider in colliders )
					model.AddCollisionHull(collider.ToArray());
				model.WithMass( 1 );

				SetModel( model.Create() );

				SetupPhysicsFromModel( PhysicsMotionType.Static, true );
			}
		}

		public override void ClientSpawn()
		{
			//SceneObject.Flags.NeedsLightProbe = false;
			base.ClientSpawn();
		}

		[Input]
		public void Wake()
		{
			startMoving = 0;
			PhysicsEnabled = true;
			EnableAllCollisions = true;
			UsePhysicsCollision = true;
		}

		[Input]
		public void Break()
		{
			PhysicsClear();
			PhysicsEnabled = false;
			logs?.ToList().ForEach( i => i.Wake() );
		}


		[Event.Tick]
		protected void OnTick() {
			if ( IsServer && PhysicsEnabled ) {
				var z = Math.Abs( Rotation.Up.z );
				var deltaZ = Math.Abs( lastZ - z );
				lastZ = z;

				if ( z < 0.1f || startMoving > 10.0f || (startMoving > 1.0f && deltaZ < 0.1f ) )
					Break();
			}
		}

		public virtual bool OnUse( Entity user ) {
			Wake();
			Velocity = (Vector3.Up + Vector3.Random) * 1024.0f;
			return false;
		}
		public virtual bool IsUsable( Entity user ) {
			return user is CPlayer;
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
					Height = sliceHeight,
					Fork = 2,
					Parent = log
				};
				ret.Add( stick );
			}


			lastPos = log.Position;
			lastDirection = direction.Value;

			return ret;
		}
	}

	public partial class CTreePart : AnimEntity, IUse
	{
		public List<Mesh> lastMeshes { get; private set; }
		public Vector3[] lastCollider { get; private set; }
		[Net, Change( nameof( CreateMesh ) )] public int Slice { get; set; } = 6;
		[Net, Change( nameof( CreateMesh ) )] public float Height { get; set; } = 32.0f;
		[Net, Change( nameof( CreateMesh ) )] public Vector3 Direction { get; set; } = Vector3.Up;

		public override void Spawn() {
			base.Spawn();
			//CreateMesh();
		}

		public override void ClientSpawn() {
			base.ClientSpawn();
			//CreateMesh();
		}

		public virtual bool OnUse( Entity user ) {
			Host.AssertServer();
			Wake();
			return false;
		}
		public virtual bool IsUsable( Entity user ) {
			if ( Parent != null )
				return false;
			return user is CPlayer;
		}

		[Input]
		public void Build() {
			CreateMesh();
		}

		[Input]
		public virtual void Wake() {
			SetParent( null );
			PhysicsEnabled = true;
			EnableAllCollisions = true;
			UsePhysicsCollision = true;
		}

		protected virtual bool HasValidProperties() {
			if ( Height == 0 || Slice <= 2 || Slice > 32 || Direction.LengthSquared == 0 )
				return false;
			return true;
		}

		protected virtual void CreateMesh() {
			if ( !HasValidProperties() )
				return;

			lastMeshes = BuildMesh();
			lastCollider = BuildCollider();

			var builder = Model.Builder;
			foreach(var mesh in lastMeshes )
				builder.AddMesh( mesh );
			if ( lastCollider != null )
				builder.AddCollisionHull( lastCollider );


			var model = builder.Create();
			SetModel( model );

			PhysicsEnabled = false;
			if ( lastCollider != null )
				SetupPhysicsFromModel( PhysicsMotionType.Dynamic, true );
		}
		protected virtual List<Mesh> BuildMesh() { return null; }
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

	public partial class CTreeStick : CTreePart
	{
		[Net, Change( nameof( CreateMesh ) )] public int Iteration { get; set; } = 1;
		[Net, Change( nameof( CreateMesh ) )] public int Fork { get; set; } = 1;

		protected override bool HasValidProperties() {
			if ( !base.HasValidProperties() )
				return false;
			if ( Iteration <= 0 || Fork <= 0 )
				return false;
			return true;
		}

		[Input]
		public override void Wake() {
			Delete();
		}

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

		private void CreateLeaf( ProceduralMesh mesh, List<Vector3> positions, List<Vector3> directions, int index )
		{
			GetTangentBinormal( Vector3.Left, out Vector3 u, out Vector3 v );
			float scale = 32.0f;
			Vector2 size = new Vector2( 2048, 2048 );

			for ( float f = 0.0f; f < 1.0f; f += 0.25f ) {
				for ( int d = 0; d < 360; d += 90 ) {
					var dist = Vector3.Lerp( positions[index], positions[index + 1], f);

					var tiles = Tiles();
					var tile = tiles[Rand.Int( 0, tiles.Count - 1 )];

					Vector2 min = tile[0];
					Vector2 attach = Vector2.Lerp( tile[0], tile[tile.Count - 1], 0.5f );
					Vector2 direction = Vector2.Lerp( tile[tile.Count / 2], tile[tile.Count / 2 + 1], 0.5f ) - attach;

					foreach ( var t in tile ) {
						if ( t.x < min.x )
							min.x = t.x;
						if ( t.y < min.y )
							min.y = t.y;
					}

					var rot1 = Rotation.LookAt( new Vector3( direction.x, 0, direction.y ).Normal );
					var rot2 = Rotation.LookAt( directions[index + 1] );
					var rot3 = Rotation.FromRoll( d + Rand.Float(0f, 45f) );

					var pivot = new Vector3( (attach.x - min.x) / scale, 0, -(attach.y - min.y) / scale );

					int s = mesh.Count;
					foreach ( var p in tile ) {
						var pos = new Vector3( (p.x - min.x) / scale, 0, -(p.y - min.y) / scale );
						pos = (pos - pivot) * rot1 * rot2 * rot3;
						pos += dist;

						mesh.Add( new SimpleVertex() {
							normal = Vector3.Left,
							position = pos,
							tangent = u,
							texcoord = new Vector2( p.x / size.x, p.y / size.y )
						} );
					}
					for ( int i = 0; i <= tile.Count / 3; i++ ) {
						mesh.Add( s + i + 0 );
						mesh.Add( s + i + 1 );
						mesh.Add( s + tile.Count - 2 - i );

						mesh.Add( s + tile.Count - 2 - i );
						mesh.Add( s + tile.Count - 1 - i );
						mesh.Add( s + i + 0 );
					}
				}
			}
		}
		private void CreateStick( ProceduralMesh mesh, List<Vector3> positions, List<Vector3> directions, int index, Vector2 srcSize, Vector2 dstSize)
		{
			int s = mesh.Count;

			for ( int i = 0; i <= Slice; i++ ) {
				var normal = GetCircleNormal( i, Slice );
				var texCoord = new Vector2( (float)i / Slice, 0.0f );

				var pos = normal;
				pos.x *= dstSize.x / 2;
				pos.y *= dstSize.y / 2;
				pos += Vector3.Up * Height;
				pos *= Rotation.LookAt( directions[index + 1] );
				pos += positions[index];

				GetTangentBinormal( normal, out Vector3 u, out Vector3 v );

				mesh.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = texCoord
				} );

				pos = normal;
				pos.x *= srcSize.x / 2;
				pos.y *= srcSize.y / 2;
				pos += Vector3.Up * 0;
				pos *= Rotation.LookAt( directions[index + 0] );
				pos += positions[index];

				texCoord.y = 1.0f;
				mesh.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = v, // u ?
					texcoord = texCoord
				} );
			}

			for ( int i = 0; i < Slice; i++ ) {
				mesh.Add( s + i * 2 );
				mesh.Add( s + i * 2 + 1 );
				mesh.Add( s + i * 2 + 2 );

				mesh.Add( s + i * 2 + 1 );
				mesh.Add( s + i * 2 + 3 );
				mesh.Add( s + i * 2 + 2 );
			}
		}

		protected void Build( ProceduralMesh stick, ProceduralMesh leaf, Vector3 startPosition, Vector2 Size, int length, int fork) {
			if ( length <= 0 )
				return;
			if ( Size.Length <= 1f )
				return;

			for ( int j = 0; j < fork; j++ ) {
				var pos = new List<Vector3>();
				var dir = new List<Vector3>();

				pos.Add( startPosition );
				dir.Add( Vector3.Random.Normal );

				for ( int i = 0; i < length + 1; i++ ) {
					dir.Add( Vector3.Lerp( dir[i], Vector3.Random.Normal, 0.25f ).Normal );
					pos.Add( pos[i] + (Vector3.Up * Height) * Rotation.LookAt( dir[i+1] ) );
				}


				for ( int i = 0; i < length; i++ ) {
					float a = MathX.LerpTo( 1, 0.0f, (float)(i + 0) / length );
					float b = MathX.LerpTo( 1, 0.0f, (float)(i + 1) / length );

					CreateStick( stick, pos, dir, i, Size * a, Size * b);
					CreateLeaf( leaf, pos, dir, i );

					if ( i >= length/2 )
						Build( stick, leaf, pos[i], Size*(a+b)/2, length-1, 1);
				}
			}
		}

		protected override List<Mesh> BuildMesh() {
			var stick = new ProceduralMesh( Material.Load( "models/sbox_props/trees/oak/oak_bark.vmat" ) );
			var leaf = new ProceduralMesh( Material.Load( "models/sbox_props/trees/oak/oak_branch.vmat" ) );

			var Size = new Vector2( 4, 4 );

			Build( stick, leaf, Vector3.Zero, Size, Iteration, Fork );

			var mesh = new List<Mesh>();
			mesh.Add( stick.Build() );
			mesh.Add( leaf.Build() );

			return mesh;
		}
	}

	public partial class CTreeLog : CTreePart {
		[Net, Change( nameof( CreateMesh ) )] public Vector2 SrcSize { get; set; }
		[Net, Change( nameof( CreateMesh ) )] public Vector2 DstSize { get; set; }
		[Net, Change( nameof( CreateMesh ) )] public bool TopCap { get; set; }
		[Net, Change( nameof( CreateMesh ) )] public bool BotCap { get; set; }

		protected override bool HasValidProperties() {
			if ( !base.HasValidProperties() )
				return false;
			if ( SrcSize.x + SrcSize.y == 0 || DstSize.x + DstSize.y == 0 )
				return false;
			return true;
		}

		[Input]
		public override void Wake() {
			BotCap = TopCap = true;
			CreateMesh();
			base.Wake();
		}

		protected override List<Mesh> BuildMesh( ) {
			var mesh = new ProceduralMesh( Material.Load( "models/sbox_props/trees/oak/oak_bark.vmat" ) );
			//var mesh = new ProceduralMesh( Material.Load( "materials/dev/reflectivity_30.vmat" ) );

			for ( int i = 0; i <= Slice; i++ ) {
				var normal = GetCircleNormal( i, Slice);
				var texCoord = new Vector2( (float)i / Slice, 0.0f );

				var pos = normal + Vector3.Up * Height;
				pos.x *= DstSize.x / 2;
				pos.y *= DstSize.y / 2;
				pos += Direction;
				GetTangentBinormal( normal, out Vector3 u, out Vector3 v );

				mesh.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = texCoord
				} );

				pos = normal + Vector3.Zero * Height; // Vector3.Down
				pos.x *= SrcSize.x / 2;
				pos.y *= SrcSize.y / 2;
				texCoord.y = 1.0f;
				mesh.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = v, // u ?
					texcoord = texCoord
				} );
			}

			for ( int i = 0; i < Slice; i++ ) {
				mesh.Add( i * 2 );
				mesh.Add( i * 2 + 1 );
				mesh.Add( i * 2 + 2 );

				mesh.Add( i * 2 + 1 );
				mesh.Add( i * 2 + 3 );
				mesh.Add( i * 2 + 2 );
			}

			if ( TopCap ) 
				CreateCap( mesh, Slice, Height, Vector3.Up, DstSize, Direction );
			if( BotCap )
				CreateCap( mesh, Slice, Height, Vector3.Zero, SrcSize, Vector3.Zero );

			return new List<Mesh> { mesh.Build() };
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

		private void CreateCap( ProceduralMesh mesh, int tesselation, float height, Vector3 normal,Vector2 size, Vector3 direction ) {
			for ( int i = 0; i < tesselation - 2; i++ ) {
				if ( normal.z > 0 ) {
					mesh.Add( mesh.Count );
					mesh.Add( mesh.Count + (i + 1) % tesselation );
					mesh.Add( mesh.Count + (i + 2) % tesselation );
				}
				else {
					mesh.Add( mesh.Count );
					mesh.Add( mesh.Count + (i + 2) % tesselation );
					mesh.Add( mesh.Count + (i + 1) % tesselation );
				}
			}

			// Create cap vertices.
			for ( int i = 0; i < tesselation; i++ ) {
				var pos = GetCircleNormal( i, tesselation ) + normal * height;
				pos.x *= size.x / 2;
				pos.y *= size.y / 2;
				pos += direction;
				GetTangentBinormal( normal, out Vector3 u, out Vector3 v );

				mesh.Add( new SimpleVertex() {
					normal = normal,
					position = pos,
					tangent = u,
					texcoord = Planar( (Position + pos) / 32, u, v )
				} );
			}
		}
	}
}
