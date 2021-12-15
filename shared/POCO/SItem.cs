using System.Collections.Generic;
using Sandbox;

namespace charleroi
{
	[Library]
	public interface SItem
	{
		public ulong Id { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public float MaxCarry { get; set; }
	}

	[Library]
	public class ListOfSItem : List<SItem>
	{
	}
}
