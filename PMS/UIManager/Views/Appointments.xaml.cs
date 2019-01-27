using PMS.UIComponents;
using System.Windows.Controls;
using MahApps.Metro.SimpleChildWindow;
using System.Windows;
using MahApps.Metro.Controls;
using PMS.UIManager.Views.ChildWindows;
using System;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls.Dialogs;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using Spire.Pdf.Tables;
using System.Reflection;

namespace PMS.UIManager.Views
{
    /// <summary>
    /// Interaction logic for Appointments.xaml
    /// </summary>
    public partial class Appointments : UserControl
    {
		//Required for changing the label from another class.
		internal static Appointments app;

		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private PMSUtil pmsutil;

		internal string Current_Date
		{
			get { return dayActivitiesTitle.Content.ToString(); }
			set { Dispatcher.Invoke(new Action(() => { dayActivitiesTitle.Content = value; })); }
		}

		public Appointments()
        {
			//Required for changing the label from another class.
			app = this;
            InitializeComponent();
			dayActivitiesTitle.Content = DateTime.Now.ToString("MMMM d, yyyy");
			SyncOverview();
		}
		private void SyncOverview() {
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
						cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE appointment_date > @min AND appointment_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								Year.Content = "This Year: " + db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Today
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE appointment_date = @date;";
						cmd.Parameters.AddWithValue("@date", DateTime.Today.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								Day.Content = "This day: " + db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//This Week
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						DayOfWeek day = DateTime.Now.DayOfWeek;
						int days = day - DayOfWeek.Monday;
						DateTime start = DateTime.Now.AddDays(-days);
						DateTime end = start.AddDays(6);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE appointment_date > @min AND appointment_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								Week.Content = "This Week: " + db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//This Month
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, cDate.Month, 1);
						var end = start.AddMonths(1).AddDays(-1);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM appointments WHERE appointment_date > @min AND appointment_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								Month.Content = "This Month: " + db_reader.GetInt32("COUNT(*)");
							}
						}
					}
				}
			}
		}
		private async void AddAppointment_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddAppointmentWindow(), this.AppointmentMainGrid);
		}

		private async void GenerateList(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			var controller = await metroWindow.ShowProgressAsync("Generating...", "Please wait while the system is generating the list of schedules for today.");
			controller.SetIndeterminate();
			GenerateDailyReport();
			// Close...
			await controller.CloseAsync();
		}
		private void GenerateDailyReport() {

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
			new PdfSolidBrush(Color.Black),
			245, 0);

			page.Canvas.DrawString("Legazpi City, Albay",
			new PdfFont(PdfFontFamily.TimesRoman, 13f),
			new PdfSolidBrush(Color.Black),
			280, 20);

			page.Canvas.DrawLine(new PdfPen(Color.Black), new PointF(1,49), new PointF(530,49));

			page.Canvas.DrawString("PMS Scheduling Module Report",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(Color.Black),
			350, 52);

			page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(Color.Black),
			10, 52);

			page.Canvas.DrawLine(new PdfPen(Color.Black), new PointF(1, 70), new PointF(530, 70));

			page.Canvas.DrawString(cDate.ToString("MMMM dd, yyyy").ToUpper() + " (" + cDate.ToString("dddd").ToUpper() + ")",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(Color.Black),
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
			pdfDoc.SaveToFile(@"..\..\daily_list.pdf");
			//launch the pdf document
			System.Diagnostics.Process.Start(@"..\..\daily_list.pdf");
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

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date = @date ORDER BY appointment_time ASC;";
						cmd.Parameters.AddWithValue("@date", cDate.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								//Year.Content = "This Year: " + db_reader.GetInt32("COUNT(*)");
								dtNames.Rows.Add(DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("remarks"));
							}
						}
					}
				}
			}
			return dtNames;
		}
		private string GetAType(string tid)
		{
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
	}
}
