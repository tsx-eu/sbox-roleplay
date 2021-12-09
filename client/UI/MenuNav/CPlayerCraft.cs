using Sandbox;
using Sandbox.UI;

namespace charleroi.client.UI.MenuNav
{
	class CPlayerCraft : Panel
	{

		public CPlayerCraft()
		{
			SetTemplate( "/client/UI/MenuNav/CPlayerCraft.html" );
		}

		public override void Tick()
		{

		}

	}


	class CPlayerCraftList : Panel
	{
		public CPlayerCraftList()                               // <CPlayerCraftList>, si das le HTML y'a déjà une classe, il le récup automiquement.
		{
			var player = Local.Client.Pawn as CPlayer;

			DeleteChildren( true );
			foreach ( var craft in Game.Instance.Crafts )
			{
				var panel = AddChild<Panel>( "Craft" );             // <div class="Craft">
				if ( craft.Level > player.CurrentXP )
					panel.AddClass( "nolvl" );

				var img = panel.AddChild<Panel>( "img" );               // <div class="img"></div>
				if ( craft.Level > player.CurrentXP )
					img.AddChild<Label>( "lvl" ).Text = craft.Level.ToString();

				var item = panel.AddChild<Panel>( "item" );             // <div class="item">

				var name = item.AddChild<Panel>( "name" );                  // <div class="name">
				var labelName = name.AddChild<Label>();                 // <label>
				labelName.Text = craft.Name;                                // Acide citrique 2

				var description = item.AddChild<Panel>( "description" );    // <div class="description">
				var labelDescription = description.AddChild<Label>();       // <label>
				labelDescription.Text = craft.Description;                  // L'acide citrique est un acide tricarboxylique
			}
		}

	}
}
