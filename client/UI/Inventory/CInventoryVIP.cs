using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace charleroi.client.UI.Inventory
{
	class CInventoryVIP : Panel
	{
		private readonly IList<CInventoryItem> cItems = new List<CInventoryItem>();

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
			var currentPlayer = (CPlayer)Local.Pawn;



			if ( currentPlayer.ItemsBag == null || currentPlayer.ItemsBag.Count == 0 )
			{

			}

			if ( cItems.Count == 0 )
			{
				//Add occupied slots
				foreach ( var TupleItem in currentPlayer.ItemsBag )
				{
					cItems.Add( new CInventoryItem( this, TupleItem ) );
				}
				// Add empty slots
				for ( int i = 0; i < 16- currentPlayer.ItemsBag.Count; i++ )
				{
					cItems.Add( new CInventoryItem(this, null) );
				}
			}
			

		}
	}
}
