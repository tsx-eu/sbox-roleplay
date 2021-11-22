using Sandbox;
using System;

namespace charleroi
{
	public interface SPlayer
	{
		public ulong SteamID { get; set; }
		public float Health { get; set; }
		public float Thirst { get; set; }
		public float Hunger { get; set; }
		public string Job { get; set; }
		public string Clothes { get; set; }
	}
}
