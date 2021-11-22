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

		Dictionary<string, Sandbox.UI.Button> Buttons;

		public CPlayerMenu()
		{
			StyleSheet.Load( "/client/UI/CPlayerMenu.scss" );
			Buttons = new Dictionary<string, Sandbox.UI.Button>();
			Inner = Add.Panel( "inner" );


			PageList = Inner.Add.Panel( "pagelist" );
			PageContainer = Inner.Add.Panel( "pagecontainer" );

			AddPage( "user_content", "Inventaire", () => PageContainer.AddChild<CPlayerContent> () );
			AddPage( "user_job", "Métiers", () => PageContainer.AddChild<CPlayerJob>() );
			AddPage( "user_skill", "Compétences", () => PageContainer.AddChild<CPlayerSkill>() );
			AddPage( "user_family", "Famille", () => PageContainer.AddChild<CPlayerFamily>() );
			AddPage( "user_shop", "Boutique", () => PageContainer.AddChild<CPlayerShop>() );
			AddPage( "user_mail", "Courriers", () => PageContainer.AddChild<CPlayerMail>() );
			AddPage( "user_option", "Options", () => PageContainer.AddChild<CPlayerOption>() );


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

		public override void Tick()
		{
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
