using charleroi.client.WorldUI;
using Sandbox;
using System;

namespace charleroi.client
{
	public partial class CItem : SItem, IUse
	{
		public ulong Id { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public float MaxCarry { get; set; }

		public bool IsUsable( Entity user ) {
			return true;
		}

		public bool OnUse( Entity user ) {
			return false;
		}
	}


	public class CItemQuantity
	{
		public int Quantity { get; set; }
		public CItem Item { get; set; }

		public CItemQuantity(int qt, CItem it) {
			Quantity = qt;
			Item = it;
		}
	}
}
