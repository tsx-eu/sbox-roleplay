using System;

namespace charleroi
{
	[Serializable]
	public class SItem
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public float MaxCarry { get; private set; } = 64;
	}
}
