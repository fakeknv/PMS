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
		private string completedBy;

		Requests req1;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddRequestWindow(Requests req)
		{
			req1 = req;
			InitializeComponent();
		}
		/// <summary>
		/// Retrieves Current Date and Time from the Server.
		/// </summary>
		private string GetServerDateTime()
		{
			dbman = new DBConnectionManager();

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
		private string GenerateReqID()
		{
			return "REQ-" + DateTime.Now.ToString("yyyymmssf");
		}
		private int InsertRequest()
		{
			//TODO
			try
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO request(request_id, type, status, req_record_name, req_birthday, req_record_date, req_parent1, req_parent2, purpose, req_date, req_time, completion_date, completion_time, placed_by, completed_by)" + 
					"VALUES(@request_id, @type, @status, @req_record_name, @req_birthday, @req_record_date, @req_parent1, @req_parent2, @purpose, @req_date, @req_time, @completion_date, @completion_time, @placed_by, @completed_by)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@request_id", GenerateReqID());
				cmd.Parameters.AddWithValue("@type", reqType);
				cmd.Parameters.AddWithValue("@status", status);
				cmd.Parameters.AddWithValue("@req_record_name", name);
				cmd.Parameters.AddWithValue("@req_birthday", bday);
				cmd.Parameters.AddWithValue("@req_record_date", recdate);
				cmd.Parameters.AddWithValue("@req_parent1", parent1);
				cmd.Parameters.AddWithValue("@req_parent2", parent2);
				cmd.Parameters.AddWithValue("@purpose", purpose);
				cmd.Parameters.AddWithValue("@req_date", curDate);
				cmd.Parameters.AddWithValue("@req_time", curTime);
				cmd.Parameters.AddWithValue("@completion_date", comDate);
				cmd.Parameters.AddWithValue("@completion_time", comTime);
				cmd.Parameters.AddWithValue("@placed_by", placedBy);
				cmd.Parameters.AddWithValue("@completed_by", completedBy);
				int stat_code = cmd.ExecuteNonQuery();
				return stat_code;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}", ex.ToString());
				return 0;
			}
		}
		private void AddReqConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			Login li = new Login();

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
			name = ReqName.Text;
			bday = Convert.ToDateTime(ReqBday.Text).ToString("yyyy-MM-dd");
			recdate = Convert.ToDateTime(ReqRecDate.Text).ToString("yyyy-MM-dd");
			parent1 = Parent1.Text;
			parent2 = Parent2.Text;
			purpose = Purpose.Text;
			status = "Initializing";
			
			string[] dt = GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			cTime = Convert.ToDateTime(dt[1]);
			curDate = cDate.ToString("yyyy-MM-dd");
			curTime = cTime.ToString("HH:mm:ss");
			placedBy = Application.Current.Resources["uid"].ToString();
			if (InsertRequest() > 0)
			{
				req1.SyncRequests();
				this.Close();
			}
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
