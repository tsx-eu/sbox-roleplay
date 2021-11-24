using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI
{
	public partial class CPlayerMenuCraft : Panel {
		public static CPlayerMenuCraft Instance { get; private set; }
		private static TimeSince LastOpen;
		private CEntityCrafttable ent;

		public CPlayerMenuCraft() {
			SetTemplate( "/client/UI/CPlayerMenuCraft.html" );
		}

		[ClientRpc]
		public static void Show(Entity ent)
		{
			if ( Instance == null && LastOpen >= 0.5f ) {
				Instance = Local.Hud.AddChild<CPlayerMenuCraft>();
				Instance.ent = ent as CEntityCrafttable;
				LastOpen = 0;
			}
		}

		public override void Tick()
		{
			base.Tick();

			if ( Input.Pressed( InputButton.Use ) && LastOpen >= 0.5f ) {
				LastOpen = 0;
				Instance.Delete();
				Instance = null;
			}
		}

	}
}
