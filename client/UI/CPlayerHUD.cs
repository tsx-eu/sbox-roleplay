using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI
{
	class CPlayerHUD : Panel {
		public string Job { get; set; }

		public CPlayerHUD() {
			SetTemplate( "/client/UI/CPlayerHUD.html" );
		}

		public override void Tick() {
			CPlayer client = Local.Pawn as CPlayer;
			if ( client == null ) return;

			Job = client.Job;
		}

	}
}
