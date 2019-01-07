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
	public partial class BatchPrintWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private System.Collections.IList _items;

		private ObservableCollection<RecordEntryBaptismal> records;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public BatchPrintWindow(System.Collections.IList items)
		{
			_items = items;
			pmsutil = new PMSUtil();
			InitializeComponent();
			GetResidingPriests();
			PrintingFee.Value = Convert.ToDouble(pmsutil.GetPrintFee("Baptismal"));
			records = new ObservableCollection<RecordEntryBaptismal>();

			for (int i = 0; i < items.Count; i++)
			{
				RecordEntryBaptismal recordx = (RecordEntryBaptismal)items[i];
				Console.WriteLine(recordx.RecordID);
				records.Add(new RecordEntryBaptismal()
				{
					RecordID = recordx.RecordID,
					EntryNumber = recordx.EntryNumber,
					BaptismalYear = recordx.BaptismalYear,
					BaptismalDate = recordx.BaptismalDate,
					FullName = recordx.FullName,
					BirthDate = recordx.BirthDate,
					Legitimacy = recordx.Legitimacy,
					PlaceOfBirth = recordx.PlaceOfBirth,
					Parent1 = recordx.Parent1,
					Parent2 = recordx.Parent2,
					Godparent1 = recordx.Godparent1,
					Godparent2 = recordx.Godparent2,
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
				RecordEntryBaptismal recordx = (RecordEntryBaptismal)_items[i];

				string x1, x2;
				string[] bspl = DateTime.Parse(recordx.BirthDate).ToString("MM/dd/yyyy").Split('/');
				string bsuff = GetDaySuffix(int.Parse(bspl[1]));
				string bmon = PrepMonth(int.Parse(bspl[0]));
				if (int.Parse(bspl[2]) > 1999)
				{
					x1 = "";
					bspl[2] = bspl[2].Remove(0, 2);
				}
				else
				{
					x1 = "X";
				}

				string[] dspl = DateTime.Parse(recordx.BaptismalDate + "," + recordx.BaptismalYear).ToString("MM/dd/yyyy").Split('/');
				string dsuff = GetDaySuffix(int.Parse(dspl[1]));
				string dmon = PrepMonth(int.Parse(dspl[0]));
				if (int.Parse(dspl[2]) > 1999)
				{
					x2 = "";
					dspl[2] = dspl[2].Remove(0, 2);
				}
				else
				{
					x2 = "X";
				}

				Document doc = new Document();
				doc.LoadFromFile("Data\\temp_baptismal.docx");
				doc.LoadFromFile("Data\\temp_baptismal.docx");
				doc.Replace("name", recordx.FullName, true, true);
				doc.Replace("father", recordx.Parent1, true, true);
				doc.Replace("mother", recordx.Parent2, true, true);
				doc.Replace("born", recordx.PlaceOfBirth, true, true);
				doc.Replace("day1", int.Parse(bspl[1]) + bsuff, true, true);
				doc.Replace("month1", bmon, true, true);
				doc.Replace("X1", x1, true, true);
				doc.Replace("year1", DateTime.Parse(recordx.BirthDate).ToString("yyyy"), true, true);
				doc.Replace("day2", int.Parse(dspl[1]) + dsuff, true, true);
				doc.Replace("month2", dmon, true, true);
				doc.Replace("X2", x2, true, true);
				doc.Replace("year2", DateTime.Parse(recordx.BaptismalDate + "," + recordx.BaptismalYear).ToString("yyyy"), true, true);
				doc.Replace("church", pmsutil.GetChurchName(), true, true);
				doc.Replace("by", signature, true, true);
				doc.Replace("sponsor1", recordx.Godparent1, true, true);
				doc.Replace("sponsor2", recordx.Godparent2, true, true);
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
				pmsutil.InsertTransaction("Baptismal Cert.", "Paying", recordx.RecordID, Convert.ToDouble(pmsutil.GetPrintFee("Baptismal")));
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
