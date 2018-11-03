using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;

namespace PMS
{
	class PMSUtil
	{
		private DBConnectionManager dbman;
		private string ret;

		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;

		internal string GenRecordID()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(record_id) FROM records;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = "PMS-" + (db_reader.GetInt32("COUNT(record_id)")+1);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string GenRecordLogID()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(log_id) FROM records_log;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = "LOG-" + (db_reader.GetInt32("COUNT(log_id)") + 1);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
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
		/// <summary>
		/// Logs insertion of insertion records.
		/// </summary>
		internal string LogRecordInsertion(string recordID) {
			string ret = "";

			string logID = GenRecordLogID();
			string logger = Application.Current.Resources["uid"].ToString();
			string[] dt = GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			cTime = DateTime.Parse(dt[1] + " " + dt[2]);
			curDate = cDate.ToString("yyyy-MM-dd");
			curTime = cTime.ToString("HH:mm:ss");

			MySqlCommand cmd = dbman.DBConnect().CreateCommand();
			cmd.CommandText =
				"INSERT INTO records_log(log_id, log_code, log_date, log_time, log_creator, record_id)" +
				"VALUES(@log_id, @log_code, @log_date, @log_time, @log_creator, @record_id)";
			cmd.Prepare();
			cmd.Parameters.AddWithValue("@log_id", logID);
			cmd.Parameters.AddWithValue("@log_code", "LOGC-01");
			cmd.Parameters.AddWithValue("@log_date", curDate);
			cmd.Parameters.AddWithValue("@log_time", curTime);
			cmd.Parameters.AddWithValue("@log_creator", logger);
			cmd.Parameters.AddWithValue("@record_id", recordID);
			ret = cmd.ExecuteNonQuery().ToString();
			dbman.DBClose();
			return ret;
		}
	}
}
