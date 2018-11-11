using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for ViewRemarksWindow.xaml
	/// </summary>
	public partial class ViewHistoryWindow : ChildWindow
	{
		//MYSQL
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		public ViewHistoryWindow(string recordID)
        {
            InitializeComponent();
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM records_log, accounts WHERE record_id = @record_id;";
				cmd.Parameters.AddWithValue("@record_id", recordID);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("log_code") == "LOGC-01") {
						HistoryContainer.Items.Add("Recorded by " + pmsutil.GetUsername(db_reader.GetString("log_creator")) + " on " + DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy") + " " + DateTime.Parse(db_reader.GetString("log_time")).ToString("HH:mm tt"));
					}else if (db_reader.GetString("log_code") == "LOGC-02"){
						HistoryContainer.Items.Add("Edited by " + pmsutil.GetUsername(db_reader.GetString("log_creator")) + " on " + DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy") + " " + DateTime.Parse(db_reader.GetString("log_time")).ToString("HH:mm tt"));
					}
					else if (db_reader.GetString("log_code") == "LOGC-03")
					{
						HistoryContainer.Items.Add("Certificate printed by " + pmsutil.GetUsername(db_reader.GetString("log_creator")) + " on " + DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy") + " " + DateTime.Parse(db_reader.GetString("log_time")).ToString("HH:mm tt") + ". Purpose: Reference.");
					}
					else if (db_reader.GetString("log_code") == "LOGC-04")
					{
						HistoryContainer.Items.Add("Certificate printed by " + pmsutil.GetUsername(db_reader.GetString("log_creator")) + " on " + DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy") + " " + DateTime.Parse(db_reader.GetString("log_time")).ToString("HH:mm tt") + ". Purpose: Marriage.");
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}

		private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
