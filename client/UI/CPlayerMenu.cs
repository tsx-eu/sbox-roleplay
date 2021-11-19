using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;

namespace charleroi.UI
{
	public partial class CPlayerMenu : Panel
	{
		private bool IsOpen = false;
		private TimeSince LastOpen;
		private List<(Panel, Panel)> Pages = new();
		private int ActivePage = -1;
		private Panel NavigationPanel;

		public CPlayerMenu()
		{
			StyleSheet.Load( "/client/ui/CPlayerMenu.scss" );

			Panel menuPanel = Add.Panel( "menu" );
			NavigationPanel = menuPanel.Add.Panel( "navbar" );

			Panel mainArea = menuPanel.Add.Panel( "mainarea" );
			// Pages
			Panel homePage = mainArea.Add.Panel( "page" );
			homePage.Add.Label( "Page persso + inventaire" );
			AddPage( homePage, "Inventaire", new Color( 1f, 0.3f, 0.3f ) );

			Panel jobsPage = mainArea.Add.Panel( "page" );
			jobsPage.Add.Label( "Récap métier + LvL" );
			AddPage( jobsPage, "Métiers", new Color( 1f, 0.3f, 0.3f ) );

			Panel specilityPage = mainArea.Add.Panel( "page" );
			specilityPage.Add.Label( "Récap compétence secondaire + craft" );
			AddPage( specilityPage, "Compétences", new Color( 1f, 0.3f, 0.3f ) );

			Panel familyPage = mainArea.Add.Panel( "page" );
			familyPage.Add.Label( "Page de gang" );
			AddPage( familyPage, "Famille", new Color( 1f, 0.3f, 0.3f ) );

			Panel shopPage = mainArea.Add.Panel( "page" );
			shopPage.Add.Label( "Page du shop" );
			AddPage( shopPage, "Boutique", new Color( 1f, 0.3f, 0.3f ) );

			Panel mailPage = mainArea.Add.Panel( "page" );
			mailPage.Add.Label( "Mail / sms ?" );
			AddPage( mailPage, "Courriers", new Color( 1f, 0.3f, 0.3f ) );

			Panel optionPage = mainArea.Add.Panel( "page" );
			optionPage.Add.Label( "Réglage mais quoi ?" );
			AddPage( optionPage, "Options", new Color( 1f, 0.3f, 0.3f ) );
		}

		private void AddPage( Panel panel, string name, Color buttonColor )
		{
			int pageKey = Pages.Count;

			Panel button = NavigationPanel.Add.Label( name, "navbutton" );
			button.Style.BorderBottomColor = buttonColor;
			button.AddEventListener( "onclick", () => {
				SetActivePage( pageKey );
			} );

			Pages.Add( (panel, button) );

			if ( Pages.Count <= 1 )
			{
				SetActivePage( pageKey );
			}
		}

		private void SetActivePage( int pageKey )
		{
			if ( ActivePage >= 0 )
			{
				(Panel, Panel) activeInfo = Pages[ActivePage];
				activeInfo.Item1.SetClass( "active", false );
				activeInfo.Item2.SetClass( "active", false );
			}

			ActivePage = pageKey;

			(Panel, Panel) pageInfo = Pages[pageKey];
			pageInfo.Item1.SetClass( "active", true );
			pageInfo.Item2.SetClass( "active", true );
		}

		public override void Tick()
		{
			base.Tick();

			if ( Input.Pressed( InputButton.Menu ) && LastOpen >= .1f )
			{
				IsOpen = !IsOpen;
				LastOpen = 0;
			}

			// IsOpen = true;

			SetClass( "open", IsOpen );
		}
	}
}
