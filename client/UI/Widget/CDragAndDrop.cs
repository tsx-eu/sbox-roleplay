using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi.client.UI
{
	class CDragAndDrop : Panel
	{
		Panel fake;
		private bool isDragging;

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

		public void OnDrag()
		{
			isDragging = true;
			fake = Local.Hud.AddChild<Panel>();
		}

		public void OnDrop() {
			isDragging = false;
			fake.Delete();
		}




		public override void Tick()
		{
			base.Tick();

			if( isDragging ) {
				var pos = Local.Hud.MousePosition;
				pos = Local.Hud.PanelPositionToScreenPosition( pos );
				
				fake.Style.Position = PositionMode.Absolute;
				fake.Style.Width = Length.Pixels( Box.ClipRect.width );
				fake.Style.Height = Length.Pixels( Box.ClipRect.height );
				fake.Style.Top = Length.Pixels( pos.y );
				fake.Style.Left = Length.Pixels( pos.x );
				fake.Style.BackgroundColor = Color.Red;
				fake.Style.ZIndex = 99999999;
			}

		}
	}
}
