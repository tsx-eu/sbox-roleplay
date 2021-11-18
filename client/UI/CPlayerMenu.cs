using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;


namespace charleroi.UI
{
	class CPlayerMenu : Panel
	{
		IDictionary<string,Panel> tabs;
		string currentTab;

		public CPlayerMenu() {
			tabs = new Dictionary<string,Panel>();

			tabs.Add( "main", AddChild<CPlayerMenuMain>() );
			tabs.Add( "craft", AddChild<CPlayerMenuCraft>() );

			foreach(var t in tabs) {
				t.Value.Style.Display = DisplayMode.None;
			}
			changeTab();

			StyleSheet.Parse( "/client/UI/CPlayerMenu.scss" );
			SetClass( "hidden", true );
		}

		public void changeTab(string newTab = "main") {
			if ( newTab == currentTab )
				return;

			if( !tabs.ContainsKey(newTab) ) {
				Log.Error( String.Format( "tabs {0} doesn't exist", newTab ) );
				return;
			}

			tabs[currentTab].Style.Display = DisplayMode.None;
			tabs[newTab].Style.Display = DisplayMode.Flex;
			currentTab = newTab;
		}
		public override void Tick() {
			base.Tick();

			if ( Input.Pressed( InputButton.Menu ) ) {
				var avatar = Parent.ChildrenOfType<CPlayerHUD>().SingleOrDefault();
				bool status = HasClass( "hidden" );

				Style.PointerEvents = status ? "" : "all";
				AcceptsFocus = !status;
				SetClass( "hidden", !status );
				avatar.SetClass( "hidden", status );
			}

			if( Input.Pressed( InputButton.Slot1 ) )
				changeTab( "main" );

			if ( Input.Pressed( InputButton.Slot2 ) )
				changeTab( "craft" );

			if ( Input.Pressed( InputButton.Slot3 ) )
				changeTab( "bite" );
		}
	}


}
