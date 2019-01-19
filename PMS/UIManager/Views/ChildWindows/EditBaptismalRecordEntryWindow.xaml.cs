using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Data.SQLite;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class EditBaptismalRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int pageNum;
		private int entryNum;
		private string baptismDate;
		private string birthDate;
		private string legitimacy;
		private string fullName;
		private string birthPlace;
		private string parent1;
		private string parent2;
		private string sponsor1;
		private string sponsor2;
		private int stipend;
		private string minister;
		private string remarks;

		private string recordID;


		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public EditBaptismalRecordEntryWindow(string targRecord)
		{
			
			pmsutil = new PMSUtil();
			recordID = targRecord;
			InitializeComponent();
			Stipend.Value = FetchBaptismalStipend();

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
														string stm = "SELECT * FROM baptismal_records WHERE record_id='" + db_reader2.GetString("record_id") + "';";

														using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
														{
															using (SQLiteDataReader rdr = cmdx.ExecuteReader())
															{
																while (rdr.Read())
																{
																	DateTime dateOfBirth = Convert.ToDateTime(rdr["birthday"].ToString());

																	EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
																	PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
																	BaptismDate.Text = db_reader.GetString("record_date");
																	Birthdate.Text = dateOfBirth.ToString("MMM dd, yyyy");
																	FullName.Text = db_reader.GetString("recordholder_fullname");
																	Legitimacy.Text = rdr["legitimacy"].ToString();
																	Stipend.Value = Convert.ToDouble(rdr["stipend"]);
																	PlaceOfBirth.Text = rdr["place_of_birth"].ToString();
																	Parent1.Text = db_reader.GetString("parent1_fullname");
																	Parent2.Text = db_reader.GetString("parent2_fullname");
																	Sponsor1.Text = rdr["sponsor1"].ToString();
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
													EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
													PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
													BaptismDate.Text = db_reader.GetString("record_date");
													Birthdate.Text = db_reader.GetString("birthday");
													FullName.Text = db_reader.GetString("recordholder_fullname");
													Legitimacy.Text = db_reader.GetString("legitimacy");
													Stipend.Value = Convert.ToDouble(db_reader.GetString("stipend"));
													PlaceOfBirth.Text = db_reader.GetString("place_of_birth");
													Parent1.Text = db_reader.GetString("parent1_fullname");
													Parent2.Text = db_reader.GetString("parent2_fullname");
													Sponsor1.Text = db_reader.GetString("sponsor1");
													Sponsor2.Text = db_reader.GetString("sponsor2");
													Minister.Text = db_reader.GetString("minister");
													Remarks.Text = db_reader.GetString("remarks");
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
										cmd2.CommandText = "SELECT * FROM records, baptismal_records WHERE records.record_id = @rid AND records.record_id = baptismal_records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@rid", recordID);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												EntryNum.Value = Convert.ToDouble(db_reader2.GetString("entry_number"));
												PageNum.Value = Convert.ToDouble(db_reader2.GetString("page_number"));
												BaptismDate.Text = db_reader2.GetString("record_date");
												Birthdate.Text = db_reader2.GetString("birthday");
												FullName.Text = db_reader2.GetString("recordholder_fullname");
												Legitimacy.Text = db_reader2.GetString("legitimacy");
												Stipend.Value = Convert.ToDouble(db_reader2.GetString("stipend"));
												PlaceOfBirth.Text = db_reader2.GetString("place_of_birth");
												Parent1.Text = db_reader2.GetString("parent1_fullname");
												Parent2.Text = db_reader2.GetString("parent2_fullname");
												Sponsor1.Text = db_reader2.GetString("sponsor1");
												Sponsor2.Text = db_reader2.GetString("sponsor2");
												Minister.Text = db_reader2.GetString("minister");
												Remarks.Text = db_reader2.GetString("remarks");
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
											cmd2.Prepare();
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@page_number", pageNum);
											cmd2.Parameters.AddWithValue("@entry_number", entryNum);
											cmd2.Parameters.AddWithValue("@record_date", baptismDate);
											cmd2.Parameters.AddWithValue("@recordholder_fullname", fullName);
											cmd2.Parameters.AddWithValue("@parent1_fullname", parent1);
											cmd2.Parameters.AddWithValue("@parent2_fullname", parent2);
											ret = cmd2.ExecuteNonQuery();
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
														CommandText = "UPDATE baptismal_records SET birthday = @birthday, legitimacy = @legitimacy, place_of_birth = @place_of_birth, sponsor1 = @sponsor1, sponsor2 = @sponsor2, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;"
													};
													command.Parameters.Add(new SQLiteParameter("@record_id", recordID));
													command.Parameters.Add(new SQLiteParameter("@birthday", birthDate));
													command.Parameters.Add(new SQLiteParameter("@legitimacy", legitimacy));
													command.Parameters.Add(new SQLiteParameter("@place_of_birth", birthPlace));
													command.Parameters.Add(new SQLiteParameter("@sponsor1", sponsor1));
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
											ret = 0;
										}
									}
								}
								else {
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										try
										{
											MySqlCommand cmd2 = conn3.CreateCommand();
											cmd2.CommandText =
												"UPDATE records SET page_number = @page_number, entry_number = @entry_number, record_date = @record_date, recordholder_fullname = @recordholder_fullname, parent1_fullname = @parent1_fullname, parent2_fullname = @parent2_fullname WHERE record_id = @record_id;";
											cmd2.Prepare();
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@page_number", pageNum);
											cmd2.Parameters.AddWithValue("@entry_number", entryNum);
											cmd2.Parameters.AddWithValue("@record_date", baptismDate);
											cmd2.Parameters.AddWithValue("@recordholder_fullname", fullName);
											cmd2.Parameters.AddWithValue("@parent1_fullname", parent1);
											cmd2.Parameters.AddWithValue("@parent2_fullname", parent2);
											ret = cmd2.ExecuteNonQuery();
											conn3.Close();

											conn3.Open();
											cmd2 = conn3.CreateCommand();
											cmd2.CommandText =
												"UPDATE baptismal_records SET birthday = @birthday, legitimacy = @legitimacy, place_of_birth = @place_of_birth, sponsor1 = @sponsor1, sponsor2 = @sponsor2, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;";
											cmd2.Prepare();
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@birthday", birthDate);
											cmd2.Parameters.AddWithValue("@legitimacy", legitimacy);
											cmd2.Parameters.AddWithValue("@place_of_birth", birthPlace);
											cmd2.Parameters.AddWithValue("@sponsor1", sponsor1);
											cmd2.Parameters.AddWithValue("@sponsor2", sponsor2);
											cmd2.Parameters.AddWithValue("@stipend", stipend);
											cmd2.Parameters.AddWithValue("@minister", minister);
											cmd2.Parameters.AddWithValue("@remarks", remarks);
											ret = cmd2.ExecuteNonQuery();
											conn3.Close();
											string tmp = pmsutil.LogRecord(recordID, "LOGC-02");
										}
										catch (MySqlException ex)
										{
											Console.WriteLine("Error: {0}", ex.ToString());
											ret = 0;
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
		private int FetchBaptismalStipend() {
			int ret = 0;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Baptismal Stipend';";
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
		private bool CheckInputs()
		{
			bool ret = true;

			if (string.IsNullOrWhiteSpace(BaptismDate.Text))
			{
				BaptismDateValidator.Visibility = Visibility.Visible;
				BaptismDateValidator.ToolTip = "This field is required.";
				BaptismDateValidator.Foreground = Brushes.Red;
				BaptismDate.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (EntryNum.Value < 0)
			{
				EntryNumValidator.Visibility = Visibility.Visible;
				EntryNumValidator.ToolTip = "Must be greater than zero.";
				EntryNumValidator.Foreground = Brushes.Red;
				EntryNum.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (PageNum.Value < 0)
			{
				EntryNumValidator.Visibility = Visibility.Visible;
				EntryNumValidator.ToolTip = "Must be greater than zero.";
				EntryNumValidator.Foreground = Brushes.Red;
				PageNum.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Birthdate.Text))
			{
				BirthDateValidator.Visibility = Visibility.Visible;
				BirthDateValidator.ToolTip = "This field is required.";
				BirthDateValidator.Foreground = Brushes.Red;
				Birthdate.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(FullName.Text))
			{
				NameValidator.Visibility = Visibility.Visible;
				NameValidator.ToolTip = "This field is required.";
				NameValidator.Foreground = Brushes.Red;
				FullName.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (Stipend.Value == 0)
			{
				StipendValidator.Visibility = Visibility.Visible;
				StipendValidator.ToolTip = "Notice: Stipend is set to zero.";
				StipendValidator.Foreground = Brushes.Orange;
				Stipend.BorderBrush = Brushes.Orange;
				MsgStipend();
				ret = true;
			}
			if (string.IsNullOrWhiteSpace(PlaceOfBirth.Text))
			{
				PlaceOfBirthValidator.Visibility = Visibility.Visible;
				PlaceOfBirthValidator.ToolTip = "This field is required. Type unknown if its not applicable.";
				PlaceOfBirthValidator.Foreground = Brushes.Red;
				PlaceOfBirth.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sponsor1.Text))
			{
				Sponsor1Validator.Visibility = Visibility.Visible;
				Sponsor1Validator.ToolTip = "This field is required.";
				Sponsor1Validator.Foreground = Brushes.Red;
				Sponsor1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sponsor2.Text))
			{
				Sponsor2Validator.Visibility = Visibility.Visible;
				Sponsor2Validator.ToolTip = "This field is required.";
				Sponsor2Validator.Foreground = Brushes.Red;
				Sponsor2.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent1.Text))
			{
				Parent1Validator.Visibility = Visibility.Visible;
				Parent1Validator.ToolTip = "This field is required.";
				Parent1Validator.Foreground = Brushes.Red;
				Parent1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Minister.Text))
			{
				MinisterValidator.Visibility = Visibility.Visible;
				MinisterValidator.ToolTip = "This field is required.";
				MinisterValidator.Foreground = Brushes.Red;
				Minister.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Legitimacy.Text))
			{
				LegitimacyValidator.Visibility = Visibility.Visible;
				LegitimacyValidator.ToolTip = "This field is required.";
				LegitimacyValidator.Foreground = Brushes.Red;
				Legitimacy.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
		}
		private async void MsgStipend()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Notice", "Stipend is set to zero. Re-check input before proceeding.");
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void EditRecConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			if (CheckInputs() == true) {

				legitimacy = Legitimacy.Text;
				entryNum = Convert.ToInt32(EntryNum.Value);
				pageNum = Convert.ToInt32(PageNum.Value);
				baptismDate = Convert.ToDateTime(BaptismDate.Text).ToString("yyyy-MM-dd");
				birthDate = Convert.ToDateTime(Birthdate.Text).ToString("yyyy-MM-dd");
				fullName = ValidateInp(FullName.Text);
				birthPlace = PlaceOfBirth.Text;
				parent1 = ValidateInp(Parent1.Text);
				parent2 = ValidateInp(Parent2.Text);
				sponsor1 = ValidateInp(Sponsor1.Text);
				sponsor2 = ValidateInp(Sponsor2.Text);
				stipend = Convert.ToInt32(Stipend.Value);
				minister = ValidateInp(Minister.Text);
				remarks = ValidateInp(Remarks.Text);
				if (UpdateEntry() > 0)
				{
					MsgSuccess();
					this.Close();
				}
				else
				{
					MsgFail();
				}
			}
			else {

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

			PlaceOfBirthSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_birth FROM baptismal_records WHERE " +
					"place_of_birth LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + PlaceOfBirth.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					PlaceOfBirthSuggestionArea.Items.Add(db_reader.GetString("place_of_birth"));
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

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM baptismal_records WHERE " +
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

				Suggestions2.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			PlaceOfBirthSuggestionArea.SelectedItem = item;
			PlaceOfBirth.Text = PlaceOfBirthSuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion2_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions2.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
		}
	}
}
