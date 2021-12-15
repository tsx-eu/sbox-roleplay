using System.Collections.Generic;
using charleroi.server.DAL;
using Sandbox;

namespace charleroi
{
	[Library]
	public interface SItem : IStorable
	{
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public float MaxCarry { get; set; }
	}

	[Library]
	public class ListOfSItem : List<SItem>
	{
	}
}
