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
	public partial class ConfirmArchivalWindow : ChildWindow
	{
		//MYSQL Related Stuff
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

		public ConfirmArchivalWindow(Archives a, int book_Num, string archiveDrive)
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
		private void Phase2()
		{
			SQLiteConnectionStringBuilder connectionString = new SQLiteConnectionStringBuilder
			{
				FailIfMissing = true,
				DataSource = arcDrivePath
			};


			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM registers, records WHERE registers.book_number = records.book_number AND registers.book_number = @book_number;";
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("book_type") == "Baptismal") {
						if (dbman.DBConnect().State == ConnectionState.Open)
						{
							MySqlCommand cmd_tmp = dbman.DBConnect().CreateCommand();
							cmd_tmp.CommandText = "SELECT * FROM records, baptismal_records WHERE records.record_id = baptismal_records.record_id AND records.book_number = @book_number;";
							cmd_tmp.Parameters.AddWithValue("@book_number", bookNum);
							MySqlDataReader db_readertmp = cmd_tmp.ExecuteReader();
							while (db_readertmp.Read())
							{
								//Copy the selected register's record to the archive drive
								using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
								{

									// open the connection:
									connection.Open();

									SQLiteCommand command = new SQLiteCommand(null, connection)
									{
										CommandText = "INSERT INTO baptismal_records (record_id, birthday, legitimacy, place_of_birth, sponsor1, sponsor2, stipend, minister, remarks) VALUES (@record_id, @birthday, @legitimacy, @place_of_birth, @sponsor1, @sponsor2, @stipend, @minister, @remarks);"
									};
									command.Parameters.Add(new SQLiteParameter("@record_id", db_readertmp.GetString("record_id")));
									command.Parameters.Add(new SQLiteParameter("@birthday", db_readertmp.GetString("birthday")));
									command.Parameters.Add(new SQLiteParameter("@legitimacy", db_readertmp.GetString("legitimacy")));
									command.Parameters.Add(new SQLiteParameter("@place_of_birth", db_readertmp.GetString("place_of_birth")));
									command.Parameters.Add(new SQLiteParameter("@sponsor1", db_readertmp.GetString("sponsor1")));
									command.Parameters.Add(new SQLiteParameter("@sponsor2", db_readertmp.GetString("sponsor2")));
									command.Parameters.Add(new SQLiteParameter("@stipend", db_readertmp.GetFloat("stipend")));
									command.Parameters.Add(new SQLiteParameter("@minister", db_readertmp.GetString("minister")));
									command.Parameters.Add(new SQLiteParameter("@remarks", db_readertmp.GetString("remarks")));
									// Call Prepare after setting the Commandtext and Parameters.
									command.Prepare();
									command.ExecuteNonQuery();

									//Delete the copied records
									MySqlCommand cmd_tmp2 = dbman.DBConnect().CreateCommand();
									cmd_tmp2.CommandText = "DELETE FROM baptismal_records WHERE record_id = @record_id;";
									cmd_tmp2.Parameters.AddWithValue("@record_id", db_readertmp.GetString("record_id"));
									cmd_tmp2.ExecuteNonQuery();
								}
							}
						}
					} else if (db_reader.GetString("book_type") == "Confirmation") {
						if (dbman.DBConnect().State == ConnectionState.Open)
						{
							MySqlCommand cmd_tmp = dbman.DBConnect().CreateCommand();
							cmd_tmp.CommandText = "SELECT * FROM records, confirmation_records WHERE records.record_id = confirmation_records.record_id AND records.book_number = @book_number;";
							cmd_tmp.Parameters.AddWithValue("@book_number", bookNum);
							MySqlDataReader db_readertmp = cmd_tmp.ExecuteReader();
							while (db_readertmp.Read())
							{
								//Copy the selected register's record to the archive drive
								using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
								{

									// open the connection:
									connection.Open();

									SQLiteCommand command = new SQLiteCommand(null, connection)
									{
										CommandText = "INSERT INTO confirmation_records (record_id, age, parochia, place_of_baptism, sponsor, sponsor2, stipend, minister, remarks) VALUES (@record_id, @age, @parochia, @place_of_baptism, @sponsor, @sponsor2, @stipend, @minister, @remarks);"
									};
									command.Parameters.Add(new SQLiteParameter("@record_id", db_readertmp.GetString("record_id")));
									command.Parameters.Add(new SQLiteParameter("@age", db_readertmp.GetInt32("age")));
									command.Parameters.Add(new SQLiteParameter("@parochia", db_readertmp.GetString("parochia")));
									command.Parameters.Add(new SQLiteParameter("@place_of_baptism", db_readertmp.GetString("place_of_baptism")));
									command.Parameters.Add(new SQLiteParameter("@sponsor", db_readertmp.GetString("sponsor")));
									command.Parameters.Add(new SQLiteParameter("@sponsor2", db_readertmp.GetString("sponsor2")));
									command.Parameters.Add(new SQLiteParameter("@stipend", db_readertmp.GetFloat("stipend")));
									command.Parameters.Add(new SQLiteParameter("@minister", db_readertmp.GetString("minister")));
									command.Parameters.Add(new SQLiteParameter("@remarks", db_readertmp.GetString("remarks")));
									// Call Prepare after setting the Commandtext and Parameters.
									command.Prepare();
									command.ExecuteNonQuery();

									//Delete the copied records
									MySqlCommand cmd_tmp2 = dbman.DBConnect().CreateCommand();
									cmd_tmp2.CommandText = "DELETE FROM confirmation_records WHERE record_id = @record_id;";
									cmd_tmp2.Parameters.AddWithValue("@record_id", db_readertmp.GetString("record_id"));
									cmd_tmp2.ExecuteNonQuery();
								}
							}
						}
					} else if (db_reader.GetString("book_type") == "Matrimonial") {
						if (dbman.DBConnect().State == ConnectionState.Open)
						{
							MySqlCommand cmd_tmp = dbman.DBConnect().CreateCommand();
							cmd_tmp.CommandText = "SELECT * FROM records, matrimonial_records WHERE records.record_id = matrimonial_records.record_id AND records.book_number = @book_number;";
							cmd_tmp.Parameters.AddWithValue("@book_number", bookNum);
							MySqlDataReader db_readertmp = cmd_tmp.ExecuteReader();
							while (db_readertmp.Read())
							{
								//Copy the selected register's record to the archive drive
								using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
								{

									// open the connection:
									connection.Open();

									SQLiteCommand command = new SQLiteCommand(null, connection)
									{
										CommandText = "INSERT INTO matrimonial_records (record_id, recordholder2_fullname, parent1_fullname2, parent2_fullname2, status1, status2, age1, age2, place_of_origin1, place_of_origin2, residence1, residence2, witness1, witness2, witness1address, witness2address, stipend, minister, remarks) VALUES (@record_id, @recordholder2_fullname, @parent1_fullname2, @parent2_fullname2, @status1, @status2, @age1, @age2, @place_of_origin1, @place_of_origin2, @residence1, @residence2, @witness1, @witness2, @witness1address, @witness2address, @stipend, @minister, @remarks);"
									};
									command.Parameters.Add(new SQLiteParameter("@record_id", db_readertmp.GetString("record_id")));
									command.Parameters.Add(new SQLiteParameter("@recordholder2_fullname", db_readertmp.GetString("recordholder2_fullname")));
									command.Parameters.Add(new SQLiteParameter("@parent1_fullname2", db_readertmp.GetString("parent1_fullname2")));
									command.Parameters.Add(new SQLiteParameter("@parent2_fullname2", db_readertmp.GetString("parent2_fullname2")));
									command.Parameters.Add(new SQLiteParameter("@status1", db_readertmp.GetString("status1")));
									command.Parameters.Add(new SQLiteParameter("@status2", db_readertmp.GetString("status2")));
									command.Parameters.Add(new SQLiteParameter("@age1", db_readertmp.GetInt32("age1")));
									command.Parameters.Add(new SQLiteParameter("@age2", db_readertmp.GetInt32("age2")));
									command.Parameters.Add(new SQLiteParameter("@place_of_origin1", db_readertmp.GetString("place_of_origin1")));
									command.Parameters.Add(new SQLiteParameter("@place_of_origin2", db_readertmp.GetString("place_of_origin2")));
									command.Parameters.Add(new SQLiteParameter("@residence1", db_readertmp.GetString("residence1")));
									command.Parameters.Add(new SQLiteParameter("@residence2", db_readertmp.GetString("residence2")));
									command.Parameters.Add(new SQLiteParameter("@witness1", db_readertmp.GetString("witness1")));
									command.Parameters.Add(new SQLiteParameter("@witness2", db_readertmp.GetString("witness2")));
									command.Parameters.Add(new SQLiteParameter("@witness1address", db_readertmp.GetString("witness1address")));
									command.Parameters.Add(new SQLiteParameter("@witness2address", db_readertmp.GetString("witness2address")));
									command.Parameters.Add(new SQLiteParameter("@stipend", db_readertmp.GetFloat("stipend")));
									command.Parameters.Add(new SQLiteParameter("@minister", db_readertmp.GetString("minister")));
									command.Parameters.Add(new SQLiteParameter("@remarks", db_readertmp.GetString("remarks")));
									// Call Prepare after setting the Commandtext and Parameters.
									command.Prepare();
									command.ExecuteNonQuery();

									//Delete the copied records
									MySqlCommand cmd_tmp2 = dbman.DBConnect().CreateCommand();
									cmd_tmp2.CommandText = "DELETE FROM matrimonial_records WHERE record_id = @record_id;";
									cmd_tmp2.Parameters.AddWithValue("@record_id", db_readertmp.GetString("record_id"));
									cmd_tmp2.ExecuteNonQuery();
								}
							}
						}
					} else if (db_reader.GetString("book_type") == "Burial") {
						if (dbman.DBConnect().State == ConnectionState.Open)
						{
							MySqlCommand cmd_tmp = dbman.DBConnect().CreateCommand();
							cmd_tmp.CommandText = "SELECT * FROM records, burial_records WHERE records.record_id = burial_records.record_id AND records.book_number = @book_number;";
							cmd_tmp.Parameters.AddWithValue("@book_number", bookNum);
							MySqlDataReader db_readertmp = cmd_tmp.ExecuteReader();
							while (db_readertmp.Read())
							{
								//Copy the selected register's record to the archive drive
								using (SQLiteConnection connection = new SQLiteConnection(connectionString.ToString()))
								{

									// open the connection:
									connection.Open();

									SQLiteCommand command = new SQLiteCommand(null, connection)
									{
										CommandText = "INSERT INTO burial_records (record_id, burial_date, age, status, residence, residence2, sacrament, cause_of_death, place_of_interment, stipend, minister, remarks) VALUES (@record_id, @burial_date, @age, @status, @residence, @residence2, @sacrament, @cause_of_death, @place_of_interment, @stipend, @minister, @remarks);"
									};
									command.Parameters.Add(new SQLiteParameter("@record_id", db_readertmp.GetString("record_id")));
									command.Parameters.Add(new SQLiteParameter("@burial_date", db_readertmp.GetString("burial_date")));
									command.Parameters.Add(new SQLiteParameter("@age", db_readertmp.GetInt32("age")));
									command.Parameters.Add(new SQLiteParameter("@status", db_readertmp.GetString("status")));
									command.Parameters.Add(new SQLiteParameter("@residence", db_readertmp.GetString("residence")));
									command.Parameters.Add(new SQLiteParameter("@residence2", db_readertmp.GetString("residence2")));
									command.Parameters.Add(new SQLiteParameter("@sacrament", db_readertmp.GetString("sacrament")));
									command.Parameters.Add(new SQLiteParameter("@cause_of_death", db_readertmp.GetString("cause_of_death")));
									command.Parameters.Add(new SQLiteParameter("@place_of_interment", db_readertmp.GetString("place_of_interment")));
									command.Parameters.Add(new SQLiteParameter("@stipend", db_readertmp.GetFloat("stipend")));
									command.Parameters.Add(new SQLiteParameter("@minister", db_readertmp.GetString("minister")));
									command.Parameters.Add(new SQLiteParameter("@remarks", db_readertmp.GetString("remarks")));
									// Call Prepare after setting the Commandtext and Parameters.
									command.Prepare();
									command.ExecuteNonQuery();

									//Delete the copied records
									MySqlCommand cmd_tmp2 = dbman.DBConnect().CreateCommand();
									cmd_tmp2.CommandText = "DELETE FROM burial_records WHERE record_id = @record_id;";
									cmd_tmp2.Parameters.AddWithValue("@record_id", db_readertmp.GetString("record_id"));
									cmd_tmp2.ExecuteNonQuery();
								}
							}
						}
					}
				}
				//close Connection
				dbman.DBClose();
			}
		}
		private void ConfirmArchival_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			ArchivalProgBar.IsIndeterminate = true;
			path = @"\archive.db";
			if (pmsutil.CheckArchiveDrive(path) != "dc")
			{
				MsgSuccess();
			}
			else
			{
				MsgFail();
			}
			BackgroundWorker worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
			worker.DoWork += ArchiveItems;
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
				MsgSuccess();
			}
			else
			{
				MsgFail();
			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected register has been archived successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void ArchiveItems(object sender, DoWorkEventArgs e)
		{
			string uid = Application.Current.Resources["uid"].ToString();
			string[] dt = pmsutil.GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			cTime = DateTime.Parse(dt[1] + " " + dt[2]);
			curDate = cDate.ToString("yyyy-MM-dd");
			curTime = cTime.ToString("HH:mm:ss");

			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			//TODO
			try
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO archives(book_number, date_archived, time_archived, archived_by)" +
					"VALUES(@book_number, @date_archived, @time_archived, @archived_by)";
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				cmd.Parameters.AddWithValue("@date_archived", curDate);
				cmd.Parameters.AddWithValue("@time_archived", curTime);
				cmd.Parameters.AddWithValue("@archived_by", uid);
				cmd.Prepare();
				int stat_code = cmd.ExecuteNonQuery();
				dbman = new DBConnectionManager();
				//TODO
				try
				{
					//Phase 1.2
					cmd = dbman.DBConnect().CreateCommand();
					cmd.CommandText =
						"UPDATE registers SET status = @status WHERE book_number = @book_number;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					cmd.Parameters.AddWithValue("@status", "Archived");
					cmd.Prepare();
					stat_code = cmd.ExecuteNonQuery();
					dbman.DBClose();
				}
				catch (MySqlException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
					//return 0;
				}
				dbman.DBClose();
				Phase2();
				//return stat_code;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}", ex.ToString());
				//return 0;
			}
		}
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
