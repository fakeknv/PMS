using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media;

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
			RegTypeValidator.Visibility = Visibility.Hidden;
			RegTypeValidator.Foreground = Brushes.Transparent;
			RegisterType.BorderBrush = Brushes.Transparent;

			BookNumValidator.Visibility = Visibility.Hidden;
			BookNumValidator.Foreground = Brushes.Transparent;
			BookNo.BorderBrush = Brushes.Transparent;

			RegNumValidator.Visibility = Visibility.Hidden;
			RegNumValidator.Foreground = Brushes.Transparent;
			RegisterNo.BorderBrush = Brushes.Transparent;

			BookNameValidator.Visibility = Visibility.Hidden;
			BookNameValidator.Foreground = Brushes.Transparent;
			Book.BorderBrush = Brushes.Transparent;

			CreationDateValidator.Visibility = Visibility.Hidden;
			CreationDateValidator.Foreground = Brushes.Transparent;
			CreationDate.BorderBrush = Brushes.Transparent;

			bool ret = true;

			if (string.IsNullOrWhiteSpace(RegisterType.Text))
			{
				RegTypeValidator.Visibility = Visibility.Visible;
				RegTypeValidator.ToolTip = "This field is requried.";
				RegTypeValidator.Foreground = Brushes.Red;
				RegisterType.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(BookNo.Text))
			{
				BookNumValidator.Visibility = Visibility.Visible;
				BookNumValidator.ToolTip = "This field is requried.";
				BookNumValidator.Foreground = Brushes.Red;
				BookNo.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(RegisterNo.Value.ToString()))
			{
				RegNumValidator.Visibility = Visibility.Visible;
				RegNumValidator.ToolTip = "This field is requried.";
				RegNumValidator.Foreground = Brushes.Red;
				RegisterNo.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Book.Text))
			{
				BookNameValidator.Visibility = Visibility.Visible;
				BookNameValidator.ToolTip = "This field is requried.";
				BookNameValidator.Foreground = Brushes.Red;
				Book.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(CreationDate.Text))
			{
				CreationDateValidator.Visibility = Visibility.Visible;
				CreationDateValidator.ToolTip = "This field is requried.";
				CreationDateValidator.Foreground = Brushes.Red;
				CreationDate.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
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
				bookNum = Convert.ToInt32(BookNo.Text);
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
