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

			AddPage( "price_change", "Inventaire", () => PageContainer.AddChild<CPlayerInventory>() );
			AddPage( "card_travel", "Métiers", () => PageContainer.AddChild<CPlayerJob>() );
			AddPage( "science", "Compétences", () => PageContainer.AddChild<CPlayerSkill>() );
			AddPage( "groups", "Famille", () => PageContainer.AddChild<CPlayerFamily>() );
			AddPage( "shopping_cart", "Boutique", () => PageContainer.AddChild<CPlayerShop>() );
			AddPage( "mail_outline", "Courriers", () => PageContainer.AddChild<CPlayerMail>() );
			AddPage( "miscellaneous_services", "Options", () => PageContainer.AddChild<CPlayerOption>() );

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
