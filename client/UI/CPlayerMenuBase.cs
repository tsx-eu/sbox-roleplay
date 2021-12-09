using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

using charleroi.client.UI.MenuNav;
using charleroi.client.UI.Inventory;

namespace charleroi.client.UI
{
	public abstract class CPlayerMenuBase : Panel
	{
		protected static CPlayerMenuBase Instance { get; set; }

		protected static TimeSince LastOpen;
		public Panel Inner { get; set; }
		public Panel PageList { get; set; }
		public Panel PageContainer { get; set; }

		private Dictionary<string, Button> Buttons;
		private Dictionary<string, string> Titles;


		public CPlayerMenuBase()
		{
			StyleSheet.Load( "/client/UI/CPlayerMenu.scss" );
			Buttons = new Dictionary<string, Button>();
			Titles = new Dictionary<string, string>();
			Inner = Add.Panel( "inner" );


			PageList = Inner.Add.Panel( "pagelist" );
			PageContainer = Inner.Add.Panel( "pagecontainer" );
		}

		protected void LoadDefaultPage() {
			Buttons.First().Value.CreateEvent( "onclick" );
		}

		protected void AddPage( string icon, string name, string title, Func<Panel> act = null )
		{
			var button = PageList.Add.Button( name, () => {
				SwitchPage( name );
				act?.Invoke().AddClass( "page" );
			});
			button.Icon = icon;

			Buttons[name] = button;
			Titles[name] = title;
		}

		protected void SwitchPage( string name )
		{
			PageContainer.DeleteChildren();

			foreach ( var button in Buttons )
			{
				button.Value.SetClass( "active", button.Key == name );
			}
		}

		[Event.Hotload]
		public static void Hotloaded()
		{
			if ( Instance != null )
				Instance.Delete();
		}

		public override void Tick() {
			base.Tick();

			if ( (Input.Pressed( InputButton.Menu )|| Input.Pressed( InputButton.Use )) && LastOpen >= 0.5f )
			{
				LastOpen = 0;
				Instance.Delete();
				Instance = null;
				return;
			}

		}
		public override void Delete( bool immediate = false )
		{
			LastOpen = 0;
			base.Delete( immediate );
			Instance = null;
		}

	}
}
