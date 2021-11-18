using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace charleroi.UI
{
	class CPlayerMenuMain : Panel {
		public CPlayerMenuMain() {
			SetTemplate( "/client/UI/CPlayerMenuMain.html" );
		}

		public void changeTab()
		{
			Log.Info( "Demande pour changer son l'onglet craft" );
			(Parent as CPlayerMenu)?.changeTab("craft");
		}
	}
}
