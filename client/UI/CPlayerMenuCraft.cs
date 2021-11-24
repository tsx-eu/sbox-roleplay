using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI
{
	public partial class CPlayerMenuCraft : Panel {

		public static CPlayerMenuCraft Instance { get; private set; }

		public CPlayerMenuCraft() {
			SetTemplate( "/client/UI/CPlayerMenuCraft.html" );
		}

		[ClientRpc]
		public static void Show()
		{
			if ( Instance == null ) {
				Instance = Local.Hud.AddChild<CPlayerMenuCraft>();
				_ = Instance.HideLayer();
			}
		}

		public async Task HideLayer() {
			await Task.DelaySeconds( 10.0f );
			CPlayerMenuCraft.Hide();
		}

		public static void Hide()
		{
			if ( Instance != null ) {
				Instance.Delete();
				Instance = null;
			}
		}
	}
}
