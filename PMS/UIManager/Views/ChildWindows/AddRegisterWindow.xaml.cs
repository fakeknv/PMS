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
	public partial class AddRegisterWindow : ChildWindow
	{
		DBConnectionManager dbman;

		private string regType;
		private int bookNum;
		private int registerNum;
		private string book;
		private string creationDate;
		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;

		Registers reg1;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddRegisterWindow(Registers reg)
		{
			reg1 = reg;
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
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private int InsertRegister()
		{
			dbman = new DBConnectionManager();
			//TODO
			try
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO registers(book_number, register_number, book, book_type, creation_date, addition_date, addition_time)" +
					"VALUES(@book_number, @register_number, @book, @book_type, @creation_date, @additionDate, @addition_time)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				cmd.Parameters.AddWithValue("@register_number", registerNum);
				cmd.Parameters.AddWithValue("@book", book);
				cmd.Parameters.AddWithValue("@book_type", regType);
				cmd.Parameters.AddWithValue("@creation_date", creationDate);
				cmd.Parameters.AddWithValue("@additionDate", curDate);
				cmd.Parameters.AddWithValue("@addition_time", curTime);
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
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void AddRegCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void AddRegConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			switch (RegisterType.SelectedIndex)
			{
				case 0:
					regType = "Baptismal";
					break;
				case 1:
					regType = "Marriage";
					break;
				case 2:
					regType = "Confirmation";
					break;
				case 3:
					regType = "Death";
					break;
				default:
					regType = "NULL";
					break;
			}
			bookNum = Convert.ToInt32(BookNo.Text);
			registerNum = Convert.ToInt32(RegisterNo.Text);
			book = Book.Text;
			creationDate = Convert.ToDateTime(CreationDate.Text).ToString("yyyy-MM-dd");

			string[] dt = GetServerDateTime().Split(null);
			cDate = Convert.ToDateTime(dt[0]);
			cTime = Convert.ToDateTime(dt[1]);
			curDate = cDate.ToString("yyyy-MM-dd");
			curTime = cTime.ToString("HH:mm:ss");

			if (InsertRegister() > 0)
			{
				reg1.SyncRegisters();
				this.Close();
			}
		}
	}
}
