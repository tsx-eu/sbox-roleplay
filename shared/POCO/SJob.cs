using Sandbox;
using System.Collections.Generic;

namespace charleroi
{
	[Library]
	public interface SJob : IStorable
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageURL { get; set; }
	}


	[Library]
	public class ListOfSJob : List<SJob>
	{
	}
}
