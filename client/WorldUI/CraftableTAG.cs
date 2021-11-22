using Sandbox;
using Sandbox.UI;


namespace charleroi.client.WorldUI
{
	partial class CraftableTAG : Nametag
	{

		public CraftableTAG( Entity ent, string text ) : base( ent, text )
		{
			SetTemplate( "/client/WorldUI/CraftableTAG.html" );
		}

		public override void Tick() {

			var table = entity as CEntityCrafttable;
			if ( table != null ) {
				// TODO, label, progression...
			}

			base.Tick();
		}
	}
}
