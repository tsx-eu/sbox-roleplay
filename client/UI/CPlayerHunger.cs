using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi.client.UI
{
	class CPlayerHunger : Panel
	{
		private Label _Hungry;
		private Panel _HungryBar;


		public CPlayerHunger()
		{
			Style.Width = Length.Percent( 100.0f );
			_HungryBar = Add.Panel( "hunger-bar-value" );
			_Hungry = Add.Label( "0", "hunger-value" );

		}
		public override void Tick()
		{
			base.Tick();
			var currentPlayer = (CPlayer)Local.Pawn;

			_HungryBar.Style.Width = Length.Percent( currentPlayer.Hunger );
			_Hungry.Text = $"{currentPlayer.Hunger.CeilToInt()} %";

		}
	}
}
