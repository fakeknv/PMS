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
	public partial class AddBaptismalRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int pageNum;
		private int bookNum;
		private int entryNum;
		private string baptismDate;
		private string birthDate;
		private string legitimacy;
		private string fullName;
		private string birthPlace;
		private string parent1;
		private string parent2;
		private string sponsor1;
		private string sponsor2;
		private int stipend;
		private string minister;
		private string remarks;

		private ViewRecordEntries vre;

		public AddBaptismalRecordEntryWindow(ViewRecordEntries viewRE, int targBook)
		{
			vre = viewRE;
			pmsutil = new PMSUtil();
			InitializeComponent();
			bookNum = targBook;
			Stipend.Value = FetchBaptismalStipend();
			FetchBookEntryNum();
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
				cmd.Parameters.AddWithValue("@record_date", baptismDate);
				cmd.Parameters.AddWithValue("@recordholder_fullname", fullName);
				cmd.Parameters.AddWithValue("@parent1_fullname", parent1);
				cmd.Parameters.AddWithValue("@parent2_fullname", parent2);
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				//Phase 2
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO baptismal_records(record_id, birthday, legitimacy, place_of_birth, sponsor1, sponsor2, stipend, minister, remarks)" +
					"VALUES(@record_id, @birthday, @legitimacy, @place_of_birth, @sponsor1, @sponsor2, @stipend, @minister, @remarks)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@record_id", recID);
				cmd.Parameters.AddWithValue("@birthday", birthDate);
				cmd.Parameters.AddWithValue("@legitimacy", legitimacy);
				cmd.Parameters.AddWithValue("@place_of_birth", birthPlace);
				cmd.Parameters.AddWithValue("@sponsor1", sponsor1);
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
		private void FetchBookEntryNum() {
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
			EntryNum.Value = ret+1;
		}
		/// <summary>
		/// Fetches default confirmation stipend value.
		/// </summary>
		private int FetchBaptismalStipend() {
			int ret = 0;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Baptismal Stipend';";
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
		private bool CheckInputs()
		{
			var bc = new BrushConverter();

			BaptismDateValidator.Visibility = Visibility.Hidden;
			BaptismDateValidator.Foreground = Brushes.Transparent;
			BaptismDate.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			BirthDateValidator.Visibility = Visibility.Hidden;
			BirthDateValidator.Foreground = Brushes.Transparent;
			Birthdate.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			NameValidator.Visibility = Visibility.Hidden;
			NameValidator.Foreground = Brushes.Transparent;
			FullName.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			PlaceOfBirthValidator.Visibility = Visibility.Hidden;
			PlaceOfBirthValidator.Foreground = Brushes.Transparent;
			PlaceOfBirth.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Sponsor1Validator.Visibility = Visibility.Hidden;
			Sponsor1Validator.Foreground = Brushes.Transparent;
			Sponsor1.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Sponsor2Validator.Visibility = Visibility.Hidden;
			Sponsor2Validator.Foreground = Brushes.Transparent;
			Sponsor2.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Parent1Validator.Visibility = Visibility.Hidden;
			Parent1Validator.Foreground = Brushes.Transparent;
			Parent1.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			MinisterValidator.Visibility = Visibility.Hidden;
			MinisterValidator.Foreground = Brushes.Transparent;
			Minister.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			LegitimacyValidator.Visibility = Visibility.Hidden;
			LegitimacyValidator.Foreground = Brushes.Transparent;
			Legitimacy.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			bool ret = true;

			if (string.IsNullOrWhiteSpace(BaptismDate.Text))
			{
				BaptismDateValidator.Visibility = Visibility.Visible;
				BaptismDateValidator.ToolTip = "This field is required.";
				BaptismDateValidator.Foreground = Brushes.Red;
				BaptismDate.BorderBrush = Brushes.Red;

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
			if (string.IsNullOrWhiteSpace(Birthdate.Text))
			{
				BirthDateValidator.Visibility = Visibility.Visible;
				BirthDateValidator.ToolTip = "This field is required.";
				BirthDateValidator.Foreground = Brushes.Red;
				Birthdate.BorderBrush = Brushes.Red;

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
			if (string.IsNullOrWhiteSpace(PlaceOfBirth.Text))
			{
				PlaceOfBirthValidator.Visibility = Visibility.Visible;
				PlaceOfBirthValidator.ToolTip = "This field is required. Type unknown if its not applicable.";
				PlaceOfBirthValidator.Foreground = Brushes.Red;
				PlaceOfBirth.BorderBrush = Brushes.Red;

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
			if (string.IsNullOrWhiteSpace(Sponsor2.Text))
			{
				Sponsor2Validator.Visibility = Visibility.Visible;
				Sponsor2Validator.ToolTip = "This field is required.";
				Sponsor2Validator.Foreground = Brushes.Red;
				Sponsor2.BorderBrush = Brushes.Red;

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
			if (string.IsNullOrWhiteSpace(Legitimacy.Text))
			{
				LegitimacyValidator.Visibility = Visibility.Visible;
				LegitimacyValidator.ToolTip = "This field is required.";
				LegitimacyValidator.Foreground = Brushes.Red;
				Legitimacy.BorderBrush = Brushes.Red;

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
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void AddRecConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				legitimacy = Legitimacy.Text;
				entryNum = Convert.ToInt32(EntryNum.Value);
				pageNum = Convert.ToInt32(PageNum.Value);
				baptismDate = Convert.ToDateTime(BaptismDate.Text).ToString("yyyy-MM-dd");
				birthDate = Convert.ToDateTime(Birthdate.Text).ToString("yyyy-MM-dd");
				fullName = ValidateInp(FullName.Text);
				birthPlace = PlaceOfBirth.Text;
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

			PlaceOfBirthSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_birth FROM baptismal_records WHERE " +
					"place_of_birth LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + PlaceOfBirth.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					PlaceOfBirthSuggestionArea.Items.Add(db_reader.GetString("place_of_birth"));
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

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM baptismal_records WHERE " +
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

				Suggestions2.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			PlaceOfBirthSuggestionArea.SelectedItem = item;
			PlaceOfBirth.Text = PlaceOfBirthSuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion2_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions2.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
		}
	}
}
