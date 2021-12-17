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

			foreach (var skill in Game.Instance.Skills) {
				var label = AddChild<Label>( "skillname" );
				label.Text = skill.Name;
			}
		}

	}
}
