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
	class CInventory : Panel
	{
		private readonly IList<CItem> cItems = new List<CItem>();

		public CInventory()
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
				currentPlayer.ItemsBag = new List<TupleQuantitySItem>()
				{
					new TupleQuantitySItem(6,new SItem(){Id = new Guid(), Name = "Pomme", ShortDescription = "Une pink lady" } ),
					new TupleQuantitySItem(1,new SItem(){Id = new Guid(), Name = "Diamant", ShortDescription = "Vaut son pesant d'or" } ),
					new TupleQuantitySItem(3,new SItem(){Id = new Guid(), Name = "Redbull", ShortDescription = "te donne des ailes" } ),
					new TupleQuantitySItem(64,new SItem(){Id = new Guid(), Name = "Caillou", ShortDescription = "Est une forme de lythothérapie, si lancé très fort sur la tête de quelqu'un" } ),
					new TupleQuantitySItem(12,new SItem(){Id = new Guid(), Name = "Cuivre", ShortDescription = "On s'en sert principalement pour faire des cables" } ),
					new TupleQuantitySItem(1,new SItem(){Id = new Guid(), Name = "Coca", ShortDescription = "toujours plus de sucre" } ),
					new TupleQuantitySItem(2,new SItem(){Id = new Guid(), Name = "Sandwich", ShortDescription = "en forme de triangle" } ),
					new TupleQuantitySItem(21,new SItem(){Id = new Guid(), Name = "Boulon", ShortDescription = "utilisé pour le craft" } ),
					new TupleQuantitySItem(10,new SItem(){Id = new Guid(), Name = "Barre de fer", ShortDescription = "utilisé pour le craft" } ),
					new TupleQuantitySItem(10,new SItem(){Id = new Guid(), Name = "Ferraille", ShortDescription = "utilisé pour le craft" } ),
					new TupleQuantitySItem(10,new SItem(){Id = new Guid(), Name = "Ecrou", ShortDescription = "utilisé pour le craft" } ),
					new TupleQuantitySItem(1,new SItem(){Id = new Guid(), Name = "Tournevis", ShortDescription = "utilisé pour le craft" } ),
					new TupleQuantitySItem(1,new SItem(){Id = new Guid(), Name = "Cutter", ShortDescription = "Coupe plutôt bien" } ),
					new TupleQuantitySItem(7,new SItem(){Id = new Guid(), Name = "Scotch", ShortDescription = "utilisé pour le craft" } )
				};
			}

			if ( cItems.Count == 0 )
			{
				//Add occupied slots
				foreach ( var TupleItem in currentPlayer.ItemsBag )
				{
					cItems.Add( new CItem( TupleItem, this ) );
				}
				// Add empty slots
				for ( int i = 0; i < 40- currentPlayer.ItemsBag.Count; i++ )
				{
					cItems.Add( new CItem(this) );
				}
			}
			

		}
	}
}
