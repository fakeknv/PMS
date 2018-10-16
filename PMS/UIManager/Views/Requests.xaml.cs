using PMS.UIComponents;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using MahApps.Metro.Controls;
using PMS.UIManager.Views.ChildWindows;
using System.Data;
using MySql.Data.MySqlClient;
using System;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Tasks.xaml
	/// </summary>
	public partial class Requests : UserControl
	{
		//MYSQL Related Stuff
		private DBConnectionManager dbman;

		public Requests()
		{
			InitializeComponent();
			SyncRequests();
		}
		internal void SyncRequests()
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM request;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					//Console.WriteLine(db_reader.GetString("request_id"));
					RequestItem ri = new RequestItem();
					ri.RequestIDHolder.Content = db_reader.GetString("request_id");
					ri.StatusHolder.Content = db_reader.GetString("status");
					ri.NameHolder.Content = db_reader.GetString("req_record_name");
					ri.TypeHolder.Content = db_reader.GetString("type");
					ri.DateAddedHolder.Content = DateTime.Parse(db_reader.GetString("req_time")).ToString("hh:mm tt") + " " + DateTime.Parse(db_reader.GetString("req_date")).ToString("MMMM dd yyyy");
					RequestItemsContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}

		}
		private async void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRequestWindow(this), this.RequestMainGrid);
		}
		private void ManualSyncButton_Click(object sender, RoutedEventArgs e)
		{
			SyncRequests();
		}
	}
}