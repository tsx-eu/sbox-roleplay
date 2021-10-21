using System;
using System.Net.Http; // HTTP CLIENT;

namespace Charleroi
{
	class Database
	{
		static Database instance = null;
		private static object objLock = new object();
		private Database()
		{
			//string host = "mysql.zaretti.be";
			//string user = "roleplay";
			//string pass = "=IDe.Um2fR7p_L/4S";
			//string data = "roleplay";
			//string port = "33006";

			// doesn't work: var con = new MySqlConnection( @"server=mysql.zaretti.be;uid=roleplay;pwd;database=roleplay;port=33006" );

			//Console.WriteLine( con.ServerVersion );

			// Doesn't work
			//using ( HttpClient client = new HttpClient() )
			//{
			//	var getClient = client.GetAsync( "https://ts-x.azurewebsites.net/player" );   
			//	var result = getClient.Result;
			//}


		}

		public static Database GetInstance()
		{
			lock ( objLock )
			{
				if ( instance == null )
					instance = new Database();
				return instance;
			}
		}
	}
}
