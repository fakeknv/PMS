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
using LiveCharts;
using LiveCharts.Wpf;
using System.Windows.Media;
using LiveCharts.Defaults;

namespace PMS.UIManager.Views
{

	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Reports : UserControl
	{
		private MySqlConnection conn;
		private DBConnectionManager dbman;

		private PMSUtil pmsutil;

		public Func<ChartPoint, string> PointLabel { get; set; }

		public SeriesCollection SeriesCollection { get; set; }
		public string[] Labels { get; set; }
		public Func<double, string> YFormatter { get; set; }

		public Reports()
		{
			InitializeComponent();
			SyncCharts();
			SyncSummary();
			SyncPieTransactions();
			//ReportType.SelectionChanged += UpdateChart;
			//StatusType.SelectionChanged += UpdateChart;
		}
		private void SyncCharts() {
			PointLabel = chartPoint =>
				string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);
			


			SeriesCollection = new SeriesCollection
			{
				new LineSeries
				{
					Title = "Baptismal",
					Values = new ChartValues<double> { CountTransactions("Baptismal Cert.", "Paying", 1), CountTransactions("Baptismal Cert.", "Paying", 2), CountTransactions("Baptismal Cert.", "Paying", 3), CountTransactions("Baptismal Cert.", "Paying", 4), CountTransactions("Baptismal Cert.", "Paying", 5), CountTransactions("Baptismal Cert.", "Paying", 6), CountTransactions("Baptismal Cert.", "Paying", 7), CountTransactions("Baptismal Cert.", "Paying", 8), CountTransactions("Baptismal Cert.", "Paying", 9), CountTransactions("Baptismal Cert.", "Paying", 10), CountTransactions("Baptismal Cert.", "Paying", 11), CountTransactions("Baptismal Cert.", "Paying", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				},
				new LineSeries
				{
					Title = "Confirmation",
					Values = new ChartValues<double> { CountTransactions("Confirmation Cert.", "Paying", 1), CountTransactions("Confirmation Cert.", "Paying", 2), CountTransactions("Confirmation Cert.", "Paying", 3), CountTransactions("Confirmation Cert.", "Paying", 4), CountTransactions("Confirmation Cert.", "Paying", 5), CountTransactions("Confirmation Cert.", "Paying", 6), CountTransactions("Confirmation Cert.", "Paying", 7), CountTransactions("Confirmation Cert.", "Paying", 8), CountTransactions("Confirmation Cert.", "Paying", 9), CountTransactions("Confirmation Cert.", "Paying", 10), CountTransactions("Confirmation Cert.", "Paying", 11), CountTransactions("Confirmation Cert.", "Paying", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				},
				new LineSeries
				{
					Title = "Matrimonial",
					Values = new ChartValues<double> { CountTransactions("Matrimonial Cert.", "Paying", 1), CountTransactions("Matrimonial Cert.", "Paying", 2), CountTransactions("Matrimonial Cert.", "Paying", 3), CountTransactions("Matrimonial Cert.", "Paying", 4), CountTransactions("Matrimonial Cert.", "Paying", 5), CountTransactions("Matrimonial Cert.", "Paying", 6), CountTransactions("Matrimonial Cert.", "Paying", 7), CountTransactions("Matrimonial Cert.", "Paying", 8), CountTransactions("Matrimonial Cert.", "Paying", 9), CountTransactions("Matrimonial Cert.", "Paying", 10), CountTransactions("Matrimonial Cert.", "Paying", 11), CountTransactions("Matrimonial Cert.", "Paying", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				},
				new LineSeries
				{
					Title = "Burial",
					Values = new ChartValues<double> { CountTransactions("Burial Cert.", "Paying", 1), CountTransactions("Burial Cert.", "Paying", 2), CountTransactions("Burial Cert.", "Paying", 3), CountTransactions("Burial Cert.", "Paying", 4), CountTransactions("Burial Cert.", "Paying", 5), CountTransactions("Burial Cert.", "Paying", 6), CountTransactions("Burial Cert.", "Paying", 7), CountTransactions("Burial Cert.", "Paying", 8), CountTransactions("Burial Cert.", "Paying", 9), CountTransactions("Burial Cert.", "Paying", 10), CountTransactions("Burial Cert.", "Paying", 11), CountTransactions("Burial Cert.", "Paying", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				}
			};

			Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
			YFormatter = value => value.ToString();


			DataContext = this;
		}
		private void SyncSummary() {
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Total Transactions
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions;";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								TotalTransactions.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Total Transactions Amount
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions;";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double fee = 0d;
							while (db_reader.Read())
							{
								fee += db_reader.GetDouble("fee");
							}
							Amount1.Content = "\u20b1 " + Convert.ToDouble(string.Format("{0:N3}", fee));
						}
					}
					//Weekly Transactions
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						DayOfWeek day = DateTime.Now.DayOfWeek;
						int days = day - DayOfWeek.Monday;
						DateTime start = DateTime.Now.AddDays(-days);
						DateTime end = start.AddDays(6);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ThisWeeksTransactions.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Weekly Transactions Amount
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						DayOfWeek day = DateTime.Now.DayOfWeek;
						int days = day - DayOfWeek.Monday;
						DateTime start = DateTime.Now.AddDays(-days);
						DateTime end = start.AddDays(6);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double fee = 0d;
							while (db_reader.Read())
							{
								fee += db_reader.GetDouble("fee");
							}
							Amount2.Content = "\u20b1 " + Convert.ToDouble(string.Format("{0:N3}", fee));
						}
					}
					//Monthly Transactions
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, cDate.Month, 1);
						var end = start.AddMonths(1).AddDays(-1);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ThisMonthsTransactions.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Monthly Transactions Amount
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, cDate.Month, 1);
						var end = start.AddMonths(1).AddDays(-1);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double fee = 0d;
							while (db_reader.Read())
							{
								fee += db_reader.GetDouble("fee");
							}
							Amount3.Content = "\u20b1 " + Convert.ToDouble(string.Format("{0:N3}", fee));
						}
					}
					//Yearly Transactions
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ThisYearsTransactions.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Yearly Transactions Amount
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double fee = 0d;
							while (db_reader.Read())
							{
								fee += db_reader.GetDouble("fee");
							}
							Amount4.Content = "\u20b1 " + Convert.ToDouble(string.Format("{0:N3}", fee));
						}
					}
					//Certificate Retrieval
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Matrimonial Cert.' OR type = 'Burial Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								TotalCertificateRetrieval.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Certificate Retrieval Others
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								OtherCount.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Certificate Retrieval Baptismal
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Baptismal Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								BaptismalCount.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Certificate Retrieval Confirmation
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Confirmation Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ConfirmationCount.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Certificate Retrieval Matrimonial
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Matrimonial Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								MatrimonialCount.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Certificate Retrieval Burial
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Burial Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								BurialCount.Content = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
				}
			}
		}
		private int CountTransactions(string type, string status, int month) {
			int ret = 0;
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						if (status == "All")
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type AND tran_date > @month_min AND tran_date < @month_max;";
							cmd.Parameters.AddWithValue("@type", type);
						}
						else
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type AND status = @status AND tran_date > @month_min AND tran_date < @month_max;";
							cmd.Parameters.AddWithValue("@type", type);
							cmd.Parameters.AddWithValue("@status", status);
						}
						cmd.Parameters.AddWithValue("@month_min", cDate.ToString("yyyy-" + month + "-01"));
						cmd.Parameters.AddWithValue("@month_max", cDate.ToString("yyyy-" + month+1 + "-01"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ret = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
				}
			}
			return ret;
		}
		private double CountAmount(string type, string status, int month)
		{
			double ret = 0d;
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						if (status == "All")
						{
							cmd.CommandText = "SELECT fee FROM transactions WHERE type = @type AND tran_date > @month_min AND tran_date < @month_max;";
							cmd.Parameters.AddWithValue("@type", type);
						}
						else
						{
							cmd.CommandText = "SELECT fee FROM transactions WHERE type = @type AND status = @status AND tran_date > @month_min AND tran_date < @month_max;";
							cmd.Parameters.AddWithValue("@type", type);
							cmd.Parameters.AddWithValue("@status", status);
						}
						cmd.Parameters.AddWithValue("@month_min", cDate.ToString("yyyy-" + month + "-01"));
						cmd.Parameters.AddWithValue("@month_max", cDate.ToString("yyyy-" + month + 1 + "-01"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ret += db_reader.GetDouble("fee");
							}
						}
					}
				}
			}
			return Convert.ToDouble(string.Format("{0:N3}", ret));
		}
		private void SyncChanges(object sender, SelectionChangedEventArgs e)
		{
			
		}
		private void UpdateChart(object sender, EventArgs e)
		{
			if (ReportType.SelectedIndex == 0) {
				YLabel.Title = "Transactions";
				SeriesCollection[0].Values.Clear();
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 1)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 2)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 3)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 4)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 5)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 6)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 7)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 8)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 9)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 10)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 11)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountTransactions("Baptismal Cert.", StatusType.Text, 12)));

				SeriesCollection[1].Values.Clear();
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 1)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 2)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 3)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 4)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 5)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 6)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 7)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 8)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 9)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 10)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 11)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountTransactions("Confirmation Cert.", StatusType.Text, 12)));

				SeriesCollection[2].Values.Clear();
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 1)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 2)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 3)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 4)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 5)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 6)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 7)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 8)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 9)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 10)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 11)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountTransactions("Matrimonial Cert.", StatusType.Text, 12)));

				SeriesCollection[3].Values.Clear();
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 1)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 2)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 3)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 4)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 5)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 6)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 7)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 8)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 9)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 10)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 11)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountTransactions("Burial Cert.", StatusType.Text, 12)));
			}
			else if (ReportType.SelectedIndex == 1) {
				YLabel.Title = "Pesos";
				SeriesCollection[0].Values.Clear();
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 1)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 2)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 3)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 4)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 5)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 6)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 7)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 8)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 9)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 10)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 11)));
				SeriesCollection[0].Values.Add(Convert.ToDouble(CountAmount("Baptismal Cert.", StatusType.Text, 12)));

				SeriesCollection[1].Values.Clear();
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 1)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 2)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 3)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 4)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 5)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 6)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 7)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 8)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 9)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 10)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 11)));
				SeriesCollection[1].Values.Add(Convert.ToDouble(CountAmount("Confirmation Cert.", StatusType.Text, 12)));

				SeriesCollection[2].Values.Clear();
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 1)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 2)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 3)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 4)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 5)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 6)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 7)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 8)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 9)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 10)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 11)));
				SeriesCollection[2].Values.Add(Convert.ToDouble(CountAmount("Matrimonial Cert.", StatusType.Text, 12)));

				SeriesCollection[3].Values.Clear();
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 1)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 2)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 3)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 4)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 5)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 6)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 7)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 8)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 9)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 10)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 11)));
				SeriesCollection[3].Values.Add(Convert.ToDouble(CountAmount("Burial Cert.", StatusType.Text, 12)));
			}
		}
		private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
		{
			var chart = (LiveCharts.Wpf.PieChart)chartpoint.ChartView;

			//clear selected slice.
			foreach (PieSeries series in chart.Series)
				series.PushOut = 0;

			var selectedSeries = (PieSeries)chartpoint.SeriesView;
			selectedSeries.PushOut = 8;
		}
		private void GenReport1(object sender, RoutedEventArgs e)
		{
			////create a new pdf document
			//PdfDocument pdfDoc = new PdfDocument();

			//PdfPageBase page = pdfDoc.Pages.Add();
			//page.Canvas.DrawString("St. Raphael Parish of Legazpi City",
			//new PdfFont(PdfFontFamily.Helvetica, 13f),
			//new PdfSolidBrush(Color.Black),
			//160, 10);

			//page.Canvas.DrawString("Monthly Transactions Report",
			//new PdfFont(PdfFontFamily.Helvetica, 12f),
			//new PdfSolidBrush(Color.Black),
			//180, 28);

			//page.Canvas.DrawString("Generated On: " + DateTime.Now.ToString("MMM dd, yyyy hh:mm tt"),
			//new PdfFont(PdfFontFamily.Helvetica, 12f),
			//new PdfSolidBrush(Color.Black),
			//155, 45);

			//page.Canvas.DrawString("Total Transactions: 1234",
			//new PdfFont(PdfFontFamily.Helvetica, 12f),
			//new PdfSolidBrush(Color.Black),
			//10, 80);

			//page.Canvas.DrawString("Total Amount: 123456",
			//new PdfFont(PdfFontFamily.Helvetica, 12f),
			//new PdfSolidBrush(Color.Black),
			//10, 100);

			//PdfTable table = new PdfTable();
			//table.Style.CellPadding = 2;
			//table.Style.DefaultStyle.BackgroundBrush = PdfBrushes.SkyBlue;
			//table.Style.DefaultStyle.Font = new PdfTrueTypeFont(new Font("Arial", 10f));

			//table.Style.AlternateStyle = new PdfCellStyle();
			//table.Style.AlternateStyle.BackgroundBrush = PdfBrushes.LightYellow;
			//table.Style.AlternateStyle.Font = new PdfTrueTypeFont(new Font("Arial", 10f));

			//table.Style.HeaderSource = PdfHeaderSource.ColumnCaptions;
			//table.Style.HeaderStyle.BackgroundBrush = PdfBrushes.CadetBlue;
			//table.Style.HeaderStyle.Font = new PdfFont(PdfFontFamily.Helvetica, 13f);
			//table.Style.HeaderStyle.StringFormat = new PdfStringFormat(PdfTextAlignment.Center);

			//table.Style.ShowHeader = true;

			//table.DataSourceType = PdfTableDataSourceType.TableDirect;

			//table.DataSource = GenerateList();

			////Set the width of column  
			//float width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1);
			//table.Columns[0].Width = width * 0.24f * width;
			//table.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			//table.Columns[1].Width = width * 0.21f * width;
			//table.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			//table.Columns[2].Width = width * 0.24f * width;
			//table.Columns[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			//table.Columns[3].Width = width * 0.24f * width;
			//table.Columns[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			//table.Columns[4].Width = width * 0.24f * width;
			//table.Columns[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			//table.Columns[5].Width = width * 0.24f * width;
			//table.Columns[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			//table.Columns[6].Width = width * 0.24f * width;
			//table.Columns[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			//table.Draw(page, new PointF(0, 120));

			////save
			//pdfDoc.SaveToFile(@"..\..\sample.pdf");

			////launch the pdf document
			//System.Diagnostics.Process.Start(@"..\..\sample.pdf");
		}

		private void UpdatePieChart(object sender, EventArgs e)
		{
			if (PieRerortType.SelectedIndex == 0) {
				SyncPieTransactions();
			}
			else {
				SyncPieAmounts();
			}
		}
		private void SyncPieTransactions()
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Baptismal Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								BaptismalVal.Values = new ChartValues<ObservableValue> { new ObservableValue(db_reader.GetInt32("COUNT(*)")) };
							}
						}
					}
					//Confirmation Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								ConfirmationVal.Values = new ChartValues<ObservableValue> { new ObservableValue(db_reader.GetInt32("COUNT(*)")) };
							}
						}
					}
					//Matrimonial Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								MatrimonialVal.Values = new ChartValues<ObservableValue> { new ObservableValue(db_reader.GetInt32("COUNT(*)")) };
							}
						}
					}
					//Burial Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								BurialVal.Values = new ChartValues<ObservableValue> { new ObservableValue(db_reader.GetInt32("COUNT(*)")) };
							}
						}
					}
					//Others Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								OthersVal.Values = new ChartValues<ObservableValue> { new ObservableValue(db_reader.GetInt32("COUNT(*)")) };
							}
						}
					}
				}
			}
		}
		private void SyncPieAmounts()
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Baptismal Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double _fee = 0d;
							while (db_reader.Read())
							{
								_fee += db_reader.GetDouble("fee");
							}
							BaptismalVal.Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(string.Format("{0:N3}", _fee))) };
						}
					}
					//Confirmation Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double _fee = 0d;
							while (db_reader.Read())
							{
								_fee += db_reader.GetDouble("fee");
							}
							ConfirmationVal.Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(string.Format("{0:N3}", _fee))) };
						}
					}
					//Matrimonial Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double _fee = 0d;
							while (db_reader.Read())
							{
								_fee += db_reader.GetDouble("fee");
							}
							MatrimonialVal.Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(string.Format("{0:N3}", _fee))) };
						}
					}
					//Burial Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE type = @type;";
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double _fee = 0d;
							while (db_reader.Read())
							{
								_fee += db_reader.GetDouble("fee");
							}
							BurialVal.Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(string.Format("{0:N3}", _fee))) };
						}
					}
					//Others Val
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.';";
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							double _fee = 0d;
							while (db_reader.Read())
							{
								_fee += db_reader.GetDouble("fee");
							}
							OthersVal.Values = new ChartValues<ObservableValue> { new ObservableValue(Convert.ToDouble(string.Format("{0:N3}", _fee))) };
						}
					}
				}
			}
		}
	}
}