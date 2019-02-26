using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using MahApps.Metro.Controls;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using PMS.UIManager.Views.ChildViews;
using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRecordEntryWindow.xaml
	/// </summary>
	public partial class AddBurialRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
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
		private string imageURI;

		private ViewRecordEntries vre;

		public AddBurialRecordEntryWindow(ViewRecordEntries viewRE, int targBook)
		{
			vre = viewRE;
			pmsutil = new PMSUtil();
			InitializeComponent();
			bookNum = targBook;
			FetchBookEntryNum();
			Stipend.Value = FetchBurialStipend();
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
			var bc = new BrushConverter();

			DeathDate.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			BurialDate.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			FullName.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			PlaceOfInterment.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Parent1.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Residence1.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Minister.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			Sacrament.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			CauseOfDeath.BorderBrush = (Brush)bc.ConvertFrom("#FFCCCCCC");

			bool ret = true;

			if (string.IsNullOrWhiteSpace(DeathDate.Text))
			{
				DeathDate.ToolTip = "This field is required.";
				DeathDate.BorderBrush = Brushes.Red;
				Icon3.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (EntryNum.Value < 0)
			{
				EntryNum.ToolTip = "Must be greater than zero.";
				EntryNum.BorderBrush = Brushes.Red;
				Icon2.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (PageNum.Value < 0)
			{
				PageNum.ToolTip = "Must be greater than zero.";
				PageNum.BorderBrush = Brushes.Red;
				Icon1.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(BurialDate.Text))
			{
				BurialDate.ToolTip = "This field is required.";
				BurialDate.BorderBrush = Brushes.Red;
				Icon4.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(FullName.Text))
			{
				FullName.ToolTip = "This field is required.";
				FullName.BorderBrush = Brushes.Red;
				Icon5.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (CheckRequirements() == false)
			{
				MsgError();

				FullName.ToolTip = "This person already has a record.";
				FullName.BorderBrush = Brushes.Red;
				Icon5.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (Stipend.Value == 0)
			{
				Stipend.ToolTip = "Notice: Stipend is set to zero.";
				Stipend.BorderBrush = Brushes.Orange;
				Icon8.BorderBrush = Brushes.Red;

				MsgStipend();
				ret = true;
			}
			if (string.IsNullOrWhiteSpace(PlaceOfInterment.Text))
			{
				PlaceOfInterment.ToolTip = "This field is required.";
				PlaceOfInterment.BorderBrush = Brushes.Red;
				Icon11.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Parent1.Text))
			{
				Parent1.ToolTip = "This field is required.";
				Parent1.BorderBrush = Brushes.Red;
				Icon12.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Minister.Text))
			{
				Minister.ToolTip = "This field is required.";
				Minister.BorderBrush = Brushes.Red;
				Icon16.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Sacrament.Text))
			{
				Sacrament.ToolTip = "This field is required.";
				Sacrament.BorderBrush = Brushes.Red;
				Icon9.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(CauseOfDeath.Text))
			{
				CauseOfDeath.ToolTip = "This field is required.";
				CauseOfDeath.BorderBrush = Brushes.Red;
				Icon10.BorderBrush = Brushes.Red;

				ret = false;
			}
			if (string.IsNullOrWhiteSpace(Residence1.Text))
			{
				Residence1.ToolTip = "This field is required.";
				Residence1.BorderBrush = Brushes.Red;
				Icon14.BorderBrush = Brushes.Red;

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
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TODO
				try
				{
					string recID = pmsutil.GenRecordID();
					MySqlCommand cmd = conn.CreateCommand();
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
					conn.Close();

					conn.Open();
					//Phase 2
					cmd = conn.CreateCommand();
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
					conn.Close();

					conn.Open();
					string dirID = pmsutil.GenDirectoryID();
					string block = "Not Specified";
					string lot = "Not Specified";
					string plot = "Not Specified";
					string rconnum = "Not Specified";
					byte[] ImageData;
					//Phase 3
					if (!string.IsNullOrWhiteSpace(Block.Text))
					{
						block = Block.Text;
					}
					if (!string.IsNullOrWhiteSpace(Lot.Text))
					{
						lot = Lot.Text;
					}
					if (!string.IsNullOrWhiteSpace(Plot.Text))
					{
						plot = Plot.Text;
					}
					if (!string.IsNullOrWhiteSpace(RContactNo.Text))
					{
						rconnum = RContactNo.Text;
					}
					if (!string.IsNullOrWhiteSpace(imageURI))
					{
						FileStream fs = new FileStream(imageURI, FileMode.Open, FileAccess.Read);
						BinaryReader br = new BinaryReader(fs);
						ImageData = br.ReadBytes((int)fs.Length);
						br.Close();
						fs.Close();
					}
					else {
						ImageData = null;
					}
					cmd = conn.CreateCommand();
					cmd.CommandText =
						"INSERT INTO burial_directory(directory_id, record_id, block, lot, plot, gravestone, relative_contact_number)" +
						"VALUES(@directory_id, @record_id, @block, @lot, @plot, @gravestone, @relative_contact_number)";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@directory_id", dirID);
					cmd.Parameters.AddWithValue("@record_id", recID);
					cmd.Parameters.AddWithValue("@block", block);
					cmd.Parameters.AddWithValue("@lot", lot);
					cmd.Parameters.AddWithValue("@plot", plot);
					cmd.Parameters.AddWithValue("@gravestone", ImageData);
					cmd.Parameters.AddWithValue("@relative_contact_number", rconnum);
					stat_code = cmd.ExecuteNonQuery();
					conn.Close();

					string tmp = pmsutil.LogRecord(recID, "LOGC-01");
					return stat_code;
				}
				catch (MySqlException ex)
				{
					Console.WriteLine("Error: {0}", ex.ToString());
					return 0;
				}
			}
		}
		private async void MsgError()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "A record of the same name and type already exists.");
		}
		private bool CheckRequirements()
		{
			bool ret = false;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT COUNT(*) FROM records, registers WHERE registers.book_type = 'Burial' AND registers.book_number = records.book_number AND records.recordholder_fullname = @fname;";
				cmd.Parameters.AddWithValue("@fname", FullName.Text);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("COUNT(*)") == 0)
					{
						ret = true;
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = false;
			}
			return ret;
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
			if (CheckInputs() == true)
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
					MsgSuccess();
					vre.Sync(bookNum);
					this.Close();
				}
				else
				{
					MsgFail();
				}
			}
			else
			{

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

		private void ImagePicker_Click(object sender, RoutedEventArgs e)
		{
			Console.WriteLine("asd");
			OpenFileDialog op = new OpenFileDialog
			{
				Title = "Select a picture",
				Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
			  "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
			  "Portable Network Graphic (*.png)|*.png"
			};
			if (op.ShowDialog() == true)
			{
				ImagePreview.Source = new BitmapImage(new Uri(op.FileName));
				imageURI = op.FileName;
			}
		}
	}
}
