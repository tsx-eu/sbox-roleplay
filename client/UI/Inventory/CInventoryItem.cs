using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi.client.UI.Inventory
{
	class CInventoryItem : Panel
	{
		private Label _Quantity;
		private Label _Name;
		private Label _ShortDescription;
		private CItemQuantity item;

		public CInventoryItem( Panel parent, CItemQuantity tupleItem = null)
		{
			Parent = parent;
			item = tupleItem;

			if ( tupleItem == null ) {
				_Name = Add.Label( "", "item-name" );
			}
			else
			{
				this.Style.BackgroundImage = Texture.Load( "https://www.coteaux-nantais.com/sites/coteaux-nantais.com/files/braeburn.png" );
				this.Style.BackgroundSizeX = Length.Percent( 95.0f );
				this.Style.BackgroundSizeY = Length.Percent( 95.0f );

				_Name = Add.Label( tupleItem.Item.Name, "item-name" ); // deplacer le nom de l'item dans le pop over avec la desc
				_Quantity = Add.Label( tupleItem.Quantity.ToString(), "item-quantity" );
				_ShortDescription = Add.Label( tupleItem.Item.ShortDescription, "item-description" );// prévoir un z-index 7

				// TODO: ajouter un delais de 2secondes avant affichage
				this.AddEventListener( "onmouseover", () => {
					_Name.Style.Display = DisplayMode.None;
					_Quantity.Style.Display = DisplayMode.None;
					_ShortDescription.Style.Display = DisplayMode.Flex;
				} );

				this.AddEventListener( "onmouseout", () => {
					_Name.Style.Display = DisplayMode.Flex;
					_Quantity.Style.Display = DisplayMode.Flex;
					_ShortDescription.Style.Display = DisplayMode.None;
				} );


				// TODO ServerRPC
				this.AddEventListener( "onclick", () => {
					tupleItem.Quantity = tupleItem.Quantity == 0 ? 0 : tupleItem.Quantity - 2;
					_Quantity.Text = tupleItem.Quantity.ToString();
				} );
			}
		}
	}
}
