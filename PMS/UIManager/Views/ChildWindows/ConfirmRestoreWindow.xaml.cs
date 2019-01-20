using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using System.Data.SQLite;
using System.Data.SqlClient;
using System.IO;
using System.ComponentModel;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class ConfirmRestoreWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn, conn2;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int bookNum;

		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;

		private Archives arc;
		private string arcDrivePath;
		private string path;

		private int stat_code2 = 0;

		public ConfirmRestoreWindow(Archives a, int book_Num, string archiveDrive)
		{
			arc = a;
			arcDrivePath = archiveDrive;
			bookNum = book_Num;
			pmsutil = new PMSUtil();
			InitializeComponent();
		}
		private int ConfirmArchival()
		{
			return 0;
		}
		private void ConfirmRestore_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ArchivalProgBar.IsIndeterminate = true;
			path = @"\archive.db";
			BackgroundWorker worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
			worker.DoWork += RestoreItems;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			worker.RunWorkerAsync(10000);
		}
		void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//if (e.UserState != null)
			//EntriesHolder.Items.Add(e.UserState);
		}
		void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//SyncChanges();
			ArchivalProgBar.IsIndeterminate = false;
			if (pmsutil.CheckArchiveDrive(path) != "dc")
			{
				if (stat_code2 > 0) {
					MsgSuccess();
				}
				else {
					MsgFail();
				}
			}
			else
			{
				MsgFail();
			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected register has been restored successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void RestoreItems(object sender, DoWorkEventArgs e)
		{
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "DELETE FROM archives WHERE book_number = @book_number;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					int stat_code = cmd.ExecuteNonQuery();
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "UPDATE registers SET status = 'Normal' WHERE book_number = @book_number;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					stat_code = cmd.ExecuteNonQuery();
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM records, registers WHERE registers.book_number = records.book_number AND records.book_number = @book_number;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string archiveDrive = pmsutil.CheckArchiveDrive(path);

						if (db_reader.GetString("book_type") == "Baptismal")
						{
							SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
							{
								FailIfMissing = true,
								DataSource = archiveDrive
							};
							using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
							{
								// open the connection:
								connection.Open();
								string stm = "SELECT * FROM baptismal_records WHERE record_id='" + db_reader.GetString("record_id") + "' LIMIT 1;";

								using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
								{
									using (SQLiteDataReader rdr = cmdx.ExecuteReader())
									{
										while (rdr.Read())
										{
											DateTime dateOfBirth = Convert.ToDateTime(rdr["birthday"].ToString());
											using (conn2 = new MySqlConnection(dbman.GetConnStr()))
											{
												conn2.Open();
												if (conn2.State == ConnectionState.Open)
												{
													MySqlCommand cmd2 = conn2.CreateCommand();
													cmd2.CommandText =
														"INSERT INTO baptismal_records(record_id, birthday, legitimacy, place_of_birth, sponsor1, sponsor2, stipend, minister, remarks)" +
														"VALUES(@record_id, @birthday, @legitimacy, @place_of_birth, @sponsor1, @sponsor2, @stipend, @minister, @remarks)";
													cmd2.Prepare();
													cmd2.Parameters.AddWithValue("@record_id", db_reader.GetString("record_id"));
													cmd2.Parameters.AddWithValue("@birthday", dateOfBirth.ToString("MMM dd, yyyy"));
													cmd2.Parameters.AddWithValue("@legitimacy", rdr["legitimacy"].ToString());
													cmd2.Parameters.AddWithValue("@place_of_birth", rdr["place_of_birth"].ToString());
													cmd2.Parameters.AddWithValue("@sponsor1", rdr["sponsor1"].ToString());
													cmd2.Parameters.AddWithValue("@sponsor2", rdr["sponsor2"].ToString());
													cmd2.Parameters.AddWithValue("@stipend", Convert.ToDouble(rdr["stipend"]));
													cmd2.Parameters.AddWithValue("@minister", rdr["minister"].ToString());
													cmd2.Parameters.AddWithValue("@remarks", rdr["remarks"].ToString());
													stat_code2 = cmd2.ExecuteNonQuery();
													conn2.Close();
													//string tmp = pmsutil.LogRecord(recID, "LOGC-01");
												}
											}
										}
									}
								}
								SQLiteCommand command = new SQLiteCommand(null, connection)
								{
									CommandText = "DELETE FROM baptismal_records WHERE record_id = @rid;"
								};
								command.Parameters.Add(new SQLiteParameter("@rid", db_reader.GetString("record_id")));
								// Call Prepare after setting the Commandtext and Parameters.
								command.Prepare();
								command.ExecuteNonQuery();
							}
						}
						else if (db_reader.GetString("book_type") == "Confirmation")
						{
							SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
							{
								FailIfMissing = true,
								DataSource = archiveDrive
							};
							using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
							{
								// open the connection:
								connection.Open();
								string stm = "SELECT * FROM confirmation_records WHERE record_id='" + db_reader.GetString("record_id") + "' LIMIT 1;";

								using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
								{
									using (SQLiteDataReader rdr = cmdx.ExecuteReader())
									{
										while (rdr.Read())
										{
											using (conn2 = new MySqlConnection(dbman.GetConnStr()))
											{
												conn2.Open();
												if (conn2.State == ConnectionState.Open)
												{
													MySqlCommand cmd2 = conn2.CreateCommand();
													cmd2.CommandText =
														"INSERT INTO confirmation_records(record_id, age, parochia, province, place_of_baptism, sponsor, sponsor2, stipend, minister, remarks)" +
														"VALUES(@record_id, @age, @parish, @province, @place_of_baptism, @sponsor, @sponsor2, @stipend, @minister, @remarks)";
													cmd2.Prepare();
													cmd2.Parameters.AddWithValue("@record_id", db_reader.GetString("record_id"));
													cmd2.Parameters.AddWithValue("@age", rdr["age"]);
													cmd2.Parameters.AddWithValue("@parish", rdr["parish"]);
													cmd2.Parameters.AddWithValue("@province", rdr["province"]);
													cmd2.Parameters.AddWithValue("@place_of_baptism", rdr["place_of_baptism"].ToString());
													cmd2.Parameters.AddWithValue("@sponsor", rdr["sponsor"].ToString());
													cmd2.Parameters.AddWithValue("@sponsor2", rdr["sponsor2"].ToString());
													cmd2.Parameters.AddWithValue("@stipend", Convert.ToDouble(rdr["stipend"]));
													cmd2.Parameters.AddWithValue("@minister", rdr["minister"].ToString());
													cmd2.Parameters.AddWithValue("@remarks", rdr["remarks"].ToString());
													stat_code2 = cmd2.ExecuteNonQuery();
													conn2.Close();
													//string tmp = pmsutil.LogRecord(recID, "LOGC-01");
												}
											}
										}
									}
								}
								SQLiteCommand command = new SQLiteCommand(null, connection)
								{
									CommandText = "DELETE FROM confirmation_records WHERE record_id = @rid;"
								};
								command.Parameters.Add(new SQLiteParameter("@rid", db_reader.GetString("record_id")));
								// Call Prepare after setting the Commandtext and Parameters.
								command.Prepare();
								command.ExecuteNonQuery();
							}
						}
						else if (db_reader.GetString("book_type") == "Matrimonial")
						{
							SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
							{
								FailIfMissing = true,
								DataSource = archiveDrive
							};
							using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
							{
								// open the connection:
								connection.Open();
								string stm = "SELECT * FROM matrimonial_records WHERE record_id='" + db_reader.GetString("record_id") + "' LIMIT 1;";

								using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
								{
									using (SQLiteDataReader rdr = cmdx.ExecuteReader())
									{
										while (rdr.Read())
										{
											using (conn2 = new MySqlConnection(dbman.GetConnStr()))
											{
												conn2.Open();
												if (conn2.State == ConnectionState.Open)
												{
													MySqlCommand cmd2 = conn2.CreateCommand();
													cmd2.CommandText =
														"INSERT INTO matrimonial_records(record_id, recordholder2_fullname, parent1_fullname2, parent2_fullname2, status1, status2, age1, age2, place_of_origin1, place_of_origin2, residence1, residence2, witness1, witness2, witness1address, witness2address, stipend, minister, remarks)" +
														"VALUES(@record_id, @recordholder2_fullname, @parent1_fullname2, @parent2_fullname2, @status1, @status2, @age1, @age2, @place_of_origin1, @place_of_origin2, @residence1, @residence2, @witness1, @witness2, @witness1address, @witness2address, @stipend, @minister, @remarks)";
													cmd2.Prepare();
													cmd2.Parameters.AddWithValue("@record_id", db_reader.GetString("record_id"));
													cmd2.Parameters.AddWithValue("@recordholder2_fullname", rdr["recordholder2_fullname"]);
													cmd2.Parameters.AddWithValue("@parent1_fullname2", rdr["parent1_fullname2"]);
													cmd2.Parameters.AddWithValue("@parent2_fullname2", rdr["parent2_fullname2"]);
													cmd2.Parameters.AddWithValue("@status1", rdr["status1"]);
													cmd2.Parameters.AddWithValue("@status2", rdr["status2"]);
													cmd2.Parameters.AddWithValue("@age1", rdr["age1"]);
													cmd2.Parameters.AddWithValue("@age2", rdr["age2"]);
													cmd2.Parameters.AddWithValue("@place_of_origin1", rdr["place_of_origin1"]);
													cmd2.Parameters.AddWithValue("@place_of_origin2", rdr["place_of_origin2"]);
													cmd2.Parameters.AddWithValue("@residence1", rdr["residence1"]);
													cmd2.Parameters.AddWithValue("@residence2", rdr["residence2"]);
													cmd2.Parameters.AddWithValue("@witness1", rdr["witness1"]);
													cmd2.Parameters.AddWithValue("@witness2", rdr["witness2"]);
													cmd2.Parameters.AddWithValue("@witness1address", rdr["witness1address"]);
													cmd2.Parameters.AddWithValue("@witness2address", rdr["witness2address"]);
													cmd2.Parameters.AddWithValue("@stipend", Convert.ToDouble(rdr["stipend"]));
													cmd2.Parameters.AddWithValue("@minister", rdr["minister"].ToString());
													cmd2.Parameters.AddWithValue("@remarks", rdr["remarks"].ToString());
													stat_code2 = cmd2.ExecuteNonQuery();
													conn2.Close();
													//string tmp = pmsutil.LogRecord(recID, "LOGC-01");
												}
											}
										}
									}
								}
								SQLiteCommand command = new SQLiteCommand(null, connection)
								{
									CommandText = "DELETE FROM matrimonial_records WHERE record_id = @rid;"
								};
								command.Parameters.Add(new SQLiteParameter("@rid", db_reader.GetString("record_id")));
								// Call Prepare after setting the Commandtext and Parameters.
								command.Prepare();
								command.ExecuteNonQuery();
							}
						}
						else if (db_reader.GetString("book_type") == "Burial")
						{
							SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
							{
								FailIfMissing = true,
								DataSource = archiveDrive
							};
							using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
							{
								// open the connection:
								connection.Open();
								string stm = "SELECT * FROM burial_records WHERE record_id='" + db_reader.GetString("record_id") + "' LIMIT 1;";

								using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
								{
									using (SQLiteDataReader rdr = cmdx.ExecuteReader())
									{
										while (rdr.Read())
										{
											using (conn2 = new MySqlConnection(dbman.GetConnStr()))
											{
												conn2.Open();
												if (conn2.State == ConnectionState.Open)
												{
													MySqlCommand cmd2 = conn2.CreateCommand();
													cmd2.CommandText =
													"INSERT INTO burial_records(record_id, burial_date, age, status, residence, residence2, sacrament, cause_of_death, place_of_interment, stipend, minister, remarks)" +
													"VALUES(@record_id, @burial_date, @age, @status, @residence, @residence2, @sacrament, @cause_of_death, @place_of_interment, @stipend, @minister, @remarks)";
													cmd2.Prepare();
													cmd2.Parameters.AddWithValue("@record_id", db_reader.GetString("record_id"));
													cmd2.Parameters.AddWithValue("@burial_date", rdr["burial_date"]);
													cmd2.Parameters.AddWithValue("@age", rdr["age"]);
													cmd2.Parameters.AddWithValue("@status", rdr["status"]);
													cmd2.Parameters.AddWithValue("@residence", rdr["residence"]);
													cmd2.Parameters.AddWithValue("@residence2", rdr["residence2"]);
													cmd2.Parameters.AddWithValue("@sacrament", rdr["sacrament"]);
													cmd2.Parameters.AddWithValue("@cause_of_death", rdr["cause_of_death"]);
													cmd2.Parameters.AddWithValue("@place_of_interment", rdr["place_of_interment"]);
													cmd2.Parameters.AddWithValue("@stipend", Convert.ToDouble(rdr["stipend"]));
													cmd2.Parameters.AddWithValue("@minister", rdr["minister"].ToString());
													cmd2.Parameters.AddWithValue("@remarks", rdr["remarks"].ToString());
													stat_code2 = cmd2.ExecuteNonQuery();
													conn.Close();
												}
											}
										}
									}
								}

								SQLiteCommand command = new SQLiteCommand(null, connection)
								{
									CommandText = "DELETE FROM baptismal_records WHERE record_id = @rid;"
								};
								command.Parameters.Add(new SQLiteParameter("@rid", db_reader.GetString("record_id")));
								// Call Prepare after setting the Commandtext and Parameters.
								command.Prepare();
								command.ExecuteNonQuery();
							}
						}
					}
				}
				else {

				}
			}
		}
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
