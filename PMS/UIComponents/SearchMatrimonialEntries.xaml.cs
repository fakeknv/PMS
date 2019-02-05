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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for ConfirmationEntries.xaml
    /// </summary>
    public partial class SearchMatrimonialEntries : UserControl
    {
		//MYSQL
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private string _query, _type;
		private int _coverage;

		private PMSUtil pmsutil;

		private ObservableCollection<RecordEntryMatrimonial> records;
		private ObservableCollection<RecordEntryMatrimonial> records_final;

		public SearchMatrimonialEntries(string query, string type, int coverage)
        {
			_query = query;
			_type = type;
			_coverage = coverage;
			InitializeComponent();
			SyncMatrimonialEntries(query, type);

			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private void SyncMatrimonialEntries(string query, string type)
		{
			pmsutil = new PMSUtil();
			records = new ObservableCollection<RecordEntryMatrimonial>();
			records_final = new ObservableCollection<RecordEntryMatrimonial>();

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
											string stm = "SELECT * FROM matrimonial_records WHERE record_id='" + db_reader2.GetString("record_id") + "';";

											using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
											{
												using (SQLiteDataReader rdr = cmdx.ExecuteReader())
												{
													while (rdr.Read())
													{
														App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
														{
															records.Add(new RecordEntryMatrimonial()
															{
																RecordID = db_reader2.GetString("record_id"),
																EntryNumber = db_reader2.GetInt32("entry_number"),
																MarriageYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
																MarriageDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
																FullName1 = db_reader2.GetString("recordholder_fullname"),
																FullName2 = rdr["recordholder2_fullname"].ToString(),
																Status1 = rdr["status1"].ToString(),
																Status2 = rdr["status2"].ToString(),
																Age1 = Convert.ToInt32(rdr["age1"]),
																Age2 = Convert.ToInt32(rdr["age2"]),
																Hometown1 = rdr["place_of_origin1"].ToString(),
																Hometown2 = rdr["place_of_origin2"].ToString(),
																Residence1 = rdr["residence1"].ToString(),
																Residence2 = rdr["residence2"].ToString(),
																Parent1 = db_reader2.GetString("parent1_fullname"),
																Parent2 = db_reader2.GetString("parent2_fullname"),
																Parent3 = rdr["parent1_fullname2"].ToString(),
																Parent4 = rdr["parent2_fullname2"].ToString(),
																Witness1 = rdr["witness1"].ToString(),
																Witness2 = rdr["witness2"].ToString(),
																W1Residence = rdr["witness1address"].ToString(),
																W2Residence = rdr["witness2address"].ToString(),
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
											records.Add(new RecordEntryMatrimonial()
											{
												RecordID = db_reader2.GetString("record_id"),
												EntryNumber = db_reader2.GetInt32("entry_number"),
												MarriageYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
												MarriageDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
												FullName1 = db_reader2.GetString("recordholder_fullname"),
												FullName2 = "---",
												Status1 = "---",
												Status2 = "---",
												Age1 = 0,
												Age2 = 0,
												Hometown1 = "---",
												Hometown2 = "---",
												Residence1 = "---",
												Residence2 = "---",
												Parent1 = "---",
												Parent2 = "---",
												Parent3 = "---",
												Parent4 = "---",
												Witness1 = "---",
												Witness2 = "---",
												W1Residence = "---",
												W2Residence = "---",
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
										cmd3.CommandText = "SELECT DISTINCT * FROM matrimonial_records WHERE record_id = @rid;";
										cmd3.Parameters.AddWithValue("@rid", db_reader2.GetString("record_id"));
										cmd3.Prepare();
										using (MySqlDataReader db_reader3 = cmd3.ExecuteReader())
										{
											while (db_reader3.Read())
											{
												App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
												{
													records.Add(new RecordEntryMatrimonial()
													{
														RecordID = db_reader2.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														MarriageYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														MarriageDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName1 = db_reader2.GetString("recordholder_fullname"),
														FullName2 = db_reader3.GetString("recordholder2_fullname"),
														Status1 = db_reader3.GetString("status1"),
														Status2 = db_reader3.GetString("status2"),
														Age1 = db_reader3.GetInt32("age1"),
														Age2 = db_reader3.GetInt32("age2"),
														Hometown1 = db_reader3.GetString("place_of_origin1"),
														Hometown2 = db_reader3.GetString("place_of_origin2"),
														Residence1 = db_reader3.GetString("residence1"),
														Residence2 = db_reader3.GetString("residence2"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Parent3 = db_reader3.GetString("parent1_fullname2"),
														Parent4 = db_reader3.GetString("parent2_fullname2"),
														Witness1 = db_reader3.GetString("witness1"),
														Witness2 = db_reader3.GetString("witness2"),
														W1Residence = db_reader3.GetString("witness1address"),
														W2Residence = db_reader3.GetString("witness2address"),
														Stipend = db_reader3.GetFloat("stipend"),
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

							records_final.Add(new RecordEntryMatrimonial()
							{
								RecordID = cur.RecordID,
								EntryNumber = cur.EntryNumber,
								MarriageYear = cur.MarriageYear,
								MarriageDate = cur.MarriageDate,
								FullName1 = cur.FullName1,
								FullName2 = cur.FullName2,
								Status1 = cur.Status1,
								Status2 = cur.Status2,
								Age1 = cur.Age1,
								Age2 = cur.Age2,
								Hometown1 = cur.Hometown1,
								Hometown2 = cur.Hometown2,
								Residence1 = cur.Residence1,
								Residence2 = cur.Residence2,
								Parent1 = cur.Parent1,
								Parent2 = cur.Parent2,
								Parent3 = cur.Parent3,
								Parent4 = cur.Parent4,
								Witness1 = cur.Witness1,
								Witness2 = cur.Witness2,
								W1Residence = cur.W1Residence,
								W2Residence = cur.W2Residence,
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
				else {

				}
			}
		}
		private async void MsgNoItemSelected()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Oops!", "There is no item selected. Please try again.");
		}
		private async void Remarks_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryMatrimonial record = (RecordEntryMatrimonial)EntriesHolder.SelectedItem;
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
			if (EntriesHolder.SelectedItems.Count == 1)
			{
				RecordEntryMatrimonial record = (RecordEntryMatrimonial)EntriesHolder.SelectedItem;

				if (record == null)
				{
					MsgNoItemSelected();
				}
				else
				{
					var metroWindow = (Application.Current.MainWindow as MetroWindow);
					await metroWindow.ShowChildWindowAsync(new PrintMatrimonialRecordEntryWindow(record.RecordID));
				}
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ConfirmBatchPrintWindow(EntriesHolder.SelectedItems, "Matrimonial"));
			}
		}
		private async void Edit_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryMatrimonial record = (RecordEntryMatrimonial)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new EditMatrimonialRecordEntryWindow(record.RecordID));
			}
		}
		private async void History_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryMatrimonial record = (RecordEntryMatrimonial)EntriesHolder.SelectedItem;
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
		private void SyncChanges()
		{
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
			SyncMatrimonialEntries(_query, _type);
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncMatrimonialEntries(_query, _type);
		}
		private void AddItems(object sender, DoWorkEventArgs e)
		{
			
		}
	}
}
