using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI.MenuNav
{
	class CPlayerInventory : Panel
	{
		public string Job { get; set; }

		public CPlayerInventory()
		{
			SetTemplate( "/client/UI/MenuNav/CPlayerInventory.html" );
		}

		public override void Tick()
		{
			CPlayer client = Local.Pawn as CPlayer;
			if ( client == null ) return;

			Job = client.Job;
		}

	}
}
