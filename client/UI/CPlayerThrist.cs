using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

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
