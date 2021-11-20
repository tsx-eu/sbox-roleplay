using charleroi;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Linq;

namespace charleroi.UI
{
	class CPlayerContent : Panel
	{
		public string Job { get; set; }

		public CPlayerContent()
		{
			SetTemplate( "/client/UI/MenuNav/CPlayerContent.html" );
		}

		public override void Tick()
		{
			CPlayer client = Local.Pawn as CPlayer;
			if ( client == null ) return;

			Job = client.Job;
		}

	}
}
