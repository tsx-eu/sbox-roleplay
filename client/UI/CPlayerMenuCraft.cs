using System.Threading.Tasks;
using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI
{
	public partial class CPlayerMenuCraft : Panel {
		public static CPlayerMenuCraft Instance { get; private set; }
		private static TimeSince LastOpen;
		private CEntityCrafttable ent;
		private TimeSince LastRay;

		public CPlayerMenuCraft() {
			SetTemplate( "/client/UI/CPlayerMenuCraft.html" );
		}
		public override void Delete( bool immediate = false ) {
			LastOpen = 0;
			base.Delete( immediate );
			Instance = null;
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
				Instance.Delete();
				return;
			}

			if ( !ent.IsValid() ) {
				Instance.Delete();
				return;
			}

			if ( LastRay > 0.2f ) {
				LastRay = 0;
				var player = Local.Client.Pawn;
				float dist = 128.0f;

				var trace = Trace.Ray( player.EyePos, player.EyePos + player.EyeRot.Forward * dist )
					.Ignore( player )
					.EntitiesOnly()
					.Radius( 2.0f )
					.Run();

				if ( trace.Entity != ent ) {
					Instance.Delete();
					return;
				}
			}

			return;

		}

	}
}
