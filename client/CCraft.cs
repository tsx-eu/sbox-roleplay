using System;
using System.Collections.Generic;
using Sandbox;

namespace charleroi.client
{
	public partial class CCraft : BaseNetworkable, SCraft
	{
		[Net]public ulong Id { get; set; }
		[Net]public string Name { get; set; }
		[Net]public string Description { get; set; }
		[Net] public string ImageURL { get; set; }
		[Net] public string Ingredients { get; set; }
		[Net] public int Level { get; set; }

	}
}
