﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi.server.POCO
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
