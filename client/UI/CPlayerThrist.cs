using Charleroi;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi.client.UI
{
	class CPlayerThrist : Panel
	{
		private Label _Thirst;
		private Panel _ThirstBar;


		public CPlayerThrist()
		{
			Style.Width = Length.Percent( 100.0f );
			_ThirstBar = Add.Panel( "thirst-bar-value" );
			_Thirst = Add.Label( "0", "thirst-value" );

		}
		public override void Tick()
		{
			base.Tick();
			var currentPlayer = (CPlayer)Local.Pawn;

			_ThirstBar.Style.Width = Length.Percent( currentPlayer.Thirst );
			_Thirst.Text = $"{currentPlayer.Thirst.CeilToInt()} %";

		}
	}
}
