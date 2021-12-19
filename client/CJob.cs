using System;
using System.Collections.Generic;
using Sandbox;

namespace charleroi.client
{
	public partial class CJob : BaseNetworkable, SJob
	{
		[Net] public ulong Id { get; set; }
		[Net] public string Name { get; set; }
		[Net] public string Description { get; set; }
		[Net] public string ImageURL { get; set; }
	}

}
