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
using System.Windows.Controls;
using Spire.Pdf.Graphics;
using System.Drawing;
using Spire.Pdf.Tables;
using System.Reflection;
using Spire.Pdf;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class ManageEventsWindow : ChildWindow
	{
		//MYSQL Related Stuff
		private MySqlConnection conn;
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private ObservableCollection<EventsItem> _events;
		private ObservableCollection<EventsItem> _events_final;

		private DateTime _dt;
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ManageEventsWindow(DateTime dt)
		{
			_dt = dt;
			pmsutil = new PMSUtil();
			InitializeComponent();

			SelectedDate.Content = "Selected Date: " + dt.ToString("MMM dd, yyyy");

			SyncEvent(dt);

			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;
		}
		private void SyncEvent(DateTime dt) {
			_events = new ObservableCollection<EventsItem>();
			_events_final = new ObservableCollection<EventsItem>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;

			ObservableCollection<EventsItem> events = new ObservableCollection<EventsItem>();

			dbman = new DBConnectionManager();

			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date = @sdate;";
						cmd.Parameters.AddWithValue("@sdate", dt.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							string status = "null";

							while (db_reader.Read())
							{
								if (IsCustom(db_reader.GetString("appointment_type")) == true)
								{
									if (DateTime.Parse(db_reader.GetString("appointment_date")) < DateTime.Now)
									{
										status = "Finished";
									}
									else if (DateTime.Parse(db_reader.GetString("appointment_date")) == DateTime.Now) {
										status = "Pending";
									}
									else 
									{
										status = "Unfinished";
									}
								}
								else
								{
									if (DateTime.Parse(db_reader.GetString("appointment_date")) < DateTime.Now)
									{
										status = "Finished";
									}
									else if (DateTime.Parse(db_reader.GetString("appointment_date")) == DateTime.Now)
									{
										status = "Pending";
									}
									else
									{
										status = "Unfinished";
									}
								}
								events.Add(new EventsItem()
								{
									Date = DateTime.Parse(db_reader.GetString("appointment_date")).ToString("MMM dd, yyyy"),
									Time = DateTime.Parse(db_reader.GetString("appointment_time")).ToString("h:mm tt"),
									Type = GetAType(db_reader.GetString("appointment_type")),
									Status = status,
									Sponsor = db_reader.GetString("requested_by"),
									Info = db_reader.GetString("remarks"),
									Priest = GetPriest(db_reader.GetString("assigned_priest")),
									Page = page
								});
								count++;
								if (count == itemsPerPage)
								{
									page++;
									count = 0;
								}
							}
							foreach (var cur in _events)
							{
								if (cur.Page == CurrentPage.Value)
								{
									_events_final.Add(new EventsItem()
									{
										Date = cur.Date,
										Time = cur.Time,
										Type = cur.Type,
										Status = cur.Status,
										Sponsor = cur.Sponsor,
										Info = cur.Info,
										Priest = cur.Priest,
										Page = cur.Page
									});
								}
							}
							EventsHolder.Items.Refresh();
							EventsHolder.ItemsSource = _events_final;
							EventsHolder.Items.Refresh();
							CurrentPage.Maximum = page;
						}
					}
				}
			}
			EventsHolder.ItemsSource = events;
		}
		private string GetPriest(string pid)
		{
			string ret = "";
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT priest_name FROM residing_priests WHERE priest_id = @pid LIMIT 1;";
				cmd.Parameters.AddWithValue("@pid", pid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("priest_name");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = "";
			}
			return ret;
		}
		private string GetAType(string tid) {
			string ret = "";
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT appointment_type FROM appointment_types WHERE type_id = @tid;";
				cmd.Parameters.AddWithValue("@tid", tid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("appointment_type");
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{
				ret = "";
			}
			return ret;
		}
		private bool IsCustom(string tid) {
			bool ret = false;
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT custom FROM appointment_types WHERE type_id = @tid;";
				cmd.Parameters.AddWithValue("@tid", tid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetInt32("custom") == 1)
					{
						ret = false;
					}
					else {
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
		/// Interaction logic for the AddRegConfirm button. Gathers and prepares the data
		/// for database insertion.
		/// </summary>
		private async void ConfirmPayment_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddAppointmentWindow());
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncEvent(_dt);
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncEvent(_dt);
		}

		private void GenerateReport_Click(object sender, RoutedEventArgs e)
		{
			if (ReportType.SelectedIndex == 0) {
				string[] dt = pmsutil.GetServerDateTime().Split(null);
				DateTime cDate = Convert.ToDateTime(dt[0]);
				DateTime cTime = DateTime.Parse(dt[1] + " " + dt[2]);

				PdfDocument pdfDoc = new PdfDocument();

				PdfPageBase page = pdfDoc.Pages.Add();
				var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("PMS.Assets.st_raphael_logo_dark.png");
				PdfImage logo = PdfImage.FromStream(stream);
				float _width = 200;
				float height = 80;
				float x = (page.Canvas.ClientSize.Width - _width) / 2;
				page.Canvas.DrawImage(logo, 0, -25, _width, height);

				page.Canvas.DrawString("Peñaranda St, Legazpi Port District",
				new PdfFont(PdfFontFamily.TimesRoman, 13f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				245, 0);

				page.Canvas.DrawString("Legazpi City, Albay",
				new PdfFont(PdfFontFamily.TimesRoman, 13f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				280, 20);

				page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 49), new PointF(530, 49));

				page.Canvas.DrawString("PMS Scheduling Module Report",
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				350, 52);

				page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				10, 52);

				page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 70), new PointF(530, 70));

				page.Canvas.DrawString(cDate.ToString("MMMM dd, yyyy").ToUpper() + " (" + cDate.ToString("dddd").ToUpper() + ")",
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				10, 90);

				PdfTable table = new PdfTable();
				table.Style.CellPadding = 2;
				table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Times New Roman", 11f));

				table.Style.AlternateStyle = new PdfCellStyle();
				table.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font("Times New Roman", 11f));

				table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
				table.Style.HeaderStyle.Font = new PdfFont(PdfFontFamily.TimesRoman, 13f);
				table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

				table.Style.ShowHeader = true;

				table.DataSourceType = PdfTableDataSourceType.TableDirect;
				table.DataSource = GenerateList();
				//Set the width of column  
				float width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1);
				table.Columns[0].Width = width * 0.24f * width;
				table.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Columns[1].Width = width * 0.21f * width;
				table.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Columns[2].Width = width * 0.24f * width;
				table.Columns[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Columns[3].Width = width * 0.24f * width;
				table.Columns[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Draw(page, new PointF(10, 120));

				//save
				pdfDoc.SaveToFile(@"..\..\gen_list.pdf");
				//launch the pdf document
				System.Diagnostics.Process.Start(@"..\..\gen_list.pdf");
			}
			else {
				string[] dt = pmsutil.GetServerDateTime().Split(null);
				DateTime cDate = Convert.ToDateTime(dt[0]);
				DateTime cTime = DateTime.Parse(dt[1] + " " + dt[2]);

				PdfDocument pdfDoc = new PdfDocument();

				PdfPageBase page = pdfDoc.Pages.Add();
				var stream = this.GetType().GetTypeInfo().Assembly.GetManifestResourceStream("PMS.Assets.st_raphael_logo_dark.png");
				PdfImage logo = PdfImage.FromStream(stream);
				float _width = 200;
				float height = 80;
				float x = (page.Canvas.ClientSize.Width - _width) / 2;
				page.Canvas.DrawImage(logo, 0, -25, _width, height);

				page.Canvas.DrawString("Peñaranda St, Legazpi Port District",
				new PdfFont(PdfFontFamily.TimesRoman, 13f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				245, 0);

				page.Canvas.DrawString("Legazpi City, Albay",
				new PdfFont(PdfFontFamily.TimesRoman, 13f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				280, 20);

				page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 49), new PointF(530, 49));

				page.Canvas.DrawString("PMS Scheduling Module Report",
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				350, 52);

				page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				10, 52);

				page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 70), new PointF(530, 70));

				page.Canvas.DrawString(cDate.ToString("MMMM dd, yyyy").ToUpper() + " (" + cDate.ToString("dddd").ToUpper() + ")",
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(System.Drawing.Color.Black),
				10, 90);

				PdfTable table = new PdfTable();
				table.Style.CellPadding = 2;
				table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Times New Roman", 11f));

				table.Style.AlternateStyle = new PdfCellStyle();
				table.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font("Times New Roman", 11f));

				table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
				table.Style.HeaderStyle.Font = new PdfFont(PdfFontFamily.TimesRoman, 13f);
				table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

				table.Style.ShowHeader = true;

				table.DataSourceType = PdfTableDataSourceType.TableDirect;
				table.DataSource = GenerateList2();
				//Set the width of column  
				float width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1);
				table.Columns[0].Width = width * 0.24f * width;
				table.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Columns[1].Width = width * 0.21f * width;
				table.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Columns[2].Width = width * 0.24f * width;
				table.Columns[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Columns[3].Width = width * 0.24f * width;
				table.Columns[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
				table.Draw(page, new PointF(10, 120));

				//save
				pdfDoc.SaveToFile(@"..\..\gen_list.pdf");
				//launch the pdf document
				System.Diagnostics.Process.Start(@"..\..\gen_list.pdf");
			}
		}
		private DataTable GenerateList()
		{
			DataTable dtNames = new DataTable();
			dtNames.Columns.Add("Time", typeof(string));
			dtNames.Columns.Add("Type", typeof(string));
			dtNames.Columns.Add("Sponsor", typeof(string));
			dtNames.Columns.Add("Additional Info", typeof(string));

			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Year
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date = @date ORDER BY appointment_time ASC;";
						cmd.Parameters.AddWithValue("@date", _dt.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (IsCustom(db_reader.GetString("appointment_type")) == false) {
									dtNames.Rows.Add(DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("remarks"));
								}
							}
						}
					}
				}
			}
			return dtNames;
		}
		private DataTable GenerateList2()
		{
			DataTable dtNames = new DataTable();
			dtNames.Columns.Add("Time", typeof(string));
			dtNames.Columns.Add("Type", typeof(string));
			dtNames.Columns.Add("Sponsor", typeof(string));
			dtNames.Columns.Add("Additional Info", typeof(string));

			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Year
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date = @date ORDER BY appointment_time ASC;";
						cmd.Parameters.AddWithValue("@date", _dt.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								if (IsCustom(db_reader.GetString("appointment_type")) == true)
								{
									dtNames.Rows.Add(DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("remarks"));
								}
							}
						}
					}
				}
			}
			return dtNames;
		}
	}
}
