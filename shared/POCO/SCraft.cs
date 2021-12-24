using charleroi.client;
using charleroi.server.DAL;
using Sandbox;
using System.Collections.Generic;

namespace charleroi
{
	[Library]
	public interface SCraft : IStorable
	{
		public new ulong Id { get; set; }
		public CItem Item { get; set; }
		public string Description { get; set; }
		public IList<CItem> Ingredients { get; set; }
		public int Level { get; set; }
	}

	[Library]
	public class ListOfSCraft : List<CCraft>
	{
	}
}
