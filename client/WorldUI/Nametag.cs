using Sandbox;
using Sandbox.UI;


namespace charleroi.client.WorldUI
{
	partial class Nametag : WorldPanel
	{
		public string name { get; set; }
		private CPlayer owner;
		private Entity entity;
		private Transform baseTransform;

		public Nametag( CPlayer client, Entity ent )
		{
			name = ent.Name;
			owner = client;
			entity = ent;
			baseTransform = new Transform( ent.Position + Vector3.Up * 32, ent.Rotation );

			Transform = baseTransform;

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


			var playerPosition = CurrentView.Position;
			playerPosition.z = Position.z;

			var targetRotation = Rotation.LookAt( playerPosition - Position );
			var transform = baseTransform;
			transform.Rotation = Rotation.Lerp( transform.Rotation, targetRotation, 0.6f );

			Transform = transform;
		}
	}
}
