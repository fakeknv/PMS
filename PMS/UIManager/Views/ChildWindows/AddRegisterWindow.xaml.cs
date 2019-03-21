using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media;
using System.Data;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class AddRegisterWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

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
		private MySqlConnection conn;

		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddRegisterWindow(Registers reg)
		{
			reg1 = reg;
			pmsutil = new PMSUtil();
			InitializeComponent();
		}
		/// <summary>
		/// Inserts the request to the database.
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
		private bool CheckInputs()
		{
			var bc = new BrushConverter();
			
			bool ret = true;

			if (string.IsNullOrWhiteSpace(RegisterType.Text))
			{
				RegisterType.ToolTip = "This field is required.";
				RegisterType.BorderBrush = Brushes.Red;
				RegisterTypeIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (BookNo.Value < 0 || string.IsNullOrWhiteSpace(BookNo.Value.ToString()))
			{
				BookNo.ToolTip = "This field is required.";
				BookNo.BorderBrush = Brushes.Red;
				BookNoIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(RegisterNo.Value.ToString()))
			{
				RegisterNo.ToolTip = "This field is required.";
				RegisterNo.BorderBrush = Brushes.Red;
				RegisterNoIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Book.Text))
			{
				Book.ToolTip = "This field is required.";
				Book.BorderBrush = Brushes.Red;
				BookIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(CreationDate.Text))
			{
				CreationDate.ToolTip = "This field is required.";
				CreationDate.BorderBrush = Brushes.Red;
				CreationDateIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (CheckDupli() == true)
			{
				BookNo.ToolTip = "This field is required.";
				BookNo.BorderBrush = Brushes.Red;
				BookNoIcon.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
		}
		internal bool CheckDupli()
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM registers WHERE book_number = @bnum";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@bnum", BookNo.Value);
					using (MySqlDataReader db_reader = cmd.ExecuteReader())
					{
						while (db_reader.Read())
						{
							if (db_reader.GetInt32("COUNT(*)") > 0)
							{
								return true;
							}
						}
					}
				}
			}
			return false;
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void AddRegConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				switch (RegisterType.SelectedIndex)
				{
					case 0:
						regType = "Baptismal";
						break;
					case 1:
						regType = "Matrimonial";
						break;
					case 2:
						regType = "Confirmation";
						break;
					case 3:
						regType = "Burial";
						break;
					default:
						regType = "NULL";
						break;
				}
				bookNum = Convert.ToInt32(BookNo.Value);
				registerNum = Convert.ToInt32(RegisterNo.Value);
				book = Book.Text;
				creationDate = Convert.ToDateTime(CreationDate.Text).ToString("yyyy-MM-dd");

				string[] dt = pmsutil.GetServerDateTime().Split(null);
				cDate = Convert.ToDateTime(dt[0]);
				cTime = DateTime.Parse(dt[1] + " " + dt[2]);
				curDate = cDate.ToString("yyyy-MM-dd");
				curTime = cTime.ToString("HH:mm:ss");

				if (InsertRegister() > 0)
				{
					reg1.SyncRegisters();
					var metroWindow = (Application.Current.MainWindow as MetroWindow);
					pmsutil.LogAccount("Added Register Number: " + RegisterNo.Value + " Type: " + regType);
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
			await metroWindow.ShowMessageAsync("Success!", "The register has been added successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void AddRegCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
