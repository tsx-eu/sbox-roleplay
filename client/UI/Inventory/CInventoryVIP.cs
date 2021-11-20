using charleroi.server.POCO;
using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi.client.UI.Inventory
{
	class CInventoryVIP : Panel
	{
		private readonly IList<CItem> cItems = new List<CItem>();

		public CInventoryVIP()
		{
			cItems = new List<CItem>();


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
					cItems.Add( new CItem( TupleItem, this ) );
				}
				// Add empty slots
				for ( int i = 0; i < 16- currentPlayer.ItemsBag.Count; i++ )
				{
					cItems.Add( new CItem(this) );
				}
			}
			

		}
	}
}
