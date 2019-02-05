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
	public partial class BatchPrintWindow3 : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private System.Collections.IList _items;

		private ObservableCollection<RecordEntryBurial> records;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public BatchPrintWindow3(System.Collections.IList items)
		{
			_items = items;
			pmsutil = new PMSUtil();
			InitializeComponent();
			GetResidingPriests();
			PrintingFee.Value = Convert.ToDouble(pmsutil.GetPrintFee("Burial"));
			records = new ObservableCollection<RecordEntryBurial>();

			for (int i = 0; i < items.Count; i++)
			{
				RecordEntryBurial recordx = (RecordEntryBurial)items[i];
				Console.WriteLine(recordx.RecordID);
				records.Add(new RecordEntryBurial()
				{
					RecordID = recordx.RecordID,
					EntryNumber = recordx.EntryNumber,
					DeathYear = recordx.DeathYear,
					DeathDate = recordx.DeathDate,
					BurialYear = recordx.BurialYear,
					BurialDate = recordx.BurialDate,
					FullName = recordx.FullName,
					Age = Convert.ToInt32(recordx.Age),
					Status = recordx.Status,
					Residence1 = recordx.Residence2,
					Residence2 = recordx.Residence1,
					Sacrament = recordx.Sacrament,
					CauseOfDeath = recordx.CauseOfDeath,
					PlaceOfInterment = recordx.PlaceOfInterment,
					Parent1 = recordx.Parent1,
					Parent2 = recordx.Parent2,
					Stipend = Convert.ToDouble(recordx.Stipend),
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
				RecordEntryBurial recordx = (RecordEntryBurial)_items[i];

				string[] bspl = DateTime.Parse(recordx.BurialDate + "," + recordx.BurialYear).ToString("MM/dd/yyyy").Split('/');
				string bmon = PrepMonth(int.Parse(bspl[0]));

				string[] dspl = DateTime.Parse(recordx.DeathDate + "," + recordx.DeathYear).ToString("MM/dd/yyyy").Split('/');
				string dmon = PrepMonth(int.Parse(dspl[0]));

				Document doc = new Document();
				doc.LoadFromFile("Data\\temp_death.docx");
				doc.Replace("name", recordx.FullName, true, true);
				doc.Replace("age", Convert.ToString(recordx.Age), true, true);
				doc.Replace("nationality", "Filipino", true, true);
				doc.Replace("residence", recordx.Residence1, true, true);
				doc.Replace("civil", recordx.Status, true, true);

				if (string.IsNullOrEmpty(recordx.Parent2))
				{
					doc.Replace("father", " ", true, true);
					doc.Replace("mother", " ", true, true);
					doc.Replace("spouse", recordx.Parent1, true, true);
				}
				else {
					doc.Replace("father", recordx.Parent1, true, true);
					doc.Replace("mother", recordx.Parent2, true, true);
					doc.Replace("spouse", " ", true, true);
				}
				doc.Replace("date_of_birth", bmon + " " + bspl[1] + ", " + bspl[2], true, true);
				doc.Replace("cause_of_death", recordx.CauseOfDeath, true, true);
				doc.Replace("date_of_burial", dmon + " " + dspl[1] + ", " + dspl[2], true, true);
				doc.Replace("place_of_burial", recordx.PlaceOfInterment, true, true);
				doc.Replace("priest", recordx.Minister, true, true);
				doc.Replace("sign", signature, true, true);
				doc.Replace("no", recordx.EntryNumber.ToString(), true, true);
				doc.Replace("page", GetPNum(recordx.RecordID).ToString(), true, true);
				string[] date = DateTime.Now.ToString("MMMM,d,yyyy").Split(','); ;
				doc.Replace("month", date[0], true, true);
				doc.Replace("day", date[1], true, true);
				doc.SaveToFile("Data\\print-" + i + ".docx", FileFormat.Docx);

				//Load Document
				Document document = new Document();
				document.LoadFromFile(@"Data\\print-" + i + ".docx");

				//Convert Word to PDF
				document.SaveToFile("Output\\print_file-" + i + ".pdf", FileFormat.PDF);

				App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
				{
					if (SkipPreview.IsChecked == true)
					{
						Spire.Pdf.PdfDocument docx = new Spire.Pdf.PdfDocument();
						docx.LoadFromFile(@"Output\\print_file-" + i + ".pdf");
						docx.PrintDocument.Print();
					}
					else
					{
						System.Diagnostics.Process.Start("Output\\print_file-" + i + ".pdf");
					}


					//Reference
					string tmp = pmsutil.LogRecord(recordx.RecordID, "LOGC-03");
				});
				pmsutil.InsertTransaction("Burial Cert.", "Paying", recordx.RecordID, Convert.ToDouble(pmsutil.GetPrintFee("Burial")));

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
