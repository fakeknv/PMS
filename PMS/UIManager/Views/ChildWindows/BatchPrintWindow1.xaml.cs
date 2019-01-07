using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PMS.UIComponents;
using System.Collections.ObjectModel;
using Spire.Doc;
using System.Diagnostics;
using System.ComponentModel;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class BatchPrintWindow1 : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private System.Collections.IList _items;

		private ObservableCollection<RecordEntryConfirmation> records;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public BatchPrintWindow1(System.Collections.IList items)
		{
			_items = items;
			pmsutil = new PMSUtil();
			InitializeComponent();
			GetResidingPriests();
			PrintingFee.Value = Convert.ToDouble(pmsutil.GetPrintFee("Confirmation"));
			records = new ObservableCollection<RecordEntryConfirmation>();

			for (int i = 0; i < items.Count; i++)
			{
				RecordEntryConfirmation recordx = (RecordEntryConfirmation)items[i];
				Console.WriteLine(recordx.RecordID);
				records.Add(new RecordEntryConfirmation()
				{
					RecordID = recordx.RecordID,
					EntryNumber = recordx.EntryNumber,
					ConfirmationYear = recordx.ConfirmationYear,
					ConfirmationDate = recordx.ConfirmationDate,
					FullName = recordx.FullName,
					Age = Convert.ToInt32(recordx.Age),
					Parish = recordx.Parish,
					Province = recordx.Province,
					PlaceOfBaptism = recordx.PlaceOfBaptism,
					Parent1 = recordx.Parent1,
					Parent2 = recordx.Parent2,
					Sponsor1 = recordx.Sponsor1,
					Sponsor2 = recordx.Sponsor2,
					Stipend = recordx.Stipend,
					Minister = recordx.Minister
				});
			}
			EntriesHolder.Items.Refresh();
			EntriesHolder.ItemsSource = records;
			EntriesHolder.Items.Refresh();
		}
		private void GetResidingPriests()
		{
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM residing_priests;";
				cmd.Parameters.AddWithValue("@key_name", "Church Name");
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
		private string GetBNum(string rid)
		{
			string ret = "";
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT book_number FROM records WHERE record_id = @rid LIMIT 1;";
				cmd.Parameters.AddWithValue("@rid", rid);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("book_number");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
		}
		private string GetPNum(string rid)
		{
			string ret = "";
			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT page_number FROM records WHERE record_id = @rid LIMIT 1;";
				cmd.Parameters.AddWithValue("@rid", rid);
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("page_number");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
			return ret;
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
		/// <summary>
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private void ConfirmPrint_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			PLabel.Visibility = Visibility.Visible;
			QueueCounter.Visibility = Visibility.Visible;
			QueuePBar.Visibility = Visibility.Visible;
			
			BackgroundWorker worker = new BackgroundWorker
			{
				WorkerReportsProgress = true
			};
			worker.DoWork += BatchPrint;
			worker.ProgressChanged += Worker_ProgressChanged;
			worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
			worker.RunWorkerAsync(10000);
		}
		void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
			//if (e.UserState != null)
			//EntriesHolder.Items.Add(e.UserState);
		}
		void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			//SyncChanges();
			QueuePBar.IsIndeterminate = false;
			PLabel.Visibility = Visibility.Hidden;
			QueueCounter.Visibility = Visibility.Hidden;
			QueuePBar.Visibility = Visibility.Hidden;
			QueueCounter.Content = "0/0";
			//MsgSuccess();
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected records has been added to the print queue.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
		private void BatchPrint(object sender, DoWorkEventArgs e) {
			int tick = 0;
			int total = _items.Count;
			string signature = "";
			string purpose = "";
			double fee = 0d;

			App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
			{
				QueueCounter.Content = tick + "/" + total;
				QueuePBar.IsIndeterminate = true;

				signature = Signatory.Text;
				purpose = Purpose.Text;
				fee = Convert.ToDouble(PrintingFee.Value);
			});

			for (int i = 0; i < _items.Count; i++)
			{
				App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
				{
					QueueCounter.Content = tick + "/" + total;
				});
				RecordEntryConfirmation recordx = (RecordEntryConfirmation)_items[i];

				string x1;

				string[] spl = DateTime.Parse(recordx.ConfirmationDate + "," + recordx.ConfirmationYear).ToString("MM/dd/yyyy").Split('/');
				string suff = GetDaySuffix(int.Parse(spl[1]));
				string mon = PrepMonth(int.Parse(spl[0]));
				if (int.Parse(spl[2]) > 1999)
				{
					x1 = "";
					spl[2] = spl[2].Remove(0, 2);
				}
				else
				{
					x1 = "X";
				}

				Document doc = new Document();
				doc.LoadFromFile("Data\\temp_confirmation.docx");
				doc.Replace("name", recordx.FullName, true, true);
				doc.Replace("day", int.Parse(spl[1]) + suff, true, true);
				doc.Replace("month", mon, true, true);
				doc.Replace("X", x1, true, true);
				doc.Replace("year", spl[2], true, true);
				doc.Replace("by", recordx.Minister, true, true);
				doc.Replace("no", recordx.EntryNumber.ToString(), true, true);
				doc.Replace("book", GetBNum(recordx.RecordID).ToString(), true, true);
				doc.Replace("page", GetPNum(recordx.RecordID).ToString(), true, true);
				doc.Replace("no", recordx.EntryNumber.ToString(), true, true);
				doc.Replace("priest", recordx.Minister, true, true);
				doc.Replace("purpose", purpose, true, true);
				doc.Replace("date", DateTime.Now.ToString("MMMM d, yyyy"), true, true);
				doc.SaveToFile("Data\\print-" + i + ".docx", FileFormat.Docx);

				string fpath = "Data\\print-" + i + ".docx";

				ProcessStartInfo info = new ProcessStartInfo(fpath.Trim())
				{
					Verb = "Print",
					CreateNoWindow = true,
					WindowStyle = ProcessWindowStyle.Hidden
				};
				Process.Start(info);
				App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
				{
					if (Purpose.SelectedIndex == 0)
					{
						//Reference
						string tmp = pmsutil.LogRecord(recordx.RecordID, "LOGC-03");
					}
					else
					{
						//Marriage
						string tmp = pmsutil.LogRecord(recordx.RecordID, "LOGC-04");
					}
				});
				pmsutil.InsertTransaction("Confirmation Cert.", "Paying", recordx.RecordID, Convert.ToDouble(pmsutil.GetPrintFee("Confirmation")));
				tick++;
			}
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
