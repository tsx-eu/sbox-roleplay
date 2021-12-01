using System;
using charleroi.client.UI.Inventory;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi.client.UI
{
	class CDragAndDrop : Panel
	{
		Panel fake;
		private bool isDragging;
		private Vector2 pos;

		public CDragAndDrop()
		{
			AddEventListener( "ondragselect", () => {
				if ( !isDragging ) {
					OnDrag();
				}
			} );
		}

		protected override void OnMouseUp( MousePanelEvent e ) {
			if ( isDragging ) {
				OnDrop();
			}

			base.OnMouseUp( e );
		}

		public void OnDrag() {
			isDragging = true;
			fake = Clone( this );

			Local.Hud.AddChild( fake );

		}

		public void OnDrop() {
			isDragging = false;
			fake.Delete();
		}

		private Panel Clone(Panel item) {
			Panel p;
			if ( item is Label i1 ) {
				Label q = new Label();
				q.Text = i1.Text;
				p = q;
			}
			else if ( item is CInventoryItem )
				p = new CInventoryItem();
			else
				p = new Panel();

			p.Classes = item.Classes;
			foreach ( var i in item.Children ) {
				p.AddChild( Clone( i ) );
			}

			return p;
		}

		public override void Tick() {
			base.Tick();
			 
			if( isDragging && fake != null ) {
				pos = Local.Hud.ScreenPositionToPanelPosition( Mouse.Position );

				fake.Style.PixelSnap = 1;
				fake.Style.Position = PositionMode.Absolute;
				fake.Style.Width = Box.ClipRect.width;
				fake.Style.Height = Box.ClipRect.height;
				fake.Style.Top = pos.y * Local.Hud.ScaleFromScreen;
				fake.Style.Left = pos.x * Local.Hud.ScaleFromScreen;
				fake.Style.ZIndex = 99999999;
			}
		}
	}
}
