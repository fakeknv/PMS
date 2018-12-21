using MySql.Data.MySqlClient;
using PMS.UIComponents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using System.Drawing;
using Spire.Pdf.Tables;

namespace PMS.UIManager.Views
{

	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Reports : UserControl
	{
		private DBConnectionManager dbman;

		private PMSUtil pmsutil;

		public class Entries
		{
			public string ReceiptNo { get; set; }
			public string Type { get; set; }
			public string Status { get; set; }
			public float Fee { get; set; }
			public string DPlaced { get; set; }
			public string DCompleted { get; set; }
			public string PlacedBy { get; set; }
			public string CompletedBy { get; set; }
		}
		public Reports()
		{
			InitializeComponent();
		}

		private void SyncChanges(object sender, SelectionChangedEventArgs e)
		{
			pmsutil = new PMSUtil();
			List<Entries> transactions = new List<Entries>();

			dbman = new DBConnectionManager();
			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				if (ReportRange.SelectedIndex == 0)
				{
					ReportLabel.Content = "Showing This Week's Transactions";
				}
				else if (ReportRange.SelectedIndex == 1)
				{
					ReportLabel.Content = "Showing This Month's Transactions";

					DateTime date = DateTime.Today;
					var fDay = new DateTime(date.Year, date.Month, 1);
					var lDay = fDay.AddMonths(1).AddDays(-1);

					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					if (StatusType.SelectedIndex == 0) {
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.';";
					} else if (StatusType.SelectedIndex == 1) {
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND status = 'Paying' AND (type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.');";
					} else if (StatusType.SelectedIndex == 2) {
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND status = 'Finished' AND (type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.');";
					} else if (StatusType.SelectedIndex == 3)
					{
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND status = 'Cancelled' AND (type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.');";
					}

					cmd.Parameters.AddWithValue("@fdate", fDay);
					cmd.Parameters.AddWithValue("@ldate", lDay);


					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string completed_by = null;
						string cdate = null;
						if (db_reader.IsDBNull(8))
						{
							completed_by = "---";
						}
						else {

							completed_by = pmsutil.GetFullName(db_reader.GetString("completed_by"));
						}
						if (db_reader.IsDBNull(5))
						{
							cdate = "---";
						}
						else
						{

							cdate = Convert.ToDateTime(db_reader.GetString("completion_date")).ToString("MMM dd, yyyy");
						}
						transactions.Add(new Entries() { ReceiptNo = db_reader.GetString("or_number"), Type = db_reader.GetString("type"), Status = db_reader.GetString("status"), Fee = db_reader.GetFloat("fee"), DPlaced = Convert.ToDateTime(db_reader.GetString("tran_date")).ToString("MMM dd, yyyy"), DCompleted = cdate, PlacedBy = pmsutil.GetFullName(db_reader.GetString("placed_by")), CompletedBy = completed_by });
					}

					//close Connection
					dbman.DBClose();
					BillingDG.ItemsSource = transactions;
				}
				else if (ReportRange.SelectedIndex == 2)
				{
					ReportLabel.Content = "Showing This Year's Transactions";

					int year = DateTime.Now.Year;
					DateTime fDay = new DateTime(year, 1, 1);
					DateTime lDay = new DateTime(year, 12, 31);

					MySqlCommand cmd = dbman.DBConnect().CreateCommand();
					if (StatusType.SelectedIndex == 0)
					{
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.';";
					}
					else if (StatusType.SelectedIndex == 1)
					{
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND status = 'Paying' AND (type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.');";
					}
					else if (StatusType.SelectedIndex == 2)
					{
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND status = 'Finished' AND (type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.');";
					}
					else if (StatusType.SelectedIndex == 3)
					{
						cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @fdate AND tran_date < @ldate AND status = 'Cancelled' AND (type = 'Burial Cert.' OR type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Marriage Cert.');";
					}

					cmd.Parameters.AddWithValue("@fdate", fDay);
					cmd.Parameters.AddWithValue("@ldate", lDay);


					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string completed_by = null;
						string cdate = null;
						if (db_reader.IsDBNull(8))
						{
							completed_by = "---";
						}
						else
						{

							completed_by = pmsutil.GetFullName(db_reader.GetString("completed_by"));
						}
						if (db_reader.IsDBNull(5))
						{
							cdate = "---";
						}
						else
						{

							cdate = Convert.ToDateTime(db_reader.GetString("completion_date")).ToString("MMM dd, yyyy");
						}
						transactions.Add(new Entries() { ReceiptNo = db_reader.GetString("or_number"), Type = db_reader.GetString("type"), Status = db_reader.GetString("status"), Fee = db_reader.GetFloat("fee"), DPlaced = Convert.ToDateTime(db_reader.GetString("tran_date")).ToString("MMM dd, yyyy"), DCompleted = cdate, PlacedBy = pmsutil.GetFullName(db_reader.GetString("placed_by")), CompletedBy = completed_by });
					}

					//close Connection
					dbman.DBClose();
					BillingDG.ItemsSource = transactions;
				}
			}
			else
			{

			}
		}

		private void GenReport1(object sender, RoutedEventArgs e)
		{
			//create a new pdf document
			PdfDocument pdfDoc = new PdfDocument();

			PdfPageBase page = pdfDoc.Pages.Add();
			page.Canvas.DrawString("St. Raphael Parish of Legazpi City",
			new PdfFont(PdfFontFamily.Helvetica, 13f),
			new PdfSolidBrush(Color.Black),
			160, 10);

			page.Canvas.DrawString("Monthly Transactions Report",
			new PdfFont(PdfFontFamily.Helvetica, 12f),
			new PdfSolidBrush(Color.Black),
			180, 28);

			page.Canvas.DrawString("Generated On: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt"),
			new PdfFont(PdfFontFamily.Helvetica, 12f),
			new PdfSolidBrush(Color.Black),
			155, 45);

			page.Canvas.DrawString("Total Transactions: 1234",
			new PdfFont(PdfFontFamily.Helvetica, 12f),
			new PdfSolidBrush(Color.Black),
			10, 80);

			page.Canvas.DrawString("Total Amount: 123456",
			new PdfFont(PdfFontFamily.Helvetica, 12f),
			new PdfSolidBrush(Color.Black),
			10, 100);

			PdfTable table = new PdfTable();
			table.Style.CellPadding = 2;
			table.Style.DefaultStyle.BackgroundBrush = PdfBrushes.SkyBlue;
			table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Arial", 10f));

			table.Style.AlternateStyle = new PdfCellStyle();
			table.Style.AlternateStyle.BackgroundBrush = PdfBrushes.LightYellow;
			table.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font("Arial", 10f));

			table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
			table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
			table.Style.HeaderStyle.Font = new PdfFont(PdfFontFamily.Helvetica, 13f);
			table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

			table.Style.ShowHeader = true;

			table.DataSourceType = PdfTableDataSourceType.TableDirect;

			table.DataSource = GenerateList();

			//Set the width of column  
			float width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1);
			table.Columns[0].Width = width * 0.24f * width;
			table.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[1].Width = width * 0.21f * width;
			table.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[2].Width = width * 0.24f * width;
			table.Columns[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[3].Width = width * 0.24f * width;
			table.Columns[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[4].Width = width * 0.24f * width;
			table.Columns[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[5].Width = width * 0.24f * width;
			table.Columns[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[6].Width = width * 0.24f * width;
			table.Columns[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Draw(page, new PointF(0, 120));

			//save
			pdfDoc.SaveToFile(@"..\..\sample.pdf");

			//launch the pdf document
			System.Diagnostics.Process.Start(@"..\..\sample.pdf");
		}
		private DataTable GenerateList()
		{
			DataTable dtNames = new DataTable();
			dtNames.Columns.Add("OR Number", typeof(int));
			dtNames.Columns.Add("Type", typeof(string));
			dtNames.Columns.Add("Status", typeof(string));
			dtNames.Columns.Add("Fee", typeof(string));
			dtNames.Columns.Add("Date Placed", typeof(string));
			dtNames.Columns.Add("Date Completed", typeof(string));
			dtNames.Columns.Add("Completed By", typeof(string));

			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");
			dtNames.Rows.Add(12345, "Burial Cert.", "Finished", "100.00", "2018-11-29", "2018-11-29", "John w. Doe");

			return dtNames;
		}
	}
}