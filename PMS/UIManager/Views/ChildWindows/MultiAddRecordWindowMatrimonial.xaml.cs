using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildViews;
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
	public partial class MultiAddRecordWindowMatrimonial : ChildWindow
	{
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private int _statcode = 0;

		ObservableCollection<MRecordEntryMatrimonial> test1;

		private int _bookNum;
		private ViewRecordEntries _vre;

		public MultiAddRecordWindowMatrimonial(ViewRecordEntries vre, int bookNum)
        {
			_vre = vre;
			InitializeComponent();
			_bookNum = bookNum;
			PageNum.Value = vre.Page.Value;

			test1 = new ObservableCollection<MRecordEntryMatrimonial>();

			RecordItemsHolder.ItemsSource = test1;

		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//if (e.UserState != null)
			//EntriesHolder.Items.Add(e.UserState);
		}
		void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//SyncChanges();
			PBar.IsIndeterminate = false;
			ConfirmBtn.IsEnabled = true;
			if (_statcode > 0)
			{
				MsgSuccess();
				this.Close();
			}
			else
			{
				MsgFail();
			}
		}
		private void DoWork(object sender, DoWorkEventArgs e) {
			System.Collections.IList items = RecordItemsHolder.Items;
			for (int i = 0; i < items.Count - 1; i++)
			{
				MRecordEntryMatrimonial recordx = (MRecordEntryMatrimonial)items[i];

				bool proceed = true;
				App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
				{
					if (string.IsNullOrWhiteSpace(recordx.EntryNumber.ToString()) || recordx.EntryNumber < 1)
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.FullName1) || string.IsNullOrWhiteSpace(recordx.FullName2))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Status1) || string.IsNullOrWhiteSpace(recordx.Status2))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (recordx.Age1 < 0 || recordx.Age2 < 0 || string.IsNullOrWhiteSpace(recordx.Age1.ToString()) || string.IsNullOrWhiteSpace(recordx.Age2.ToString()))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Hometown1) || string.IsNullOrWhiteSpace(recordx.Hometown2))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Residence1) || string.IsNullOrWhiteSpace(recordx.Residence2))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.W1Residence) || string.IsNullOrWhiteSpace(recordx.W2Residence))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Parent1) || string.IsNullOrWhiteSpace(recordx.Parent1))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Witness1) || string.IsNullOrWhiteSpace(recordx.Witness2))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Stipend.ToString()))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Minister))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
				});
				if (proceed == true)
				{
					dbman = new DBConnectionManager();
					pmsutil = new PMSUtil();
					using (conn = new MySqlConnection(dbman.GetConnStr()))
					{
						conn.Open();
						if (conn.State == ConnectionState.Open)
						{
							App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
							{
								string recID = pmsutil.GenRecordID();
								MySqlCommand cmd = conn.CreateCommand();
								cmd.CommandText =
									"INSERT INTO records(record_id, book_number, page_number, entry_number, record_date, recordholder_fullname, parent1_fullname, parent2_fullname)" +
									"VALUES(@record_id, @book_number, @page_number, @entry_number, @record_date, @recordholder_fullname, @parent1_fullname, @parent2_fullname)";
								cmd.Prepare();
								cmd.Parameters.AddWithValue("@record_id", recID);
								cmd.Parameters.AddWithValue("@book_number", _bookNum);
								cmd.Parameters.AddWithValue("@page_number", PageNum.Value);
								cmd.Parameters.AddWithValue("@entry_number", recordx.EntryNumber);
								cmd.Parameters.AddWithValue("@record_date", DateTime.Parse(recordx.MarriageDate).ToString("yyyy-MM-dd"));
								cmd.Parameters.AddWithValue("@recordholder_fullname", recordx.FullName1);
								cmd.Parameters.AddWithValue("@parent1_fullname", recordx.Parent1);
								cmd.Parameters.AddWithValue("@parent2_fullname", recordx.Parent2);
								int stat_code = cmd.ExecuteNonQuery();
								conn.Close();

								conn.Open();
								//Phase 2
								cmd = dbman.DBConnect().CreateCommand();
								cmd.CommandText =
									"INSERT INTO matrimonial_records(record_id, recordholder2_fullname, parent1_fullname2, parent2_fullname2, status1, status2, age1, age2, place_of_origin1, place_of_origin2, residence1, residence2, witness1, witness2, witness1address, witness2address, stipend, minister, remarks)" +
									"VALUES(@record_id, @recordholder2_fullname, @parent1_fullname2, @parent2_fullname2, @status1, @status2, @age1, @age2, @place_of_origin1, @place_of_origin2, @residence1, @residence2, @witness1, @witness2, @witness1address, @witness2address, @stipend, @minister, @remarks)";
								cmd.Prepare();
								cmd.Parameters.AddWithValue("@record_id", recID);
								cmd.Parameters.AddWithValue("@recordholder2_fullname", recordx.FullName2);
								cmd.Parameters.AddWithValue("@parent1_fullname2", recordx.Parent3);
								cmd.Parameters.AddWithValue("@parent2_fullname2", recordx.Parent4);
								cmd.Parameters.AddWithValue("@status1", recordx.Status1);
								cmd.Parameters.AddWithValue("@status2", recordx.Status2);
								cmd.Parameters.AddWithValue("@age1", recordx.Age1);
								cmd.Parameters.AddWithValue("@age2", recordx.Age2);
								cmd.Parameters.AddWithValue("@place_of_origin1", recordx.Hometown1);
								cmd.Parameters.AddWithValue("@place_of_origin2", recordx.Hometown2);
								cmd.Parameters.AddWithValue("@residence1", recordx.Residence1);
								cmd.Parameters.AddWithValue("@residence2", recordx.Residence2);
								cmd.Parameters.AddWithValue("@witness1", recordx.Witness1);
								cmd.Parameters.AddWithValue("@witness2", recordx.Witness2);
								cmd.Parameters.AddWithValue("@witness1address", recordx.W1Residence);
								cmd.Parameters.AddWithValue("@witness2address", recordx.W2Residence);
								cmd.Parameters.AddWithValue("@stipend", Convert.ToDouble(string.Format("{0:N3}", recordx.Stipend)));
								cmd.Parameters.AddWithValue("@minister", recordx.Minister);
								cmd.Parameters.AddWithValue("@remarks", recordx.Remarks);
								stat_code = cmd.ExecuteNonQuery();

								conn.Close();
								string tmp = pmsutil.LogRecord(recID, "LOGC-01");
								//return stat_code;
								if (stat_code > 0)
								{
									MsgSuccess();
									this.Close();
								}
								else
								{
									MsgFail();
								}
							});
						}
						else
						{

						}
					}
				}
				else {

				}
			}
		}
		private void CreateAccountButton_Click(object sender, RoutedEventArgs e)
		{
			BackgroundWorker worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
			worker.DoWork += DoWork;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			worker.RunWorkerAsync(10000);

			PBar.IsIndeterminate = true;
			ConfirmBtn.IsEnabled = false;
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
			var currentRowIndex = RecordItemsHolder.Items.IndexOf(RecordItemsHolder.CurrentItem);
			nud.Value = currentRowIndex + 1;
		}

		private void SyncFee(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			pmsutil = new PMSUtil();
			NumericUpDown nud = (NumericUpDown)sender;
			nud.Value = Convert.ToDouble(string.Format("{0:N3}", pmsutil.GetPrintFee("Matrimonial")));
		}
	}
}
