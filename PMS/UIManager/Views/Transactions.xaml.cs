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
using System.Collections.ObjectModel;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Transactions.xaml
	/// </summary>
	public partial class Transactions : UserControl
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;

		private DBConnectionManager dbman;
		private PMSUtil pmsutil;
		private DBConnectionManager dbman2;

		private ObservableCollection<Transaction> transactions;
		private ObservableCollection<Transaction> transactions_final;

		public Transactions()
		{
			InitializeComponent();

			SyncTransactions();
			SyncStat();

			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		/// <summary>
		/// Updates Stats
		/// </summary>
		private void SyncStat()
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Counts Total
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions;";
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						TotalRequestsHolder.Content = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();

					conn.Open();
					//Counts Baptismal
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
					cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						StatBaptismalHolder.Content = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();

					conn.Open();
					//Counts Confirmation
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
					cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						StatConfirmationHolder.Content = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();

					conn.Open();
					//Counts Burial
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
					cmd.Parameters.AddWithValue("@type", "Burial Cert.");
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						StatBurialHolder.Content = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();

					conn.Open();
					//Counts Marriage
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
					cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						StatMatrimonialHolder.Content = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();

					conn.Open();
					//Counts Paying
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status;";
					cmd.Parameters.AddWithValue("@status", "Unpaid");
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						BadgePaying.Badge = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();

					conn.Open();
					//Counts Finished
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status;";
					cmd.Parameters.AddWithValue("@status", "Paid");
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						BadgeFinished.Badge = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();

					conn.Open();
					//Counts Cancelled
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status;";
					cmd.Parameters.AddWithValue("@status", "Cancelled");
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						BadgeCancelled.Badge = db_reader.GetString("COUNT(*)");
					}
					//close Connection
					conn.Close();
				}
			}
		}
		/// <summary>
		/// Updates the Requests list.
		/// </summary>
		internal void SyncTransactions()
		{
			transactions = new ObservableCollection<Transaction>();
			transactions_final = new ObservableCollection<Transaction>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					if (TransacType.SelectedIndex == 0)
					{
						cmd.CommandText = "SELECT * FROM transactions WHERE type LIKE '%Cert.%' ORDER BY tran_date DESC , tran_time DESC;";
					}
					else {
						cmd.CommandText = "SELECT * FROM transactions WHERE type LIKE '%Serv.%' ORDER BY tran_date DESC , tran_time DESC;";
					}
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string recname = "";
						if (db_reader.GetString("target_id").Substring(0, 3) == "PMS")
						{
							recname = GetRecordName(db_reader.GetString("target_id"));
						}
						else
						{

						}
						string dateFinished = "";
						string timeFinished = "";
						if (db_reader.GetString("status") == "Unpaid")
						{
							dateFinished = " ";
							timeFinished = " ";
						}
						else {
							dateFinished = DateTime.Parse(db_reader.GetString("completion_date")).ToString("MMMM dd, yyyy");
							timeFinished = DateTime.Parse(db_reader.GetString("completion_time")).ToString("h:mm tt");
						}
						transactions.Add(new Transaction()
						{
							TransactionID = db_reader.GetString("transaction_id"),
							Type = db_reader.GetString("type"),
							Name = recname,
							Fee = Convert.ToDouble(string.Format("{0:N3}", db_reader.GetDouble("fee"))),
							Status = db_reader.GetString("status"),
							ORNumber = db_reader.GetString("or_number"),
							DatePlaced = DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd, yyyy"),
							TimePlaced = DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt"),
							DateConfirmed = dateFinished,
							TimeConfirmed = timeFinished,
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					int temp = 1;
					foreach (var cur in transactions)
					{
						if (cur.Page == CurrentPage.Value)
						{
							transactions_final.Add(new Transaction()
							{
								No = temp,
								TransactionID = cur.TransactionID,
								Type = cur.Type,
								Name = cur.Name,
								Fee = cur.Fee,
								Status = cur.Status,
								ORNumber = cur.ORNumber,
								DatePlaced = cur.DatePlaced,
								TimePlaced = cur.TimePlaced,
								DateConfirmed = cur.DateConfirmed,
								TimeConfirmed = cur.TimeConfirmed,
								Page = cur.Page
							});
							temp++;
						}
					}
					//close Connection
					conn.Close();

					TransactionItemsContainer.Items.Refresh();
					TransactionItemsContainer.ItemsSource = transactions_final;
					TransactionItemsContainer.Items.Refresh();
					CurrentPage.Maximum = page;
				}
				else
				{

				}
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
			var bc = new BrushConverter();
			PayingFilterLabel1.Foreground = (Brush)bc.ConvertFrom("#FFFFFFFF");
			PayingFilterLabel1.Background = (Brush)bc.ConvertFrom("#FF119EDA");

			CancelledFilterLabel.Foreground = (Brush)bc.ConvertFrom("#FF545454");
			CancelledFilterLabel.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");

			FinishedFilterLabel.Foreground = (Brush)bc.ConvertFrom("#FF545454");
			FinishedFilterLabel.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");

			transactions = new ObservableCollection<Transaction>();
			transactions_final = new ObservableCollection<Transaction>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM transactions WHERE status = @status ORDER BY tran_date DESC , tran_time DESC;";
					cmd.Parameters.AddWithValue("@status", "Unpaid");
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string recname = "";
						if (db_reader.GetString("target_id").Substring(0, 3) == "PMS")
						{
							recname = GetRecordName(db_reader.GetString("target_id"));
						}
						else
						{

						}
						string dateFinished = "";
						string timeFinished = "";
						if (db_reader.GetString("status") == "Unpaid")
						{
							dateFinished = " ";
							timeFinished = " ";
						}
						else
						{
							dateFinished = DateTime.Parse(db_reader.GetString("completion_date")).ToString("MMMM dd, yyyy");
							timeFinished = DateTime.Parse(db_reader.GetString("completion_time")).ToString("h:mm tt");
						}
						transactions.Add(new Transaction()
						{
							TransactionID = db_reader.GetString("transaction_id"),
							Type = db_reader.GetString("type"),
							Name = recname,
							Fee = Convert.ToDouble(string.Format("{0:N3}", db_reader.GetDouble("fee"))),
							Status = db_reader.GetString("status"),
							ORNumber = db_reader.GetString("or_number"),
							DatePlaced = DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd, yyyy"),
							TimePlaced = DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt"),
							DateConfirmed = dateFinished,
							TimeConfirmed = timeFinished,
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					int num = 1;
					foreach (var cur in transactions)
					{
						if (cur.Page == CurrentPage.Value)
						{
							transactions_final.Add(new Transaction()
							{
								No = num,
								TransactionID = cur.TransactionID,
								Type = cur.Type,
								Name = cur.Name,
								Fee = cur.Fee,
								Status = cur.Status,
								ORNumber = cur.ORNumber,
								DatePlaced = cur.DatePlaced,
								TimePlaced = cur.TimePlaced,
								DateConfirmed = cur.DateConfirmed,
								TimeConfirmed = cur.TimeConfirmed,
								Page = cur.Page
							});
							num++;
						}
					}
					//close Connection
					conn.Close();

					TransactionItemsContainer.Items.Refresh();
					TransactionItemsContainer.ItemsSource = transactions_final;
					TransactionItemsContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// Onclick event for the Finished Filter Button. Filters the Requests to show requests
		/// that are Finished.
		/// </summary>
		private void ShowFinished_Click(object sender, RoutedEventArgs e)
		{
			var bc = new BrushConverter();
			FinishedFilterLabel.Foreground = (Brush)bc.ConvertFrom("#FFFFFFFF");
			FinishedFilterLabel.Background = (Brush)bc.ConvertFrom("#FF119EDA");

			PayingFilterLabel1.Foreground = (Brush)bc.ConvertFrom("#FF545454");
			PayingFilterLabel1.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");

			CancelledFilterLabel.Foreground = (Brush)bc.ConvertFrom("#FF545454");
			CancelledFilterLabel.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");

			transactions = new ObservableCollection<Transaction>();
			transactions_final = new ObservableCollection<Transaction>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM transactions WHERE status = @status ORDER BY tran_date DESC , tran_time DESC;";
					cmd.Parameters.AddWithValue("@status", "Paid");
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string recname = "";
						if (db_reader.GetString("target_id").Substring(0, 3) == "PMS")
						{
							recname = GetRecordName(db_reader.GetString("target_id"));
						}
						else
						{

						}
						string dateFinished = "";
						string timeFinished = "";
						if (db_reader.GetString("status") == "Unpaid")
						{
							dateFinished = " ";
							timeFinished = " ";
						}
						else
						{
							dateFinished = DateTime.Parse(db_reader.GetString("completion_date")).ToString("MMMM dd, yyyy");
							timeFinished = DateTime.Parse(db_reader.GetString("completion_time")).ToString("h:mm tt");
						}
						transactions.Add(new Transaction()
						{
							TransactionID = db_reader.GetString("transaction_id"),
							Type = db_reader.GetString("type"),
							Name = recname,
							Fee = Convert.ToDouble(string.Format("{0:N3}", db_reader.GetDouble("fee"))),
							Status = db_reader.GetString("status"),
							ORNumber = db_reader.GetString("or_number"),
							DatePlaced = DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd, yyyy"),
							TimePlaced = DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt"),
							DateConfirmed = dateFinished,
							TimeConfirmed = timeFinished,
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					int num = 1;
					foreach (var cur in transactions)
					{
						if (cur.Page == CurrentPage.Value)
						{
							transactions_final.Add(new Transaction()
							{
								No = num,
								TransactionID = cur.TransactionID,
								Type = cur.Type,
								Name = cur.Name,
								Fee = cur.Fee,
								Status = cur.Status,
								ORNumber = cur.ORNumber,
								DatePlaced = cur.DatePlaced,
								TimePlaced = cur.TimePlaced,
								DateConfirmed = cur.DateConfirmed,
								TimeConfirmed = cur.TimeConfirmed,
								Page = cur.Page
							});
							num++;
						}
					}
					//close Connection
					conn.Close();

					TransactionItemsContainer.Items.Refresh();
					TransactionItemsContainer.ItemsSource = transactions_final;
					TransactionItemsContainer.Items.Refresh();
				}
				else
				{

				}
			}
		}
		/// <summary>
		/// Onclick event for the Cancelled Filter Button. Filters the Requests to show requests
		/// that are Cancelled.
		/// </summary>
		private void ShowCancelled_Click(object sender, RoutedEventArgs e)
		{
			var bc = new BrushConverter();
			CancelledFilterLabel.Foreground = (Brush)bc.ConvertFrom("#FFFFFFFF");
			CancelledFilterLabel.Background = (Brush)bc.ConvertFrom("#FF119EDA");

			PayingFilterLabel1.Foreground = (Brush)bc.ConvertFrom("#FF545454");
			PayingFilterLabel1.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");

			FinishedFilterLabel.Foreground = (Brush)bc.ConvertFrom("#FF545454");
			FinishedFilterLabel.Background = (Brush)bc.ConvertFrom("#FFFFFFFF");

			transactions = new ObservableCollection<Transaction>();
			transactions_final = new ObservableCollection<Transaction>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM transactions WHERE status = @status ORDER BY tran_date DESC , tran_time DESC;";
					cmd.Parameters.AddWithValue("@status", "Cancelled");
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string recname = "";
						if (db_reader.GetString("target_id").Substring(0, 3) == "PMS")
						{
							recname = GetRecordName(db_reader.GetString("target_id"));
						}
						else
						{

						}
						string dateFinished = "";
						string timeFinished = "";
						if (db_reader.GetString("status") == "Unpaid")
						{
							dateFinished = " ";
							timeFinished = " ";
						}
						else
						{
							dateFinished = DateTime.Parse(db_reader.GetString("completion_date")).ToString("MMMM dd, yyyy");
							timeFinished = DateTime.Parse(db_reader.GetString("completion_time")).ToString("h:mm tt");
						}
						transactions.Add(new Transaction()
						{
							TransactionID = db_reader.GetString("transaction_id"),
							Type = db_reader.GetString("type"),
							Name = recname,
							Fee = Convert.ToDouble(string.Format("{0:N3}", db_reader.GetDouble("fee"))),
							Status = db_reader.GetString("status"),
							ORNumber = db_reader.GetString("or_number"),
							DatePlaced = DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd, yyyy"),
							TimePlaced = DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt"),
							DateConfirmed = dateFinished,
							TimeConfirmed = timeFinished,
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					int num = 1;
					foreach (var cur in transactions)
					{
						if (cur.Page == CurrentPage.Value)
						{
							transactions_final.Add(new Transaction()
							{
								No = num,
								TransactionID = cur.TransactionID,
								Type = cur.Type,
								Name = cur.Name,
								Fee = cur.Fee,
								Status = cur.Status,
								ORNumber = cur.ORNumber,
								DatePlaced = cur.DatePlaced,
								TimePlaced = cur.TimePlaced,
								DateConfirmed = cur.DateConfirmed,
								TimeConfirmed = cur.TimeConfirmed,
								Page = cur.Page
							});
							num++;
						}
					}
					//close Connection
					conn.Close();

					TransactionItemsContainer.Items.Refresh();
					TransactionItemsContainer.ItemsSource = transactions_final;
					TransactionItemsContainer.Items.Refresh();
				}
				else
				{

				}
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
			if (string.IsNullOrEmpty(SearchTransactionBox.Text)) {
				SyncTransactions();
			}
			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			ObservableCollection<Transaction> results = new ObservableCollection<Transaction>();
			System.Collections.IList items = TransactionItemsContainer.Items;
			for (int i = 0; i < items.Count - 1; i++)
			{
				Transaction item = (Transaction)items[i];
				if (item.Name.Contains(SearchTransactionBox.Text) == true || item.ORNumber.Contains(SearchTransactionBox.Text) || item.Status.Contains(SearchTransactionBox.Text) || item.Type.Contains(SearchTransactionBox.Text))
				{
					results.Add(new Transaction()
					{
						TransactionID = item.TransactionID,
						Type = item.Type,
						Name = item.Name,
						Fee = item.Fee,
						Status = item.Status,
						ORNumber = item.ORNumber,
						DatePlaced = item.DatePlaced,
						TimePlaced = item.TimePlaced,
						DateConfirmed = item.DateConfirmed,
						TimeConfirmed = item.TimeConfirmed,
						Page = page
					});
					count++;
					if (count == itemsPerPage)
					{
						page++;
						count = 0;
					}
				}
			}
			TransactionItemsContainer.Items.Refresh();
			TransactionItemsContainer.ItemsSource = results;
			TransactionItemsContainer.Items.Refresh();
		}

		private async void ConfirmPayment_Click(object sender, RoutedEventArgs e)
		{
			if (TransactionItemsContainer.SelectedItems.Count == 1)
			{
				Transaction ti = (Transaction)TransactionItemsContainer.SelectedItem;
				//Label transactionID = (Label)ti.FindName("IDLabel");

				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						MySqlCommand cmd = conn.CreateCommand();
						cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
						cmd.Parameters.AddWithValue("@transaction_id", ti.TransactionID);
						cmd.Prepare();
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							if (db_reader.GetString("status") == "Paid")
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
								await metroWindow.ShowChildWindowAsync(new ConfirmPaymentWindow(this, ti.TransactionID));
							}
						}
					}
				}
			}
			else
			{
				Transaction transaction = (Transaction)TransactionItemsContainer.SelectedItem;

				if (transaction == null)
				{
					MsgNoItemSelected();
				}
				else
				{
					int doProceed = 0;
					for (int i = 0; i < TransactionItemsContainer.SelectedItems.Count; i++)
					{
						Transaction recordx = (Transaction)TransactionItemsContainer.SelectedItems[i];
						if (CheckTrans(recordx.TransactionID) == 0)
						{
							doProceed = 0;
						}
						else if (CheckTrans(recordx.TransactionID) == 1)
						{
							doProceed = 1;
						}
						else if (CheckTrans(recordx.TransactionID) == 2)
						{
							doProceed = 2;
						}
					}

					if (doProceed == 0)
					{
						MsgAlreadyPaid();
					}
					else if (doProceed == 1)
					{
						MsgCancelled();
					}
					else
					{
						var metroWindow = (Application.Current.MainWindow as MetroWindow);
						await metroWindow.ShowChildWindowAsync(new ConfirmPaymentWindow(this, TransactionItemsContainer.SelectedItems));
					}
				}
			}
			
		}
		private int CheckTrans(string tid)
		{
			int ret = 2;
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
					cmd.Parameters.AddWithValue("@transaction_id", tid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetString("status") == "Paid")
						{
							ret = 0;
						}
						else if (db_reader.GetString("status") == "Cancelled")
						{
							ret = 1;
						}
						else
						{
							
						}
					}
				}
			}
			return ret;
		}
		private async void MsgCancelled()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "The selected transaction has already been cancelled.");
		}
		private async void MsgAlreadyPaid()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "The selected transaction has already been paid.");
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncTransactions();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncTransactions();
		}
		private async void CancelTransaction_Click(object sender, RoutedEventArgs e)
		{
			if (TransactionItemsContainer.SelectedItem == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				Transaction ti = (Transaction)TransactionItemsContainer.SelectedItem;
				//Label transactionID = (Label)ti.FindName("IDLabel");

				dbman = new DBConnectionManager();
				if (dbman.DBConnect().State == ConnectionState.Open)
				{
					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
					cmd.Parameters.AddWithValue("@transaction_id", ti.TransactionID);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetString("status") == "Paid")
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
							await metroWindow.ShowChildWindowAsync(new CancelPaymentWindow(this, ti.TransactionID));
						}
					}
				}
				else
				{

				}
			}
		}

		private void TransacType_DropDownClosed(object sender, EventArgs e)
		{
			SyncTransactions();
		}

		private async void FilterHelp_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Filters Help", "The buttons below the 'Filters' tab filters out transactions depending on their status. For example, clicking the 'Unpaid Button' will show transactions that are still pending/unpaid.");
		}
		private async void TransTypeHelp_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Transaction Type Help", "This filters out transactions depending on their type. Selecting 'Certificates' will list certificate retrieval related transactions, while selecting 'Scheduling' will list transactions related to the scheduling module.");
		}

		private async void ActionsHelp_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Actions Help", "The 'Confirm Button' confirms the selected transaction. The 'Cancel Button' on the other hand, cancels the selected transaction.");
		}
	}
}