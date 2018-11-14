using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class AddBurialRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int pageNum;
		private int bookNum;
		private int entryNum;
		private string deathDate;
		private string burialDate;
		private int age;
		private string status;
		private string fullName;
		private string sacrament;
		private string causeOfDeath;
		private string intermentPlace;
		private string parent1;
		private string parent2;
		private string residence1;
		private string residence2;
		private int stipend;
		private string minister;
		private string remarks;
		//private DateTime cDate;
		//private DateTime cTime;
		//private string curDate;
		//private string curTime;


		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public AddBurialRecordEntryWindow(int targBook)
		{
			
			pmsutil = new PMSUtil();
			InitializeComponent();
			bookNum = targBook;
			Stipend.Value = FetchBurialStipend();
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
				cmd.Parameters.AddWithValue("@record_date", deathDate);
				cmd.Parameters.AddWithValue("@recordholder_fullname", fullName);
				cmd.Parameters.AddWithValue("@parent1_fullname", parent1);
				cmd.Parameters.AddWithValue("@parent2_fullname", parent2);
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				//Phase 2
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"INSERT INTO burial_records(record_id, burial_date, age, status, residence, residence2, sacrament, cause_of_death, place_of_interment, stipend, minister, remarks)" +
					"VALUES(@record_id, @burial_date, @age, @status, @residence, @residence2, @sacrament, @cause_of_death, @place_of_interment, @stipend, @minister, @remarks)";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@record_id", recID);
				cmd.Parameters.AddWithValue("@burial_date", burialDate);
				cmd.Parameters.AddWithValue("@age", age);
				cmd.Parameters.AddWithValue("@status", status);
				cmd.Parameters.AddWithValue("@residence", residence1);
				cmd.Parameters.AddWithValue("@residence2", residence2);
				cmd.Parameters.AddWithValue("@sacrament", sacrament);
				cmd.Parameters.AddWithValue("@cause_of_death", causeOfDeath);
				cmd.Parameters.AddWithValue("@place_of_interment", intermentPlace);
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
		private int FetchBurialStipend()
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
		///// Validates input.
		///// </summary>
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
			entryNum = Convert.ToInt32(EntryNum.Value);
			pageNum = Convert.ToInt32(PageNum.Value);
			deathDate = Convert.ToDateTime(DeathDate.Text).ToString("yyyy-MM-dd");
			burialDate = Convert.ToDateTime(BurialDate.Text).ToString("yyyy-MM-dd");
			age = Convert.ToInt32(Age.Value);
			switch (Status.SelectedIndex)
			{
				case 0:
					status = "Widow";
					break;
				case 1:
					status = "Widower";
					break;
				case 2:
					status = "Single";
					break;
				case 3:
					status = "Conjugal";
					break;
				case 4:
					status = "Adult";
					break;
				case 5:
					status = "Infant";
					break;
				default:
					status = "----";
					break;
			}
			fullName = ValidateInp(FullName.Text);
			sacrament = ValidateInp(Sacrament.Text);
			causeOfDeath = ValidateInp(CauseOfDeath.Text);
			intermentPlace = ValidateInp(PlaceOfInterment.Text);
			parent1 = ValidateInp(Parent1.Text);
			parent2 = ValidateInp(Parent2.Text);
			residence1 = ValidateInp(Residence1.Text);
			residence2 = ValidateInp(Residence2.Text);
			stipend = Convert.ToInt32(Stipend.Value);
			minister = ValidateInp(Minister.Text);
			remarks = ValidateInp(Remarks.Text);
			if (InsertEntry() > 0)
			{
				this.Close();
			}
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

			SacramentSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT sacrament FROM burial_records WHERE " +
					"sacrament LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Sacrament.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					SacramentSuggestionArea.Items.Add(db_reader.GetString("sacrament"));
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

			CauseOfDeathSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT cause_of_death FROM burial_records WHERE " +
					"cause_of_death LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + CauseOfDeath.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					CauseOfDeathSuggestionArea.Items.Add(db_reader.GetString("cause_of_death"));
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
				cmd.CommandText = "SELECT DISTINCT residence FROM burial_records WHERE " +
					"residence LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + Residence1.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Residence1SuggestionArea.Items.Add(db_reader.GetString("residence"));
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
				cmd.CommandText = "SELECT DISTINCT residence2 FROM burial_records WHERE " +
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

			MinisterSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT minister FROM burial_records WHERE " +
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

				Suggestions5.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void ShowSuggestions6(object sender, System.Windows.Controls.TextChangedEventArgs e)
		{
			dbman = new DBConnectionManager();

			PlaceOfIntermentSuggestionArea.Items.Clear();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT DISTINCT place_of_interment FROM burial_records WHERE " +
					"place_of_interment LIKE @query;";
				cmd.Parameters.AddWithValue("@query", "%" + PlaceOfInterment.Text + "%");
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					PlaceOfIntermentSuggestionArea.Items.Add(db_reader.GetString("place_of_interment"));
				}
				//close Connection
				dbman.DBClose();

				Suggestions6.Visibility = System.Windows.Visibility.Visible;
			}
			else
			{

			}
		}
		private void Suggestion_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			SacramentSuggestionArea.SelectedItem = item;
			Sacrament.Text = SacramentSuggestionArea.SelectedItem.ToString();
			Suggestions1.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click2(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			CauseOfDeathSuggestionArea.SelectedItem = item;
			CauseOfDeath.Text = CauseOfDeathSuggestionArea.SelectedItem.ToString();
			Suggestions2.Visibility = Visibility.Hidden;
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
			MinisterSuggestionArea.SelectedItem = item;
			Minister.Text = MinisterSuggestionArea.SelectedItem.ToString();
			Suggestions5.Visibility = Visibility.Hidden;
		}
		private void Suggestion_Click6(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var cb = sender as TextBlock;
			var item = cb.DataContext;
			PlaceOfIntermentSuggestionArea.SelectedItem = item;
			PlaceOfInterment.Text = PlaceOfIntermentSuggestionArea.SelectedItem.ToString();
			Suggestions6.Visibility = Visibility.Hidden;
		}
		private void Hide(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
		{
			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
			Suggestions5.Visibility = Visibility.Hidden;
			Suggestions6.Visibility = Visibility.Hidden;
		}
	}
}
