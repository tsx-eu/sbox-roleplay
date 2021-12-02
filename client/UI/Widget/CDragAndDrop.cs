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

		private TimeSince lastMouseUp;
		private TimeSince lastMouseDown;
		private Vector2 lastMouseDownPosition;
		private Vector2 delta;

		public CDragAndDrop() {

			AddEventListener( "onmousedown", () => {
				if ( !isDragging && lastMouseUp > 0.1 && lastMouseDown > 0.1 )
					OnDrag();
				lastMouseDown = 0;
			} );

			Local.Hud.AddEventListener( "onmouseup", () => {
				if ( isDragging )
					OnDrop();
				lastMouseUp = 0;
			} );
		}

		public void OnDrag() {
			isDragging = true;
			delta = MousePosition;

			fake = Clone( this );
			fake.Style.Position = PositionMode.Absolute;
			fake.Style.Width = Box.Rect.width;
			fake.Style.Height = Box.Rect.height;
			fake.Style.ZIndex = 99999999;
			fake.Style.Opacity = (Style.Opacity.HasValue ? Style.Opacity : 1.0f) * 0.75f;

			OnMouse();

			Local.Hud.AddChild( fake );
		}

		public void OnDrop() {
			fake.Delete(true);
			fake = null;
			isDragging = false;
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

		private void OnMouse() {
			var pos = Local.Hud.ScreenPositionToPanelPosition( Mouse.Position ) - delta;

			fake.Style.Top = pos.y * Local.Hud.ScaleFromScreen;
			fake.Style.Left = pos.x * Local.Hud.ScaleFromScreen;
		}

		public override void Tick() {
			base.Tick();
			 
			if( isDragging && fake != null ) {
				OnMouse();
			}
		}
	}
}
