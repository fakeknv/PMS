using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Spire.Doc;
using System.Diagnostics;
using MahApps.Metro.Controls.Dialogs;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class PrintBurialRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string recordID;
		private int pageNum;
		private int entryNum;
		private string deathDate;
		private string burialDate;
		private int age;
		private string status;
		private string fullName;
		private string sacrament;
		private string causeOfDeath;
		private string intermentPlace;
		private string residence1;
		private string residence2;
		private int stipend;
		private string minister;
		private string remarks;

		private string spouse, p1, p2;
		private MySqlConnection conn;

		public PrintBurialRecordEntryWindow(string targRecord)
		{
			
			pmsutil = new PMSUtil();
			InitializeComponent();
			recordID = targRecord;
			GetResidingPriests();
			CheckAccess(targRecord);

			PrintingFee.Value = Convert.ToDouble(pmsutil.GetPrintFee("Burial"));
			Stipend.Value = FetchBurialStipend();

			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM burial_records, records WHERE records.record_id = @record_id AND records.record_id = burial_records.record_id LIMIT 1;";
				cmd.Parameters.AddWithValue("@record_id", targRecord);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
					PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
					DeathDate.Text = db_reader.GetString("record_date");
					BurialDate.Text = db_reader.GetString("burial_date");
					Age.Value = Convert.ToDouble(db_reader.GetString("age"));
					FullName.Text = db_reader.GetString("recordholder_fullname");
					Age.Value = Convert.ToDouble(db_reader.GetString("age"));
					Status.Text = db_reader.GetString("status");
					Parent1.Text = db_reader.GetString("parent1_fullname");
					Parent2.Text = db_reader.GetString("parent2_fullname");
					Residence1.Text = db_reader.GetString("residence");
					Residence2.Text = db_reader.GetString("residence2");
					Sacrament.Text = db_reader.GetString("sacrament");
					CauseOfDeath.Text = db_reader.GetString("cause_of_death");
					PlaceOfInterment.Text = db_reader.GetString("place_of_interment");
					Stipend.Value = Convert.ToDouble(db_reader.GetString("stipend"));
					Minister.Text = db_reader.GetString("minister");
					Remarks.Text = db_reader.GetString("remarks");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}

			Suggestions1.Visibility = Visibility.Hidden;
			Suggestions2.Visibility = Visibility.Hidden;
			Suggestions3.Visibility = Visibility.Hidden;
			Suggestions4.Visibility = Visibility.Hidden;
			Suggestions5.Visibility = Visibility.Hidden;
			Suggestions6.Visibility = Visibility.Hidden;
		}
		internal void CheckAccess(string record_id)
		{
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(*) FROM records_log WHERE record_id = @record_id AND log_code = 'LOGC-03' OR log_code = 'LOGC-04';";
					cmd.Parameters.AddWithValue("@record_id", record_id);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0)
						{
							MsgNotice();
						}
					}
				}
			}
		}
		private async void MsgNotice()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Notice!", "This record has been printed before. Please check access history for more info.");
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int PrintEntry()
		{
			string[] bspl = BurialDate.Text.Split('/');
			string bmon = PrepMonth(int.Parse(bspl[0]));

			string[] dspl = DeathDate.Text.Split('/');
			string dmon = PrepMonth(int.Parse(dspl[0]));

			Document doc = new Document();
			doc.LoadFromFile("Data\\temp_death.docx");
			doc.Replace("name", fullName, true, true);
			doc.Replace("age", Convert.ToString(age), true, true);
			doc.Replace("nationality", Nationality.Text, true, true);
			doc.Replace("residence", residence1, true, true);
			doc.Replace("civil", status, true, true);
			doc.Replace("father", p1, true, true);
			doc.Replace("mother", p2, true, true);
			doc.Replace("spouse", spouse, true, true);
			doc.Replace("date_of_birth", bmon + " " + bspl[1] + ", " + bspl[2], true, true);
			doc.Replace("cause_of_death", causeOfDeath, true, true);
			doc.Replace("date_of_burial", dmon + " " + dspl[1] + ", " + dspl[2], true, true);
			doc.Replace("place_of_burial", intermentPlace, true, true);
			doc.Replace("priest", minister, true, true);
			doc.Replace("sign", Signatory.Text, true, true);
			doc.Replace("no", Convert.ToString(entryNum), true, true);
			doc.Replace("page", Convert.ToString(pageNum), true, true);
			string[] date = DateTime.Now.ToString("MMMM,d,yyyy").Split(','); ;
			doc.Replace("month", date[0], true, true);
			doc.Replace("day", date[1], true, true);
			doc.Replace("YY", date[2].Remove(0, 2), true, true);
			doc.SaveToFile("Data\\print.docx", FileFormat.Docx);

			//Load Document
			Document document = new Document();
			document.LoadFromFile(@"Data\\print.docx");

			//Convert Word to PDF
			document.SaveToFile("Output\\print_file.pdf", FileFormat.PDF);

			System.Diagnostics.Process.Start("Output\\print_file.pdf");

			//Reference
			string tmp = pmsutil.LogRecord(recordID, "LOGC-03");
			pmsutil.InsertTransaction("Burial Cert.", "Unpaid", recordID, Convert.ToDouble(PrintingFee.Value));
			return 1;
		}
		private void GetResidingPriests()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM residing_priests WHERE priest_name != 'NA';";
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					Signatory.Items.Add(db_reader.GetString("priest_name"));
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}
		private string GetDaySuffix(int day)
		{
			switch (day)
			{
				case 1:
				case 21:
				case 31:
					return "st";
				case 2:
				case 22:
					return "nd";
				case 3:
				case 23:
					return "rd";
				default:
					return "th";
			}
		}
		private string PrepMonth(int mon)
		{
			switch (mon)
			{
				case 1:
					return "January";
				case 2:
					return "February";
				case 3:
					return "March";
				case 4:
					return "April";
				case 5:
					return "May";
				case 6:
					return "June";
				case 7:
					return "July";
				case 8:
					return "August";
				case 9:
					return "September";
				case 10:
					return "October";
				case 11:
					return "November";
				default:
					return "December";
			}
		}
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
				return "---";
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
		private void PrintRecConfirm(object sender, System.Windows.RoutedEventArgs e)
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
			p1 = ValidateInp(Parent1.Text);
			p2 = ValidateInp(Parent2.Text);
			residence1 = ValidateInp(Residence1.Text);
			residence2 = ValidateInp(Residence2.Text);
			stipend = Convert.ToInt32(Stipend.Value);
			minister = ValidateInp(Minister.Text);
			remarks = ValidateInp(Remarks.Text);
			if (PrintEntry() > 0)
			{
				MsgSuccess();
				this.Close();
			}
			else
			{
				MsgFail();
			}
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected record has been added to the print queue.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void EditRecCancel(object sender, System.Windows.RoutedEventArgs e)
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
		private void Parents_Checked(object sender, RoutedEventArgs e)
		{
			spouse = "";
			p1 = Parent1.Text;
			p2 = Parent2.Text;
			Parent2.IsEnabled = true;
		}

		private void Spouse_Checked(object sender, RoutedEventArgs e)
		{
			spouse = Parent1.Text;
			p1 = "";
			p2 = "";
			Parent2.IsEnabled = false;
		}
	}
}
