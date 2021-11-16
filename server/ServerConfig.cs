using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace charleroi
{
	class ServerConfig
	{
		public static string WSUrl { get; } = "ws://localhost:8080/ws";
		public static string WSUser { get; } = "api";
		public static string WSPass { get; } = "api";
	}
}
