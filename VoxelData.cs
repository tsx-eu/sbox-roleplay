using Sandbox;
using System;
using System.Linq;

namespace Voxels
{
	public interface IVoxelData
	{
		bool Clear();
		void UpdateMesh( IVoxelMeshWriter writer, int lod, bool render, bool collision );

		bool Add<T>( T sdf, BBox bounds, Matrix transform, byte materialIndex )
			where T : ISignedDistanceField;
		bool Subtract<T>( T sdf, BBox bounds, Matrix transform, byte materialIndex )
			where T : ISignedDistanceField;
	}

	public partial class ArrayVoxelData : BaseNetworkable, IVoxelData, INetworkSerializer
	{
		public const int MaxSubdivisions = 5;

		public int Subdivisions { get; private set; }
		public NormalStyle NormalStyle { get; private set; }

		public int NetReadCount { get; private set; }

		private Voxel[] _voxels;
		private Vector3i _size;
		private Vector3i _renderedSize;
		private Vector3 _scale;

		private bool _cleared;
		private int _margin;

		public ArrayVoxelData()
		{

		}

		public ArrayVoxelData( int subdivisions, NormalStyle normalStyle )
		{
			if ( subdivisions < 0 || subdivisions > MaxSubdivisions )
			{
				throw new ArgumentOutOfRangeException( nameof(subdivisions),
					$"Expected {nameof(subdivisions)} to be between 0 and {MaxSubdivisions}." );
			}

			Init( subdivisions, normalStyle );
		}

		private void Init( int subdivisions, NormalStyle normalStyle )
		{
			Subdivisions = subdivisions;
			NormalStyle = normalStyle;

			_cleared = true;
			_margin = normalStyle == NormalStyle.Flat ? 0 : 1;

			var resolution = 1 << Subdivisions;

			_renderedSize = resolution;
			_size = _renderedSize + _margin * 2 + 1;
			_scale = 1f / resolution;
		}

		public bool Clear()
		{
			if ( _cleared || _voxels == null ) return false;

			Array.Clear( _voxels, 0, _voxels.Length );

			_cleared = true;

			return true;
		}

		public void UpdateMesh( IVoxelMeshWriter writer, int lod, bool render, bool collision )
		{
			if ( _voxels == null || _cleared || !render && !collision ) return;

			writer.Write( _voxels, _size, _margin, _size - _margin, lod, render ? NormalStyle : NormalStyle.Flat, render, collision );
		}

		private bool PrepareVoxelsForEditing( BBox bounds, out Vector3i outerMin, out Vector3i outerMax )
		{
			if ( _voxels == null )
			{
				_voxels = new Voxel[_size.x * _size.y * _size.z];
			}

			outerMin = Vector3i.Max( Vector3i.Floor( bounds.Mins * _renderedSize ) - _margin - 1, 0 );
			outerMax = Vector3i.Min( Vector3i.Ceiling( bounds.Maxs * _renderedSize ) + 2 + _margin, _size );

			return outerMin.x < outerMax.x && outerMin.y < outerMax.y && outerMin.z < outerMax.z;
		}

		public bool Add<T>( T sdf, BBox bounds, Matrix transform, byte materialIndex )
			where T : ISignedDistanceField
		{
			if ( !PrepareVoxelsForEditing( bounds, out var outerMin, out var outerMax ) )
			{
				return false;
			}

			var changed = false;

			foreach ( var (index3, index) in _size.EnumerateArray3D( outerMin, outerMax ) )
			{
				var pos = transform.Transform( (index3 - _margin) * _scale );
				var next = new Voxel( sdf[pos], materialIndex );
				var prev = _voxels[index];

				_voxels[index] = prev + next;

				changed |= prev.RawValue < 255 && next.RawValue > 0;
			}

			if ( changed ) _cleared = false;

			return changed;
		}

		public bool Subtract<T>( T sdf, BBox bounds, Matrix transform, byte materialIndex )
			where T : ISignedDistanceField
		{
			if ( !PrepareVoxelsForEditing( bounds, out var outerMin, out var outerMax ) )
			{
				return false;
			}

			var changed = false;

			foreach ( var (index3, index) in _size.EnumerateArray3D( outerMin, outerMax ) )
			{
				var pos = transform.Transform( (index3 - _margin) * _scale );
				var next = new Voxel( sdf[pos], materialIndex );
				var prev = _voxels[index];

				_voxels[index] = prev - next;

				changed |= prev.RawValue > 0 && next.RawValue > 0;
			}

			if ( changed ) _cleared = false;

			return changed;
		}

		public void Read( ref NetRead read )
		{
			var subDivs = read.Read<int>();
			var normalStyle = read.Read<NormalStyle>();

			if ( Subdivisions != subDivs || NormalStyle != normalStyle )
			{
				Init( subDivs, normalStyle );
			}

			_voxels = read.ReadUnmanagedArray( _voxels );
			_cleared = false;

			++NetReadCount;
		}

		public void Write( NetWrite write )
		{
			write.Write( Subdivisions );
			write.Write( NormalStyle );
			write.WriteUnmanagedArray( _voxels );
		}

		public override string ToString()
		{
			return $"(Subdivisions: {Subdivisions}, {(_voxels != null ? "Has Data" :  "No Data")})";
		}
	}
}
