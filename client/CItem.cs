using System;
using System.Collections.Generic;
using Sandbox;

namespace charleroi.client
{
	public partial class CItem : BaseNetworkable, SItem, IUse
	{
		[Net]
		public ulong Id { get; set; }
		[Net]
		public string Name { get; set; }
		[Net]
		public string ShortDescription { get; set; }
		[Net]
		public float MaxCarry { get; set; }

		[Net]
		public static IDictionary<ulong, CItem> Dictionnary { get; set; } = new Dictionary<ulong, CItem>();

		public bool IsUsable( Entity user ) {
			return true;
		}

		public bool OnUse( Entity user ) {
			return false;
		}
	}


	public partial class CItemQuantity : BaseNetworkable
	{
		[Net]
		public int Quantity { get; set; } = 0;

		[Net]
		public CItem Item { get; set; } = null;
	}
}
