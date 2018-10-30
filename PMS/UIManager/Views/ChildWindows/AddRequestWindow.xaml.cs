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
		#pragma warning disable 0649
		//MYSQL Related Stuff
		private DBConnectionManager dbman;
		#pragma warning restore 0649

		private PMSUtil pmsutil;

		private string reqType;
		private string name;
		private string bday;
		private string recdate;
		private string parent1;
		private string parent2;
		private string purpose;
		private string status;
		#pragma warning disable 0649
		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;
		private string comDate;
		private string comTime;
		private string placedBy;
		private string completedBy;
		#pragma warning restore 0649
		Requests req1;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddRequestWindow(Requests req)
		{
			req1 = req;
			pmsutil = new PMSUtil();
			InitializeComponent();
		}
		private string GenerateReqID()
		{
			return "REQ-" + DateTime.Now.ToString("yyyymmssf");
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int InsertRequest()
		{
			//TODO
			try
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO requests(request_id, type, status, req_record_name, req_birthday, req_record_date, req_parent1, req_parent2, purpose, req_date, req_time, completion_date, completion_time, placed_by, completed_by)" + 
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
		/// <summary>
		/// Interaction logic for the AddReqConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void AddReqConfirm(object sender, System.Windows.RoutedEventArgs e)
		{

			switch (RequestType.SelectedIndex)
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
			
			string[] dt = pmsutil.GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			DateTime.Parse(dt[1] + " " + dt[2]);
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
		/// Interaction logic for the AddReqCancel button. Closes the AddRequestForm Window.
		/// </summary>
		private void AddReqCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
