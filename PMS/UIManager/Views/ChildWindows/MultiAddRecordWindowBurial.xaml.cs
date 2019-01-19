using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using Microsoft.Win32;
using MySql.Data.MySqlClient;
using PMS.UIComponents;
using PMS.UIManager.Views.ChildViews;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PMS.UIManager.Views.ChildWindows
{

	/// <summary>
	/// Interaction logic for AddAccountWindow.xaml
	/// </summary>
	public partial class MultiAddRecordWindowBurial : ChildWindow
	{
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private int _statcode = 0;

		ObservableCollection<MRecordEntryBurial> test1;

		private int _bookNum;
		private ViewRecordEntries _vre;

		public MultiAddRecordWindowBurial(ViewRecordEntries vre, int bookNum)
        {
			_vre = vre;
			InitializeComponent();
			_bookNum = bookNum;
			PageNum.Value = vre.Page.Value;

			test1 = new ObservableCollection<MRecordEntryBurial>();

			RecordItemsHolder.ItemsSource = test1;

			this.DataContext = this;
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
		private void DoWork(object sender, DoWorkEventArgs e)
		{
			System.Collections.IList items = RecordItemsHolder.Items;
			for (int i = 0; i < items.Count - 1; i++)
			{
				MRecordEntryBurial recordx = (MRecordEntryBurial)items[i];

				bool proceed = true;
				App.Current.Dispatcher.Invoke((Action)delegate // <--- HERE
				{
					if (string.IsNullOrWhiteSpace(recordx.EntryNumber.ToString()) || recordx.EntryNumber < 1)
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.FullName))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Status))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (recordx.Age < 0)
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.DeathDate))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.BurialDate))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.PlaceOfInterment))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Parent1) || string.IsNullOrWhiteSpace(recordx.Residence1))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.CauseOfDeath))
					{
						ValidatorMsg.Visibility = Visibility.Visible;
						ValidatorIcon.Visibility = Visibility.Visible;
						proceed = false;
					}
					if (string.IsNullOrWhiteSpace(recordx.Sacrament))
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
								MySqlCommand cmd = dbman.DBConnect().CreateCommand();
								cmd.CommandText =
									"INSERT INTO records(record_id, book_number, page_number, entry_number, record_date, recordholder_fullname, parent1_fullname, parent2_fullname)" +
									"VALUES(@record_id, @book_number, @page_number, @entry_number, @record_date, @recordholder_fullname, @parent1_fullname, @parent2_fullname)";
								cmd.Prepare();
								cmd.Parameters.AddWithValue("@record_id", recID);
								cmd.Parameters.AddWithValue("@book_number", _bookNum);
								cmd.Parameters.AddWithValue("@page_number", PageNum.Value);
								cmd.Parameters.AddWithValue("@entry_number", recordx.EntryNumber);
								cmd.Parameters.AddWithValue("@record_date", DateTime.Parse(recordx.DeathDate).ToString("yyyy-MM-dd"));
								cmd.Parameters.AddWithValue("@recordholder_fullname", recordx.FullName);
								cmd.Parameters.AddWithValue("@parent1_fullname", recordx.Parent1);
								cmd.Parameters.AddWithValue("@parent2_fullname", recordx.Parent2);
								int stat_code = cmd.ExecuteNonQuery();
								dbman.DBClose();
								//Phase 2
								cmd = conn.CreateCommand();
								cmd.CommandText =
									"INSERT INTO burial_records(record_id, burial_date, age, status, residence, residence2, sacrament, cause_of_death, place_of_interment, stipend, minister, remarks)" +
									"VALUES(@record_id, @burial_date, @age, @status, @residence, @residence2, @sacrament, @cause_of_death, @place_of_interment, @stipend, @minister, @remarks)";
								cmd.Prepare();
								cmd.Parameters.AddWithValue("@record_id", recID);
								cmd.Parameters.AddWithValue("@burial_date", DateTime.Parse(recordx.DeathDate).ToString("yyyy-MM-dd"));
								cmd.Parameters.AddWithValue("@age", recordx.Age);
								cmd.Parameters.AddWithValue("@status", recordx.Status);
								cmd.Parameters.AddWithValue("@residence", recordx.Residence1);
								cmd.Parameters.AddWithValue("@residence2", recordx.Residence2);
								cmd.Parameters.AddWithValue("@sacrament", recordx.Sacrament);
								cmd.Parameters.AddWithValue("@cause_of_death", recordx.CauseOfDeath);
								cmd.Parameters.AddWithValue("@place_of_interment", recordx.PlaceOfInterment);
								cmd.Parameters.AddWithValue("@stipend", Convert.ToDouble(string.Format("{0:N3}", recordx.Stipend)));
								cmd.Parameters.AddWithValue("@minister", recordx.Minister);
								cmd.Parameters.AddWithValue("@remarks", recordx.Remarks);
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
								if (!string.IsNullOrWhiteSpace(recordx.Block))
								{
									block = recordx.Block;
								}
								if (!string.IsNullOrWhiteSpace(recordx.Lot))
								{
									lot = recordx.Lot;
								}
								if (!string.IsNullOrWhiteSpace(recordx.Plot))
								{
									plot = recordx.Plot;
								}
								if (!string.IsNullOrWhiteSpace(recordx.RConNum))
								{
									rconnum = recordx.RConNum;
								}
								if (!string.IsNullOrWhiteSpace(recordx.ImageURI))
								{
									FileStream fs = new FileStream(recordx.ImageURI, FileMode.Open, FileAccess.Read);
									BinaryReader br = new BinaryReader(fs);
									ImageData = br.ReadBytes((int)fs.Length);
									br.Close();
									fs.Close();
								}
								else
								{
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
				else
				{

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
				TextBox tb = (TextBox)sender;
				tb.Text = op.FileName;
			}
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
			nud.Value = Convert.ToDouble(string.Format("{0:N3}", pmsutil.GetPrintFee("Burial")));
		}
	}
}
