using Sandbox;
using System;

namespace charleroi
{
	public interface SCraft
	{
		public ulong Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageURL { get; set; }
		public string Ingredients { get; set; }
		public int Level { get; set; }
	}
}
