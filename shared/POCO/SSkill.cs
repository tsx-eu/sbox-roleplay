using charleroi.client;
using charleroi.server.DAL;
using Sandbox;
using System.Collections.Generic;

namespace charleroi
{
	[Library]
	public interface SSkill : IStorable
	{
		public new ulong Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageURL { get; set; }
	}

	[Library]
	public class ListOfSSkill : List<CSkill>
	{
	}
}
