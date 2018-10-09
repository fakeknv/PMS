using MahApps.Metro.SimpleChildWindow;
using System;
using System.Windows;
using MySql.Data.MySqlClient;


namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class AddRequestWindow : ChildWindow
	{
		private MySqlConnection connection;
		private string server;
		private string database;
		private string uid;
		private string password;
		private string ssl_mode;

		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddRequestWindow()
		{
			InitializeComponent();
			server = "localhost";
			database = "prms_db";
			uid = "prms";
			password = "prms2018!";
			ssl_mode = "none";

			string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
			database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";" + "SSLMODE=" + ssl_mode + ";";

			connection = new MySqlConnection(connectionString);
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
			string query = "SELECT NOW() FROM DUAL;";

			if (this.DBConnect() == true)
			{
				//Create Command
				MySqlCommand cmd = new MySqlCommand(query, connection);

				curDateTime = cmd.ExecuteScalar().ToString();

				//close Connection
				this.DBClose();

				return curDateTime;
			}
			else
			{
				//close Connection
				this.DBClose();

				return null;
			}
		}
		private void InsertRequest()
		{
			//TODO
		}
		private void AddReqConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			string reqType;
			string name = ReqName.Text;
			string bday = ReqBday.Text;
			string recdate = ReqRecDate.Text;
			string parent1 = Parent1.Text;
			string parent2 = Parent2.Text;
			string purpose = Purpose.Text;
			string status = "Initializing";
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
			DateTime cDate = Convert.ToDateTime(dt[0]);
			DateTime cTime = Convert.ToDateTime(dt[1]);
			string curDate = cDate.ToString("yyyy-MM-dd");
			string curTime = cTime.ToString("HH:mm:ss");
			MessageBox.Show(curTime);
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
