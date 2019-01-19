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
	public partial class AddMatrimonialRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int pageNum;
		private int bookNum;
		private int entryNum;
		private string marriageDate;
		private string fullName1;
		private string fullName2;
		private int age1;
		private int age2;
		private string status1;
		private string status2;
		private string hometown1;
		private string hometown2;
		private string residence1;
		private string residence2;
		private string residence3;
		private string residence4;
		private string parent1;
		private string parent2;
		private string parent3;
		private string parent4;
		private string sponsor1;
		private string sponsor2;
		private int stipend;
		private string minister;
		private string remarks;

		private ViewRecordEntries vre;

		public AddMatrimonialRecordEntryWindow(ViewRecordEntries viewRE, int targBook)
		{
			vre = viewRE;
			pmsutil = new PMSUtil();
			InitializeComponent();
			bookNum = targBook;
			Stipend.Value = FetchMatrimonialStipend();
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
			MarriageDateValidator.Visibility = Visibility.Hidden;
			MarriageDateValidator.Foreground = Brushes.Transparent;
			MarriageDate.BorderBrush = Brushes.Transparent;

			HusbandValidator.Visibility = Visibility.Hidden;
			HusbandValidator.Foreground = Brushes.Transparent;
			FullName1.BorderBrush = Brushes.Transparent;

			WifeValidator.Visibility = Visibility.Hidden;
			WifeValidator.Foreground = Brushes.Transparent;
			FullName2.BorderBrush = Brushes.Transparent;

			MinisterValidator.Visibility = Visibility.Hidden;
			MinisterValidator.Foreground = Brushes.Transparent;
			Minister.BorderBrush = Brushes.Transparent;

			bool ret = true;

			if (string.IsNullOrWhiteSpace(MarriageDate.Text))
			{
				MarriageDateValidator.Visibility = Visibility.Visible;
				MarriageDateValidator.ToolTip = "This field is required.";
				MarriageDateValidator.Foreground = Brushes.Red;
				MarriageDate.BorderBrush = Brushes.Red;

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
			if (Stipend.Value == 0)
			{
				StipendValidator.Visibility = Visibility.Visible;
				StipendValidator.ToolTip = "Notice: Stipend is set to zero.";
				StipendValidator.Foreground = Brushes.Orange;
				Stipend.BorderBrush = Brushes.Orange;
				MsgStipend();
				ret = true;
			}
			if (string.IsNullOrWhiteSpace(FullName1.Text))
			{
				HusbandValidator.Visibility = Visibility.Visible;
				HusbandValidator.ToolTip = "This field is required.";
				HusbandValidator.Foreground = Brushes.Red;
				FullName1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(FullName2.Text))
			{
				WifeValidator.Visibility = Visibility.Visible;
				WifeValidator.ToolTip = "This field is required.";
				WifeValidator.Foreground = Brushes.Red;
				FullName2.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent1.Text))
			{
				GroupValidator1.Visibility = Visibility.Visible;
				GroupValidator1.ToolTip = "This field is required.";
				GroupValidator1.Foreground = Brushes.Red;
				Parent1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent3.Text))
			{
				GroupValidator1.Visibility = Visibility.Visible;
				GroupValidator1.ToolTip = "This field is required.";
				GroupValidator1.Foreground = Brushes.Red;
				Parent3.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent2.Text))
			{
				GroupValidator2.Visibility = Visibility.Visible;
				GroupValidator2.ToolTip = "This field is required.";
				GroupValidator2.Foreground = Brushes.Red;
				Parent2.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent4.Text))
			{
				GroupValidator2.Visibility = Visibility.Visible;
				GroupValidator2.ToolTip = "This field is required.";
				GroupValidator2.Foreground = Brushes.Red;
				Parent4.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Hometown1.Text))
			{
				Hometown1Validator.Visibility = Visibility.Visible;
				Hometown1Validator.ToolTip = "This field is required.";
				Hometown1Validator.Foreground = Brushes.Red;
				Hometown1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Hometown2.Text))
			{
				Hometown2Validator.Visibility = Visibility.Visible;
				Hometown2Validator.ToolTip = "This field is required.";
				Hometown2Validator.Foreground = Brushes.Red;
				Hometown2.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence1.Text))
			{
				Residence1Validator.Visibility = Visibility.Visible;
				Residence1Validator.ToolTip = "This field is required.";
				Residence1Validator.Foreground = Brushes.Red;
				Residence1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence2.Text))
			{
				Residence2Validator.Visibility = Visibility.Visible;
				Residence2Validator.ToolTip = "This field is required.";
				Residence2Validator.Foreground = Brushes.Red;
				Residence2.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence3.Text))
			{
				GroupValidator1.Visibility = Visibility.Visible;
				GroupValidator1.ToolTip = "This field is required.";
				GroupValidator1.Foreground = Brushes.Red;
				Residence3.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence4.Text))
			{
				GroupValidator2.Visibility = Visibility.Visible;
				GroupValidator2.ToolTip = "This field is required.";
				GroupValidator2.Foreground = Brushes.Red;
				Residence4.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sponsor1.Text))
			{
				GroupValidator1.Visibility = Visibility.Visible;
				GroupValidator1.ToolTip = "This field is required.";
				GroupValidator1.Foreground = Brushes.Red;
				Sponsor1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sponsor2.Text))
			{
				GroupValidator2.Visibility = Visibility.Visible;
				GroupValidator2.ToolTip = "This field is required.";
				GroupValidator2.Foreground = Brushes.Red;
				Sponsor2.BorderBrush = Brushes.Red;

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
			if (string.IsNullOrWhiteSpace(Age1.Value.ToString()))
			{
				Age1Validator.Visibility = Visibility.Visible;
				Age1Validator.ToolTip = "This field is required.";
				Age1Validator.Foreground = Brushes.Red;
				Age1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Age2.Value.ToString()))
			{
				Age2Validator.Visibility = Visibility.Visible;
				Age2Validator.ToolTip = "This field is required.";
				Age2Validator.Foreground = Brushes.Red;
				Age2.BorderBrush = Brushes.Red;

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
				cmd.Parameters.AddWithValue("@record_date", marriageDate);
				cmd.Parameters.AddWithValue("@recordholder_fullname", fullName1);
				cmd.Parameters.AddWithValue("@parent1_fullname", parent1);
				cmd.Parameters.AddWithValue("@parent2_fullname", parent2);
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				//Phase 2
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO matrimonial_records(record_id, recordholder2_fullname, parent1_fullname2, parent2_fullname2, status1, status2, age1, age2, place_of_origin1, place_of_origin2, residence1, residence2, witness1, witness2, witness1address, witness2address, stipend, minister, remarks)" +
					"VALUES(@record_id, @recordholder2_fullname, @parent1_fullname2, @parent2_fullname2, @status1, @status2, @age1, @age2, @place_of_origin1, @place_of_origin2, @residence1, @residence2, @witness1, @witness2, @witness1address, @witness2address, @stipend, @minister, @remarks)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@record_id", recID);
				cmd.Parameters.AddWithValue("@recordholder2_fullname", fullName2);
				cmd.Parameters.AddWithValue("@parent1_fullname2", parent3);
				cmd.Parameters.AddWithValue("@parent2_fullname2", parent4);
				cmd.Parameters.AddWithValue("@status1", status1);
				cmd.Parameters.AddWithValue("@status2", status2);
				cmd.Parameters.AddWithValue("@age1", age1);
				cmd.Parameters.AddWithValue("@age2", age2);
				cmd.Parameters.AddWithValue("@place_of_origin1", hometown1);
				cmd.Parameters.AddWithValue("@place_of_origin2", hometown2);
				cmd.Parameters.AddWithValue("@residence1", residence1);
				cmd.Parameters.AddWithValue("@residence2", residence2);
				cmd.Parameters.AddWithValue("@witness1", sponsor1);
				cmd.Parameters.AddWithValue("@witness2", sponsor2);
				cmd.Parameters.AddWithValue("@witness1address", residence3);
				cmd.Parameters.AddWithValue("@witness2address", residence4);
				cmd.Parameters.AddWithValue("@stipend", stipend);
				cmd.Parameters.AddWithValue("@minister", minister);
				cmd.Parameters.AddWithValue("@remarks", remarks);
				stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				string tmp = pmsutil.LogRecord(recID,"LOGC-01");
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
		private int FetchMatrimonialStipend()
		{
			int ret = 0;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT key_value FROM settings WHERE key_name = 'Matrimonial Stipend';";
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
		///// <summary>
		/// Validates input.
		/// </summary>
		private string ValidateInp(string targ)
		{
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
				marriageDate = Convert.ToDateTime(MarriageDate.Text).ToString("yyyy-MM-dd");
				age1 = Convert.ToInt32(Age1.Value);
				age2 = Convert.ToInt32(Age2.Value);
				fullName1 = ValidateInp(FullName1.Text);
				fullName2 = ValidateInp(FullName2.Text);
				switch (Status1.SelectedIndex)
				{
					case 0:
						status1 = "Widow";
						break;
					case 1:
						status1 = "Widower";
						break;
					case 2:
						status1 = "Single";
						break;
					case 3:
						status1 = "Conjugal";
						break;
					case 4:
						status1 = "Adult";
						break;
					case 5:
						status1 = "Infant";
						break;
					default:
						status1 = "----";
						break;
				}
				switch (Status2.SelectedIndex)
				{
					case 0:
						status2 = "Widow";
						break;
					case 1:
						status2 = "Widower";
						break;
					case 2:
						status2 = "Single";
						break;
					case 3:
						status2 = "Conjugal";
						break;
					case 4:
						status2 = "Adult";
						break;
					case 5:
						status2 = "Infant";
						break;
					default:
						status2 = "----";
						break;
				}
				hometown1 = ValidateInp(Hometown1.Text);
				hometown2 = ValidateInp(Hometown2.Text);
				residence1 = ValidateInp(Residence1.Text);
				residence2 = ValidateInp(Residence2.Text);
				parent1 = ValidateInp(Parent1.Text);
				parent2 = ValidateInp(Parent2.Text);
				parent3 = ValidateInp(Parent3.Text);
				parent4 = ValidateInp(Parent4.Text);
				sponsor1 = ValidateInp(Sponsor1.Text);
				sponsor2 = ValidateInp(Sponsor2.Text);
				residence3 = ValidateInp(Residence3.Text);
				residence4 = ValidateInp(Residence4.Text);
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

			Hometown1SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_origin1 FROM matrimonial_records WHERE " +
					"place_of_origin1 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Hometown1.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Hometown1SuggestionArea.Items.Add(db_reader.GetString("place_of_origin1"));
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

			Hometown2SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_origin2 FROM matrimonial_records WHERE " +
					"place_of_origin2 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Hometown2.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Hometown2SuggestionArea.Items.Add(db_reader.GetString("place_of_origin2"));
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

			Residence1SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT residence1 FROM matrimonial_records WHERE " +
					"residence1 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence1.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence1SuggestionArea.Items.Add(db_reader.GetString("residence1"));
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

			Residence2SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT residence2 FROM matrimonial_records WHERE " +
					"residence2 LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence2.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence2SuggestionArea.Items.Add(db_reader.GetString("residence2"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions4.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions5(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			Residence3SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT witness1address FROM matrimonial_records WHERE " +
					"witness1address LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence3.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence3SuggestionArea.Items.Add(db_reader.GetString("witness1address"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions5.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions6(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			Residence4SuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT witness2address FROM matrimonial_records WHERE " +
					"witness2address LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence4.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence4SuggestionArea.Items.Add(db_reader.GetString("witness2address"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions6.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions7(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM matrimonial_records WHERE " +
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

				Suggestions7.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click1(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Hometown1SuggestionArea.SelectedItem = item;
			Hometown1.Text = Hometown1SuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click2(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Hometown2SuggestionArea.SelectedItem = item;
			Hometown2.Text = Hometown2SuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click3(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Residence1SuggestionArea.SelectedItem = item;
			Residence1.Text = Residence1SuggestionArea.SelectedItem.ToString();
			Suggestions3.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click4(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Residence2SuggestionArea.SelectedItem = item;
			Residence2.Text = Residence2SuggestionArea.SelectedItem.ToString();
			Suggestions4.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click5(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Residence3SuggestionArea.SelectedItem = item;
			Residence3.Text = Residence3SuggestionArea.SelectedItem.ToString();
			Suggestions5.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click6(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			Residence4SuggestionArea.SelectedItem = item;
			Residence4.Text = Residence4SuggestionArea.SelectedItem.ToString();
			Suggestions6.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click7(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions7.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
			Suggestions5.Visibility = Visibility.Hidden;
			Suggestions6.Visibility = Visibility.Hidden;
			Suggestions7.Visibility = Visibility.Hidden;
		}
	}
}
