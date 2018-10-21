using PMS.UIComponents;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using MahApps.Metro.Controls;
using PMS.UIManager.Views.ChildWindows;
using System.Data;
using MySql.Data.MySqlClient;
using System;
using System.Windows.Media;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Requests.xaml
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
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					TotalRequestsHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Baptismal
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Baptismal");
				cmd.Prepare();
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
				cmd.Prepare();
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
				cmd.Prepare();
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
				cmd.Parameters.AddWithValue("@type", "Matrimonial");
				cmd.Prepare();
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					StatMatrimonialHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();


				//Counts Searching
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM requests WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Searching");
				cmd.Prepare();
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
				cmd.Prepare();
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
				cmd.Prepare();
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
				cmd.Prepare();
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
				cmd.Prepare();
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
		/// Onclick event for the CreateRequestButton. Shows the AddRequestWindow that allows 
		/// the user to add a request.
		/// </summary>
		private async void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddRequestWindow(this), this.RequestMainGrid);
		}
		/// <summary>
		/// Onclick event for the ManualSyncButton. Calls SyncRequest to manually update the
		/// Request list.
		/// </summary>
		private void ManualSyncButton_Click(object sender, RoutedEventArgs e)
		{
			SyncRequests();
		}
		/// <summary>
		/// Onclick event for the Searching Filter Button. Filters the Requests to show requests
		/// that are Searching.
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
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
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
		/// Onclick event for the Searching Filter Button. Filters the Requests to show requests
		/// that are Searching.
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
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
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
		/// Onclick event for the Paying Filter Button. Filters the Requests to show requests
		/// that are Paying.
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
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
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
		/// Onclick event for the Finished Filter Button. Filters the Requests to show requests
		/// that are Finished.
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
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
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
		/// Onclick event for the Cancelled Filter Button. Filters the Requests to show requests
		/// that are Cancelled.
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
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
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
		/// Onclick event for the Cancel Request Button. Cancels a request.
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
		/// <summary>
		/// Livesearch interaction logic. This listens to the SearchRequestBox for the query letter 
		/// by letter then fetches the results that matches with the query and updates the Request 
		/// list.
		/// </summary>
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
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
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