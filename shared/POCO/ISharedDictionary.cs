using System.Collections.Generic;

namespace charleroi.shared.POCO
{
	public interface ISharedDictionary<T>
	{
		public static IDictionary<ulong, T> Dictionnary { get; set; }
	}
}
