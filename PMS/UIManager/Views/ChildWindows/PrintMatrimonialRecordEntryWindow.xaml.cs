using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using Spire.Doc;
using MahApps.Metro.Controls.Dialogs;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class PrintMatrimonialRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string recordID;
		private int pageNum;
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


		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public PrintMatrimonialRecordEntryWindow(string targRecord)
		{
			
			pmsutil = new PMSUtil();
			recordID = targRecord;
			InitializeComponent();
			GetResidingPriests();
			PrintingFee.Value = Convert.ToDouble(pmsutil.GetPrintFee("Matrimonial"));
			recordID = targRecord;
			Stipend.Value = FetchMatrimonialStipend();

			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM matrimonial_records, records WHERE records.record_id = @record_id AND records.record_id = matrimonial_records.record_id LIMIT 1;";
				cmd.Parameters.AddWithValue("@record_id", targRecord);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
					PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
					MarriageDate.Text = db_reader.GetString("record_date");
					Age1.Value = Convert.ToDouble(db_reader.GetString("age1"));
					Age2.Value = Convert.ToDouble(db_reader.GetString("age2"));
					Status1.Text = db_reader.GetString("status1");
					Status2.Text = db_reader.GetString("status2");
					FullName1.Text = db_reader.GetString("recordholder_fullname");
					FullName2.Text = db_reader.GetString("recordholder2_fullname");
					Hometown1.Text = db_reader.GetString("place_of_origin1");
					Hometown2.Text = db_reader.GetString("place_of_origin2");
					Residence1.Text = db_reader.GetString("residence1");
					Residence2.Text = db_reader.GetString("residence2");
					Stipend.Value = Convert.ToDouble(db_reader.GetString("stipend"));
					Parent1.Text = db_reader.GetString("parent1_fullname");
					Parent2.Text = db_reader.GetString("parent2_fullname");
					Parent3.Text = db_reader.GetString("parent1_fullname2");
					Parent4.Text = db_reader.GetString("parent2_fullname2");
					Sponsor1.Text = db_reader.GetString("witness1");
					Sponsor2.Text = db_reader.GetString("witness2");
					Residence3.Text = db_reader.GetString("witness1address");
					Residence4.Text = db_reader.GetString("witness2address");
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
			Suggestions7.Visibility = Visibility.Hidden;
		}
		private void GetResidingPriests()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM residing_priests;";
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
		private int PrintEntry()
		{
			string[] bspl = MarriageDate.Text.Split('/');
			string bsuff = GetDaySuffix(int.Parse(bspl[1]));
			string bmon = PrepMonth(int.Parse(bspl[0]));

			Document doc = new Document();
			doc.LoadFromFile("Data\\temp_marriage.docx");
			doc.Replace("name", FullName1.Text, true, true);
			doc.Replace("name2", FullName2.Text, true, true);
			doc.Replace("age", Age1.Value.ToString(), true, true);
			doc.Replace("age2", Age2.Value.ToString(), true, true);
			doc.Replace("nationality", "Filipino", true, true);
			doc.Replace("nationality2", "Filipino", true, true);
			doc.Replace("residence", Residence1.Text, true, true);
			doc.Replace("residence2", Residence2.Text, true, true);
			doc.Replace("civil", Status1.Text, true, true);
			doc.Replace("civil2", Status2.Text, true, true);
			doc.Replace("father", Parent1.Text, true, true);
			doc.Replace("father2", Parent3.Text, true, true);
			doc.Replace("mother", Parent2.Text, true, true);
			doc.Replace("mother2", Parent4.Text, true, true);
			doc.Replace("witness", Sponsor1.Text, true, true);
			doc.Replace("witness2", Sponsor2.Text, true, true);
			doc.Replace("place", "St. Raphael Parish", true, true);
			doc.Replace("date", bmon + " " + bspl[1] + bsuff + ", " + bspl[2], true, true);
			doc.Replace("priest", Minister.Text, true, true);
			doc.Replace("sign", Signatory.Text, true, true);
			doc.Replace("no", EntryNum.Value.ToString(), true, true);
			doc.Replace("page", PageNum.Value.ToString(), true, true);
			string[] date = DateTime.Now.ToStrin­g("MMMM,d,yyyy").Spl­it(',');
			doc.Replace("month", date[0], true, true);
			date[1] = date[1] + GetDaySuffix(int.Parse(date[1]));
			doc.Replace("days", date[1], true, true);
			doc.Replace("YY", date[2].Remove(0, 2), true, true);
			doc.SaveToFile("Data\\print.docx", FileFormat.Docx);

			string fpath = "Data\\print.docx";

			ProcessStartInfo info = new ProcessStartInfo(fpa­th.Trim())
			{
				Verb = "Print",
				CreateNoWindow = true,
				WindowStyle = ProcessWindowStyle.H­idden
			};
			Process.Start(info);
			return 1;
		}
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
			marriageDate = Convert.ToDateTime(MarriageDate.Text).ToString("yyyy-MM-dd");
			age1 = Convert.ToInt32(Age1.Value);
			age2 = Convert.ToInt32(Age2.Value);
			fullName1 = ValidateInp(FullName1.Text);
			fullName2 = ValidateInp(FullName2.Text);
			status1 = ValidateInp(Status1.Text);
			status2 = ValidateInp(Status2.Text);
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
		private void PrintRecCancel(object sender, System.Windows.RoutedEventArgs e)
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
