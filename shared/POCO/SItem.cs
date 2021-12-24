using System.Collections.Generic;
using charleroi.client;
using charleroi.server.DAL;
using Sandbox;

namespace charleroi
{
	[Library]
	public interface SItem : IStorable
	{
		public new ulong Id { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public string ImageURL { get; set; }
		public float MaxCarry { get; set; }
	}

	[Library]
	public class ListOfSItem : List<CItem>
	{
	}
}
