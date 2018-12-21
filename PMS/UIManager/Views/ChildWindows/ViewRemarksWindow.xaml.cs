using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.SQLite;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for ViewRemarksWindow.xaml
	/// </summary>
	public partial class ViewRemarksWindow : ChildWindow
	{
		//MYSQL
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private PMSUtil pmsutil;

		public ViewRemarksWindow(string recordID)
        {
            InitializeComponent();

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
						cmd.CommandText = "SELECT * FROM registers, records WHERE records.record_id = @rid AND records.book_number = registers.book_number LIMIT 1;";
						cmd.Parameters.AddWithValue("@rid", recordID);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (db_reader.GetString("status") == "Archived")
								{
									pmsutil = new PMSUtil();

									string archiveDrive = "init";
									string path = @"\archive.db";

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
											string stm = "SELECT * FROM confirmation_records, matrimonial_records, baptismal_records, burial_records WHERE burial_records.record_id ='" + recordID + "' OR baptismal_records.record_id ='" + recordID + "' OR matrimonial_records.record_id ='" + recordID + "' OR confirmation_records.record_id ='" + recordID + "' LIMIT 1;";
											using (SQLiteCommand cmdx = new SQLiteCommand(stm, connection))
											{
												using (SQLiteDataReader rdr = cmdx.ExecuteReader())
												{
													while (rdr.Read())
													{
														RemarksContainer.Text = rdr["remarks"].ToString();
													}
												}
											}
										}
									}
									else
									{
										RemarksContainer.Text = "Archive drive not connected.";
									}
								}
								else
								{
									using (MySqlConnection conn3 = new MySqlConnection(dbman.GetConnStr()))
									{
										conn3.Open();
										MySqlCommand cmd2 = conn3.CreateCommand();
										cmd2.CommandText = "SELECT * FROM confirmation_records, matrimonial_records, baptismal_records, burial_records WHERE burial_records.record_id = @record_id OR baptismal_records.record_id = @record_id OR matrimonial_records.record_id = @record_id OR confirmation_records.record_id = @record_id LIMIT 1;";
										cmd2.Parameters.AddWithValue("@record_id", recordID);
										cmd2.Prepare();

										using (MySqlDataReader db_reader2 = cmd2.ExecuteReader())
										{
											while (db_reader2.Read())
											{
												RemarksContainer.Text = db_reader2.GetString("remarks");
											}
										}
									}
								}
							}
						}
					}
				}
			}
			//if (dbman.DBConnect().State == ConnectionState.Open)
			//{
			//	MySqlCommand cmd = dbman.DBConnect().CreateCommand();
			//	cmd.CommandText = "SELECT * FROM baptismal_records, matrimonial_records, confirmation_records, burial_records WHERE burial_records.record_id = @record_id OR baptismal_records.record_id = @record_id OR matrimonial_records.record_id = @record_id OR confirmation_records.record_id = @record_id LIMIT 1;";
			//	cmd.Parameters.AddWithValue("@record_id", recordID);
			//	cmd.Prepare();
			//	MySqlDataReader db_reader = cmd.ExecuteReader();
			//	while (db_reader.Read())
			//	{
			//		RemarksContainer.Text = db_reader.GetString("remarks");
			//	}
			//	//close Connection
			//	dbman.DBClose();
			//}
			//else
			//{

			//}
		}

		private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
