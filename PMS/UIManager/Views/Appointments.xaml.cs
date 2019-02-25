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

			DateTime date = DateTime.Now;
			var min = new DateTime(date.Year, date.Month, 1);
			var max = min.AddMonths(1).AddDays(-1);
			ListMinDate.SelectedDate = min;
			ListMaxDate.SelectedDate = max;

			FetchAptType();
		}
		internal void FetchAptType() {
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM appointment_types;";
					cmd.Prepare();
					using (MySqlDataReader db_reader = cmd.ExecuteReader())
					{
						while (db_reader.Read())
						{
							Label lbl = new Label();
							lbl.HorizontalAlignment = HorizontalAlignment.Stretch;
							var bc = new System.Windows.Media.BrushConverter();
							lbl.Foreground = (System.Windows.Media.Brush)bc.ConvertFrom("#FF3E3E3E");
							lbl.MouseDown += DragTest;
							lbl.Content = new EventTypeItemDraggable(db_reader.GetString("appointment_type"), null);
							AppointmentTypeList.Items.Add(lbl);
						}
					}
				}
			}
		}
		private string GetStatus(string aid)
		{
			string ret = "";
			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT status FROM transactions WHERE target_id = @aid LIMIT 1;";
				cmd.Parameters.AddWithValue("@aid", aid);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					ret = db_reader.GetString("status");
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
		private bool IsCustom(string tid)
		{
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
					else
					{
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
		private async void AddAppointment_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new AddAppointmentWindow2(), this.AppointmentMainGrid);
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
			pmsutil = new PMSUtil();

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

			page.Canvas.DrawLine(new PdfPen(Color.Black), new PointF(1, 49), new PointF(530, 49));

			page.Canvas.DrawString("PMS Scheduling Module Report",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(Color.Black),
			350, 52);

			page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(Color.Black),
			10, 52);

			page.Canvas.DrawString("Type: " + ListType.Text,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			380, 90);

			page.Canvas.DrawLine(new PdfPen(Color.Black), new PointF(1, 70), new PointF(530, 70));

			if (ListType.SelectedIndex == 1) {
				page.Canvas.DrawString(cDate.ToString("MMMM dd, yyyy").ToUpper() + " (" + cDate.ToString("dddd").ToUpper() + ")",
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(Color.Black),
				10, 90);
			}
			else {
				page.Canvas.DrawString(DateTime.Parse(ListMinDate.SelectedDate.ToString()).ToString("MMMM dd, yyyy").ToUpper() + " TO " + DateTime.Parse(ListMaxDate.SelectedDate.ToString()).ToString("MMMM dd, yyyy").ToUpper(),
				new PdfFont(PdfFontFamily.TimesRoman, 12f),
				new PdfSolidBrush(Color.Black),
				10, 90);
			}

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
			table.Columns[4].Width = width * 0.24f * width;
			table.Columns[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			if (ListType.SelectedIndex == 2)
			{
				table.Columns[5].Width = width * 0.24f * width;
				table.Columns[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			}
			table.Draw(page, new PointF(10, 120));

			string fname = "Scheduling_Report-" + ListType.Text + "-" + DateTime.Now.ToString("MMM_dd_yyyy") + ".pdf";
			//save
			pdfDoc.SaveToFile(@"..\..\" + fname);
			//launch the pdf document
			System.Diagnostics.Process.Start(@"..\..\" + fname);
		}
		private DataTable GenerateList()
		{
			DataTable dtNames = new DataTable();

			if (ListType.SelectedIndex == 2) {
				dtNames.Columns.Add("No", typeof(string));
				dtNames.Columns.Add("Time", typeof(string));
				dtNames.Columns.Add("Type", typeof(string));
				dtNames.Columns.Add("Sponsor", typeof(string));
				dtNames.Columns.Add("Venue", typeof(string));
				dtNames.Columns.Add("Remarks", typeof(string));

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
							cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date > @min AND appointment_date < @max ORDER BY appointment_date ASC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(ListMinDate.SelectedDate.ToString()));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(ListMaxDate.SelectedDate.ToString()));
							cmd.Prepare();
							using (MySqlDataReader db_reader = cmd.ExecuteReader())
							{
								int count = 1;
								while (db_reader.Read())
								{
									if (ListType.SelectedIndex == 0)
									{
										dtNames.Rows.Add(count, DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("venue"), db_reader.GetString("remarks"));
									}
									else if (ListType.SelectedIndex == 1)
									{
										if (IsCustom(db_reader.GetString("appointment_type")) == false)
										{
											dtNames.Rows.Add(count, DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("venue"), db_reader.GetString("remarks"));
										}
									}
									else
									{
										if (IsCustom(db_reader.GetString("appointment_type")) == true)
										{
											dtNames.Rows.Add(count, DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("venue"), db_reader.GetString("remarks"));
										}
									}
									count++;
								}
							}
						}
					}
				}
			}
			else {
				dtNames.Columns.Add("No", typeof(string));
				dtNames.Columns.Add("Time", typeof(string));
				dtNames.Columns.Add("Type", typeof(string));
				dtNames.Columns.Add("Sponsor", typeof(string));
				dtNames.Columns.Add("Remarks", typeof(string));

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
							cmd.CommandText = "SELECT * FROM appointments WHERE appointment_date > @min AND appointment_date < @max ORDER BY appointment_date ASC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(ListMinDate.SelectedDate.ToString()));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(ListMaxDate.SelectedDate.ToString()));
							cmd.Prepare();
							using (MySqlDataReader db_reader = cmd.ExecuteReader())
							{
								int count = 1;
								while (db_reader.Read())
								{
									if (ListType.SelectedIndex == 0)
									{
										dtNames.Rows.Add(count, DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("remarks"));
									}
									else if (ListType.SelectedIndex == 1)
									{
										if (IsCustom(db_reader.GetString("appointment_type")) == false)
										{
											dtNames.Rows.Add(count, DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("remarks"));
										}
									}
									else
									{
										if (IsCustom(db_reader.GetString("appointment_type")) == true)
										{
											dtNames.Rows.Add(count, DateTime.Parse(db_reader.GetString("appointment_time")).ToString("hh:mm tt"), GetAType(db_reader.GetString("appointment_type")), db_reader.GetString("requested_by"), db_reader.GetString("remarks"));
										}
									}
									count++;
								}
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
		private async void ActionsHelp_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Actions Help", "You can add appointments and schedule mass in this menu. The system will list available priests on the given date and time.");
		}

		private void DragTest(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			Label _lbl = sender as Label;
			// Initialize the drag & drop operation
			DataObject dragData = new DataObject("myFormat", _lbl);
			DragDrop.DoDragDrop(_lbl, dragData, DragDropEffects.Move);
		}
	}
}
