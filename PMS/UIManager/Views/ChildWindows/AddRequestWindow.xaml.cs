using MahApps.Metro.SimpleChildWindow;
using System;
using System.Windows;
using MySql.Data.MySqlClient;
using System.Data;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class AddRequestWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private DBConnectionManager dbman;

		private MySqlConnection connection;
		private string server;
		private string database;
		private string uid;
		private string password;
		private string ssl_mode;

		private string reqType;
		private string name;
		private string bday;
		private string recdate;
		private string parent1;
		private string parent2;
		private string purpose;
		private string status;
		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;
		private string comDate;
		private string comTime;
		private string placedBy;
		private string compeltedBy;

		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddRequestWindow()
		{
			InitializeComponent();
			dbman = new DBConnectionManager();

			/*server = "localhost";
			database = "prms_db";
			uid = "prms";
			password = "prms2018!";
			ssl_mode = "none";

			string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
			database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";" + "SSLMODE=" + ssl_mode + ";";

			connection = new MySqlConnection(connectionString);*/
		}
		/// <summary>
		/// Connects to the DB Server.
		/// </summary>
		private bool DBConnect()
		{
			try
			{
				connection.Open();
				return true;
			}
			catch (MySqlException ex)
			{
				switch (ex.Number)
				{
					case 0:
						MessageBox.Show(ex.ToString());
						break;

					case 1045:
						MessageBox.Show("Invalid username/password, please try again");
						break;
				}
				return false;
			}
		}
		/// <summary>
		/// Terminates connection to the DB Server.
		/// </summary>
		private bool DBClose()
		{
			try
			{
				connection.Close();
				return true;
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(ex.Message);
				return false;
			}
		}

		/// <summary>
		/// Retrieves Current Date and Time from the Server.
		/// </summary>
		private string GetServerDateTime()
		{
			
			string curDateTime = "";

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT NOW() FROM DUAL;";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					curDateTime = db_reader.GetString("NOW()");
				}
				//close Connection
				dbman.DBClose();

				return curDateTime;
			}
			else
			{
				//close Connection
				dbman.DBClose();

				return null;
			}
		}
		private void InsertRequest()
		{
			//TODO
		}
		private void AddReqConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			Login li = new Login();

			
			name = ReqName.Text;
			bday = ReqBday.Text;
			recdate = ReqRecDate.Text;
			parent1 = Parent1.Text;
			parent2 = Parent2.Text;
			purpose = Purpose.Text;
			status = "Initializing";
			switch (CMBRequestType.SelectedIndex)
			{
				case 0:
					reqType = "Baptismal";
					break;
				case 1:
					reqType = "Marriage";
					break;
				case 2:
					reqType = "Confirmation";
					break;
				case 3:
					reqType = "Death";
					break;
				default:
					reqType = "NULL";
					break;
			}
			string[] dt = GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			cTime = Convert.ToDateTime(dt[1]);
			curDate = cDate.ToString("yyyy-MM-dd");
			curTime = cTime.ToString("HH:mm:ss");
			comDate = null;
			comTime = null;
			placedBy = "";
			compeltedBy = null;

			MessageBox.Show(curDate);
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void AddReqCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
