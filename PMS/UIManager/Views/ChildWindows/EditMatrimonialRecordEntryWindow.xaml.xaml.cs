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
	public partial class EditMatrimonialRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string recordID;
		private int pageNum;
		private int entryNum;
		private string marriageDate;
		private string fullName1;
		private string fullName2;
		private int age1;
		private int age2;
		private string status1;
		private string status2;
		private string hometown1;
		private string hometown2;
		private string residence1;
		private string residence2;
		private string residence3;
		private string residence4;
		private string parent1;
		private string parent2;
		private string parent3;
		private string parent4;
		private string sponsor1;
		private string sponsor2;
		private int stipend;
		private string minister;
		private string remarks;


		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public EditMatrimonialRecordEntryWindow(string targRecord)
		{
			pmsutil = new PMSUtil();
			recordID = targRecord;
			InitializeComponent();
			Stipend.Value = FetchMatrimonialStipend();

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
														string stm = "SELECT * FROM matrimonial_records WHERE record_id='" + db_reader2.GetString("record_id") + "';";

														using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
														{
															using (SQLiteDataReader rdr = cmdx.ExecuteReader())
															{
																while (rdr.Read())
																{
																	EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
																	PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
																	MarriageDate.Text = db_reader.GetString("record_date");
																	Age1.Value = Convert.ToInt32(rdr["age1"]);
																	Age2.Value = Convert.ToInt32(rdr["age2"]);
																	Status1.Text = rdr["status1"].ToString();
																	Status2.Text = rdr["status2"].ToString();
																	FullName1.Text = db_reader.GetString("recordholder_fullname");
																	FullName2.Text = rdr["recordholder2_fullname"].ToString();
																	Hometown1.Text = rdr["place_of_origin1"].ToString();
																	Hometown2.Text = rdr["place_of_origin2"].ToString();
																	Residence1.Text = rdr["residence1"].ToString();
																	Residence2.Text = rdr["residence2"].ToString();
																	Stipend.Value = Convert.ToDouble(rdr["stipend"]);
																	Parent1.Text = db_reader.GetString("parent1_fullname");
																	Parent2.Text = db_reader.GetString("parent2_fullname");
																	Parent3.Text = rdr["parent1_fullname2"].ToString();
																	Parent4.Text = rdr["parent2_fullname2"].ToString();
																	Sponsor1.Text = rdr["witness1"].ToString();
																	Sponsor2.Text = rdr["witness2"].ToString();
																	Residence3.Text = rdr["witness1address"].ToString();
																	Residence4.Text = rdr["witness2address"].ToString();
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
													MarriageDate.Text = db_reader.GetString("record_date");
													Age1.Value = 0;
													Age2.Value = 0;
													Status1.Text = "---";
													Status2.Text = "---";
													FullName1.Text = db_reader.GetString("recordholder_fullname");
													FullName2.Text = "---";
													Hometown1.Text = "---";
													Hometown2.Text = "---";
													Residence1.Text = "---";
													Residence2.Text = "---";
													Stipend.Value = 0;
													Parent1.Text = db_reader.GetString("parent1_fullname");
													Parent2.Text = db_reader.GetString("parent2_fullname");
													Parent3.Text = "---";
													Parent4.Text = "---";
													Sponsor1.Text = "---";
													Sponsor2.Text = "---";
													Residence3.Text = "---";
													Residence4.Text = "---";
													Minister.Text = "---";
													Remarks.Text = "---";
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
										cmd2.CommandText = "SELECT * FROM records, matrimonial_records WHERE records.record_id = @rid AND records.record_id = matrimonial_records.record_id ORDER BY records.entry_number ASC;";
										cmd2.Parameters.AddWithValue("@rid", recordID);
										cmd2.Prepare();
										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												EntryNum.Value = Convert.ToDouble(db_reader2.GetString("entry_number"));
												PageNum.Value = Convert.ToDouble(db_reader2.GetString("page_number"));
												MarriageDate.Text = db_reader2.GetString("record_date");
												Age1.Value = Convert.ToDouble(db_reader2.GetString("age1"));
												Age2.Value = Convert.ToDouble(db_reader2.GetString("age2"));
												Status1.Text = db_reader2.GetString("status1");
												Status2.Text = db_reader2.GetString("status2");
												FullName1.Text = db_reader2.GetString("recordholder_fullname");
												FullName2.Text = db_reader2.GetString("recordholder2_fullname");
												Hometown1.Text = db_reader2.GetString("place_of_origin1");
												Hometown2.Text = db_reader2.GetString("place_of_origin2");
												Residence1.Text = db_reader2.GetString("residence1");
												Residence2.Text = db_reader2.GetString("residence2");
												Stipend.Value = Convert.ToDouble(db_reader2.GetString("stipend"));
												Parent1.Text = db_reader2.GetString("parent1_fullname");
												Parent2.Text = db_reader2.GetString("parent2_fullname");
												Parent3.Text = db_reader2.GetString("parent1_fullname2");
												Parent4.Text = db_reader2.GetString("parent2_fullname2");
												Sponsor1.Text = db_reader2.GetString("witness1");
												Sponsor2.Text = db_reader2.GetString("witness2");
												Residence3.Text = db_reader2.GetString("witness1address");
												Residence4.Text = db_reader2.GetString("witness2address");
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
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
			Suggestions5.Visibility = Visibility.Hidden;
			Suggestions6.Visibility = Visibility.Hidden;
			Suggestions7.Visibility = Visibility.Hidden;
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
											cmd2.Parameters.AddWithValue("@record_date", marriageDate);
											cmd2.Parameters.AddWithValue("@recordholder_fullname", fullName1);
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
														CommandText = "UPDATE matrimonial_records SET recordholder2_fullname = @recordholder2_fullname, parent1_fullname2 = @parent1_fullname2, parent2_fullname2 = @parent2_fullname2, status1 = @status1, status2 = @status2, age1 = @age1, age2 = @age2, place_of_origin1 = @place_of_origin1, place_of_origin2 = @place_of_origin2, residence1 = @residence1, residence2 = @residence2, witness1 = @witness1, witness2 = @witness2, witness1address = @witness1address, witness2address = @witness2address, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;"
													};
													command.Parameters.Add(new SQLiteParameter("@record_id", recordID));
													command.Parameters.Add(new SQLiteParameter("@recordholder2_fullname", fullName2));
													command.Parameters.Add(new SQLiteParameter("@parent1_fullname2", parent3));
													command.Parameters.Add(new SQLiteParameter("@parent2_fullname2", parent4));
													command.Parameters.Add(new SQLiteParameter("@status1", status1));
													command.Parameters.Add(new SQLiteParameter("@status2", status2));
													command.Parameters.Add(new SQLiteParameter("@age1", age1));
													command.Parameters.Add(new SQLiteParameter("@age2", age2));
													command.Parameters.Add(new SQLiteParameter("@place_of_origin1", hometown1));
													command.Parameters.Add(new SQLiteParameter("@place_of_origin2", hometown2));
													command.Parameters.Add(new SQLiteParameter("@residence1", residence1));
													command.Parameters.Add(new SQLiteParameter("@residence2", residence2));
													command.Parameters.Add(new SQLiteParameter("@witness1", sponsor1));
													command.Parameters.Add(new SQLiteParameter("@witness2", sponsor2));
													command.Parameters.Add(new SQLiteParameter("@witness1address", residence3));
													command.Parameters.Add(new SQLiteParameter("@witness2address", residence4));
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
											ret = 0;
										}
									}
								}
								else
								{
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										try
										{
											MySqlCommand cmd2 = dbman.DBConnect().CreateCommand();
											cmd2.CommandText =
												"UPDATE records SET page_number = @page_number, entry_number = @entry_number, record_date = @record_date, recordholder_fullname = @recordholder_fullname, parent1_fullname = @parent1_fullname, parent2_fullname = @parent2_fullname WHERE record_id = @record_id;";
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@page_number", pageNum);
											cmd2.Parameters.AddWithValue("@entry_number", entryNum);
											cmd2.Parameters.AddWithValue("@record_date", marriageDate);
											cmd2.Parameters.AddWithValue("@recordholder_fullname", fullName1);
											cmd2.Parameters.AddWithValue("@parent1_fullname", parent1);
											cmd2.Parameters.AddWithValue("@parent2_fullname", parent2);
											cmd2.Prepare();
											int stat_code = cmd2.ExecuteNonQuery();
											conn3.Close();

											conn3.Open();
											cmd2 = conn3.CreateCommand();
											cmd2.CommandText =
												"UPDATE matrimonial_records SET recordholder2_fullname = @recordholder2_fullname, parent1_fullname2 = @parent1_fullname2, parent2_fullname2 = @parent2_fullname2, status1 = @status1, status2 = @status2, age1 = @age1, age2 = @age2, place_of_origin1 = @place_of_origin1, place_of_origin2 = @place_of_origin2, residence1 = @residence1, residence2 = @residence2, witness1 = @witness1, witness2 = @witness2, witness1address = @witness1address, witness2address = @witness2address, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;";
											cmd2.Prepare();
											cmd2.Parameters.AddWithValue("@record_id", recordID);
											cmd2.Parameters.AddWithValue("@recordholder2_fullname", fullName2);
											cmd2.Parameters.AddWithValue("@parent1_fullname2", parent3);
											cmd2.Parameters.AddWithValue("@parent2_fullname2", parent4);
											cmd2.Parameters.AddWithValue("@status1", status1);
											cmd2.Parameters.AddWithValue("@status2", status2);
											cmd2.Parameters.AddWithValue("@age1", age1);
											cmd2.Parameters.AddWithValue("@age2", age2);
											cmd2.Parameters.AddWithValue("@place_of_origin1", hometown1);
											cmd2.Parameters.AddWithValue("@place_of_origin2", hometown2);
											cmd2.Parameters.AddWithValue("@residence1", residence1);
											cmd2.Parameters.AddWithValue("@residence2", residence2);
											cmd2.Parameters.AddWithValue("@witness1", sponsor1);
											cmd2.Parameters.AddWithValue("@witness2", sponsor2);
											cmd2.Parameters.AddWithValue("@witness1address", residence3);
											cmd2.Parameters.AddWithValue("@witness2address", residence4);
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
		private int FetchMatrimonialStipend()
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
		/// Validates input.
		/// </summary>
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
		private bool CheckInputs()
		{
			var bc = new BrushConverter();

			MarriageDate.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			FullName1.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			FullName2.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Minister.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			bool ret = true;

			if (string.IsNullOrWhiteSpace(MarriageDate.Text))
			{
				MarriageDate.ToolTip = "This field is required.";
				MarriageDate.BorderBrush = Brushes.Red;
				Icon3.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (EntryNum.Value < 0)
			{
				EntryNum.ToolTip = "Must be greater than zero.";
				EntryNum.BorderBrush = Brushes.Red;
				Icon2.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (PageNum.Value < 0)
			{
				PageNum.ToolTip = "Must be greater than zero.";
				PageNum.BorderBrush = Brushes.Red;
				Icon1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (Stipend.Value == 0)
			{
				Stipend.ToolTip = "Notice: Stipend is set to zero.";
				Stipend.BorderBrush = Brushes.Orange;
				Icon4.BorderBrush = Brushes.Red;

				MsgStipend();
				ret = true;
			}
			if (string.IsNullOrWhiteSpace(FullName1.Text))
			{
				FullName1.ToolTip = "This field is required.";
				FullName1.BorderBrush = Brushes.Red;
				Icon5.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(FullName2.Text))
			{
				FullName2.ToolTip = "This field is required.";
				FullName2.BorderBrush = Brushes.Red;
				Icon12.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent1.Text))
			{
				Parent1.ToolTip = "This field is required.";
				Parent1.BorderBrush = Brushes.Red;
				Icon10.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent3.Text))
			{
				Parent3.ToolTip = "This field is required.";
				Parent3.BorderBrush = Brushes.Red;
				Icon17.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent2.Text))
			{
				Parent2.ToolTip = "This field is required.";
				Parent2.BorderBrush = Brushes.Red;
				Icon11.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent4.Text))
			{
				Parent4.ToolTip = "This field is required.";
				Parent4.BorderBrush = Brushes.Red;
				Icon18.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Hometown1.Text))
			{
				Hometown1.ToolTip = "This field is required.";
				Hometown1.BorderBrush = Brushes.Red;
				Icon8.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Hometown2.Text))
			{
				Hometown2.ToolTip = "This field is required.";
				Hometown2.BorderBrush = Brushes.Red;
				Icon15.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence1.Text))
			{
				Residence1.ToolTip = "This field is required.";
				Residence1.BorderBrush = Brushes.Red;
				Icon9.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence2.Text))
			{
				Residence2.ToolTip = "This field is required.";
				Residence2.BorderBrush = Brushes.Red;
				Icon16.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence3.Text))
			{
				Residence3.ToolTip = "This field is required.";
				Residence3.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence4.Text))
			{
				Residence4.ToolTip = "This field is required.";
				Residence4.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sponsor1.Text))
			{
				Sponsor1.ToolTip = "This field is required.";
				Sponsor1.BorderBrush = Brushes.Red;
				Icon19.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sponsor2.Text))
			{
				Sponsor2.ToolTip = "This field is required.";
				Sponsor2.BorderBrush = Brushes.Red;
				Icon21.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Minister.Text))
			{
				Minister.ToolTip = "This field is required.";
				Minister.BorderBrush = Brushes.Red;
				Icon20.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Age1.Value.ToString()))
			{
				Age1.ToolTip = "This field is required.";
				Age1.BorderBrush = Brushes.Red;
				Icon6.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Age2.Value.ToString()))
			{
				Age2.ToolTip = "This field is required.";
				Age2.BorderBrush = Brushes.Red;
				Icon13.BorderBrush = Brushes.Red;

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
				entryNum = Convert.ToInt32(EntryNum.Value);
				pageNum = Convert.ToInt32(PageNum.Value);
				marriageDate = Convert.ToDateTime(MarriageDate.Text).ToString("yyyy-MM-dd");
				age1 = Convert.ToInt32(Age1.Value);
				age2 = Convert.ToInt32(Age2.Value);
				fullName1 = ValidateInp(FullName1.Text);
				fullName2 = ValidateInp(FullName2.Text);
				switch (Status1.SelectedIndex)
				{
					case 0:
						status1 = "Widow";
						break;
					case 1:
						status1 = "Widower";
						break;
					case 2:
						status1 = "Single";
						break;
					case 3:
						status1 = "Conjugal";
						break;
					case 4:
						status1 = "Adult";
						break;
					case 5:
						status1 = "Infant";
						break;
					default:
						status1 = "----";
						break;
				}
				switch (Status2.SelectedIndex)
				{
					case 0:
						status2 = "Widow";
						break;
					case 1:
						status2 = "Widower";
						break;
					case 2:
						status2 = "Single";
						break;
					case 3:
						status2 = "Conjugal";
						break;
					case 4:
						status2 = "Adult";
						break;
					case 5:
						status2 = "Infant";
						break;
					default:
						status2 = "----";
						break;
				}
				hometown1 = ValidateInp(Hometown1.Text);
				hometown2 = ValidateInp(Hometown2.Text);
				residence1 = ValidateInp(Residence1.Text);
				residence2 = ValidateInp(Residence2.Text);
				parent1 = ValidateInp(Parent1.Text);
				parent2 = ValidateInp(Parent2.Text);
				parent3 = ValidateInp(Parent3.Text);
				parent4 = ValidateInp(Parent4.Text);
				sponsor1 = ValidateInp(Sponsor1.Text);
				sponsor2 = ValidateInp(Sponsor2.Text);
				residence3 = ValidateInp(Residence3.Text);
				residence4 = ValidateInp(Residence4.Text);
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

			Hometown1SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_origin1 FROM matrimonial_records WHERE " +
					"place_of_origin1 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Hometown1.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Hometown1SuggestionArea.Items.Add(db_reader.GetString("place_of_origin1"));
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

			Hometown2SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_origin2 FROM matrimonial_records WHERE " +
					"place_of_origin2 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Hometown2.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Hometown2SuggestionArea.Items.Add(db_reader.GetString("place_of_origin2"));
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
				cmd.CommandText = "SELECT DISTINCT residence1 FROM matrimonial_records WHERE " +
					"residence1 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence1.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence1SuggestionArea.Items.Add(db_reader.GetString("residence1"));
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
				cmd.CommandText = "SELECT DISTINCT residence2 FROM matrimonial_records WHERE " +
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

			Residence3SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT witness1address FROM matrimonial_records WHERE " +
					"witness1address LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence3.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence3SuggestionArea.Items.Add(db_reader.GetString("witness1address"));
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

			Residence4SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT witness2address FROM matrimonial_records WHERE " +
					"witness2address LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence4.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence4SuggestionArea.Items.Add(db_reader.GetString("witness2address"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions6.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions7(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM matrimonial_records WHERE " +
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

				Suggestions7.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click1(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Hometown1SuggestionArea.SelectedItem = item;
			Hometown1.Text = Hometown1SuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click2(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Hometown2SuggestionArea.SelectedItem = item;
			Hometown2.Text = Hometown2SuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
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
			Residence3SuggestionArea.SelectedItem = item;
			Residence3.Text = Residence3SuggestionArea.SelectedItem.ToString();
			Suggestions5.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click6(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Residence4SuggestionArea.SelectedItem = item;
			Residence4.Text = Residence4SuggestionArea.SelectedItem.ToString();
			Suggestions6.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click7(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions7.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
			Suggestions5.Visibility = Visibility.Hidden;
			Suggestions6.Visibility = Visibility.Hidden;
			Suggestions7.Visibility = Visibility.Hidden;
		}
	}
}
