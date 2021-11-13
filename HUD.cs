using charleroi.UI;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi
{
	public class HUD : Sandbox.HudEntity<RootPanel> {
		public HUD() {
			if ( IsClient ) {
				RootPanel.StyleSheet.Load( "/HUD.scss" );
				RootPanel.AddChild<CPlayerHUD>();
				RootPanel.AddChild<CPlayerMenu>();
			}
		}
	}
}
