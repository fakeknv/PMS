using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIManager.Views.ChildWindows;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for ConfirmationEntries.xaml
    /// </summary>
    public partial class BaptismalEntries : UserControl
    {
		//MYSQL
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private int pnum, bnum;

		private string cmd_tmp;
		private string qry;

		private PMSUtil pmsutil;

		private ObservableCollection<RecordEntryBaptismal> records;

		public BaptismalEntries(int bookNum, int pageNum)
        {
			pnum = pageNum;
			bnum = bookNum;
            InitializeComponent();
			SyncBaptismalEntries(bookNum, pageNum);
		}
		private void SyncBaptismalEntries(int targBook, int pageNum)
		{
			records = new ObservableCollection<RecordEntryBaptismal>();

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
														string stm = "SELECT * FROM baptismal_records WHERE record_id='" + db_reader2.GetString("record_id") + "';";

														using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
														{
															using (SQLiteDataReader rdr = cmdx.ExecuteReader())
															{
																while (rdr.Read())
																{
																	DateTime dateOfBirth = Convert.ToDateTime(rdr["birthday"].ToString());
																	App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
																	{
																		records.Add(new RecordEntryBaptismal()
																		{
																			RecordID = db_reader2.GetString("record_id"),
																			EntryNumber = db_reader2.GetInt32("entry_number"),
																			BaptismalYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
																			BaptismalDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
																			FullName = db_reader2.GetString("recordholder_fullname"),
																			BirthDate = dateOfBirth.ToString("MMM dd, yyyy"),
																			Legitimacy = rdr["legitimacy"].ToString(),
																			PlaceOfBirth = rdr["place_of_birth"].ToString(),
																			Parent1 = db_reader2.GetString("parent1_fullname"),
																			Parent2 = db_reader2.GetString("parent2_fullname"),
																			Godparent1 = rdr["sponsor1"].ToString(),
																			Godparent2 = rdr["sponsor2"].ToString(),
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
														records.Add(new RecordEntryBaptismal()
														{
															RecordID = db_reader2.GetString("record_id"),
															EntryNumber = db_reader2.GetInt32("entry_number"),
															BaptismalYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
															BaptismalDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
															FullName = db_reader2.GetString("recordholder_fullname"),
															BirthDate = "---",
															Legitimacy = "---",
															PlaceOfBirth = "---",
															Parent1 = db_reader2.GetString("parent1_fullname"),
															Parent2 = db_reader2.GetString("parent2_fullname"),
															Godparent1 = "---",
															Godparent2 = "---",
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
										cmd2.CommandText = "SELECT * FROM records, baptismal_records WHERE records.book_number = @book_number AND records.page_number = @page_number AND records.record_id = baptismal_records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@book_number", targBook);
										cmd2.Parameters.AddWithValue("@page_number", pageNum);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
												{
													records.Add(new RecordEntryBaptismal()
													{
														RecordID = db_reader2.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														BaptismalYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														BaptismalDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName = db_reader2.GetString("recordholder_fullname"),
														BirthDate = DateTime.Parse(db_reader2.GetString("birthday")).ToString("MMM dd, yyyy"),
														Legitimacy = db_reader2.GetString("legitimacy"),
														PlaceOfBirth = db_reader2.GetString("place_of_birth"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Godparent1 = db_reader2.GetString("sponsor1"),
														Godparent2 = db_reader2.GetString("sponsor2"),
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
			RecordEntryBaptismal record = (RecordEntryBaptismal)EntriesHolder.SelectedItem;
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
			if (EntriesHolder.SelectedItems.Count == 1) {
				RecordEntryBaptismal record = (RecordEntryBaptismal)EntriesHolder.SelectedItem;

				if (record == null)
				{
					MsgNoItemSelected();
				}
				else
				{
					var metroWindow = (Application.Current.MainWindow as MetroWindow);
					await metroWindow.ShowChildWindowAsync(new PrintBaptismalRecordEntryWindow(record.RecordID));
				}
			}
			else {
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new ConfirmBatchPrintWindow(EntriesHolder.SelectedItems));
			}
		}

		private async void Edit_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryBaptismal record = (RecordEntryBaptismal)EntriesHolder.SelectedItem;
			if (record == null)
			{
				MsgNoItemSelected();
			}
			else
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				await metroWindow.ShowChildWindowAsync(new EditBaptismalRecordEntryWindow(record.RecordID));
			}
		}

		private async void History_Click(object sender, RoutedEventArgs e)
		{
			RecordEntryBaptismal record = (RecordEntryBaptismal)EntriesHolder.SelectedItem;
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
			cmd_tmp = "SELECT * FROM records, baptismal_records WHERE records.book_number = @book_number AND records.record_id = baptismal_records.record_id AND (records.recordholder_fullname LIKE @query OR records.parent1_fullname LIKE @query OR records.parent2_fullname LIKE @query OR baptismal_records.sponsor1 LIKE @query OR baptismal_records.sponsor2 LIKE @query) GROUP BY records.record_id ORDER BY records.entry_number ASC;";
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
			records = new ObservableCollection<RecordEntryBaptismal>();

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
										cmd2.CommandText = cmd_tmp;
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
														string stm = "SELECT * FROM confirmation_records WHERE record_id='" + db_reader2.GetString("record_id") + "';";

														using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
														{
															using (SQLiteDataReader rdr = cmdx.ExecuteReader())
															{
																while (rdr.Read())
																{
																	App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
																	{
																		records.Add(new RecordEntryBaptismal()
																		{
																			RecordID = db_reader2.GetString("record_id"),
																			EntryNumber = db_reader2.GetInt32("entry_number"),
																			BaptismalYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
																			BaptismalDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
																			FullName = db_reader2.GetString("recordholder_fullname"),
																			BirthDate = DateTime.Parse(rdr["birthday"].ToString()).ToString("MMM dd, yyyy"),
																			Legitimacy = rdr["legitimacy"].ToString(),
																			PlaceOfBirth = rdr["place_of_birth"].ToString(),
																			Parent1 = db_reader2.GetString("parent1_fullname"),
																			Parent2 = db_reader2.GetString("parent2_fullname"),
																			Godparent1 = rdr["sponsor1"].ToString(),
																			Godparent2 = rdr["sponsor2"].ToString(),
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
														records.Add(new RecordEntryBaptismal()
														{
															RecordID = db_reader2.GetString("record_id"),
															EntryNumber = db_reader2.GetInt32("entry_number"),
															BaptismalYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
															BaptismalDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
															FullName = db_reader2.GetString("recordholder_fullname"),
															BirthDate = "---",
															Legitimacy = "---",
															PlaceOfBirth = "---",
															Parent1 = db_reader2.GetString("parent1_fullname"),
															Parent2 = db_reader2.GetString("parent2_fullname"),
															Godparent1 = "---",
															Godparent2 = "---",
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
													records.Add(new RecordEntryBaptismal()
													{
														RecordID = db_reader2.GetString("record_id"),
														EntryNumber = db_reader2.GetInt32("entry_number"),
														BaptismalYear = DateTime.Parse(db_reader2.GetString("record_date")).ToString("yyyy"),
														BaptismalDate = DateTime.Parse(db_reader2.GetString("record_date")).ToString("MMM dd"),
														FullName = db_reader2.GetString("recordholder_fullname"),
														BirthDate = DateTime.Parse(db_reader2.GetString("birthday")).ToString("MMM dd, yyyy"),
														Legitimacy = db_reader2.GetString("legitimacy"),
														PlaceOfBirth = db_reader2.GetString("place_of_birth"),
														Parent1 = db_reader2.GetString("parent1_fullname"),
														Parent2 = db_reader2.GetString("parent2_fullname"),
														Godparent1 = db_reader2.GetString("sponsor1"),
														Godparent2 = db_reader2.GetString("sponsor2"),
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
