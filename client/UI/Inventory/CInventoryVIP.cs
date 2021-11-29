using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace charleroi.client.UI.Inventory
{
	class CInventoryVIP : Panel
	{
		private readonly IList<CInventoryItem> cItems = new List<CInventoryItem>();
		private static int MaxItem = 20;

		public CInventoryVIP()
		{
			cItems = new List<CInventoryItem>();


		}

		public override void Tick()
		{
			base.Tick();


			PopulateInventory();

		}

		private void PopulateInventory()
		{
			while ( cItems.Count < MaxItem )
			{
				cItems.Add( new CInventoryItem( this, null ) );
			}
		}
	}
}
