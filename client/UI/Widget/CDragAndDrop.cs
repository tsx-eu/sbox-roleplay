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
			fake = Local.Hud.AddChild<Panel>();
			CloneTo( this, fake);
		}

		public void OnDrop() {
			isDragging = false;
			fake.Delete();
		}

		private void CloneTo(Panel src, Panel dst) {
			dst.Style.BackgroundColor = Color.Red;
			dst.Style.BackgroundColor = src.Style.BackgroundColor;

			foreach ( Panel item in src.Children ) {
				var p = dst.Add.Panel();
				dst.AddChild( p );

				CloneTo( item, p );
			}
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
