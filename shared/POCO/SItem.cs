using System.Collections.Generic;

namespace charleroi
{
	public interface SItem
	{
		public ulong Id { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public float MaxCarry { get; set; }
	}
}
