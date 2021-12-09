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

		protected Dictionary<string, Button> Buttons;

		public CPlayerMenuBase()
		{
			StyleSheet.Load( "/client/UI/CPlayerMenu.scss" );
			Buttons = new Dictionary<string, Button>();
			Inner = Add.Panel( "inner" );


			PageList = Inner.Add.Panel( "pagelist" );
			PageContainer = Inner.Add.Panel( "pagecontainer" );
		}

		protected void AddPage( string icon, string name, Func<Panel> act = null )
		{
			var button = PageList.Add.Button( name, () => {
				SwitchPage( name );
				act?.Invoke().AddClass( "page" );
			});
			button.Icon = icon;

			Buttons[name] = button;
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
		public override void OnHotloaded()
		{
			base.OnHotloaded();
			Delete();
			Instance = null;
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
