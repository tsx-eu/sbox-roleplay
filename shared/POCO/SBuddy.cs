using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;
using charleroi.server.DAL;


namespace charleroi
{
	[Library]
	public interface SBuddy : IStorable
	{
		public ulong BuddySteamID { get; set; }
		public SBuddyPerms Flags { get; set; }

	}

	[Flags]
	public enum SBuddyPerms
	{
		None = 0,
		Root = 1
	}


	[Library]
	public class ListOfSBuddy : List<SBuddy>
	{
	}
}
