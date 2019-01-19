using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using PMS.UIManager.Views.ChildViews;
using MahApps.Metro.Controls.Dialogs;
using System.Windows.Media;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class AddConfirmationRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int pageNum;
		private int bookNum;
		private int entryNum;
		private string confirmationDate;
		private int age;
		private string fullName;
		private string parish;
		private string province;
		private string baptismPlace;
		private string parent1;
		private string parent2;
		private string sponsor1;
		private string sponsor2;
		private int stipend;
		private string minister;
		private string remarks;

		private ViewRecordEntries vre;

		public AddConfirmationRecordEntryWindow(ViewRecordEntries viewRE, int targBook)
		{
			vre = viewRE;
			pmsutil = new PMSUtil();
			InitializeComponent();
			bookNum = targBook;
			Stipend.Value = FetchConfirmationStipend();
			FetchBookEntryNum();
		}
		private void FetchBookEntryNum()
		{
			int ret = 0;
			PageNum.Value = vre.Page.Value;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT entry_number FROM records WHERE book_number = @bnum AND page_number = @pnum;";
				cmd.Parameters.AddWithValue("@bnum", bookNum);
				cmd.Parameters.AddWithValue("@pnum", vre.Page.Value);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = Convert.ToInt32(db_reader.GetString("entry_number"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = 0;
			}
			EntryNum.Value = ret + 1;
		}
		private bool CheckInputs()
		{
			ConfirmationDateValidator.Visibility = Visibility.Hidden;
			ConfirmationDateValidator.Foreground = Brushes.Transparent;
			ConfirmationDate.BorderBrush = Brushes.Transparent;

			NameValidator.Visibility = Visibility.Hidden;
			NameValidator.Foreground = Brushes.Transparent;
			FullName.BorderBrush = Brushes.Transparent;

			PlaceOfBaptismValidator.Visibility = Visibility.Hidden;
			PlaceOfBaptismValidator.Foreground = Brushes.Transparent;
			PlaceOfBaptism.BorderBrush = Brushes.Transparent;

			Sponsor1Validator.Visibility = Visibility.Hidden;
			Sponsor1Validator.Foreground = Brushes.Transparent;
			Sponsor1.BorderBrush = Brushes.Transparent;

			Parent1Validator.Visibility = Visibility.Hidden;
			Parent1Validator.Foreground = Brushes.Transparent;
			Parent1.BorderBrush = Brushes.Transparent;

			MinisterValidator.Visibility = Visibility.Hidden;
			MinisterValidator.Foreground = Brushes.Transparent;
			Minister.BorderBrush = Brushes.Transparent;

			Sponsor1Validator.Visibility = Visibility.Hidden;
			Sponsor1Validator.Foreground = Brushes.Transparent;
			Sponsor1.BorderBrush = Brushes.Transparent;

			ParishValidator.Visibility = Visibility.Hidden;
			ParishValidator.Foreground = Brushes.Transparent;
			Parish.BorderBrush = Brushes.Transparent;

			ProvinceValidator.Visibility = Visibility.Hidden;
			ProvinceValidator.Foreground = Brushes.Transparent;
			Province.BorderBrush = Brushes.Transparent;

			bool ret = true;

			if (string.IsNullOrWhiteSpace(ConfirmationDate.Text))
			{
				ConfirmationDateValidator.Visibility = Visibility.Visible;
				ConfirmationDateValidator.ToolTip = "This field is required.";
				ConfirmationDateValidator.Foreground = Brushes.Red;
				ConfirmationDate.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (EntryNum.Value < 0)
			{
				EntryNumValidator.Visibility = Visibility.Visible;
				EntryNumValidator.ToolTip = "Must be greater than zero.";
				EntryNumValidator.Foreground = Brushes.Red;
				EntryNum.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (PageNum.Value < 0)
			{
				EntryNumValidator.Visibility = Visibility.Visible;
				EntryNumValidator.ToolTip = "Must be greater than zero.";
				EntryNumValidator.Foreground = Brushes.Red;
				PageNum.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(FullName.Text))
			{
				NameValidator.Visibility = Visibility.Visible;
				NameValidator.ToolTip = "This field is required.";
				NameValidator.Foreground = Brushes.Red;
				FullName.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (Stipend.Value == 0)
			{
				StipendValidator.Visibility = Visibility.Visible;
				StipendValidator.ToolTip = "Notice: Stipend is set to zero.";
				StipendValidator.Foreground = Brushes.Orange;
				Stipend.BorderBrush = Brushes.Orange;
				MsgStipend();
				ret = true;
			}
			if (string.IsNullOrWhiteSpace(PlaceOfBaptism.Text))
			{
				PlaceOfBaptismValidator.Visibility = Visibility.Visible;
				PlaceOfBaptismValidator.ToolTip = "This field is required.";
				PlaceOfBaptismValidator.Foreground = Brushes.Red;
				PlaceOfBaptism.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sponsor1.Text))
			{
				Sponsor1Validator.Visibility = Visibility.Visible;
				Sponsor1Validator.ToolTip = "This field is required.";
				Sponsor1Validator.Foreground = Brushes.Red;
				Sponsor1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent1.Text))
			{
				Parent1Validator.Visibility = Visibility.Visible;
				Parent1Validator.ToolTip = "This field is required.";
				Parent1Validator.Foreground = Brushes.Red;
				Parent1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Minister.Text))
			{
				MinisterValidator.Visibility = Visibility.Visible;
				MinisterValidator.ToolTip = "This field is required.";
				MinisterValidator.Foreground = Brushes.Red;
				Minister.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parish.Text))
			{
				ParishValidator.Visibility = Visibility.Visible;
				ParishValidator.ToolTip = "This field is required.";
				ParishValidator.Foreground = Brushes.Red;
				Parish.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Province.Text))
			{
				ProvinceValidator.Visibility = Visibility.Visible;
				ProvinceValidator.ToolTip = "This field is required.";
				ProvinceValidator.Foreground = Brushes.Red;
				Province.BorderBrush = Brushes.Red;

				ret = false;
			}
			return ret;
		}
		private async void MsgStipend()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Notice", "Stipend is set to zero. Re-check input before proceeding.");
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int InsertEntry()
		{
			dbman = new DBConnectionManager();
			//TODO
			try
			{
				string recID = pmsutil.GenRecordID();
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO records(record_id, book_number, page_number, entry_number, record_date, recordholder_fullname, parent1_fullname, parent2_fullname)" +
					"VALUES(@record_id, @book_number, @page_number, @entry_number, @record_date, @recordholder_fullname, @parent1_fullname, @parent2_fullname)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@record_id", recID);
				cmd.Parameters.AddWithValue("@book_number", bookNum);
				cmd.Parameters.AddWithValue("@page_number", pageNum);
				cmd.Parameters.AddWithValue("@entry_number", entryNum);
				cmd.Parameters.AddWithValue("@record_date", confirmationDate);
				cmd.Parameters.AddWithValue("@recordholder_fullname", fullName);
				cmd.Parameters.AddWithValue("@parent1_fullname", parent1);
				cmd.Parameters.AddWithValue("@parent2_fullname", parent2);
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				//Phase 2
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO confirmation_records(record_id, age, parochia, province, place_of_baptism, sponsor, sponsor2, stipend, minister, remarks)" +
					"VALUES(@record_id, @age, @parish, @province, @place_of_baptism, @sponsor, @sponsor2, @stipend, @minister, @remarks)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@record_id", recID);
				cmd.Parameters.AddWithValue("@age", age);
				cmd.Parameters.AddWithValue("@parish", parish);
				cmd.Parameters.AddWithValue("@province", province);
				cmd.Parameters.AddWithValue("@place_of_baptism", baptismPlace);
				cmd.Parameters.AddWithValue("@sponsor", sponsor1);
				cmd.Parameters.AddWithValue("@sponsor2", sponsor2);
				cmd.Parameters.AddWithValue("@stipend", stipend);
				cmd.Parameters.AddWithValue("@minister", minister);
				cmd.Parameters.AddWithValue("@remarks", remarks);
				stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				string tmp = pmsutil.LogRecord(recID, "LOGC-01");
				return stat_code;
			}
			catch (MySqlException ex)
			{
				Console.WriteLine("Error: {0}", ex.ToString());
				return 0;
			}
		}
		/// <summary>
		/// Fetches default confirmation stipend value.
		/// </summary>
		private int FetchConfirmationStipend() {
			int ret = 0;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Confirmation Stipend';";
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = Convert.ToInt32(db_reader.GetString("key_value"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = 0;
			}
			return ret;
		}
		/// <summary>
		/// Validates input.
		/// </summary>
		private string ValidateInp(string targ) {
			if (String.IsNullOrEmpty(targ))
			{
				return "Unknown";
			}
			else
			{
				return targ;
			}
		}
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void AddRecConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				entryNum = Convert.ToInt32(EntryNum.Value);
				pageNum = Convert.ToInt32(PageNum.Value);
				confirmationDate = Convert.ToDateTime(ConfirmationDate.Text).ToString("yyyy-MM-dd");
				age = Convert.ToInt32(Age.Value);
				fullName = ValidateInp(FullName.Text);
				parish = ValidateInp(Parish.Text);
				province = ValidateInp(Province.Text);
				baptismPlace = PlaceOfBaptism.Text;
				parent1 = ValidateInp(Parent1.Text);
				parent2 = ValidateInp(Parent2.Text);
				sponsor1 = ValidateInp(Sponsor1.Text);
				sponsor2 = ValidateInp(Sponsor2.Text);
				stipend = Convert.ToInt32(Stipend.Value);
				minister = ValidateInp(Minister.Text);
				remarks = ValidateInp(Remarks.Text);
				if (InsertEntry() > 0)
				{
					MsgSuccess();
					vre.Sync(bookNum);
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
			await metroWindow.ShowMessageAsync("Success!", "The record has been added to the register successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void AddRecCancel(object sender, System.Windows.RoutedEventArgs e)
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
		private void ShowSuggestions2(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			ParishSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT parochia FROM confirmation_records WHERE " +
					"parochia LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Parish.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ParishSuggestionArea.Items.Add(db_reader.GetString("parochia"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions2.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions3(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			ProvinceSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT province FROM confirmation_records WHERE " +
					"province LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Province.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ProvinceSuggestionArea.Items.Add(db_reader.GetString("province"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions3.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions4(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM confirmation_records WHERE " +
					"minister LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Minister.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					MinisterSuggestionArea.Items.Add(db_reader.GetString("minister"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions4.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			PlaceOfBaptismSuggestionArea.SelectedItem = item;
			PlaceOfBaptism.Text = PlaceOfBaptismSuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion2_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			ParishSuggestionArea.SelectedItem = item;
			Parish.Text = ParishSuggestionArea.SelectedItem.ToString();
			Suggestions2.Visibility = Visibility.Hidden;
		}
		private void Suggestion3_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			ProvinceSuggestionArea.SelectedItem = item;
			Province.Text = ProvinceSuggestionArea.SelectedItem.ToString();
			Suggestions3.Visibility = Visibility.Hidden;
		}
		private void Suggestion4_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions4.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
		}
	}
}
