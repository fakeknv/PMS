using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PMS.UIManager.Views.ChildWindows
{

	/// <summary>
	/// Interaction logic for AddAccountWindow.xaml
	/// </summary>
	public partial class MultiAddRecordWindowBaptismal : ChildWindow
	{
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		ObservableCollection<MRecordEntryBaptismal> test1;

		private int _bookNum;

		public MultiAddRecordWindowBaptismal(int bookNum)
        {
			
            InitializeComponent();
			_bookNum = bookNum;


			test1 = new ObservableCollection<MRecordEntryBaptismal>();

			RecordItemsHolder.ItemsSource = test1;

			this.DataContext = this;
		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			System.Collections.IList items = RecordItemsHolder.Items;
			for (int i = 0; i < items.Count-1; i++)
			{
				MRecordEntryBaptismal recordx = (MRecordEntryBaptismal)items[i];
				Console.WriteLine(recordx.BaptismalDate);
				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						string recID = pmsutil.GenRecordID();
						MySqlCommand cmd = dbman.DBConnect().CreateCommand();
						cmd.CommandText =
							"INSERT INTO records(record_id, book_number, page_number, entry_number, record_date, recordholder_fullname, parent1_fullname, parent2_fullname)" +
							"VALUES(@record_id, @book_number, @page_number, @entry_number, @record_date, @recordholder_fullname, @parent1_fullname, @parent2_fullname)";
						cmd.Prepare();
						cmd.Parameters.AddWithValue("@record_id", recID);
						cmd.Parameters.AddWithValue("@book_number", _bookNum);
						cmd.Parameters.AddWithValue("@page_number", PageNum.Value);
						cmd.Parameters.AddWithValue("@entry_number", recordx.EntryNumber);
						cmd.Parameters.AddWithValue("@record_date", DateTime.Parse(recordx.BaptismalDate).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@recordholder_fullname", recordx.FullName);
						cmd.Parameters.AddWithValue("@parent1_fullname", recordx.Parent1);
						cmd.Parameters.AddWithValue("@parent2_fullname", recordx.Parent2);
						int stat_code = cmd.ExecuteNonQuery();
						dbman.DBClose();
						//Phase 2
						cmd = dbman.DBConnect().CreateCommand();
						cmd.CommandText =
							"INSERT INTO baptismal_records(record_id, birthday, legitimacy, place_of_birth, sponsor1, sponsor2, stipend, minister, remarks)" +
							"VALUES(@record_id, @birthday, @legitimacy, @place_of_birth, @sponsor1, @sponsor2, @stipend, @minister, @remarks)";
						cmd.Prepare();
						cmd.Parameters.AddWithValue("@record_id", recID);
						cmd.Parameters.AddWithValue("@birthday", DateTime.Parse(recordx.BirthDate).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@legitimacy", recordx.Legitimacy);
						cmd.Parameters.AddWithValue("@place_of_birth", recordx.PlaceOfBirth);
						cmd.Parameters.AddWithValue("@sponsor1", recordx.Godparent1);
						cmd.Parameters.AddWithValue("@sponsor2", recordx.Godparent2);
						cmd.Parameters.AddWithValue("@stipend", recordx.Stipend);
						cmd.Parameters.AddWithValue("@minister", recordx.Minister);
						cmd.Parameters.AddWithValue("@remarks", recordx.Remarks);
						stat_code = cmd.ExecuteNonQuery();
						dbman.DBClose();
						string tmp = pmsutil.LogRecord(recID, "LOGC-01");
						//return stat_code;
						if (stat_code > 0) {
							MsgSuccess();
							this.Close();
						}
						else {
							MsgFail();
						}
					}
					else
					{

					}
				}
			}
			//MessageBox.Show(items.Count.ToString());
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The records has been added successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void EnableCustom(object sender, SelectionChangedEventArgs e)
		{
			
		}

		private void SyncEntryNum(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			NumericUpDown nud = (NumericUpDown)sender;
			nud.Value = RecordItemsHolder.Items.Count - 1;
		}

		private void SyncFee(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			pmsutil = new PMSUtil();
			NumericUpDown nud = (NumericUpDown)sender;
			nud.Value = Convert.ToDouble(pmsutil.GetPrintFee("Baptismal"));
		}
	}
}
