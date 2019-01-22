using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using System;
using System.Collections.ObjectModel;
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

		private ObservableCollection<HistoryItem> history;

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

				history = new ObservableCollection<HistoryItem>();
				while (db_reader.Read())
				{
					if (db_reader.GetString("log_code") == "LOGC-01") {
						history.Add(new HistoryItem()
						{
							Details = "Recorded by " + pmsutil.GetUsername(db_reader.GetString("log_creator")),
							Date = DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy"),
							Time = DateTime.Parse(db_reader.GetString("log_time")).ToString("hh:mm tt"),
							Purpose = "NA"
						});
					}else if (db_reader.GetString("log_code") == "LOGC-02"){
						history.Add(new HistoryItem()
						{
							Details = "Edited by " + pmsutil.GetUsername(db_reader.GetString("log_creator")),
							Date = DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy"),
							Time = DateTime.Parse(db_reader.GetString("log_time")).ToString("hh:mm tt"),
							Purpose = "NA"
						});
					}
					else if (db_reader.GetString("log_code") == "LOGC-03")
					{
						history.Add(new HistoryItem()
						{
							Details = "Certificate printed by " + pmsutil.GetUsername(db_reader.GetString("log_creator")),
							Date = DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy"),
							Time = DateTime.Parse(db_reader.GetString("log_time")).ToString("hh:mm tt"),
							Purpose = "For Reference"
						});
					}
					else if (db_reader.GetString("log_code") == "LOGC-04")
					{
						history.Add(new HistoryItem()
						{
							Details = "Certificate printed by " + pmsutil.GetUsername(db_reader.GetString("log_creator")),
							Date = DateTime.Parse(db_reader.GetString("log_date")).ToString("MMM dd, yyyy"),
							Time = DateTime.Parse(db_reader.GetString("log_time")).ToString("HH:mm tt"),
							Purpose = "For Marriage"
						});
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			HistoryContainer2.ItemsSource = history;
		}

		private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
