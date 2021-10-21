using Charleroi.UI;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace Charleroi
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
