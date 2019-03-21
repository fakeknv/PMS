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
	public partial class EditRegisterWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int _bookNum;

		private string regType;
		private int bookNum;
		private int registerNum;
		private string book;
		private string creationDate;

		Registers reg1;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public EditRegisterWindow(Registers reg, int bookNumx)
		{
			_bookNum = bookNumx;
			reg1 = reg;
			pmsutil = new PMSUtil();
			InitializeComponent();
			BookNo.Value = bookNumx;

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
						cmd.CommandText = "SELECT * FROM registers WHERE book_number = @book_num LIMIT 1;";
						cmd.Parameters.AddWithValue("@book_num", bookNumx);
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (db_reader.GetString("book_type") == "Baptismal") {
									RegisterType.SelectedIndex = 0;
								}
								else if (db_reader.GetString("book_type") == "Confirmation")
								{
									RegisterType.SelectedIndex = 1;
								}
								else if (db_reader.GetString("book_type") == "Matrimonial")
								{
									RegisterType.SelectedIndex = 2;
								}
								else if (db_reader.GetString("book_type") == "Burial")
								{
									RegisterType.SelectedIndex = 3;
								}
								RegisterNo.Value = db_reader.GetInt32("register_number");
								Book.Text = db_reader.GetString("book");
								CreationDate.Text = db_reader.GetString("creation_date");
							}
						}
					}
				}
			}
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int UpdateRegister()
		{
			int ret = 0;
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText =
					"UPDATE registers SET book_number = @book_num, register_number = @register_number, book = @book, creation_date = @creation_date WHERE book_number = @book_number;";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@book_num", bookNum);
					cmd.Parameters.AddWithValue("@book_number", _bookNum);
					cmd.Parameters.AddWithValue("@register_number", registerNum);
					cmd.Parameters.AddWithValue("@book", book);
					cmd.Parameters.AddWithValue("@creation_date", creationDate);
					int stat_code = cmd.ExecuteNonQuery();
					conn.Close();
					ret = stat_code;
				}
				else
				{

				}
			}
			return ret;
		}
		private bool CheckInputs()
		{
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
			return ret;
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void AddRegConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				regType = RegisterType.Text;
				bookNum = Convert.ToInt32(BookNo.Value);
				registerNum = Convert.ToInt32(RegisterNo.Value);
				book = Book.Text;
				creationDate = Convert.ToDateTime(CreationDate.Text).ToString("yyyy-MM-dd");

				if (UpdateRegister() > 0)
				{
					pmsutil.LogAccount("Edited Register No " + RegisterNo.Value + " Type: " + regType);
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
			await metroWindow.ShowMessageAsync("Success!", "The register has been updated successfully.");
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
