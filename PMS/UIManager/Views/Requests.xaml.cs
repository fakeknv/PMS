using PMS.UIComponents;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using MahApps.Metro.Controls;
using PMS.UIManager.Views.ChildWindows;
using System.Data;
using MySql.Data.MySqlClient;
using System;
using System.Web.UI.WebControls;
using System.Windows.Media;

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
		/// <summary>
		/// Updates Stats
		/// </summary>
		private void SyncStat()
		{
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				//Counts Total
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					StatTotalHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Baptismal
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Baptismal");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					StatBaptismalHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Confirmation
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Confirmation");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					StatConfirmationHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Burial
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Burial");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					StatBurialHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Marriage
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Marriage");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					StatMarriageHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();


				//Counts Searching
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Searching");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					BadgeSearching.Badge = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Printing
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Printing");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					BadgePrinting.Badge = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Paying
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Paying");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					BadgePaying.Badge = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Finished
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Finished");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					BadgeFinished.Badge = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Cancelled
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Cancelled");
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					BadgeCancelled.Badge = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		/// <summary>
		/// Updates the Requests list.
		/// </summary>
		internal void SyncRequests()
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM requests;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					string reqID = db_reader.GetString("request_id");
					//Console.WriteLine(db_reader.GetString("request_id"));
					RequestItem ri = new RequestItem();
					ri.RequestIDHolder.Content = db_reader.GetString("request_id");
					ri.StatusHolder.Content = db_reader.GetString("status");
					if (db_reader.GetString("status") == "Finished") {
						var bc = new BrushConverter();
						ri.StatusHolderBG.Background = (Brush)bc.ConvertFrom("#FF46C37B");
					}
					else if (db_reader.GetString("status") == "Cancelled")
					{
						var bc = new BrushConverter();
						ri.StatusHolderBG.Background = (Brush)bc.ConvertFrom("#777777");
					}
					ri.NameHolder.Content = db_reader.GetString("req_record_name");
					ri.TypeHolder.Content = db_reader.GetString("type");
					ri.DateAddedHolder.Content = DateTime.Parse(db_reader.GetString("req_time")).ToString("hh:mm tt") + " " + DateTime.Parse(db_reader.GetString("req_date")).ToString("MMMM dd yyyy");
					//ri.RequestCancelButton.Click += RequestCancelled_Click;
					ri.RequestCancelButton.Click  += (sender, e) => { RequestCancelled_Click(sender, e, reqID); };
					RequestItemsContainer.Items.Add(ri);
				}
				//close Connection
				dbman.DBClose();
				SyncStat();
			}
			else
			{

			}

		}
		/// <summary>
		/// Onclick event for the CreateRequestButton.
		/// </summary>
		private async void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRequestWindow(this), this.RequestMainGrid);
		}
		/// <summary>
		/// Onclick event for the ManualSyncButton.
		/// </summary>
		private void ManualSyncButton_Click(object sender, RoutedEventArgs e)
		{
			SyncRequests();
		}
		/// <summary>
		/// Onclick event for the Searching Filter Button.
		/// </summary>
		private void ShowSearching_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Searching");
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
				SyncStat();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Searching Filter Button.
		/// </summary>
		private void ShowPrinting_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Printing");
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
				SyncStat();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Searching Filter Button.
		/// </summary>
		private void ShowPaying_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Paying");
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
				SyncStat();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Searching Filter Button.
		/// </summary>
		private void ShowFinished_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Finished");
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
				SyncStat();
			}
			else
			{

			}
		}
		/// <summary>
		/// Onclick event for the Searching Filter Button.
		/// </summary>
		private void ShowCancelled_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Cancelled");
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
				SyncStat();
			}
			else
			{

			}
		}

		/// <summary>
		/// Onclick event for the Cancel Request Button.
		/// </summary>
		private void RequestCancelled_Click(object sender, EventArgs e, string reqID)
		{
			try
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "UPDATE requests SET status = @new_status WHERE request_id = @request_id;";
				cmd.Parameters.AddWithValue("@new_status", "Cancelled");
				cmd.Parameters.AddWithValue("@request_id", reqID);
				cmd.Prepare();
				cmd.ExecuteNonQuery();
				SyncRequests();
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}", ex.ToString());
			}
		}
		private void SearchRequestBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			RequestItemsContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM requests WHERE " +
					"request_id LIKE @query OR " +
					"type LIKE @query OR " +
					"status LIKE @query OR " +
					"req_record_name LIKE @query";
				cmd.Parameters.AddWithValue("@query", "%" + SearchRequestBox.Text + "%");
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
	}
}