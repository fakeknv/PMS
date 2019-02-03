using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Spire.Doc;
using System.Diagnostics;
using System.Data.SQLite;
using MahApps.Metro.Controls.Dialogs;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class PrintConfirmationRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string recordID;
		private int pageNum;
		private int bookNum;
		private int entryNum;
		private string confirmationDate;
		private int age;
		private string fullName;
		private string baptismPlace;
		private string parent1;
		private string parent2;
		private string sponsor1;
		private string sponsor2;
		private string minister;
		private string remarks;


		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public PrintConfirmationRecordEntryWindow(string targRecord)
		{
			
			pmsutil = new PMSUtil();
			recordID = targRecord;
			InitializeComponent();
			GetResidingPriests();
			CheckAccess(targRecord);

			PrintingFee.Value = Convert.ToDouble(pmsutil.GetPrintFee("Confirmation"));
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
						cmd.CommandText = "SELECT * FROM records, registers WHERE records.record_id = @rid AND records.book_number = registers.book_number LIMIT 1;";
						cmd.Parameters.AddWithValue("@rid", recordID);
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
										cmd2.CommandText = "SELECT * FROM records WHERE records.record_id = @rid ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@rid", recordID);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											string archiveDrive = "init";
											string path = @"\archive.db";
											while (db_reader2.Read())
											{
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
																	bookNum = db_reader2.GetInt32("book_number");
																	EntryNum.Value = db_reader2.GetInt32("entry_number");
																	PageNum.Value = db_reader2.GetInt32("page_number");
																	ConfirmationDate.Text = db_reader2.GetString("record_date");
																	FullName.Text = db_reader2.GetString("recordholder_fullname");
																	Age.Value = Convert.ToInt32(rdr["age"]);
																	PlaceOfBaptism.Text = rdr["place_of_baptism"].ToString();
																	Parent1.Text = db_reader2.GetString("parent1_fullname");
																	Parent2.Text = db_reader2.GetString("parent2_fullname");
																	Sponsor1.Text = rdr["sponsor"].ToString();
																	Sponsor2.Text = rdr["sponsor2"].ToString();
																	Minister.Text = rdr["minister"].ToString();
																	Remarks.Text = rdr["remarks"].ToString();
																}
															}
														}
													}
												}
												else
												{
													archiveDrive = "init";
													bookNum = db_reader2.GetInt32("book_number");
													EntryNum.Value = db_reader2.GetInt32("entry_number");
													PageNum.Value = db_reader2.GetInt32("page_number");
													ConfirmationDate.Text = db_reader2.GetString("record_date");
													FullName.Text = db_reader2.GetString("recordholder_fullname");
													Age.Value = db_reader2.GetInt32("age");
													PlaceOfBaptism.Text = db_reader2.GetString("place_of_baptism");
													Parent1.Text = db_reader2.GetString("parent1_fullname");
													Parent2.Text = db_reader2.GetString("parent2_fullname");
													Sponsor1.Text = db_reader2.GetString("sponsor");
													Sponsor2.Text = db_reader2.GetString("sponsor2");
													Minister.Text = db_reader2.GetString("minister");
													Remarks.Text = db_reader2.GetString("remarks");
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
										cmd2.CommandText = "SELECT * FROM records, confirmation_records WHERE records.record_id = @rid AND records.record_id = confirmation_records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@rid", recordID);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												bookNum = db_reader2.GetInt32("book_number");
												EntryNum.Value = db_reader2.GetInt32("entry_number");
												PageNum.Value = db_reader2.GetInt32("page_number");
												ConfirmationDate.Text = db_reader2.GetString("record_date");
												FullName.Text = db_reader2.GetString("recordholder_fullname");
												Age.Value = db_reader2.GetInt32("age");
												PlaceOfBaptism.Text = db_reader2.GetString("place_of_baptism");
												Parent1.Text = db_reader2.GetString("parent1_fullname");
												Parent2.Text = db_reader2.GetString("parent2_fullname");
												Sponsor1.Text = db_reader2.GetString("sponsor");
												Sponsor2.Text = db_reader2.GetString("sponsor2");
												Minister.Text = db_reader2.GetString("minister");
											}
										}
									}
								}
							}
						}
					}
				}
			}

			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
		}
		internal void CheckAccess(string record_id) {
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM records_log WHERE record_id = @record_id AND log_code = 'LOGC-03' OR log_code = 'LOGC-04';";
					cmd.Parameters.AddWithValue("@record_id", record_id);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0) {
							MsgNotice();
						}
					}
				}
			}
		}
		private async void MsgNotice()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Notice!", "This record has been printed before. Please check access history for more info.");
		}
		private void GetResidingPriests()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM residing_priests;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Signatory.Items.Add(db_reader.GetString("priest_name"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private string GetDaySuffix(int day)
		{
			switch (day)
			{
				case 1:
				case 21:
				case 31:
					return "st";
				case 2:
				case 22:
					return "nd";
				case 3:
				case 23:
					return "rd";
				default:
					return "th";
			}
		}
		private string PrepMonth(int mon)
		{
			switch (mon)
			{
				case 1:
					return "January";
				case 2:
					return "February";
				case 3:
					return "March";
				case 4:
					return "April";
				case 5:
					return "May";
				case 6:
					return "June";
				case 7:
					return "July";
				case 8:
					return "August";
				case 9:
					return "September";
				case 10:
					return "October";
				case 11:
					return "November";
				default:
					return "December";
			}
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int PrintEntry()
		{
			string x1;

			string[] spl = ConfirmationDate.Text.Split('/');
			string suff = GetDaySuffix(int.Parse(spl[1]));
			string mon = PrepMonth(int.Parse(spl[0]));
			if (int.Parse(spl[2]) > 1999)
			{
				x1 = "";
				spl[2] = spl[2].Remove(0, 2);
			}
			else
			{
				x1 = "X";
			}

			Document doc = new Document();
			doc.LoadFromFile("Data\\temp_confirmation.docx");
			doc.Replace("name", fullName, true, true);
			doc.Replace("day", int.Parse(spl[1]) + suff, true, true);
			doc.Replace("month", mon, true, true);
			doc.Replace("X", x1, true, true);
			doc.Replace("year", spl[2], true, true);
			doc.Replace("by", minister, true, true);
			doc.Replace("no", entryNum.ToString(), true, true);
			doc.Replace("page", pageNum.ToString(), true, true);
			doc.Replace("book", bookNum.ToString(), true, true);
			doc.Replace("priest", Signatory.Text, true, true);
			doc.Replace("purpose", Purpose.Text, true, true);
			doc.Replace("date", DateTime.Now.ToString("MMMM d, yyyy"), true, true);
			doc.SaveToFile("Data\\print.docx", FileFormat.Docx);

			//Load Document
			Document document = new Document();
			document.LoadFromFile(@"Data\\print.docx");

			//Convert Word to PDF
			document.SaveToFile("Output\\print_file.pdf", FileFormat.PDF);

			System.Diagnostics.Process.Start("Output\\print_file.pdf");

			if (Purpose.SelectedIndex == 0)
			{
				//Reference
				string tmp = pmsutil.LogRecord(recordID, "LOGC-03");
			}
			else
			{
				//Marriage
				string tmp = pmsutil.LogRecord(recordID, "LOGC-04");
			}
			if (Purpose.SelectedIndex == 0)
			{
				//Reference
				string tmp = pmsutil.LogRecord(recordID, "LOGC-03");
			}
			else
			{
				//Marriage
				string tmp = pmsutil.LogRecord(recordID, "LOGC-04");
			}
			pmsutil.InsertTransaction("Confirmation Cert.", "Paying", recordID, Convert.ToDouble(PrintingFee.Value));
			return 1;
		}
		/// <summary>
		/// Fetches default confirmation stipend value.
		/// </summary>
		private int FetchConfirmationStipend() {
			int ret = 0;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Confirmation Stipend';";
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = Convert.ToInt32(db_reader.GetString("key_value"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = 0;
			}
			return ret;
		}
		/// <summary>
		/// Validates input.
		/// </summary>
		private string ValidateInp(string targ) {
			if (String.IsNullOrEmpty(targ))
			{
				return targ;
			}
			else
			{
				return targ;
			}
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void PrintRecConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			entryNum = Convert.ToInt32(EntryNum.Value);
			pageNum = Convert.ToInt32(PageNum.Value);
			confirmationDate = Convert.ToDateTime(ConfirmationDate.Text).ToString("yyyy-MM-dd");
			age = Convert.ToInt32(Age.Value);
			fullName = ValidateInp(FullName.Text);
			baptismPlace = PlaceOfBaptism.Text;
			parent1 = ValidateInp(Parent1.Text);
			parent2 = ValidateInp(Parent2.Text);
			sponsor1 = ValidateInp(Sponsor1.Text);
			sponsor2 = ValidateInp(Sponsor2.Text);
			minister = ValidateInp(Minister.Text);
			remarks = ValidateInp(Remarks.Text);
			if (PrintEntry() > 0) {
				MsgSuccess();
				this.Close();
			}
			else
			{
				MsgFail();
			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected record has been added to the print queue.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void PrintRecCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void ShowSuggestions1(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			PlaceOfBaptismSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_baptism FROM confirmation_records WHERE " +
					"place_of_baptism LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + PlaceOfBaptism.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					PlaceOfBaptismSuggestionArea.Items.Add(db_reader.GetString("place_of_baptism"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions1.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions4(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM confirmation_records WHERE " +
					"minister LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Minister.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					MinisterSuggestionArea.Items.Add(db_reader.GetString("minister"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions4.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			PlaceOfBaptismSuggestionArea.SelectedItem = item;
			PlaceOfBaptism.Text = PlaceOfBaptismSuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion4_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions4.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
		}
	}
}
