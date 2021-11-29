using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace charleroi.client.UI.Inventory
{
	partial class CInventory : Panel
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
			var client = Local.Pawn as CPlayer;

			DeleteChildren( true );
			cItems.Clear();
			
			foreach ( var item in Game.Instance.Items ) {
				CItemQuantity qt = new CItemQuantity { Item = item, Quantity = 2 };
				Log.Info( "adding item: " + item.Name  + " qt: " + qt.Item.Name );

				cItems.Add( new CInventoryItem( this, qt) );
				if ( cItems.Count >= MaxItem )
						break;
			}
			
			while( cItems.Count < MaxItem ) {
				cItems.Add( new CInventoryItem( this, null ) );
			}
		}

		[ClientRpc]
		public static void Refresh() {
			if( Instance != null )
				Instance.Initialize();
		}
	}
}
