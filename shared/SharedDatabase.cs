using System.Collections.Generic;
using charleroi.client;
using Sandbox;

namespace charleroi.shared
{
	public partial class SharedDatabase : BaseNetworkable
	{
		private static readonly SharedDatabase instance = new SharedDatabase();

		private SharedDatabase() {
		}

		public static SharedDatabase Instance
		{
			get { return instance; }
		}

		[Net]
		public IList<charleroi.client.CItem> Items { get; private set; } = new List<charleroi.client.CItem>();
	}
}
