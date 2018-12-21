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
	public partial class EditBurialRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string recordID;
		private int pageNum;
		private int entryNum;
		private string deathDate;
		private string burialDate;
		private int age;
		private string status;
		private string fullName;
		private string sacrament;
		private string causeOfDeath;
		private string intermentPlace;
		private string parent1;
		private string parent2;
		private string residence1;
		private string residence2;
		private int stipend;
		private string minister;
		private string remarks;
		//private DateTime cDate;
		//private DateTime cTime;
		//private string curDate;
		//private string curTime;


		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public EditBurialRecordEntryWindow(string targRecord)
		{
			
			pmsutil = new PMSUtil();
			InitializeComponent();
			recordID = targRecord;
			Stipend.Value = FetchBurialStipend();

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
														string stm = "SELECT * FROM burial_records WHERE record_id='" + db_reader2.GetString("record_id") + "';";

														using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
														{
															using (SQLiteDataReader rdr = cmdx.ExecuteReader())
															{
																while (rdr.Read())
																{
																	DateTime dateOfBurial = Convert.ToDateTime(rdr["burial_date"].ToString());

																	EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
																	PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
																	DeathDate.Text = db_reader.GetString("record_date");
																	BurialDate.Text = dateOfBurial.ToString("MMM dd, yyyy");
																	Age.Value = Convert.ToDouble(rdr["age"]);
																	FullName.Text = db_reader.GetString("recordholder_fullname");
																	Age.Value = Convert.ToDouble(rdr["age"]);
																	Status.Text = rdr["status"].ToString();
																	Parent1.Text = db_reader.GetString("parent1_fullname");
																	Parent2.Text = db_reader.GetString("parent2_fullname");
																	Residence1.Text = rdr["residence"].ToString();
																	Residence2.Text = rdr["residence2"].ToString();
																	Sacrament.Text = rdr["sacrament"].ToString();
																	CauseOfDeath.Text = rdr["cause_of_death"].ToString();
																	PlaceOfInterment.Text = rdr["place_of_interment"].ToString();
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
													EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
													PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
													DeathDate.Text = db_reader.GetString("record_date");
													//BurialDate.Text = db_reader.GetString("burial_date");
													Age.Value = 0;
													FullName.Text = db_reader.GetString("recordholder_fullname");
													Age.Value = 0;
													Status.Text = "----";
													Parent1.Text = db_reader.GetString("parent1_fullname");
													Parent2.Text = db_reader.GetString("parent2_fullname");
													Residence1.Text = "----";
													Residence2.Text = "----";
													Sacrament.Text = "----";
													CauseOfDeath.Text = "----";
													PlaceOfInterment.Text = "----";
													Stipend.Value = 0;
													Minister.Text = "----";
													Remarks.Text = "----";
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
										cmd2.CommandText = "SELECT * FROM records, burial_records WHERE records.record_id = @rid AND records.record_id = burial_records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@rid", recordID);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
												PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
												DeathDate.Text = db_reader.GetString("record_date");
												BurialDate.Text = db_reader.GetString("burial_date");
												Age.Value = Convert.ToDouble(db_reader.GetString("age"));
												FullName.Text = db_reader.GetString("recordholder_fullname");
												Age.Value = Convert.ToDouble(db_reader.GetString("age"));
												Status.Text = db_reader.GetString("status");
												Parent1.Text = db_reader.GetString("parent1_fullname");
												Parent2.Text = db_reader.GetString("parent2_fullname");
												Residence1.Text = db_reader.GetString("residence");
												Residence2.Text = db_reader.GetString("residence2");
												Sacrament.Text = db_reader.GetString("sacrament");
												CauseOfDeath.Text = db_reader.GetString("cause_of_death");
												PlaceOfInterment.Text = db_reader.GetString("place_of_interment");
												Stipend.Value = Convert.ToDouble(db_reader.GetString("stipend"));
												Minister.Text = db_reader.GetString("minister");
												Remarks.Text = db_reader.GetString("remarks");
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
			Suggestions5.Visibility = Visibility.Hidden;
			Suggestions6.Visibility = Visibility.Hidden;
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int InsertEntry()
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
											cmd2.Parameters.AddWithValue("@record_date", deathDate);
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
														CommandText = "UPDATE burial_records SET burial_date = @burial_date, age = @age, status = @status, residence = @residence, residence2 = @residence2, sacrament = @sacrament, cause_of_death = @cause_of_death, place_of_interment = @place_of_interment, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;"
													};
													command.Parameters.Add(new SQLiteParameter("@record_id", recordID));
													command.Parameters.Add(new SQLiteParameter("@record_id", recordID));
													command.Parameters.Add(new SQLiteParameter("@burial_date", burialDate));
													command.Parameters.Add(new SQLiteParameter("@age", age));
													command.Parameters.Add(new SQLiteParameter("@status", status));
													command.Parameters.Add(new SQLiteParameter("@residence", residence1));
													command.Parameters.Add(new SQLiteParameter("@residence2", residence2));
													command.Parameters.Add(new SQLiteParameter("@sacrament", sacrament));
													command.Parameters.Add(new SQLiteParameter("@cause_of_death", causeOfDeath));
													command.Parameters.Add(new SQLiteParameter("@place_of_interment", intermentPlace));
													command.Parameters.Add(new SQLiteParameter("@stipend", stipend));
													command.Parameters.Add(new SQLiteParameter("@minister", minister));
													command.Parameters.Add(new SQLiteParameter("@remarks", remarks));
													// Call Prepare after setting the Commandtext and Parameters.
													command.Prepare();
													command.ExecuteNonQuery();
												}
											}
											else
											{

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
								else
								{
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										try
										{
											string recID = pmsutil.GenRecordID();
											MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
											cmd2.CommandText =
												"UPDATE records SET page_number = @page_number, entry_number = @entry_number, record_date = @record_date, recordholder_fullname = @recordholder_fullname, parent1_fullname = @parent1_fullname, parent2_fullname = @parent2_fullname WHERE record_id = @record_id;";
											cmd2.Prepare();
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@page_number", pageNum);
											cmd2.Parameters.AddWithValue("@entry_number", entryNum);
											cmd2.Parameters.AddWithValue("@record_date", deathDate);
											cmd2.Parameters.AddWithValue("@recordholder_fullname", fullName);
											cmd2.Parameters.AddWithValue("@parent1_fullname", parent1);
											cmd2.Parameters.AddWithValue("@parent2_fullname", parent2);
											int stat_code = cmd2.ExecuteNonQuery();
											conn3.Close();

											conn3.Open();
											cmd2 = dbman.DBConnect().CreateCommand();
											cmd2.CommandText =
												"UPDATE burial_records SET burial_date = @burial_date, age = @age, status = @status, residence = @residence, residence2 = @residence2, sacrament = @sacrament, cause_of_death = @cause_of_death, place_of_interment = @place_of_interment, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;";
											cmd2.Prepare();
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@burial_date", burialDate);
											cmd2.Parameters.AddWithValue("@age", age);
											cmd2.Parameters.AddWithValue("@status", status);
											cmd2.Parameters.AddWithValue("@residence", residence1);
											cmd2.Parameters.AddWithValue("@residence2", residence2);
											cmd2.Parameters.AddWithValue("@sacrament", sacrament);
											cmd2.Parameters.AddWithValue("@cause_of_death", causeOfDeath);
											cmd2.Parameters.AddWithValue("@place_of_interment", intermentPlace);
											cmd2.Parameters.AddWithValue("@stipend", stipend);
											cmd2.Parameters.AddWithValue("@minister", minister);
											cmd2.Parameters.AddWithValue("@remarks", remarks);
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
		private int FetchBurialStipend()
		{
			int ret = 0;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Matrimonial Stipend';";
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
		///// <summary>
		///// Validates input.
		///// </summary>
		private string ValidateInp(string targ)
		{
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
			deathDate = Convert.ToDateTime(DeathDate.Text).ToString("yyyy-MM-dd");
			burialDate = Convert.ToDateTime(BurialDate.Text).ToString("yyyy-MM-dd");
			age = Convert.ToInt32(Age.Value);
			switch (Status.SelectedIndex)
			{
				case 0:
					status = "Widow";
					break;
				case 1:
					status = "Widower";
					break;
				case 2:
					status = "Single";
					break;
				case 3:
					status = "Conjugal";
					break;
				case 4:
					status = "Adult";
					break;
				case 5:
					status = "Infant";
					break;
				default:
					status = "----";
					break;
			}
			fullName = ValidateInp(FullName.Text);
			sacrament = ValidateInp(Sacrament.Text);
			causeOfDeath = ValidateInp(CauseOfDeath.Text);
			intermentPlace = ValidateInp(PlaceOfInterment.Text);
			parent1 = ValidateInp(Parent1.Text);
			parent2 = ValidateInp(Parent2.Text);
			residence1 = ValidateInp(Residence1.Text);
			residence2 = ValidateInp(Residence2.Text);
			stipend = Convert.ToInt32(Stipend.Value);
			minister = ValidateInp(Minister.Text);
			remarks = ValidateInp(Remarks.Text);
			if (InsertEntry() > 0)
			{
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

			SacramentSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT sacrament FROM burial_records WHERE " +
					"sacrament LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Sacrament.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					SacramentSuggestionArea.Items.Add(db_reader.GetString("sacrament"));
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

			CauseOfDeathSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT cause_of_death FROM burial_records WHERE " +
					"cause_of_death LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + CauseOfDeath.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					CauseOfDeathSuggestionArea.Items.Add(db_reader.GetString("cause_of_death"));
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

			Residence1SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT residence FROM burial_records WHERE " +
					"residence LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence1.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence1SuggestionArea.Items.Add(db_reader.GetString("residence"));
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

			Residence2SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT residence2 FROM burial_records WHERE " +
					"residence2 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence2.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence2SuggestionArea.Items.Add(db_reader.GetString("residence2"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions4.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions5(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM burial_records WHERE " +
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

				Suggestions5.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions6(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			PlaceOfIntermentSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_interment FROM burial_records WHERE " +
					"place_of_interment LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + PlaceOfInterment.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					PlaceOfIntermentSuggestionArea.Items.Add(db_reader.GetString("place_of_interment"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions6.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			SacramentSuggestionArea.SelectedItem = item;
			Sacrament.Text = SacramentSuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click2(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			CauseOfDeathSuggestionArea.SelectedItem = item;
			CauseOfDeath.Text = CauseOfDeathSuggestionArea.SelectedItem.ToString();
			Suggestions2.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click3(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Residence1SuggestionArea.SelectedItem = item;
			Residence1.Text = Residence1SuggestionArea.SelectedItem.ToString();
			Suggestions3.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click4(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Residence2SuggestionArea.SelectedItem = item;
			Residence2.Text = Residence2SuggestionArea.SelectedItem.ToString();
			Suggestions4.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click5(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions5.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click6(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			PlaceOfIntermentSuggestionArea.SelectedItem = item;
			PlaceOfInterment.Text = PlaceOfIntermentSuggestionArea.SelectedItem.ToString();
			Suggestions6.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
			Suggestions5.Visibility = Visibility.Hidden;
			Suggestions6.Visibility = Visibility.Hidden;
		}
	}
}
