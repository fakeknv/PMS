using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using MahApps.Metro.Controls.Dialogs;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class EditConfirmationRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string recordID;
		private int pageNum;
		private int entryNum;
		private string confirmationDate;
		private int age;
		private string fullName;
		private string parish;
		private string province;
		private string baptismPlace;
		private string parent1;
		private string parent2;
		private string sponsor1;
		private string sponsor2;
		private int stipend;
		private string minister;
		private string remarks;


		public EditConfirmationRecordEntryWindow(string targRecord)
		{
			
			pmsutil = new PMSUtil();
			recordID = targRecord;
			InitializeComponent();
			Stipend.Value = FetchConfirmationStipend();

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
																	EntryNum.Value = db_reader2.GetInt32("entry_number");
																	PageNum.Value = db_reader2.GetInt32("page_number");
																	ConfirmationDate.Text = db_reader2.GetString("record_date");
																	FullName.Text = db_reader2.GetString("recordholder_fullname");
																	Age.Value = Convert.ToInt32(rdr["age"]);
																	Parish.Text = rdr["parochia"].ToString();
																	Province.Text = rdr["province"].ToString();
																	PlaceOfBaptism.Text = rdr["place_of_baptism"].ToString();
																	Parent1.Text = db_reader2.GetString("parent1_fullname");
																	Parent2.Text = db_reader2.GetString("parent2_fullname");
																	Sponsor1.Text = rdr["sponsor"].ToString();
																	Sponsor2.Text = rdr["sponsor2"].ToString();
																	Stipend.Value = Convert.ToDouble(rdr["stipend"]);
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
													EntryNum.Value = db_reader2.GetInt32("entry_number");
													PageNum.Value = db_reader2.GetInt32("page_number");
													ConfirmationDate.Text = db_reader2.GetString("record_date");
													FullName.Text = db_reader2.GetString("recordholder_fullname");
													Age.Value = db_reader2.GetInt32("age");
													Parish.Text = db_reader2.GetString("parochia");
													Province.Text = db_reader2.GetString("province");
													PlaceOfBaptism.Text = db_reader2.GetString("place_of_baptism");
													Parent1.Text = db_reader2.GetString("parent1_fullname");
													Parent2.Text = db_reader2.GetString("parent2_fullname");
													Sponsor1.Text = db_reader2.GetString("sponsor");
													Sponsor2.Text = db_reader2.GetString("sponsor2");
													Stipend.Value = db_reader2.GetDouble("stipend");
													Minister.Text = db_reader2.GetString("minister");
													Remarks.Text = db_reader2.GetString("remarks");
												}
											}
										}
									}
								}
								else {
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
												EntryNum.Value = db_reader2.GetInt32("entry_number");
												PageNum.Value = db_reader2.GetInt32("page_number");
												ConfirmationDate.Text = db_reader2.GetString("record_date");
												FullName.Text = db_reader2.GetString("recordholder_fullname");
												Age.Value = db_reader2.GetInt32("age");
												Parish.Text = db_reader2.GetString("parochia");
												Province.Text = db_reader2.GetString("province");
												PlaceOfBaptism.Text = db_reader2.GetString("place_of_baptism");
												Parent1.Text = db_reader2.GetString("parent1_fullname");
												Parent2.Text = db_reader2.GetString("parent2_fullname");
												Sponsor1.Text = db_reader2.GetString("sponsor");
												Sponsor2.Text = db_reader2.GetString("sponsor2");
												Stipend.Value = db_reader2.GetDouble("stipend");
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
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int UpdateEntry()
		{
			int ret = 0;
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
										try
										{
											MySqlCommand cmd2 = conn3.CreateCommand();
											cmd2.CommandText =
												"UPDATE records SET page_number = @page_number, entry_number = @entry_number, record_date = @record_date, recordholder_fullname = @recordholder_fullname, parent1_fullname = @parent1_fullname, parent2_fullname = @parent2_fullname WHERE record_id = @record_id;";
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@page_number", pageNum);
											cmd2.Parameters.AddWithValue("@entry_number", entryNum);
											cmd2.Parameters.AddWithValue("@record_date", confirmationDate);
											cmd2.Parameters.AddWithValue("@recordholder_fullname", fullName);
											cmd2.Parameters.AddWithValue("@parent1_fullname", parent1);
											cmd2.Parameters.AddWithValue("@parent2_fullname", parent2);
											cmd2.Prepare();
											int stat_code = cmd2.ExecuteNonQuery();
											conn3.Close();

											conn3.Open();
											string path = @"\archive.db";
											pmsutil = new PMSUtil();
											if (pmsutil.CheckArchiveDrive(path) != "dc")
											{
												SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
												{
													FailIfMissing = true,
													DataSource = pmsutil.CheckArchiveDrive(path)
												};

												//Copy the selected register's record to the archive drive
												using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
												{

													// open the connection:
													connection.Open();

													SQLiteCommand command = new SQLiteCommand(null, connection)
													{
														CommandText = "UPDATE confirmation_records SET age = @age, parochia = @parish, province = @province, place_of_baptism = @place_of_baptism, sponsor = @sponsor, sponsor2 = @sponsor2, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;"
													};
													command.Parameters.Add(new SQLiteParameter("@record_id", recordID));
													command.Parameters.Add(new SQLiteParameter("@age", age));
													command.Parameters.Add(new SQLiteParameter("@parish", parish));
													command.Parameters.Add(new SQLiteParameter("@province", province));
													command.Parameters.Add(new SQLiteParameter("@place_of_baptism", baptismPlace));
													command.Parameters.Add(new SQLiteParameter("@sponsor", sponsor1));
													command.Parameters.Add(new SQLiteParameter("@sponsor2", sponsor2));
													command.Parameters.Add(new SQLiteParameter("@stipend", stipend));
													command.Parameters.Add(new SQLiteParameter("@minister", minister));
													command.Parameters.Add(new SQLiteParameter("@remarks", remarks));
													// Call Prepare after setting the Commandtext and Parameters.
													command.Prepare();
													command.ExecuteNonQuery();
												}
											}
											else {

											}
											conn3.Close();
											string tmp = pmsutil.LogRecord(recordID, "LOGC-02");
										}
										catch (MySqlException ex)
										{
											Console.WriteLine("Error: {0}", ex.ToString());
											return 0;
										}
									}
								}
								else {
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										try
										{
											MySqlCommand cmd2 = conn3.CreateCommand();
											cmd2.CommandText =
												"UPDATE records SET page_number = @page_number, entry_number = @entry_number, record_date = @record_date, recordholder_fullname = @recordholder_fullname, parent1_fullname = @parent1_fullname, parent2_fullname = @parent2_fullname WHERE record_id = @record_id;";
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@page_number", pageNum);
											cmd2.Parameters.AddWithValue("@entry_number", entryNum);
											cmd2.Parameters.AddWithValue("@record_date", confirmationDate);
											cmd2.Parameters.AddWithValue("@recordholder_fullname", fullName);
											cmd2.Parameters.AddWithValue("@parent1_fullname", parent1);
											cmd2.Parameters.AddWithValue("@parent2_fullname", parent2);
											cmd2.Prepare();
											int stat_code = cmd2.ExecuteNonQuery();
											conn3.Close();

											conn3.Open();
											cmd2 = conn3.CreateCommand();
											cmd2.CommandText =
												"UPDATE confirmation_records SET age = @age, parochia = @parish, province = @province, place_of_baptism = @place_of_baptism, sponsor = @sponsor, sponsor2 = @sponsor2, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;";
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@age", age);
											cmd2.Parameters.AddWithValue("@parish", parish);
											cmd2.Parameters.AddWithValue("@province", province);
											cmd2.Parameters.AddWithValue("@place_of_baptism", baptismPlace);
											cmd2.Parameters.AddWithValue("@sponsor", sponsor1);
											cmd2.Parameters.AddWithValue("@sponsor2", sponsor2);
											cmd2.Parameters.AddWithValue("@stipend", stipend);
											cmd2.Parameters.AddWithValue("@minister", minister);
											cmd2.Parameters.AddWithValue("@remarks", remarks);
											cmd2.Prepare();
											stat_code = cmd2.ExecuteNonQuery();
											conn3.Close();
											string tmp = pmsutil.LogRecord(recordID, "LOGC-02");
											return stat_code;
										}
										catch (MySqlException ex)
										{
											Console.WriteLine("Error: {0}", ex.ToString());
											return 0;
										}
									}
								}
							}
						}
					}
				}
				return ret;
			}
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
				return "---";
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
		private void EditRecConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			entryNum = Convert.ToInt32(EntryNum.Value);
			pageNum = Convert.ToInt32(PageNum.Value);
			confirmationDate = Convert.ToDateTime(ConfirmationDate.Text).ToString("yyyy-MM-dd");
			age = Convert.ToInt32(Age.Value);
			fullName = ValidateInp(FullName.Text);
			parish = ValidateInp(Parish.Text);
			province = ValidateInp(Province.Text);
			baptismPlace = PlaceOfBaptism.Text;
			parent1 = ValidateInp(Parent1.Text);
			parent2 = ValidateInp(Parent2.Text);
			sponsor1 = ValidateInp(Sponsor1.Text);
			sponsor2 = ValidateInp(Sponsor2.Text);
			stipend = Convert.ToInt32(Stipend.Value);
			minister = ValidateInp(Minister.Text);
			remarks = ValidateInp(Remarks.Text);
			if (UpdateEntry() > 0) {
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
			await metroWindow.ShowMessageAsync("Success!", "The register has been updated successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void EditRecCancel(object sender, System.Windows.RoutedEventArgs e)
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
		private void ShowSuggestions2(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			ParishSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT parochia FROM confirmation_records WHERE " +
					"parochia LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Parish.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ParishSuggestionArea.Items.Add(db_reader.GetString("parochia"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions2.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions3(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			ProvinceSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT province FROM confirmation_records WHERE " +
					"province LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Province.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ProvinceSuggestionArea.Items.Add(db_reader.GetString("province"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions3.Visibility = System.Windows.Visibility.Visible;
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
		private void Suggestion2_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			ParishSuggestionArea.SelectedItem = item;
			Parish.Text = ParishSuggestionArea.SelectedItem.ToString();
			Suggestions2.Visibility = Visibility.Hidden;
		}
		private void Suggestion3_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			ProvinceSuggestionArea.SelectedItem = item;
			Province.Text = ProvinceSuggestionArea.SelectedItem.ToString();
			Suggestions3.Visibility = Visibility.Hidden;
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
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
		}
	}
}
