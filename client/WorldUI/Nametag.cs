using Sandbox;
using Sandbox.UI;


namespace charleroi.client.WorldUI
{
	partial class Nametag : WorldPanel
	{
		public string Name { get; set; }
		protected Entity entity;

		protected Nametag() {
			AddClass( "root" );
		}

		public Nametag( Entity ent, string text )
		{
			Name = text;
			entity = ent;

			SetTemplate( "/client/WorldUI/Nametag.html" );
			AddClass( "root" );
		}

		public override void Tick()
		{
			if ( !entity.IsValid() ) {
				Delete();
				return;
			}

			var playerPosition = CurrentView.Position;
			playerPosition.z = Position.z;

			var targetRotation = Rotation.LookAt( playerPosition - Position );
			var transform = new Transform( entity.Position + Vector3.Up * 32, entity.Rotation );
			transform.Rotation = Rotation.Lerp( transform.Rotation, targetRotation, 0.6f );

			Transform = transform;
		}
	}
}
