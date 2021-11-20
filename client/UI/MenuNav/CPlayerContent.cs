using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI.MenuNav
{
	class CPlayerContent : Panel
	{
		public string Job { get; set; }

		public CPlayerContent()
		{
			SetTemplate( "/client/UI/MenuNav/CPlayerContent.html" );
		}

		public override void Tick()
		{
			CPlayer client = Local.Pawn as CPlayer;
			if ( client == null ) return;

			Job = client.Job;
		}

	}
}
