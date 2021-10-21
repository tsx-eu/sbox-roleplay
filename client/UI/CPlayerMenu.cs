using charleroi;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace Charleroi.UI
{
	class CPlayerMenu : Panel {
		public CPlayerMenu() {
			SetTemplate( "/client/UI/CPlayerMenu.html" );
		}

		public override void Tick() {
			base.Tick();

			if ( Input.Pressed( InputButton.Menu ) ) {
				var avatar = Parent.ChildrenOfType<CPlayerHUD>().SingleOrDefault();
				bool status = HasClass( "hidden" );

				if ( status ) {
					SetClass( "hidden", !status );
					avatar?.SetClass( "hidden", status );
				}
				else {
					SetClass( "hidden", !status );
					avatar?.SetClass( "hidden", status );
				}
			}
		}
	}
}
