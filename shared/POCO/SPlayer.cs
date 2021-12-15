using charleroi.server.DAL;
using Sandbox;
using System.Collections.Generic;

namespace charleroi
{
	[Library]
	public interface SPlayer : IStorable
	{
		public ulong SteamID { get; set; }
		public float Health { get; set; }
		public float Thirst { get; set; }
		public float Hunger { get; set; }
		public float CurrentXP { get; set; }
		public string Job { get; set; }
		public string Clothes { get; set; }
	}


	[Library]
	public class ListOfSPlayer : List<SPlayer>
	{
	}
}
