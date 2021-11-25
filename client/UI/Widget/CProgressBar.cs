using Sandbox;
using Sandbox.Html;
using Sandbox.UI;
using Sandbox.UI.Construct;
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
		private Label text;
		private bool loaded;
		private static float delta = 0.025f;

		public CProgressBar() {
			loaded = false;
			bar = null;
			text = null;
			AddClass( "tsx_progress" );
		}

		public override void Tick() {
			if( !loaded ) {
				loaded = true;

				var player = Local.Client.Pawn as CPlayer;
				foreach ( var childProp in typeof( CPlayer ).GetProperties() ) {
					if( HasClass(childProp.Name.ToLower()) ) {
						Bind( "Percent", player, childProp.Name);
						bar = Add.Panel( "value" );
						bar.AddClass( childProp.Name.ToLower() + "-value" );

						if ( HasClass( "text" ) ) {
							text = Add.Label( "", "text" );
						}
						return;
					}
				}

				Log.Error( "Could not find any properties ("+ Classes +") to bind to the player" );
				return;
			}

			if ( bar != null )
				bar.Style.Width = Length.Fraction( delta + (Percent/(1- delta)) );
			if ( text != null )
				text.Text = ((Percent * 1000) / 10).ToString() + "%";
		}
	}
}
