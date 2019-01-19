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
using System.Data.SQLite;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for ConfirmationEntries.xaml
    /// </summary>
    public partial class SearchConfirmationEntries : UserControl
    {
		//MYSQL
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private string _query, _type;

		private PMSUtil pmsutil;

		private ObservableCollection<RecordEntryConfirmation> records;
		private ObservableCollection<RecordEntryConfirmation> records_final;

		public SearchConfirmationEntries(string query, string type)
        {
			_query = query;
			_type = type;
            InitializeComponent();
			SyncConfirmationEntries(query, type);

			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private void SyncConfirmationEntries(string query, string type)
		{
			pmsutil = new PMSUtil();
			records = new ObservableCollection<RecordEntryConfirmation>();
			records_final = new ObservableCollection<RecordEntryConfirmation>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						conn2.Open();
						MySqlCommand cmd2 = conn2.CreateCommand();
						cmd2.CommandText = "SELECT DISTINCT * FROM records, registers WHERE registers.book_type = @book_type AND records.book_number = registers.book_number AND " +
						"(records.recordholder_fullname LIKE @query OR " +
						"records.parent1_fullname LIKE @query OR " +
						"records.parent2_fullname LIKE @query) ORDER BY records.entry_number ASC;";
						cmd2.Parameters.AddWithValue("@book_type", type);
						cmd2.Parameters.AddWithValue("@query", "%" + query + "%");
						cmd2.Prepare();

						using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
						{
							string archiveDrive = "init";

							while (db_reader2.Read())
							{
								if (db_reader2.GetString("status") == "Archived")
								{
									string path = @"\archive.db";
									pmsutil = new PMSUtil();
									if (pmsutil.CheckArchiveDrive(path) != "dc")
									{
										archiveDrive = pmsutil.CheckArchiveDrive(path);
										SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
										{
											FailIfMissing = true,
											DataSource = archiveDrive
										};
										using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
										{

											// open the connection:
											connection.Open();
											string stm = "SELECT * FROM confirmation_records WHERE record_id='" + db_reader2.GetString("record_id") + "';";

											using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
											{
												using (SQLiteDataReader rdr = cmdx.ExecuteReader())
												{
													while (rdr.Read())
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
																Age = Convert.ToInt32(rdr["age"]),
																Parish = rdr["parochia"].ToString(),
																Province = rdr["province"].ToString(),
																PlaceOfBaptism = rdr["place_of_baptism"].ToString(),
																Parent1 = db_reader2.GetString("parent1_fullname"),
																Parent2 = db_reader2.GetString("parent2_fullname"),
																Sponsor1 = rdr["sponsor"].ToString(),
																Sponsor2 = rdr["sponsor2"].ToString(),
																Stipend = Convert.ToDouble(rdr["stipend"]),
																Minister = rdr["minister"].ToString(),
																Page = page
															});
														});
													}
												}
											}

										}
									}
									else
									{
										archiveDrive = "init";
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
												Parish = "---",
												Province = "---",
												PlaceOfBaptism = "---",
												Parent1 = db_reader2.GetString("parent1_fullname"),
												Parent2 = db_reader2.GetString("parent2_fullname"),
												Sponsor1 = "---",
												Sponsor2 = "---",
												Stipend = 0,
												Minister = "---",
												Page = page
											});
										});
									}
								}
								else
								{
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										MySqlCommand cmd3 = conn3.CreateCommand();
										cmd3.CommandText = "SELECT DISTINCT * FROM confirmation_records WHERE record_id = @rid;";
										cmd3.Parameters.AddWithValue("@rid", db_reader2.GetString("record_id"));
										cmd3.Prepare();
										using (MySqlDataReader db_reader3 = cmd3.ExecuteReader())
										{
											while (db_reader3.Read())
											{
												App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
												{
													records.Add(new RecordEntryConfirmation()
													{
														RecordID = db_reader3.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														ConfirmationYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														ConfirmationDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName = db_reader2.GetString("recordholder_fullname"),
														Age = db_reader3.GetInt32("age"),
														Parish = db_reader3.GetString("parochia"),
														Province = db_reader3.GetString("province"),
														PlaceOfBaptism = db_reader3.GetString("place_of_baptism"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Sponsor1 = db_reader3.GetString("sponsor"),
														Sponsor2 = db_reader3.GetString("sponsor2"),
														Stipend = db_reader3.GetDouble("stipend"),
														Minister = db_reader3.GetString("minister"),
														Page = page
													});
												});
											}
										}
									}
								}
								count++;
								if (count == itemsPerPage)
								{
									page++;
									count = 0;
								}
							}
						}
					}
					foreach (var cur in records)
					{
						if (cur.Page == CurrentPage.Value)
						{

							records_final.Add(new RecordEntryConfirmation()
							{
								RecordID = cur.RecordID,
								EntryNumber = cur.EntryNumber,
								ConfirmationYear = cur.ConfirmationYear,
								ConfirmationDate = cur.ConfirmationDate,
								FullName = cur.FullName,
								Age = cur.Age,
								Parish = cur.Parish,
								Province = cur.Province,
								PlaceOfBaptism = cur.PlaceOfBaptism,
								Parent1 = cur.Parent1,
								Parent2 = cur.Parent2,
								Sponsor1 = cur.Sponsor1,
								Sponsor2 = cur.Sponsor2,
								Stipend = cur.Stipend,
								Minister = cur.Minister,
								Page = cur.Page
							});
						}
					}

					SyncChanges();
					System.Threading.Thread.Sleep(1);

					CurrentPage.Maximum = page;
				}
				else
				{

				}
			}
		}
		private async void MsgArchiveNotConnected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There selected item is already archived. Please connect the archive drive and try again.");
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void Remarks_Click(object sender, RoutedEventArgs e)
		{
			string path = @"\archive.db";
			RecordEntryConfirmation record = (RecordEntryConfirmation)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else if (pmsutil.IsArchived(record.RecordID) == true && pmsutil.CheckArchiveDrive(path) == "dc") {
				MsgArchiveNotConnected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ViewRemarksWindow(record.RecordID), this.ParentGrid);
			}
		}
		private async void Print_Click(object sender, RoutedEventArgs e)
		{
			if (EntriesHolder.SelectedItems.Count == 1)
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
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ConfirmBatchPrintWindow(EntriesHolder.SelectedItems, "Confirmation"));
			}
		}

		private async void Edit_Click(object sender, RoutedEventArgs e)
		{
			string path = @"\archive.db";
			RecordEntryConfirmation record = (RecordEntryConfirmation)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else if (pmsutil.IsArchived(record.RecordID) == true && pmsutil.CheckArchiveDrive(path) == "dc")
			{
				MsgArchiveNotConnected();
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
			EntriesHolder.ItemsSource = records_final;
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
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncConfirmationEntries(_query, _type);
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncConfirmationEntries(_query, _type);
		}
		private void AddItems(object sender, DoWorkEventArgs e) {
			
		}
	}
}
