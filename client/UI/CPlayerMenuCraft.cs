using System.Linq;
using System.Threading.Tasks;
using charleroi.client.UI.MenuNav;
using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI
{
	public partial class CPlayerMenuCraft : CPlayerMenuBase {

		private CEntityCrafttable ent;
		private TimeSince LastRay;

		public CPlayerMenuCraft() : base()
		{
			AddPage( "price_change", "Craft", () => PageContainer.AddChild<CPlayerCraft>() );
			AddPage( "price_change", "Stock", () => PageContainer.AddChild<CPlayerCraftStock>() );
			AddPage( "price_change", "Autorisation", () => PageContainer.AddChild<CPlayerInventory>() );
			AddPage( "price_change", "Status", () => PageContainer.AddChild<CPlayerInventory>() );
			AddPage( "price_change", "Personnage", () => PageContainer.AddChild<CPlayerInventory>() );

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
