﻿using MahApps.Metro.SimpleChildWindow;
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
	public partial class EditBaptismalRecordEntryWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private int pageNum;
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

		private string recordID;


		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public EditBaptismalRecordEntryWindow(string targRecord)
		{
			
			pmsutil = new PMSUtil();
			recordID = targRecord;
			InitializeComponent();
			Stipend.Value = FetchBaptismalStipend();

			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM baptismal_records, records WHERE records.record_id = @record_id AND records.record_id = baptismal_records.record_id LIMIT 1;";
				cmd.Parameters.AddWithValue("@record_id", targRecord);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					EntryNum.Value = Convert.ToDouble(db_reader.GetString("entry_number"));
					PageNum.Value = Convert.ToDouble(db_reader.GetString("page_number"));
					BaptismDate.Text = db_reader.GetString("record_date");
					Birthdate.Text = db_reader.GetString("birthday");
					FullName.Text = db_reader.GetString("recordholder_fullname");
					Legitimacy.Text = db_reader.GetString("legitimacy");
					Stipend.Value = Convert.ToDouble(db_reader.GetString("stipend"));
					PlaceOfBirth.Text = db_reader.GetString("place_of_birth");
					Parent1.Text = db_reader.GetString("parent1_fullname");
					Parent2.Text = db_reader.GetString("parent2_fullname");
					Sponsor1.Text = db_reader.GetString("sponsor1");
					Sponsor2.Text = db_reader.GetString("sponsor2");
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
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int UpdateEntry()
		{
			dbman = new DBConnectionManager();
			try
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"UPDATE records SET page_number = @page_number, entry_number = @entry_number, record_date = @record_date, recordholder_fullname = @recordholder_fullname, parent1_fullname = @parent1_fullname, parent2_fullname = @parent2_fullname WHERE record_id = @record_id;";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@record_id", recordID);
				cmd.Parameters.AddWithValue("@page_number", pageNum);
				cmd.Parameters.AddWithValue("@entry_number", entryNum);
				cmd.Parameters.AddWithValue("@record_date", baptismDate);
				cmd.Parameters.AddWithValue("@recordholder_fullname", fullName);
				cmd.Parameters.AddWithValue("@parent1_fullname", parent1);
				cmd.Parameters.AddWithValue("@parent2_fullname", parent2);
				int stat_code = cmd.ExecuteNonQuery();
				dbman.DBClose();
				
				cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText =
					"UPDATE baptismal_records SET birthday = @birthday, legitimacy = @legitimacy, place_of_birth = @place_of_birth, sponsor1 = @sponsor1, sponsor2 = @sponsor2, stipend = @stipend, minister = @minister, remarks = @remarks WHERE record_id = @record_id;";
				cmd.Prepare();
				cmd.Parameters.AddWithValue("@record_id", recordID);
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
				string tmp = pmsutil.LogRecord(recordID,"LOGC-02");
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
		private void EditRecConfirm(object sender, System.Windows.RoutedEventArgs e)
		{
			switch (Legitimacy.SelectedIndex)
			{
				case 0:
					legitimacy = "Legitimate";
					break;
				case 1:
					legitimacy = "Civil";
					break;
				case 2:
					legitimacy = "Illegitimate";
					break;
				default:
					legitimacy = "----";
					break;
			}
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
			if (UpdateEntry() > 0)
			{
				this.Close();
			}
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