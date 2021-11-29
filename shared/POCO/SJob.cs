using Sandbox;
using System;

namespace charleroi
{
	public interface SJob
	{
		public ulong Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public string ImageURL { get; set; }
	}
}
