using Sandbox.UI;

namespace charleroi.client.UI.MenuNav
{
	class CPlayerSkill: Panel
	{

		public CPlayerSkill()
		{
			SetTemplate( "/client/UI/MenuNav/CPlayerSkill.html" );
		}

		public override void Tick()
		{

		}
	}

	class CPlayerSkillList : Panel
	{
		public CPlayerSkillList() {
			DeleteChildren( true );

			foreach (var job in Game.Instance.Jobs) {
				var label = AddChild<Label>( "jobname" );
				label.Text = job.Name;
			}
		}

	}
}
