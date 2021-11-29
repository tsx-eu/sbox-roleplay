using Sandbox.UI;

namespace charleroi.client.UI.MenuNav
{
	class CPlayerJob: Panel
	{

		public CPlayerJob()
		{
			SetTemplate( "/client/UI/MenuNav/CPlayerJob.html" );
		}

		public override void Tick()
		{

		}
	}

	class CPlayerJobList : Panel
	{
		public CPlayerJobList() {
			DeleteChildren( true );

			foreach (var job in Game.Instance.Jobs) {
				var label = AddChild<Label>( "jobname" );
				label.Text = job.Name;
			}
		}

	}
}
