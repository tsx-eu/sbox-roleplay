using System.Linq;
using System.Threading.Tasks;
using charleroi.client.UI.MenuNav;
using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI
{
	[Library]
	[NavigatorTarget( "/client/MenuNav/" )]
	public partial class CPlayerMenuCraft : CPlayerMenuBase {

		private CEntityCrafttable ent;
		private TimeSince LastRay;

		public CPlayerMenuCraft() : base()
		{
			AddPage( "craft_change", "Craft", () => PageContainer.AddChild<CPlayerCraft>() );
			AddPage( "price_change", "Inventaire", () => PageContainer.AddChild<CPlayerInventory>() );
			AddPage( "card_travel", "Métiers", () => PageContainer.AddChild<CPlayerJob>() );


			Buttons.First().Value.CreateEvent( "onclick" );
		}


		[ClientRpc]
		public static void Show(Entity ent)
		{
			if ( Instance == null && LastOpen >= 0.5f ) {
				Instance = Local.Hud.AddChild<CPlayerMenuCraft>();
				(Instance as CPlayerMenuCraft).ent = ent as CEntityCrafttable;
				LastOpen = 0;
			}
		}

		public override void Tick()
		{
			base.Tick();


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
