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
    public partial class MatrimonialEntries : UserControl
    {
		//MYSQL
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private int bnum, pnum;

		private string qry;

		private PMSUtil pmsutil;

		private ObservableCollection<RecordEntryMatrimonial> records;

		public MatrimonialEntries(int bookNum, int pageNum)
        {
			bnum = bookNum;
			pnum = pageNum;
            InitializeComponent();
			SyncMatrimonialEntries(bookNum, pageNum);
		}
		private void SyncMatrimonialEntries(int targBook, int pageNum)
		{
			records = new ObservableCollection<RecordEntryMatrimonial>();

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
									MsgArchived();
									EditRegButton.IsEnabled = false;
									PrintRegButton.IsEnabled = false;
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										MySqlCommand cmd2 = conn3.CreateCommand();
										cmd2.CommandText = "SELECT * FROM records WHERE records.book_number = @book_number AND records.page_number = @page_number ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@book_number", targBook);
										cmd2.Parameters.AddWithValue("@page_number", pageNum);
										cmd2.Prepare();

										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											string archiveDrive = "init";
											string path = @"\archive.db";
											while (db_reader2.Read())
											{

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
																EditRegButton.IsEnabled = true;
																PrintRegButton.IsEnabled = true;
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
																			Minister = rdr["minister"].ToString()
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
															Minister = "---"
														});
													});
												}
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
										cmd2.CommandText = "SELECT * FROM records, matrimonial_records WHERE records.book_number = @book_number AND records.page_number = @page_number  AND records.record_id = matrimonial_records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@book_number", targBook);
										cmd2.Parameters.AddWithValue("@page_number", pageNum);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
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
														FullName2 = db_reader2.GetString("recordholder2_fullname"),
														Status1 = db_reader2.GetString("status1"),
														Status2 = db_reader2.GetString("status2"),
														Age1 = db_reader2.GetInt32("age1"),
														Age2 = db_reader2.GetInt32("age2"),
														Hometown1 = db_reader2.GetString("place_of_origin1"),
														Hometown2 = db_reader2.GetString("place_of_origin2"),
														Residence1 = db_reader2.GetString("residence1"),
														Residence2 = db_reader2.GetString("residence2"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Parent3 = db_reader2.GetString("parent1_fullname2"),
														Parent4 = db_reader2.GetString("parent2_fullname2"),
														Witness1 = db_reader2.GetString("witness1"),
														Witness2 = db_reader2.GetString("witness2"),
														W1Residence = db_reader2.GetString("witness1address"),
														W2Residence = db_reader2.GetString("witness2address"),
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
				//conn.Close();
			}
		}
		private async void MsgArchived()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Notice!", "This register is already archived. Editing is disabled unless archive drive is connected.");
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
		private void SyncChanges()
		{
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
		private void AddItems(object sender, DoWorkEventArgs e)
		{
			records = new ObservableCollection<RecordEntryMatrimonial>();

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
						cmd.Parameters.AddWithValue("@book_number", bnum);
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
										cmd2.CommandText = "SELECT * FROM records, matrimonial_records WHERE records.book_number = @book_number AND (records.recordholder_fullname LIKE @query OR records.parent1_fullname LIKE @query OR records.parent2_fullname LIKE @query ) GROUP BY records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@book_number", bnum);
										cmd2.Parameters.AddWithValue("@query", "%" + qry + "%");
										cmd2.Prepare();

										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											string archiveDrive = "init";
											string path = @"\archive.db";
											while (db_reader2.Read())
											{

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
																			Minister = rdr["minister"].ToString()
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
															Minister = "---"
														});
													});
												}
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
										cmd2.CommandText = "SELECT * FROM records, matrimonial_records WHERE records.book_number = @book_number AND records.record_id = matrimonial_records.record_id AND (records.recordholder_fullname LIKE @query OR matrimonial_records.recordholder2_fullname LIKE @query OR records.parent1_fullname LIKE @query OR records.parent2_fullname LIKE @query OR matrimonial_records.parent1_fullname2 LIKE @query OR matrimonial_records.parent2_fullname2 LIKE @query OR matrimonial_records.witness1 LIKE @query OR matrimonial_records.witness2 LIKE @query) GROUP BY records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@book_number", bnum);
										cmd2.Parameters.AddWithValue("@query", "%" + qry + "%");
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
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
														FullName2 = db_reader2.GetString("recordholder2_fullname"),
														Status1 = db_reader2.GetString("status1"),
														Status2 = db_reader2.GetString("status2"),
														Age1 = db_reader2.GetInt32("age1"),
														Age2 = db_reader2.GetInt32("age2"),
														Hometown1 = db_reader2.GetString("place_of_origin1"),
														Hometown2 = db_reader2.GetString("place_of_origin2"),
														Residence1 = db_reader2.GetString("residence1"),
														Residence2 = db_reader2.GetString("residence2"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Parent3 = db_reader2.GetString("parent1_fullname2"),
														Parent4 = db_reader2.GetString("parent2_fullname2"),
														Witness1 = db_reader2.GetString("witness1"),
														Witness2 = db_reader2.GetString("witness2"),
														W1Residence = db_reader2.GetString("witness1address"),
														W2Residence = db_reader2.GetString("witness2address"),
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
