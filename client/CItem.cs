using System;
using System.Collections.Generic;
using Sandbox;

namespace charleroi.client
{
	public partial class CItem : BaseNetworkable, SItem, IUse
	{
		public ulong Id { get; set; }
		[Net] public string Name { get; set; }
		[Net] public string ShortDescription { get; set; }
		[Net] public float MaxCarry { get; set; }

		public bool IsUsable( Entity user ) {
			return true;
		}

		public bool OnUse( Entity user ) {
			return false;
		}
	}


	public partial class CItemQuantity
	{
		public int Quantity { get; set; } = 0;
		public CItem Item { get; set; } = null;
	}
}
