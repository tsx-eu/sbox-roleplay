using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Collections.Generic;


namespace charleroi.client.WorldUI
{
	partial class Nametag : WorldPanel
	{
		public string name { get; set; }
		public CPlayer owner { get; set; }
		public Entity entity { get; set; }

		public Nametag( CPlayer client, Entity ent )
		{
			name = ent.Name;
			owner = client;
			entity = ent;

			Transform = new Transform( ent.Position + Vector3.Up * 32, ent.Rotation );
			SetTemplate( "/client/WorldUI/Nametag.html" );
		}

		public override void Tick()
		{
			if ( !entity.IsValid() )
				Delete();

			if ( owner.Transform.Position.Distance( Transform.Position ) > 512.0f )
				Delete();

			if ( Input.Released( InputButton.Use ) )
				Delete();
		}
	}
}
