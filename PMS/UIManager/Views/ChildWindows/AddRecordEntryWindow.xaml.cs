using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class AddRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		//private string regType;
		//private int bookNum;
		//private int registerNum;
		//private string book;
		//private string creationDate;
		//private DateTime cDate;
		//private DateTime cTime;
		//private string curDate;
		//private string curTime;

		
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddRecordEntryWindow(int targBook)
		{
			
			pmsutil = new PMSUtil();
			InitializeComponent();

			for (int i = 1; i < 11; i++)
			{
				object pageNumObj = new object();
				pageNumObj = i;
				EntryNumberComboBox.Items.Add(pageNumObj);
			}
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
				//cmd.Parameters.AddWithValue("@book_number", bookNum);
				//cmd.Parameters.AddWithValue("@register_number", registerNum);
				//cmd.Parameters.AddWithValue("@book", book);
				//cmd.Parameters.AddWithValue("@book_type", regType);
				//cmd.Parameters.AddWithValue("@creation_date", creationDate);
				//cmd.Parameters.AddWithValue("@additionDate", curDate);
				//cmd.Parameters.AddWithValue("@addition_time", curTime);
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
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void AddRegConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			//switch (RegisterType.SelectedIndex)
			//{
			//	case 0:
			//		regType = "Baptismal";
			//		break;
			//	case 1:
			//		regType = "Matrimonial";
			//		break;
			//	case 2:
			//		regType = "Confirmation";
			//		break;
			//	case 3:
			//		regType = "Burial";
			//		break;
			//	default:
			//		regType = "NULL";
			//		break;
			//}
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void AddRegCancel(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void ShowSuggestions1(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			PlaceOfBaptismSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_baptism FROM confirmation_records WHERE " +
					"place_of_baptism LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + PlaceOfBaptism.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					PlaceOfBaptismSuggestionArea.Items.Add(db_reader.GetString("place_of_baptism"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions1.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			PlaceOfBaptism.Text = PlaceOfBaptismSuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}

		private void Hide(object sender, RoutedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
		}
	}
}
