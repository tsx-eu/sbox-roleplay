using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace charleroi.client.UI
{
	class CPlayerAvatar : Panel {
		public ScenePanel heroScene;
		public SceneWorld world;
		public AnimSceneObject playerPreview;
		public List<AnimSceneObject> playerClothesPreview;

		private bool clothes = false;

		public CPlayerAvatar() {
			clothes = false;
			playerClothesPreview = new List<AnimSceneObject>();

			world = new SceneWorld();
			using ( SceneWorld.SetCurrent( world ) ) {
				playerPreview = new AnimSceneObject( Model.Load( "models/citizen/citizen.vmdl" ), Transform.Zero );
				SpotLight light = new SpotLight( new Vector3( 64, 0, 96 ), Color.White );
				light.Falloff = 0.01f;
				light.Rotation = Rotation.FromPitch( 150.0f );

				heroScene = new ScenePanel {
					World = world,
					CameraPosition = new Vector3( 128, 0, 62 ),
					CameraRotation = Rotation.FromYaw( 180.0f ),
					FieldOfView = 10.0f,

				};
				AddChild( heroScene );
				heroScene.SetClass( "heroPortrait", true );
				heroScene.Style.ZIndex = 3;
			}
		}

		public override void Tick() {
			CPlayer client = Local.Pawn as CPlayer;
			if ( client == null ) return;

			if ( playerPreview != null && heroScene != null ) {
				if ( client.GetActiveAnimator() is PlayerAnimator animator )
					CopyParams( animator, playerPreview );

				if ( clothes == false )
					CopyClothes( client, playerPreview );

				bool isBig = HasClass( "big" );
				heroScene.FieldOfView = isBig ? 25.0f : 10.0f;

				if ( heroScene.ComputedStyle != null && heroScene.ComputedStyle.Width.HasValue && heroScene.ComputedStyle.Height.HasValue ) {
					float d = 128;
					float w = heroScene.ComputedStyle.Width.Value.Value;
					float h = heroScene.ComputedStyle.Height.Value.Value;
					var fovV = 2 * Math.Atan( Math.Tan( MathX.DegreeToRadian( heroScene.FieldOfView ) / 2 ) * (h / w) );
					var sizeH = 2 * Math.Tan( fovV / 2 ) * 64;

					float f = isBig ? 0.0f : 9.0f;

					heroScene.CameraPosition = new Vector3( d, 0, (float)(64 - sizeH + f) );
				}

				playerPreview.Update( RealTime.Delta );
				playerClothesPreview.ForEach( i => i.Update( RealTime.Delta ) );
			}
		}

		private void CopyClothes( CPlayer from, AnimSceneObject to ) {
			Clothing.Container Clothing = new();
			Clothing.Deserialize( from.Clothes );

			using ( SceneWorld.SetCurrent( world ) ) {
				playerClothesPreview.Clear();

				foreach ( var c in Clothing.Clothing ) {
					if ( c.Model == "models/citizen/citizen.vmdl" ) {
						to.SetMaterialGroup( c.MaterialGroup );
						continue;
					}

					var anim = new AnimSceneObject( Model.Load( c.Model ), Transform.Zero);
					to.AddChild( "clothes", anim );

					if ( !string.IsNullOrEmpty( c.MaterialGroup ) )
						anim.SetMaterialGroup( c.MaterialGroup );

					playerClothesPreview.Add( anim );
				}

				foreach ( var group in Clothing.GetBodyGroups() ) {
					to.SetBodyGroup( group.name, group.value );
				}
			}

			clothes = true;

		}

		private void CopyParams( PlayerAnimator from, AnimSceneObject to )
		{
			try
			{
				to.SetAnimVector( "aim_eyes", (Vector3)from.Params["aim_eyes"] );
				to.SetAnimVector( "aim_head", (Vector3)from.Params["aim_head"] );
				foreach ( var animParam in from.Params )
				{
					if ( animParam.Value is int intAnimValue )
						to.SetAnimInt( animParam.Key, intAnimValue );
//					if ( animParam.Value is float floatAnimValue )
//						to.SetAnimFloat( animParam.Key, floatAnimValue );
//					if ( animParam.Value is Vector3 vector3AnimValue )
//						to.SetAnimVector( animParam.Key, vector3AnimValue );
//					if ( animParam.Value is bool boolAnimValue )
//						to.SetAnimBool( animParam.Key, boolAnimValue );
				}
			}
			catch ( Exception e ) { }
		}
	}
}
