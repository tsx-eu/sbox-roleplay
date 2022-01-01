using System.Collections.Generic;
using Sandbox;

namespace charleroi.shared
{
	public class ProceduralMesh
	{
		private Mesh mesh;
		private List<SimpleVertex> verts;
		private List<int> indices;

		public int Count { get { return verts.Count; } }

		public ProceduralMesh( Material mat )
		{
			mesh = new Mesh( mat );
			verts = new List<SimpleVertex>();
			indices = new List<int>();
		}

		public void Add( SimpleVertex vertex )
		{
			verts.Add( vertex );
		}
		public void Add( int a )
		{
			indices.Add( a );
		}
		public void Add( int a, int b, int c )
		{
			indices.Add( a );
			indices.Add( b );
			indices.Add( c );
		}

		public Mesh Build()
		{
			mesh.CreateVertexBuffer<SimpleVertex>( verts.Count, SimpleVertex.Layout, verts.ToArray() );
			mesh.CreateIndexBuffer( indices.Count, indices.ToArray() );
			return mesh;
		}
	}
}
