using System;
using System.Linq;
using charleroi.client.UI.Inventory;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace charleroi.client.UI.MenuNav
{
	class CPlayerCraft : Panel
	{

		public CPlayerCraft() : base() {
			SetTemplate( "/client/UI/MenuNav/CPlayerCraft.html" );
		}

		public override void Tick() {

		}
	}


	class CPlayerCraftList : Panel
	{
		public CPlayerCraftList() : base()
		{
			var player = Local.Client.Pawn as CPlayer;

			DeleteChildren( true );

			var copy = Game.Instance.Crafts.ToArray().ToList();
			copy.AddRange( copy );
			copy.AddRange( copy );

			int i = 0;
			foreach ( var craft in copy ) {
				i++;
				var line = AddChild<Panel>( "line" );
				if ( i % 2 == 0 )
					line.AddClass( "odd" );
				else
					line.AddClass( "even" );

				var output = line.AddChild<Panel>( "flex-start" );
				for ( int c = 0; c < Rand.Int( 1, 3 ); c++ )
					output.AddChild<CInventoryItem>();
				output.AddChild<Label>().Text = craft.Item.Name;


				var input = line.AddChild<Panel>( "flex-end" );
				for ( int c = 0; c < Rand.Int( 1, 5 ); c++ )
					input.AddChild<CInventoryItem>();
				var less = input.AddChild<Button>();
				less.Text = "-";

				var quantity = input.Add.TextEntry( "1");
				quantity.AddClass( "input-quantity" );
				quantity.Numeric = true;
				quantity.MinValue = 1;
				quantity.MaxValue = 999;
				quantity.MinLength = 1;
				quantity.MaxLength = 3;
				quantity.Style.Set( "text-align: right;" );

				var more = input.AddChild<Button>();
				more.Text = "+";

				var max = input.AddChild<Button>();
				max.Text = "max";

				less.AddEventListener( "onclick", () => {
					int val = quantity.Text.ToInt( 0 );
					if( val > 1 )
						quantity.Text = "" + (val-1);
				});
				more.AddEventListener( "onclick", () => {
					int val = quantity.Text.ToInt( 0 );
					if ( val < 999 )
						quantity.Text = "" + (val + 1);
				});
				max.AddEventListener( "onclick", () => {
					quantity.Text = "999";
				});

				var build = input.AddChild<Button>();
				build.AddEventListener( "onclick", () => {
					var p = Ancestors.Where( i => i.GetType() == typeof( CPlayerMenuCraft ) ).First() as CPlayerMenuCraft;
					CEntityCrafttable.Enqueue( p.ent.NetworkIdent, craft.Id, quantity.Text.ToInt(1) );
					quantity.Text = "1";
				} );
				build.Text = "CRAFT";

			}
		}

	}

	class CPlayerCraftQueue : Panel
	{
		static public CPlayerCraftQueue Instance;

		public CPlayerCraftQueue() : base()
		{
			Instance = this;
		}

		public override void OnParentChanged()
		{
			base.OnParentChanged();
			Log.Error( "parent changed" );

			Rebuild();
			Instance = this;
		}

		private void Rebuild()
		{
			var p = Ancestors.Where( i => i.GetType() == typeof( CPlayerMenuCraft ) ).FirstOrDefault() as CPlayerMenuCraft;

			Log.Info( p );

			if ( p?.ent?.queue?.Count > 0 ) {
				Log.Info( "rebuild2" );

				if ( HasClass( "hidden" ) )
					RemoveClass( "hidden" );

				AddChild<Label>( "titre" ).Text = "File d'attente:";

				var line = AddChild<Panel>( "line" );
				var flex = line.AddChild<Panel>( "flex-start" );

				foreach ( var item in p.ent.queue )
					flex.AddChild<CInventoryItem>();
			}
			else {
				if ( !HasClass( "hidden" ) )
					AddClass( "hidden" );
			}

		}

		[Event(GameEvent.CraftQueueUpdate)]
		public static void OnCraftQueueUpdate()
		{
			Log.Error( "got event OnCraftQueueUpdate! which is: "  + (Instance == null) + "<-- ");
			Instance?.Rebuild();
		}
	}
}
