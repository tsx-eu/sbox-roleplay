using Sandbox;
using Sandbox.Html;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace charleroi.client.UI
{
	class CProgressBar : Panel
	{
		public float Percent { get; set; } = 0.0f;
		private Panel bar;
		private bool loaded;

		public CProgressBar() {
			loaded = false;
			AddClass( "tsx_progress" );
		}

		public override void Tick() {
			if( !loaded ) {
				if ( HasClass( "currentxp" ) ) {
					Bind( "Percent", Local.Client.Pawn, "CurrentXP" );
					bar = Add.Panel( "xp-value" );

					loaded = true;
				}

				return;
			}


			bar.Style.Width = Length.Fraction(Percent);
		}
	}
}
