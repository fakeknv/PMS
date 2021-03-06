﻿using MahApps.Metro.Controls;
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
using System.Data.OleDb;
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
	public partial class MultiAddRecordWindowConfirmation : ChildWindow
	{
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private int _statcode = 0;

		ObservableCollection<MRecordEntryConfirmation> test1;

		private int _bookNum;
		private ViewRecordEntries _vre;

		public MultiAddRecordWindowConfirmation(ViewRecordEntries vre, int bookNum)
        {
			_vre = vre;
			InitializeComponent();
			_bookNum = bookNum;
			PageNum.Value = vre.Page.Value;

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
				if (_statcode == 601)
				{
					ValidatorIcon.Visibility = Visibility.Visible;
					ValidatorMsg.Visibility = Visibility.Visible;
				}
				else
				{
					ValidatorIcon.Visibility = Visibility.Hidden;
					ValidatorMsg.Visibility = Visibility.Hidden;

					_vre.Sync(_bookNum);
					MsgSuccess();
					this.Close();
				}
			}
			else
			{
				MsgFail();
			}
		}
		private async void ImportButton_Click(object sender, RoutedEventArgs e)
		{
			String file_path = "";
			OpenFileDialog opfile = new OpenFileDialog();
			opfile.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
			if (opfile.ShowDialog() == true)
			{
				file_path = opfile.FileName;
			}
			try
			{
				var metroWindow = (Application.Current.MainWindow as MetroWindow);
				var controller = await metroWindow.ShowProgressAsync("Importing...", "Please wait while the system is reading the file.");
				controller.SetIndeterminate();

				String Path = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + file_path + ";Extended Properties = \"Excel 12.0 Xml;HDR=YES\"; ";
				OleDbConnection conn = new OleDbConnection(Path);
				String s = "Sheet1";
				OleDbDataAdapter connAd = new OleDbDataAdapter("Select * from [" + s + "$]", conn);
				DataTable dt = new DataTable();
				connAd.Fill(dt);
				datacon.ItemsSource = dt.AsDataView();
				// Close...
				await controller.CloseAsync();
			}
			catch { }
		}
		private void DownloadButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "Excel |*.xlsx";
			if (saveFileDialog.ShowDialog() == true)
				File.Copy(@"Data/test_con.xlsx", saveFileDialog.FileName);
		}
		private void DoWork(object sender, DoWorkEventArgs e)
		{
			try
			{
				DataTable dt = new DataTable();
				dt = ((DataView)datacon.ItemsSource).ToTable();
				//MessageBox.Show(dt.Rows[0][0].ToString());

				for (int i = 0; i < dt.Rows.Count; i++)
				{
					dbman = new DBConnectionManager();
					pmsutil = new PMSUtil();
					using (conn = new MySqlConnection(dbman.GetConnStr()))
					{
						conn.Open();
						if (conn.State == ConnectionState.Open)
						{
							bool doProceed = false;
							//Check inputs
							for (int _tmp = 0; _tmp < 12; _tmp++)
							{
								if (String.IsNullOrEmpty(dt.Rows[i][i].ToString()) == true)
								{
									doProceed = false;
								}
								else
								{
									doProceed = true;
								}
							}
							if (doProceed == true)
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
									cmd.Parameters.AddWithValue("@entry_number", Convert.ToInt32(dt.Rows[i][0].ToString()));
									cmd.Parameters.AddWithValue("@record_date", dt.Rows[i][1].ToString());
									cmd.Parameters.AddWithValue("@recordholder_fullname", dt.Rows[i][2].ToString());
									cmd.Parameters.AddWithValue("@parent1_fullname", dt.Rows[i][7].ToString());
									cmd.Parameters.AddWithValue("@parent2_fullname", dt.Rows[i][8].ToString());
									int stat_code = cmd.ExecuteNonQuery();
									conn.Close();

									conn.Open();
									//Phase 2
									cmd = dbman.DBConnect().CreateCommand();
									cmd.CommandText =
										"INSERT INTO confirmation_records(record_id, age, parochia, province, place_of_baptism, sponsor, sponsor2, stipend, minister, remarks)" +
										"VALUES(@record_id, @age, @parish, @province, @place_of_baptism, @sponsor, @sponsor2, @stipend, @minister, @remarks)";
									cmd.Prepare();
									cmd.Parameters.AddWithValue("@record_id", recID);
									cmd.Parameters.AddWithValue("@age", Convert.ToInt32(dt.Rows[i][3].ToString()));
									cmd.Parameters.AddWithValue("@parish", dt.Rows[i][4].ToString());
									cmd.Parameters.AddWithValue("@province", dt.Rows[i][5].ToString());
									cmd.Parameters.AddWithValue("@place_of_baptism", dt.Rows[i][6].ToString());
									cmd.Parameters.AddWithValue("@sponsor", dt.Rows[i][9].ToString());
									cmd.Parameters.AddWithValue("@sponsor2", dt.Rows[i][10].ToString());
									cmd.Parameters.AddWithValue("@stipend", Convert.ToDouble(dt.Rows[i][11].ToString()));
									cmd.Parameters.AddWithValue("@minister", dt.Rows[i][12].ToString());
									cmd.Parameters.AddWithValue("@remarks", dt.Rows[i][13].ToString());
									stat_code = cmd.ExecuteNonQuery();

									conn.Close();
									string tmp = pmsutil.LogRecord(recID, "LOGC-01");
									_statcode = stat_code;
									//return stat_code;
								});
							}
							else
							{
								_statcode = 601;
							}
						}
						else
						{

						}
					}
				}
				this.Close();
			}
			catch
			{
				
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
		private void SyncFee(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			pmsutil = new PMSUtil();
			NumericUpDown nud = (NumericUpDown)sender;
			nud.Value = Convert.ToDouble(string.Format("{0:N3}", pmsutil.GetPrintFee("Confirmation")));
		}
	}
}
