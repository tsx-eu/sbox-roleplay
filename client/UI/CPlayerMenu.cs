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
	public partial class CPlayerMenu : CPlayerMenuBase
	{
		public CPlayerMenu() : base()
		{

			AddPage( "price_change", "Inventaire", "Je suis un titre", () => PageContainer.AddChild<CPlayerInventory>() );
			AddPage( "card_travel", "Métiers", "Je suis un titre", () => PageContainer.AddChild<CPlayerJob>() );
			AddPage( "science", "Compétences", "Je suis un titre", () => PageContainer.AddChild<CPlayerSkill>() );
			AddPage( "groups", "Famille", "Je suis un titre", () => PageContainer.AddChild<CPlayerFamily>() );
			AddPage( "shopping_cart", "Boutique", "Je suis un titre", () => PageContainer.AddChild<CPlayerShop>() );
			AddPage( "mail_outline", "Courriers", "Je suis un titre", () => PageContainer.AddChild<CPlayerMail>() );
			AddPage( "miscellaneous_services", "Options", "Je suis un titre", () => PageContainer.AddChild<CPlayerOption>() );


			LoadDefaultPage();
		}

		[ClientRpc]
		public static void Show()
		{
			if ( Instance == null && LastOpen >= 0.5f )
			{
				Instance = Local.Hud.AddChild<CPlayerMenu>();
				LastOpen = 0;
			}
		}

		[ClientRpc]
		public static void Hide()
		{
			Instance.Delete( true );
		}

	}
}
