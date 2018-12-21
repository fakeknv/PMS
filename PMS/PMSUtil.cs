using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
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

		internal bool IsArchived(string recordID) {
			bool ret = false;
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT registers.status FROM records, registers WHERE records.record_id = @rid AND records.book_number = registers.book_number LIMIT 1;";
				cmd.Parameters.AddWithValue("@rid", recordID);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("status") == "Archived")
					{
						ret = true;
					}
					else 
					{
						ret = false;
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string CheckArchiveDrive(string path)
		{
			string ret = "";
			DriveInfo[] allDrives = DriveInfo.GetDrives();

			foreach (DriveInfo d in allDrives)
			{
				if (d.IsReady == true && File.Exists(d.Name + path) == true)
				{
					SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
					{
						FailIfMissing = true,
						DataSource = d.Name + path
					};
					//Copy the selected register's record to the archive drive
					using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
					{
						connection.Open();
						if (connection.State.ToString() == "Open")
						{
							ret = d.Name + path;
						}
						else
						{
							ret = "dc";
						}
					}
				}
				else
				{
					ret = "dc";
				}
			}
			return ret;
		}
		internal int InsertTransaction(string type, string status, string targetID, double fee) {
			string uid = Application.Current.Resources["uid"].ToString();
			string[] dt = GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			cTime = DateTime.Parse(dt[1] + " " + dt[2]);
			curDate = cDate.ToString("yyyy-MM-dd");
			curTime = cTime.ToString("HH:mm:ss");

			dbman = new DBConnectionManager();
			//TODO
			try
			{
				string tID = GenTransactionID();
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO transactions(transaction_id, type, status, tran_date, tran_time, completion_date, completion_time, placed_by, completed_by, target_id, fee)" +
					"VALUES(@transaction_id, @type, @status, @tran_date, @tran_time, @completion_date, @completion_time, @placed_by, @completed_by, @target_id, @fee)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@transaction_id", tID);
				cmd.Parameters.AddWithValue("@type", type);
				cmd.Parameters.AddWithValue("@status", status);
				cmd.Parameters.AddWithValue("@tran_date", curDate);
				cmd.Parameters.AddWithValue("@tran_time", curTime);
				cmd.Parameters.AddWithValue("@completion_date", null);
				cmd.Parameters.AddWithValue("@completion_time", null);
				cmd.Parameters.AddWithValue("@placed_by", uid);
				cmd.Parameters.AddWithValue("@completed_by", null);
				cmd.Parameters.AddWithValue("@target_id", targetID);
				cmd.Parameters.AddWithValue("@fee", fee);
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				//string tmp = pmsutil.LogRecord(recID, "LOGC-01");
				return stat_code;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}", ex.ToString());
				return 0;
			}
		}
		internal string GetAccountType(string uid) {
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM accounts WHERE account_id = @uid LIMIT 1;";
				cmd.Parameters.AddWithValue("@uid", uid);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("account_type") == 1) {
						ret = "Administrator";
					}
					else if (db_reader.GetInt32("account_type") == 2)
					{
						ret = "Cashier";
					}
					else if (db_reader.GetInt32("account_type") == 3)
					{
						ret = "Secretary";
					}
					else if (db_reader.GetInt32("account_type") == 4)
					{
						ret = "Cemetery Personnel";
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string GetFullName(string uid) {
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM accounts, accounts_info WHERE accounts.account_id = @uid AND accounts.account_id = accounts_info.account_id LIMIT 1;";
				cmd.Parameters.AddWithValue("@uid", uid);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("name");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string GetUsername(string uid) {
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM accounts WHERE account_id = @uid LIMIT 1;";
				cmd.Parameters.AddWithValue("@uid", uid);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("user_name");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string GetPrintFee(string type) {
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = @key_name;";
				if (type == "Baptismal") {
					cmd.Parameters.AddWithValue("@key_name", "Print Fee Baptismal");
				} else if (type == "Confirmation") {
					cmd.Parameters.AddWithValue("@key_name", "Print Fee Confirmation");
				} else if (type == "Matrimonial") {
					cmd.Parameters.AddWithValue("@key_name", "Print Fee Matrimonial");
				} else if (type == "Burial") {
					cmd.Parameters.AddWithValue("@key_name", "Print Fee Burial");
				}
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("key_value");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string GetChurchName() {
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = @key_name;";
				cmd.Parameters.AddWithValue("@key_name", "Church Name");
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("key_value");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string GenAccountID()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(account_id) FROM accounts;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = "ACNT-" + (db_reader.GetInt32("COUNT(account_id)") + 1);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		internal string GenAppointmentID()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(appointment_id) FROM appointments;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = "APM-" + (db_reader.GetInt32("COUNT(appointment_id)") + 1);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
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
		internal string GenTransactionID()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(transaction_id) FROM transactions;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = "TRN-" + (db_reader.GetInt32("COUNT(transaction_id)") + 1);
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
		internal string LogRecord(string recordID, string logType) {
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
			cmd.Parameters.AddWithValue("@log_code", logType);
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
