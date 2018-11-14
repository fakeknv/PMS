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
using MahApps.Metro.Controls.Dialogs;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Transactions.xaml
	/// </summary>
	public partial class Transactions : UserControl
	{
		//MYSQL Related Stuff
		private DBConnectionManager dbman;
		//MYSQL Related Stuff
		private DBConnectionManager dbman2;

		public Transactions()
		{
			InitializeComponent();

			SyncTransactions();
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
				cmd.CommandText = "SELECT COUNT(*) FROM transactions;";
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
				cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
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
				cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
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
				cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Burial Cert.");
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
				cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
				cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
				cmd.Prepare();
				db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					StatMatrimonialHolder.Content = db_reader.GetString("COUNT(*)");
				}
				//close Connection
				dbman.DBClose();

				//Counts Paying
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status;";
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
				cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status;";
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
				cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status;";
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
		internal void SyncTransactions()
		{
			dbman = new DBConnectionManager();

			TransactionsItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM transactions ORDER BY tran_date DESC , tran_time DESC;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					string tID = db_reader.GetString("transaction_id");
					//Console.WriteLine(db_reader.GetString("request_id"));
					TransactionItem ti = new TransactionItem();
					ti.IDLabel.Content = db_reader.GetString("transaction_id");
					if (db_reader.GetString("target_id").Substring(0, 3) == "PMS") {
						ti.RecName.Content = GetRecordName(db_reader.GetString("target_id"));
					}
					else {
						
					}
					ti.TypeLabel.Content = db_reader.GetString("type");
					if (db_reader.GetString("status") == "Paying") {
						var bc = new BrushConverter();
						ti.StatusColor.Background = (Brush)bc.ConvertFrom("#46C37B");
					}
					else if (db_reader.GetString("status") == "Cancelled") {
						var bc = new BrushConverter();
						ti.StatusColor.Background = (Brush)bc.ConvertFrom("#777777");
					} else if (db_reader.GetString("status") == "Finished") {
						var bc = new BrushConverter();
						ti.StatusColor.Background = (Brush)bc.ConvertFrom("#5C90D2");
					}
					ti.StatusLabel.Content = db_reader.GetString("status");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.FeeLabel.Content = db_reader.GetString("fee");
					if (db_reader.GetString("status") == "Finished") {
						ti.TimeTagLabel.Content = "Finished: " + DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt") + " " + DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd yyyy");
					}
					else {
						ti.TimeTagLabel.Content = "Placed: " + DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt") + " " + DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd yyyy");
					}
					ti.ORNumberLabel.Content = db_reader.GetString("or_number");
					//ti.TimeTagLabel2.Content = DateTime.Parse(db_reader.GetString("tran_time")).ToString("hh:mm tt") + " " + DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd yyyy");
					TransactionsItemContainer.Items.Add(ti);
				}
				//close Connection
				dbman.DBClose();
				SyncStat();
			}
			else
			{

			}

		}
		internal string GetRecordName(string rid) {
			string ret = "";
			dbman2 = new DBConnectionManager();

			if (dbman2.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd2 = dbman2.DBConnect().CreateCommand();
				cmd2.CommandText = "SELECT * FROM records WHERE record_id = @record_id LIMIT 1;";
				cmd2.Parameters.AddWithValue("@record_id", rid);
				cmd2.Prepare();
				MySqlDataReader db_reader2 = cmd2.ExecuteReader();
				while (db_reader2.Read())
				{
					ret = db_reader2.GetString("recordholder_fullname");
				}
				//close Connection
				dbman2.DBClose();
			}
			else
			{
				ret = "";
			}
			return ret;
		}
		/// <summary>
		/// Onclick event for the ManualSyncButton. Calls SyncRequest to manually update the
		/// Request list.
		/// </summary>
		private void ManualSyncButton_Click(object sender, RoutedEventArgs e)
		{
			SyncTransactions();
		}
		/// <summary>
		/// Onclick event for the Paying Filter Button. Filters the Requests to show requests
		/// that are Paying.
		/// </summary>
		private void ShowPaying_Click(object sender, RoutedEventArgs e)
		{
			dbman = new DBConnectionManager();

			TransactionsItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM transactions WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Paying");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					TransactionItem ti = new TransactionItem();
					ti.IDLabel.Content = db_reader.GetString("transaction_id");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.StatusLabel.Content = db_reader.GetString("status");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.FeeLabel.Content = db_reader.GetString("fee");
					ti.TimeTagLabel.Content = DateTime.Parse(db_reader.GetString("tran_time")).ToString("hh:mm tt") + " " + DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd yyyy");
					TransactionsItemContainer.Items.Add(ti);
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

			TransactionsItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM transactions WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Finished");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					TransactionItem ti = new TransactionItem();
					ti.IDLabel.Content = db_reader.GetString("transaction_id");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.StatusLabel.Content = db_reader.GetString("status");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.FeeLabel.Content = db_reader.GetString("fee");
					ti.TimeTagLabel.Content = DateTime.Parse(db_reader.GetString("tran_time")).ToString("hh:mm tt") + " " + DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd yyyy");
					TransactionsItemContainer.Items.Add(ti);
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

			TransactionsItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM transactions WHERE status = @status;";
				cmd.Parameters.AddWithValue("@status", "Cancelled");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					TransactionItem ti = new TransactionItem();
					ti.IDLabel.Content = db_reader.GetString("transaction_id");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.StatusLabel.Content = db_reader.GetString("status");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.FeeLabel.Content = db_reader.GetString("fee");
					ti.TimeTagLabel.Content = DateTime.Parse(db_reader.GetString("tran_time")).ToString("hh:mm tt") + " " + DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd yyyy");
					TransactionsItemContainer.Items.Add(ti);
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
				SyncTransactions();
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
		private void SearchTransactionBox_TextChanged(object sender, TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			TransactionsItemContainer.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM transactions WHERE " +
					"transaction_id LIKE @query OR " +
					"type LIKE @query OR " +
					"status LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + SearchTransactionBox.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					TransactionItem ti = new TransactionItem();
					ti.IDLabel.Content = db_reader.GetString("transaction_id");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.StatusLabel.Content = db_reader.GetString("status");
					ti.TypeLabel.Content = db_reader.GetString("type");
					ti.FeeLabel.Content = db_reader.GetString("fee");
					ti.TimeTagLabel.Content = DateTime.Parse(db_reader.GetString("tran_time")).ToString("hh:mm tt") + " " + DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd yyyy");
					TransactionsItemContainer.Items.Add(ti);
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}

		private async void ConfirmPayment_Click(object sender, RoutedEventArgs e)
		{
			TransactionItem ti = (TransactionItem)TransactionsItemContainer.SelectedItem;
			Label transactionID = (Label)ti.FindName("IDLabel");

			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
				cmd.Parameters.AddWithValue("@transaction_id", transactionID.Content.ToString());
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("status") == "Finished")
					{
						MsgAlreadyPaid();
					}
					else if (db_reader.GetString("status") == "Cancelled")
					{
						//MessageBox.Show("Already cancelled!");
						MsgCancelled();
					}
					else
					{
						var metroWindow = (Application.Current.MainWindow as MetroWindow);
						await metroWindow.ShowChildWindowAsync(new ConfirmPaymentWindow(this, transactionID.Content.ToString()));
					}
				}
			}
			else
			{

			}
			dbman.DBClose();
		}
		private async void MsgCancelled()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "The selected transaction has already been cancelled.");
		}
		private async void MsgAlreadyPaid()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "The selected transaction has already been finished.");
		}
		private async void CancelTransaction_Click(object sender, RoutedEventArgs e)
		{
			TransactionItem ti = (TransactionItem)TransactionsItemContainer.SelectedItem;
			Label transactionID = (Label)ti.FindName("IDLabel");

			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
				cmd.Parameters.AddWithValue("@transaction_id", transactionID.Content.ToString());
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("status") == "Finished")
					{
						MsgAlreadyPaid();
					}
					else if (db_reader.GetString("status") == "Cancelled")
					{
						MsgCancelled();
					}
					else
					{
						var metroWindow = (Application.Current.MainWindow as MetroWindow);
						await metroWindow.ShowChildWindowAsync(new CancelPaymentWindow(this, transactionID.Content.ToString()));
					}
				}
			}
			else
			{

			}
			dbman.DBClose();
		}
	}
}