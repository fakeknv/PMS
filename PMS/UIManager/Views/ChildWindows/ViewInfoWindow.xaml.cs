using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using Spire.Pdf;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PMS.UIManager.Views.ChildWindows
{
    /// <summary>
    /// Interaction logic for AddAccountWindow.xaml
    /// </summary>
    public partial class ViewInfoWindow : ChildWindow
    {
		private MySqlConnection conn;
		private DBConnectionManager dbman;
		private PMSUtil pmsutil;

		private string _rid;

		//Report
		private string _name, _age, _residence, _parent1, _parent2, _deathDate, _burialDate, _causeOfDeath, _sacrament, _interment, _block, _lot, _plot;

        public ViewInfoWindow(string rid)
        {
			_rid = rid;
            InitializeComponent();
			pmsutil = new PMSUtil();

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM records, burial_records, burial_directory WHERE records.record_id = burial_records.record_id AND burial_records.record_id = burial_directory.record_id AND burial_directory.record_id = @rid LIMIT 1;";
					cmd.Parameters.AddWithValue("@rid", rid);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						_name = db_reader.GetString("recordholder_fullname");
						_age = db_reader.GetString("age");
						_residence = db_reader.GetString("residence");
						_parent1 = db_reader.GetString("parent1_fullname");
						_parent2 = db_reader.GetString("parent2_fullname");
						_deathDate = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMMM dd, yyyy");
						_burialDate = DateTime.Parse(db_reader.GetString("burial_date")).ToString("MMMM dd, yyyy");
						_causeOfDeath = db_reader.GetString("cause_of_death");
						_sacrament = db_reader.GetString("sacrament");
						_interment = db_reader.GetString("place_of_interment");

						NameHolder.Content = db_reader.GetString("recordholder_fullname").ToUpper();
						AgeHolder.Content = db_reader.GetInt32("age");
						ResidenceHolder.Content = db_reader.GetString("residence");
						Parent1Holder.Content = db_reader.GetString("parent1_fullname");
						Parent2Holder.Content = db_reader.GetString("parent2_fullname");
						DateOfDeathHolder.Content = DateTime.Parse(db_reader.GetString("record_date")).ToString("MMMM dd, yyyy");
						BurialDateHolder.Content = DateTime.Parse(db_reader.GetString("burial_date")).ToString("MMMM dd, yyyy");
						CauseOfDeathHolder.Content = db_reader.GetString("cause_of_death");
						SacramentHolder.Content = db_reader.GetString("sacrament");
						PlaceOfIntermentHolder.Text = db_reader.GetString("place_of_interment");

					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT gravestone FROM burial_directory WHERE burial_directory.record_id = @rid LIMIT 1;";
					cmd.Parameters.AddWithValue("@rid", rid);
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						var tmpy = db_reader.GetOrdinal("gravestone");
						if (db_reader.IsDBNull(tmpy))
						{
							//Load default img
						}
						else {
							byte[] data = (byte[])db_reader[0];
							using (System.IO.MemoryStream ms = new System.IO.MemoryStream(data))
							{
								var imageSource = new BitmapImage();
								imageSource.BeginInit();
								imageSource.StreamSource = ms;
								imageSource.CacheOption = BitmapCacheOption.OnLoad;
								imageSource.EndInit();

								// Assign the Source property of your image
								GravePicture.Source = imageSource;
							}
						}
					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT block, lot, plot FROM burial_directory WHERE burial_directory.record_id = @rid LIMIT 1;";
					cmd.Parameters.AddWithValue("@rid", rid);
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						_block = db_reader.GetString("block");
						_lot = db_reader.GetString("lot");
						_plot = db_reader.GetString("plot");

						BlockHolder.Content = db_reader.GetString("block");
						LotHolder.Content = db_reader.GetString("lot");
						PlotHolder.Content = db_reader.GetString("plot");
					}
					conn.Close();
				}
			}
			GetNearbyGraves();
		}
		internal void GetNearbyGraves() {
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM records, burial_records, burial_directory WHERE records.record_id = burial_records.record_id AND burial_records.record_id = burial_directory.record_id AND burial_directory.block = @block AND burial_directory.lot = @lot;";
					cmd.Parameters.AddWithValue("@block", _block);
					cmd.Parameters.AddWithValue("@lot", _lot);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetString("record_id") != _rid) {
							Nearby.Items.Add(db_reader.GetString("recordholder_fullname"));
						}
					}
				}
			}
		}
		private void Cancel_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
		private async void PrintIndexButton_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			var controller = await metroWindow.ShowProgressAsync("Generating...", "Please wait while the system is generating the report.");
			controller.SetIndeterminate();
			GenReport3Phase2();
			// Close...
			await controller.CloseAsync();
		}
		private async void UpdateInfo_Click(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowChildWindowAsync(new EditDirectoryWindow(_rid));
		}
		internal void GenReport3Phase2()
		{
			string[] dt = pmsutil.GetServerDateTime().Split(null);
			DateTime cDate = Convert.ToDateTime(dt[0]);
			DateTime cTime = DateTime.Parse(dt[1] + " " + dt[2]);

			//create a new pdf document
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

			page.Canvas.DrawString("PMS Index Report",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			350, 52);

			page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 52);

			page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 70), new PointF(530, 70));

			page.Canvas.DrawString(_name.ToUpper(),
			new PdfFont(PdfFontFamily.TimesRoman, 18f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 85);

			//page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(10, 115), new PointF(330, 115));

			page.Canvas.DrawString("Birth Details",
			new PdfFont(PdfFontFamily.TimesRoman, 12f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 125);

			page.Canvas.DrawString("Age: " + _age,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 150);

			page.Canvas.DrawString("Residence: " + _residence,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 170);

			page.Canvas.DrawString("Residence: " + _residence,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 190);

			page.Canvas.DrawString("Parents: " + _parent1 + " & " + _parent2,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 210);

			page.Canvas.DrawString("Burial Details",
			new PdfFont(PdfFontFamily.TimesRoman, 12f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 125);

			page.Canvas.DrawString("Date of Death: " + _deathDate,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 150);

			page.Canvas.DrawString("Burial Date: " + _burialDate,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 170);

			page.Canvas.DrawString("Cause of Death: " + _causeOfDeath,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 190);

			page.Canvas.DrawString("Sacrament: " + _sacrament,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 210);

			page.Canvas.DrawString("Place of Interment: ",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 230);

			page.Canvas.DrawString(_interment,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 250);

			page.Canvas.DrawString("Location: ",
			new PdfFont(PdfFontFamily.TimesRoman, 12f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 280);

			page.Canvas.DrawString("Block: " + _block,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 300);

			page.Canvas.DrawString("Lot: " + _lot,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 320);

			page.Canvas.DrawString("Plot: " + _plot,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			300, 340);

			page.Canvas.DrawString("Near the Location",
			new PdfFont(PdfFontFamily.TimesRoman, 14f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 360);

			DataTable dtNames = new DataTable();
			dtNames.Columns.Add("Name", typeof(string));
			dtNames.Columns.Add("Lot", typeof(string));
			dtNames.Columns.Add("Plot", typeof(string));

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM records, burial_records, burial_directory WHERE records.record_id = burial_records.record_id AND burial_records.record_id = burial_directory.record_id AND burial_directory.block = @block AND burial_directory.lot = @lot;";
					cmd.Parameters.AddWithValue("@block", _block);
					cmd.Parameters.AddWithValue("@lot", _lot);
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetString("record_id") != _rid)
						{
							dtNames.Rows.Add(db_reader.GetString("recordholder_fullname"), db_reader.GetString("lot"), db_reader.GetString("plot"));
						}
					}
				}
			}

			PdfTable table = new PdfTable();
			table.Style.CellPadding = 2;
			//table.Style.DefaultStyle.BackgroundBrush = PdfBrushes.SkyBlue;
			table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Times New Roman", 11f));

			table.Style.AlternateStyle = new PdfCellStyle
			{
				//table.Style.AlternateStyle.BackgroundBrush = PdfBrushes.LightYellow;
				Font = new PdfTrueTypeFont(new Font("Times New Roman", 11f))
			};

			table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
			//table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
			table.Style.HeaderStyle.Font = new PdfFont(PdfFontFamily.TimesRoman, 13f);
			table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

			table.Style.ShowHeader = true;

			table.DataSourceType = PdfTableDataSourceType.TableDirect;
			table.DataSource = dtNames;
			//Set the width of column  
			float width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1);
			table.Columns[0].Width = width * 0.24f * width;
			table.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[1].Width = width * 0.21f * width;
			table.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[2].Width = width * 0.24f * width;
			table.Columns[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Draw(page, new PointF(10, 380));

			//save
			pdfDoc.SaveToFile(@"..\..\index_report.pdf");
			//launch the pdf document
			System.Diagnostics.Process.Start(@"..\..\index_report.pdf");
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The account has been created successfully.");
		}
		private async void MsgFail()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Failed!", "The requested action failed. Please check your input and try again.");
		}
	}
}
