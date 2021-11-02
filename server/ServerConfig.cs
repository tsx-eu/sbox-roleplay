using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Charleroi
{
	class ServerConfig
	{
		public static string WSUrl { get; } = "wss://wsstore.stone.leethium.fr/ws";
		public static string WSUser { get; } = "api";
		public static string WSPass { get; } = "api";
	}
}
