using Sandbox;
using System;
using System.Collections.Generic;

namespace charleroi
{
	[Library]
	public interface SCraft
	{
		public ulong Id { get; set; }
		public SItem Item { get; set; }
		public string Description { get; set; }
		public string ImageURL { get; set; }
		public IList<SItem> Ingredients { get; set; }
		public int Level { get; set; }
	}

	[Library]
	public class ListOfSCraft : List<SCraft>
	{
	}
}
