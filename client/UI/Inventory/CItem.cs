using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi.client.UI.Inventory
{
	class CItem : Panel
	{
		private Label _Quantity;
		private Label _Name;
		private Label _ShortDescription;
		public Entity Entity { get; set; }

		private TupleQuantitySItem _TupleItem;

		public CItem( Panel parent )
		{
			Parent = parent;
			
			_Name = Add.Label( "empty", "item-name" );
		}

		public CItem( TupleQuantitySItem tupleItem, Panel parent )
		{
			Parent = parent;

			this.Style.BackgroundImage = Texture.Load( "https://www.coteaux-nantais.com/sites/coteaux-nantais.com/files/braeburn.png" );
			this.Style.BackgroundSizeX = Length.Percent( 95.0f );
			this.Style.BackgroundSizeY = Length.Percent( 95.0f );

			

			_TupleItem = tupleItem;
			_Name = Add.Label( tupleItem.Item.Name, "item-name" );
			_Quantity = Add.Label( tupleItem.Quantity.ToString(), "item-quantity" );
			_ShortDescription = Add.Label( tupleItem.Item.ShortDescription, "item-description" );

			this.AddEventListener( "onmouseover", () => {
				_Name.Style.Display = DisplayMode.None;
				_Quantity.Style.Display = DisplayMode.None;
				_ShortDescription.Style.Display = DisplayMode.Flex;
			} );

			this.AddEventListener( "onmouseout", () =>
			{
				_Name.Style.Display = DisplayMode.Flex;
				_Quantity.Style.Display = DisplayMode.Flex;
				_ShortDescription.Style.Display = DisplayMode.None;
			} );

			this.AddEventListener( "onclick", () =>
			{
				tupleItem.Quantity = tupleItem.Quantity == 0 ? 0 : tupleItem.Quantity - 2;
				_Quantity.Text = tupleItem.Quantity.ToString();
			} );
		}


	}
}
