using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi.client.UI
{
	class CPlayerHealth : Panel
	{		
		private Label _Health;
		private Panel _HealthBar;


		public CPlayerHealth()
		{
			Style.Width = Length.Percent( 100.0f );
			_HealthBar = Add.Panel( "life-bar-value" );
			_Health = Add.Label( "0", "life-value" );
			
		}
		public override void Tick()
		{
			base.Tick();
			var currentPlayer = (CPlayer)Local.Pawn;

			//_HealthBar.Style.Width = Length.Percent( currentPlayer.Health );
			_Health.Text = $"{currentPlayer.Health.CeilToInt()} %";
			
		}
	}
}
