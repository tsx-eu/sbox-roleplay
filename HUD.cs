using charleroi.client.UI;
using Sandbox.UI;

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
