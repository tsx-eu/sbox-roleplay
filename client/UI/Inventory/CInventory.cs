using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace charleroi.client.UI.Inventory
{
	class CInventory : Panel
	{
		public static CInventory Instance { get; private set; }

		private readonly IList<CInventoryItem> cItems = new List<CInventoryItem>();
		private static int MaxItem = 40;
		
		public CInventory() {
			cItems = new List<CInventoryItem>();
			Initialize();

			Instance = this;
		}

		public void Initialize() {
			var client = Local.Client as CPlayer;

			cItems.Clear();
			foreach ( var item in client.ItemsBag ) {
				cItems.Add( new CInventoryItem( this, item ) );
			}

			while( cItems.Count < MaxItem ) {
				cItems.Add( new CInventoryItem( this, null ) );
			}
		}


		public static void Refresh()
		{
			Instance.Initialize();
		}


	}
}
