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
			if ( client.ItemsBag != null ) {
				foreach ( var item in client.ItemsBag ) {
					cItems.Add( new CInventoryItem( this, item ) );
				}
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
