using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;

using charleroi.client.UI.MenuNav;
namespace charleroi.client.UI
{
	[Library]
	[NavigatorTarget( "/client/MenuNav/" )]
	public partial class CPlayerMenu : Panel
	{
		public static CPlayerMenu Instance { get; private set; }

		private bool IsOpen = false;
		private TimeSince LastOpen;
		public Panel Inner { get; set; }
		public Panel PageList { get; set; }
		public Panel PageContainer { get; set; }

		Dictionary<string, Button> Buttons;

		public CPlayerMenu()
		{
			StyleSheet.Load( "/client/UI/CPlayerMenu.scss" );
			Buttons = new Dictionary<string, Button>();
			Inner = Add.Panel( "inner" );


			PageList = Inner.Add.Panel( "pagelist" );
			PageContainer = Inner.Add.Panel( "pagecontainer" );

			AddPage( "price_change", "Inventaire", () => PageContainer.AddChild<CPlayerContent> () );
			AddPage( "card_travel", "Métiers", () => PageContainer.AddChild<CPlayerJob>() );
			AddPage( "science", "Compétences", () => PageContainer.AddChild<CPlayerSkill>() );
			AddPage( "groups", "Famille", () => PageContainer.AddChild<CPlayerFamily>() );
			AddPage( "shopping_cart", "Boutique", () => PageContainer.AddChild<CPlayerShop>() );
			AddPage( "mail_outline", "Courriers", () => PageContainer.AddChild<CPlayerMail>() );// transform en mark_email_read ou mark_email_unread si courrier ?
			AddPage( "miscellaneous_services", "Options", () => PageContainer.AddChild<CPlayerOption>() );


			Buttons.First().Value.CreateEvent( "onclick" );
			Instance = this;
		}

		void AddPage( string icon, string name, Func<Panel> act = null )
		{
			var button = PageList.Add.Button( name, () => { SwitchPage( name ); act?.Invoke().AddClass( "page" ); } );
			button.Icon = icon;

			Buttons[name] = button;
		}

		void SwitchPage( string name )
		{
			PageContainer.DeleteChildren();

			foreach ( var button in Buttons )
			{
				button.Value.SetClass( "active", button.Key == name );
			}
		}

		public override void OnHotloaded()
		{
			base.OnHotloaded();

			var activePage = Buttons.Where( x => x.Value.HasClass( "active" ) ).FirstOrDefault();
			if ( activePage.Value != null )
			{
				activePage.Value.CreateEvent( "onclick" );
			}
		}

		public override void Tick() {
			base.Tick();

			if ( Input.Pressed( InputButton.Menu ) && LastOpen >= 0.5f )
			{
				SetOpen( !IsOpen );
				LastOpen = 0;
			}

		}

		private void SetOpen(bool status) {
			IsOpen = status;
			SetClass( "open", IsOpen );
		}

		[ClientRpc]
		public static void Show( ) {
			Instance.SetOpen( true );
		}

		[ClientRpc]
		public static void Hide() {
			Instance.SetOpen( false );
		}
	}
}
