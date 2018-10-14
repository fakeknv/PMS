using MySql.Data.MySqlClient;
using PMS.UIManager;
using System.Data;
using System.Windows;

namespace PMS
{
	class DBConnectionManager
	{
		private MySqlConnection conn;
		internal MySqlConnection DBConnect()
		{
			string conn_str = "Server=localhost;Database=pms_db;Uid=pms;Pwd=pms2018!;SslMode=none";
			conn = new MySqlConnection(conn_str);
			conn.Open();
			return conn;
		}
		internal void DBClose()
		{
			try
			{
				conn.Close();
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
