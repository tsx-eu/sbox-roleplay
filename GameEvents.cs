using Sandbox;

namespace charleroi
{
	public static class GameEvent
	{
		public const string CraftQueueUpdate = "CraftQueueUpdate";
		public class CraftQueueUpdateAttribute : EventAttribute {
			public CraftQueueUpdateAttribute() : base( CraftQueueUpdate ) { }
		}
	}

}
