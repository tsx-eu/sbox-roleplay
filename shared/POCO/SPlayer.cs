using System;

namespace charleroi
{
	[Serializable]
	public class SPlayer
	{
		public ulong SteamID { get; set; }
		public float Health { get; set; }
		public float Thirst { get; set; }
		public float Hunger { get; set; }
	}
}
