using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi.server.POCO
{
	[Serializable]
	public class SItem
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
		public string ShortDescription { get; set; }
		public float MaxCarry { get; private set; } = 64;
	}
}
