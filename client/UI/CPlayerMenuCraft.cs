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

		public CPlayerMenuCraft() : base() {
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

	class CPlayerCraftList : Panel
	{
		public CPlayerCraftList()								// <CPlayerCraftList>, si das le HTML y'a déjà une classe, il le récup automiquement.
		{
			var player = Local.Client.Pawn as CPlayer;

			DeleteChildren( true );
			foreach ( var craft in Game.Instance.Crafts )
			{
				var panel = AddChild<Panel>( "Craft" );             // <div class="Craft">
				if ( craft.Level > player.CurrentXP )
					panel.AddClass( "nolvl" );

				var img = panel.AddChild<Panel>( "img" );               // <div class="img"></div>
				if ( craft.Level > player.CurrentXP )
					img.AddChild<Label>( "lvl" ).Text = craft.Level.ToString();

				var item = panel.AddChild<Panel>( "item" );				// <div class="item">

				var name = item.AddChild<Panel>( "name" );					// <div class="name">
				var labelName = name.AddChild<Label>( );					// <label>
				labelName.Text = craft.Name;								// Acide citrique 2

				var description = item.AddChild<Panel>( "description" );	// <div class="description">
				var labelDescription = description.AddChild<Label>();		// <label>
				labelDescription.Text = craft.Description;                  // L'acide citrique est un acide tricarboxylique
			}
		}

	}
}
