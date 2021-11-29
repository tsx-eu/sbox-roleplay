using System.Collections.Generic;
using charleroi.shared.POCO;

namespace charleroi
{
	public interface SItem : ISharedDictionary<SItem>
	{
		public ulong Id { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public float MaxCarry { get; set; }
	}
}
