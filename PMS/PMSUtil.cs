using MySql.Data.MySqlClient;
using System.Data;
using System.Windows;

namespace PMS
{
	class PMSUtil
	{
		private DBConnectionManager dbman;

		/// <summary>
		/// Retrieves Current Date and Time from the Server.
		/// </summary>
		internal string GetServerDateTime()
		{
			dbman = new DBConnectionManager();

			string curDateTime = "";

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT NOW() FROM DUAL;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					curDateTime = db_reader.GetString("NOW()");
				}
				//close Connection
				dbman.DBClose();

				return curDateTime;
			}
			else
			{
				//close Connection
				dbman.DBClose();

				return null;
			}
		}
	}
}
