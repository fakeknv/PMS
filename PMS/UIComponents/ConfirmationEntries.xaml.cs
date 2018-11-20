using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for ConfirmationEntries.xaml
    /// </summary>
    public partial class ConfirmationEntries : UserControl
    {
		//MYSQL
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private int bnum, pnum;

		private string cmd_tmp;
		private string qry;

		private ObservableCollection<RecordEntryConfirmation> records;

        public ConfirmationEntries(int bookNum, int pageNum)
        {
			bnum = bookNum;
			pnum = pageNum;
            InitializeComponent();
			SyncConfirmationEntries(bookNum, pageNum);
        }
		private void SyncConfirmationEntries(int targBook, int pageNum)
		{
			records = new ObservableCollection<RecordEntryConfirmation>();

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT * FROM registers WHERE book_number = @book_number LIMIT 1;";
						cmd.Parameters.AddWithValue("@book_number", targBook);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (db_reader.GetString("status") == "Archived")
								{

									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										MySqlCommand cmd2 = conn3.CreateCommand();
										cmd2.CommandText = "SELECT * FROM records, confirmation_records WHERE records.book_number = @book_number AND records.page_number = @page_number AND records.record_id = confirmation_records.record_id ORDER BY records.entry_number ASC;"; cmd2.CommandText = cmd_tmp;
										cmd2.Parameters.AddWithValue("@book_number", targBook);
										cmd2.Parameters.AddWithValue("@page_number", pageNum);
										cmd2.Prepare();

										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
												{
													records.Add(new RecordEntryConfirmation()
													{
														RecordID = db_reader2.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														ConfirmationYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														ConfirmationDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName = db_reader2.GetString("recordholder_fullname"),
														Age = 0,
														Parish = "----",
														Province = "----",
														PlaceOfBaptism = "----",
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname")
													});
												});
											}
										}
									}
								}
								else
								{
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										MySqlCommand cmd2 = conn3.CreateCommand();
										cmd2.CommandText = "SELECT * FROM records, confirmation_records WHERE records.book_number = @book_number AND records.page_number = @page_number AND records.record_id = confirmation_records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@book_number", targBook);
										cmd2.Parameters.AddWithValue("@page_number", pageNum);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
												{
													records.Add(new RecordEntryConfirmation()
													{
														RecordID = db_reader2.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														ConfirmationYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														ConfirmationDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName = db_reader2.GetString("recordholder_fullname"),
														Age = db_reader2.GetInt32("age"),
														Parish = db_reader2.GetString("parochia"),
														Province = db_reader2.GetString("province"),
														PlaceOfBaptism = db_reader2.GetString("place_of_baptism"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Sponsor1 = db_reader2.GetString("sponsor"),
														Sponsor2 = db_reader2.GetString("sponsor2"),
														Stipend = db_reader2.GetFloat("stipend"),
														Minister = db_reader2.GetString("minister")
													});
												});
											}
										}
									}
								}
							}
						}
					}
					SyncChanges();
					System.Threading.Thread.Sleep(1);
				}
				else
				{

				}
				//close Connection
				conn.Close();
			}
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void Remarks_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryConfirmation record = (RecordEntryConfirmation)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ViewRemarksWindow(record.RecordID), this.ParentGrid);
			}
		}
		private async void Print_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryConfirmation record = (RecordEntryConfirmation)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new PrintConfirmationRecordEntryWindow(record.RecordID));
			}
		}

		private async void Edit_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryConfirmation record = (RecordEntryConfirmation)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new EditConfirmationRecordEntryWindow(record.RecordID));
			}
		}

		private async void History_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryConfirmation record = (RecordEntryConfirmation)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ViewHistoryWindow(record.RecordID));
			}
		}

		private void UpdateContent(object sender, TextChangedEventArgs e)
		{
			cmd_tmp = "SELECT * FROM records, confirmation_records WHERE records.book_number = @book_number AND records.record_id = confirmation_records.record_id AND (records.recordholder_fullname LIKE @query OR records.parent1_fullname LIKE @query OR records.parent2_fullname LIKE @query OR confirmation_records.sponsor LIKE @query OR confirmation_records.sponsor2 LIKE @query) ORDER BY records.entry_number ASC;";
			qry = SearchBox.Text;

			BackgroundWorker worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
			worker.DoWork += AddItems;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			worker.RunWorkerAsync(10000);
		}
		private void SyncChanges() {
			EntriesHolder.Items.Refresh();
			EntriesHolder.ItemsSource = records;
			EntriesHolder.Items.Refresh();
		}
		void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//if (e.UserState != null)
				//EntriesHolder.Items.Add(e.UserState);
		}
		void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			SyncChanges();
		}
		private void AddItems(object sender, DoWorkEventArgs e) {
			records = new ObservableCollection<RecordEntryConfirmation>();

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr())) {
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr())) {
						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT * FROM registers WHERE book_number = @book_number LIMIT 1;";
						cmd.Parameters.AddWithValue("@book_number", bnum);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (db_reader.GetString("status") == "Archived")
								{

									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr())) {
										conn3.Open();
										MySqlCommand cmd2 = conn3.CreateCommand();
										cmd2.CommandText = cmd_tmp;
										cmd2.Parameters.AddWithValue("@book_number", bnum);
										cmd2.Parameters.AddWithValue("@query", "%" + qry + "%");
										cmd2.Prepare();

										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
												{
													records.Add(new RecordEntryConfirmation()
													{
														RecordID = db_reader2.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														ConfirmationYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														ConfirmationDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName = db_reader2.GetString("recordholder_fullname"),
														Age = db_reader2.GetInt32("age"),
														Parish = db_reader2.GetString("parochia"),
														Province = db_reader2.GetString("province"),
														PlaceOfBaptism = db_reader2.GetString("place_of_baptism"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname")
													});
												});
											}
										}
									}
								}
								else
								{
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										MySqlCommand cmd2 = conn3.CreateCommand();
										cmd2.CommandText = cmd_tmp;
										cmd2.Parameters.AddWithValue("@book_number", bnum);
										cmd2.Parameters.AddWithValue("@query", "%" + qry + "%");
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
												{
													records.Add(new RecordEntryConfirmation()
													{
														RecordID = db_reader2.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														ConfirmationYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														ConfirmationDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName = db_reader2.GetString("recordholder_fullname"),
														Age = db_reader2.GetInt32("age"),
														Parish = db_reader2.GetString("parochia"),
														Province = db_reader2.GetString("province"),
														PlaceOfBaptism = db_reader2.GetString("place_of_baptism"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Sponsor1 = db_reader2.GetString("sponsor"),
														Sponsor2 = db_reader2.GetString("sponsor2"),
														Stipend = db_reader2.GetFloat("stipend"),
														Minister = db_reader2.GetString("minister")
													});
												});
											}
										}
									}
								}
							}
						}
					}
					(sender as BackgroundWorker).ReportProgress(0);
					System.Threading.Thread.Sleep(1);
				}
				else
				{

				}
				//close Connection
				//conn.Close();
			}
		}
	}
}
