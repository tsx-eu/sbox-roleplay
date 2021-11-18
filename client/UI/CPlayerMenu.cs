using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace charleroi.UI
{
	class CPlayerMenu : Panel
	{
		public CPlayerMenu()
		{
			//SetTemplate( "/client/UI/CPlayerMenu.html" );
			StyleSheet.Load( "/client/UI/CPlayerMenu.scss" );
			var top = Add.Panel( "top" );
			{
				var tabs = top.AddChild<ButtonGroup>();
				tabs.AddClass( "navbar" );

				var body = top.Add.Panel( "body" );
				{
					tabs.Add.Button( "Buttom 1" );
					tabs.Add.Button( "Buttom 2" );
				}
			}
		}

		public override void Tick()
		{
			base.Tick();

			if ( Input.Pressed( InputButton.Menu ) )
			{
				var avatar = Parent.ChildrenOfType<CPlayerHUD>().SingleOrDefault();
				bool status = HasClass( "hidden" );

				if ( status )
				{
					SetClass( "hidden", !status );
					avatar?.SetClass( "hidden", status );
				}
				else
				{
					SetClass( "hidden", !status );
					avatar?.SetClass( "hidden", status );
				}
			}
		}
	}
}
