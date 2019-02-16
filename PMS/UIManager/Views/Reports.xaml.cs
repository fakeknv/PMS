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
using MahApps.Metro.Controls.Dialogs;
using System.Reflection;
using MahApps.Metro.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Collections.ObjectModel;

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
		public SeriesCollection SeriesCollection2 { get; set; }
		public SeriesCollection SeriesCollection3 { get; set; }

		public string[] Labels { get; set; }
		public string[] Labels3 { get; set; }

		public Func<double, string> YFormatter { get; set; }
		public Func<double, string> Formatter3 { get; set; }

		private ObservableCollection<Transaction> transactions;
		private ObservableCollection<Transaction> transactions_final;

		private ObservableCollection<Register> registers;
		private ObservableCollection<Register> registers_final;

		public Reports()
		{
			InitializeComponent();
			SyncCharts();
			SyncSummary();
			
			SyncRegisterSummary();

			DateTime date = DateTime.Now;
			var min = new DateTime(date.Year, date.Month, 1);
			var max = min.AddMonths(1).AddDays(-1);
			MinDate.SelectedDate = min;
			MaxDate.SelectedDate = max;

			SyncPieTransactions();
			SyncTransactions();
			SyncRegisters();

			ItemsPerPage.SelectionChanged += Update2;
			CurrentPage.ValueChanged += Update;

			ItemsPerPage2.SelectionChanged += Update4;
			CurrentPage2.ValueChanged += Update3;

			RepStatus.DropDownClosed += UpdateX;
			RepType.DropDownClosed += UpdateX;

			MinDate.CalendarClosed += UpdateZ;
			MaxDate.CalendarClosed += UpdateZ;
		}
		private string CheckFrequency(int bookNum)
		{
			string ret = "Very Low";
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					//Yearly
					cmd.CommandText = "SELECT COUNT(*) FROM transactions, records WHERE transactions.target_id = records.record_id AND records.book_number = @book_number AND transactions.tran_date > @min AND transactions.tran_date < @max;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					cmd.Parameters.AddWithValue("@min", DateTime.Now.ToString("yyyy-01-01"));
					cmd.Parameters.AddWithValue("@max", DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddMonths(13).AddDays(-1));
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0)
						{
							ret = "Access Frequency: Low";
						}
					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					//Monthly
					cmd.CommandText = "SELECT COUNT(*) FROM transactions, records WHERE transactions.target_id = records.record_id AND records.book_number = @book_number AND transactions.tran_date > @min AND transactions.tran_date < @max;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					cmd.Parameters.AddWithValue("@min", DateTime.Now.ToString("yyyy-MM-01"));
					cmd.Parameters.AddWithValue("@max", DateTime.Now.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0)
						{
							ret = "Moderate";
						}
					}
					conn.Close();

					conn.Open();
					cmd = conn.CreateCommand();
					//Weekly
					DayOfWeek day = DateTime.Now.DayOfWeek;
					int days = day - DayOfWeek.Monday;
					DateTime start = DateTime.Now.AddDays(-days);
					DateTime end = start.AddDays(6);

					cmd.CommandText = "SELECT COUNT(*) FROM transactions, records WHERE transactions.target_id = records.record_id AND records.book_number = @book_number AND transactions.tran_date > @min AND transactions.tran_date < @max;";
					cmd.Parameters.AddWithValue("@book_number", bookNum);
					cmd.Parameters.AddWithValue("@min", start.ToString("yyyy-MM-dd"));
					cmd.Parameters.AddWithValue("@max", end.ToString("yyyy-MM-dd"));
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetInt32("COUNT(*)") > 0)
						{
							ret = "High";
						}
					}
					//close Connection
					conn.Close();
				}
			}
			return ret;
		}
		private void Update(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncTransactions();
		}
		private void Update2(object sender, SelectionChangedEventArgs e)
		{
			SyncTransactions();
		}
		private void Update3(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			SyncRegisters();
		}
		private void Update4(object sender, SelectionChangedEventArgs e)
		{
			SyncRegisters();
		}
		internal void SyncRegisters()
		{
			registers = new ObservableCollection<Register>();
			registers_final = new ObservableCollection<Register>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage2.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;



			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					if (RegType.Text == "All")
					{
						if (RegStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM registers ORDER BY creation_date DESC;";
						}
						else
						{
							cmd.CommandText = "SELECT * FROM registers WHERE status = @status ORDER BY creation_date DESC;";
							cmd.Parameters.AddWithValue("@status", RegStatus.Text);
						}
					}
					else{
						if (RegStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM registers WHERE book_type = @type ORDER BY creation_date DESC;";
							cmd.Parameters.AddWithValue("@type", RegType.Text);
						}
						else
						{
							cmd.CommandText = "SELECT * FROM registers WHERE book_type = @type AND status = @status ORDER BY creation_date DESC;";
							cmd.Parameters.AddWithValue("@status", RegStatus.Text);
							cmd.Parameters.AddWithValue("@type", RegType.Text);
						}
					}
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string cdate = "";
						if (db_reader.GetString("creation_date") == "0000-00-00")
						{
							cdate = "";
						}
						else
						{
							cdate = DateTime.Parse(db_reader.GetString("creation_date")).ToString("MMMM dd, yyyy");
						}

						registers.Add(new Register()
						{
							Type = db_reader.GetString("book_type"),
							BookNum = db_reader.GetString("book_number"),
							Status = db_reader.GetString("status"),
							RecordCount = CountRecords(db_reader.GetInt32("book_number")).ToString(),
							CreationDate = cdate,
							Frequency = CheckFrequency(db_reader.GetInt32("book_number")),
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					foreach (var cur in registers)
					{
						if (cur.Page == CurrentPage2.Value)
						{
							registers_final.Add(new Register()
							{
								Type = cur.Type,
								BookNum = cur.BookNum,
								Status = cur.Status,
								RecordCount = cur.RecordCount,
								CreationDate = cur.CreationDate,
								Frequency = cur.Frequency,
								Page = cur.Page
							});
						}
					}
					//close Connection
					conn.Close();

					RegistersItemContainer.Items.Refresh();
					RegistersItemContainer.ItemsSource = registers_final;
					RegistersItemContainer.Items.Refresh();
					CurrentPage2.Maximum = page;
				}
				else
				{

				}
			}
		}
		internal int CountRecords(int bookNum)
		{
			int ret = 0;
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM records WHERE book_number = @booknum;";
					cmd.Parameters.AddWithValue("@booknum", bookNum);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetInt32("COUNT(book_number)");
					}
					conn.Close();
				} else {

				}
			}
			return ret;
		}
		internal void SyncTransactions()
		{
			transactions = new ObservableCollection<Transaction>();
			transactions_final = new ObservableCollection<Transaction>();

			ComboBoxItem ci = (ComboBoxItem)ItemsPerPage.SelectedItem;
			int itemsPerPage = Convert.ToInt32(ci.Content);
			int page = 1;
			int count = 0;



			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					if (RepType.Text == "All") {
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else {
							cmd.CommandText = "SELECT * FROM transactions WHERE status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					} else if (RepType.Text == "Certificates") {
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Matrimonial Cert.' OR type = 'Burial Cert.' AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Matrimonial Cert.' OR type = 'Burial Cert.' AND status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					} else if (RepType.Text == "Scheduling") {
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type != 'Baptismal Cert.' OR type != 'Confirmation Cert.' OR type != 'Matrimonial Cert.' OR type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type != 'Baptismal Cert.' OR type != 'Confirmation Cert.' OR type != 'Matrimonial Cert.' OR type != 'Burial Cert.' AND status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					} else {
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = @type AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@type", RepType.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = @type AND status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@type", RepType.Text);
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					}
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string recname = "";
						if (db_reader.GetString("target_id").Substring(0, 3) == "PMS")
						{
							recname = GetRecordName(db_reader.GetString("target_id"));
						}
						else
						{

						}
						string dateFinished = "";
						string timeFinished = "";
						if (db_reader.GetString("status") == "Unpaid")
						{
							dateFinished = " ";
							timeFinished = " ";
						}
						else
						{
							dateFinished = DateTime.Parse(db_reader.GetString("completion_date")).ToString("MMMM dd, yyyy");
							timeFinished = DateTime.Parse(db_reader.GetString("completion_time")).ToString("h:mm tt");
						}
						transactions.Add(new Transaction()
						{
							TransactionID = db_reader.GetString("transaction_id"),
							Type = db_reader.GetString("type"),
							Name = recname,
							Fee = Convert.ToDouble(string.Format("{0:N3}", db_reader.GetDouble("fee"))),
							Status = db_reader.GetString("status"),
							ORNumber = db_reader.GetString("or_number"),
							DatePlaced = DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd, yyyy"),
							TimePlaced = DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt"),
							DateConfirmed = dateFinished,
							TimeConfirmed = timeFinished,
							Page = page
						});
						count++;
						if (count == itemsPerPage)
						{
							page++;
							count = 0;
						}
					}
					int temp = 1;
					foreach (var cur in transactions)
					{
						if (cur.Page == CurrentPage.Value)
						{
							transactions_final.Add(new Transaction()
							{
								No = temp,
								TransactionID = cur.TransactionID,
								Type = cur.Type,
								Name = cur.Name,
								Fee = cur.Fee,
								Status = cur.Status,
								ORNumber = cur.ORNumber,
								DatePlaced = cur.DatePlaced,
								TimePlaced = cur.TimePlaced,
								DateConfirmed = cur.DateConfirmed,
								TimeConfirmed = cur.TimeConfirmed,
								Page = cur.Page
							});
							temp++;
						}
					}
					//close Connection
					conn.Close();

					TransactionItemsContainer.Items.Refresh();
					TransactionItemsContainer.ItemsSource = transactions_final;
					TransactionItemsContainer.Items.Refresh();
					CurrentPage.Maximum = page;
				}
				else
				{

				}
			}
		}
		internal string GetRecordName(string rid)
		{
			string ret = "";
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd2 = conn.CreateCommand();
					cmd2.CommandText = "SELECT * FROM records WHERE record_id = @record_id LIMIT 1;";
					cmd2.Parameters.AddWithValue("@record_id", rid);
					cmd2.Prepare();
					MySqlDataReader db_reader2 = cmd2.ExecuteReader();
					while (db_reader2.Read())
					{
						ret = db_reader2.GetString("recordholder_fullname");
					}
					conn.Close();
				}
			}
			return ret;
		}
		private void SyncCharts() {
			PointLabel = chartPoint =>
				string.Format("{0} ({1:P})", chartPoint.Y, chartPoint.Participation);

			SeriesCollection = new SeriesCollection
			{
				new LineSeries
				{
					Title = "Baptismal",
					Values = new ChartValues<double> { CountTransactions("Baptismal Cert.", "Unpaid", 1), CountTransactions("Baptismal Cert.", "Unpaid", 2), CountTransactions("Baptismal Cert.", "Unpaid", 3), CountTransactions("Baptismal Cert.", "Unpaid", 4), CountTransactions("Baptismal Cert.", "Unpaid", 5), CountTransactions("Baptismal Cert.", "Unpaid", 6), CountTransactions("Baptismal Cert.", "Unpaid", 7), CountTransactions("Baptismal Cert.", "Unpaid", 8), CountTransactions("Baptismal Cert.", "Unpaid", 9), CountTransactions("Baptismal Cert.", "Unpaid", 10), CountTransactions("Baptismal Cert.", "Unpaid", 11), CountTransactions("Baptismal Cert.", "Unpaid", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				},
				new LineSeries
				{
					Title = "Confirmation",
					Values = new ChartValues<double> { CountTransactions("Confirmation Cert.", "Unpaid", 1), CountTransactions("Confirmation Cert.", "Unpaid", 2), CountTransactions("Confirmation Cert.", "Unpaid", 3), CountTransactions("Confirmation Cert.", "Unpaid", 4), CountTransactions("Confirmation Cert.", "Unpaid", 5), CountTransactions("Confirmation Cert.", "Unpaid", 6), CountTransactions("Confirmation Cert.", "Unpaid", 7), CountTransactions("Confirmation Cert.", "Unpaid", 8), CountTransactions("Confirmation Cert.", "Unpaid", 9), CountTransactions("Confirmation Cert.", "Unpaid", 10), CountTransactions("Confirmation Cert.", "Unpaid", 11), CountTransactions("Confirmation Cert.", "Unpaid", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				},
				new LineSeries
				{
					Title = "Matrimonial",
					Values = new ChartValues<double> { CountTransactions("Matrimonial Cert.", "Unpaid", 1), CountTransactions("Matrimonial Cert.", "Unpaid", 2), CountTransactions("Matrimonial Cert.", "Unpaid", 3), CountTransactions("Matrimonial Cert.", "Unpaid", 4), CountTransactions("Matrimonial Cert.", "Unpaid", 5), CountTransactions("Matrimonial Cert.", "Unpaid", 6), CountTransactions("Matrimonial Cert.", "Unpaid", 7), CountTransactions("Matrimonial Cert.", "Unpaid", 8), CountTransactions("Matrimonial Cert.", "Unpaid", 9), CountTransactions("Matrimonial Cert.", "Unpaid", 10), CountTransactions("Matrimonial Cert.", "Unpaid", 11), CountTransactions("Matrimonial Cert.", "Unpaid", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				},
				new LineSeries
				{
					Title = "Burial",
					Values = new ChartValues<double> { CountTransactions("Burial Cert.", "Unpaid", 1), CountTransactions("Burial Cert.", "Unpaid", 2), CountTransactions("Burial Cert.", "Unpaid", 3), CountTransactions("Burial Cert.", "Unpaid", 4), CountTransactions("Burial Cert.", "Unpaid", 5), CountTransactions("Burial Cert.", "Unpaid", 6), CountTransactions("Burial Cert.", "Unpaid", 7), CountTransactions("Burial Cert.", "Unpaid", 8), CountTransactions("Burial Cert.", "Unpaid", 9), CountTransactions("Burial Cert.", "Unpaid", 10), CountTransactions("Burial Cert.", "Unpaid", 11), CountTransactions("Burial Cert.", "Unpaid", 12) },
					PointGeometry = DefaultGeometries.Circle,
					PointGeometrySize = 15
				}
			};

			Labels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
			YFormatter = value => value.ToString();

			SeriesCollection2 = new SeriesCollection
			{
				new PieSeries
				{
					Title = "Baptismal",
					Values = new ChartValues<ObservableValue> { new ObservableValue(8) },
					DataLabels = true
				},
				new PieSeries
				{
					Title = "Confirmation",
					Values = new ChartValues<ObservableValue> { new ObservableValue(6) },
					DataLabels = true
				},
				new PieSeries
				{
					Title = "Matrimonial",
					Values = new ChartValues<ObservableValue> { new ObservableValue(10) },
					DataLabels = true
				},
				new PieSeries
				{
					Title = "Burial",
					Values = new ChartValues<ObservableValue> { new ObservableValue(4) },
					DataLabels = true
				}
			};

			//SeriesCollection3 = new SeriesCollection
			//{
			//	new ColumnSeries
			//	{
			//		Title = "Baptismal",
			//		Values = new ChartValues<double> { 1, 2, 3}
			//	},
			//	new ColumnSeries
			//	{
			//		Title = "Confirmation",
			//		Values = new ChartValues<double> { 10, 50, 39}
			//	},
			//	new ColumnSeries
			//	{
			//		Title = "Matrimonial",
			//		Values = new ChartValues<double> { 10, 50, 39}
			//	},
			//	new ColumnSeries
			//	{
			//		Title = "Burial",
			//		Values = new ChartValues<double> { 10, 50, 39}
			//	}
			//};

			Labels3 = new[] { "Paid", "Unpaid", "Cancelled"};
			Formatter3 = value => value.ToString("N");

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
						var end = DateTime.Parse(start.ToString("yyyy-01-01")).AddMonths(13).AddDays(-1);

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
						var end = DateTime.Parse(start.ToString("yyyy-01-01")).AddMonths(13).AddDays(-1);

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
		//private void SyncChanges(object sender, SelectionChangedEventArgs e)
		//{
			
		//}
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
		private async void GenReport1(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			var controller = await metroWindow.ShowProgressAsync("Generating...", "Please wait while the system is generating the report.");
			controller.SetIndeterminate();
			GenReport1Phase2();
			// Close...
			await controller.CloseAsync();
		}
		private void GenReport1Phase2() {
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

			page.Canvas.DrawString("PMS Transactions Report",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			350, 52);

			page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 52);

			page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 70), new PointF(530, 70));

			page.Canvas.DrawString("Overview",
			new PdfFont(PdfFontFamily.TimesRoman, 16f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			230, 80);

			page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(229, 97), new PointF(295, 97));

			SaveToPng(ChartEx, "chart1.png");

			logo = PdfImage.FromFile(@"chart1.png");
			_width = 490;
			height = 230;
			x = (page.Canvas.ClientSize.Width - _width) / 2;
			page.Canvas.DrawImage(logo, 20, 80, _width, height);

			DataTable dtNames = new DataTable();
			dtNames.Columns.Add("Month", typeof(string));
			dtNames.Columns.Add("Baptismal", typeof(string));
			dtNames.Columns.Add("Confirmation", typeof(string));
			dtNames.Columns.Add("Matrimonial", typeof(string));
			dtNames.Columns.Add("Burial", typeof(string));
			dtNames.Columns.Add("Other Services", typeof(string));
			dtNames.Columns.Add("Amount", typeof(string));

			for (int i = 1; i < 12; i++) {
				dtNames.Rows.Add(DateTime.Parse("2019-" + i + "-01").ToString("MMM"), CountT(i, "Baptismal"), CountT(i, "Confirmation"), CountT(i, "Matrimonial"), CountT(i, "Burial"), CountT(i, "Others"), "PHP " + CountA(i));
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
			table.Columns[3].Width = width * 0.24f * width;
			table.Columns[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[4].Width = width * 0.24f * width;
			table.Columns[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[5].Width = width * 0.24f * width;
			table.Columns[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[6].Width = width * 0.24f * width;
			table.Columns[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Draw(page, new PointF(10, 340));

			string fname = "Transactions_Report-" + DateTime.Now.ToString("MMM_dd_yyyy") + ".pdf";
			//save
			pdfDoc.SaveToFile(@"..\..\" + fname);
			//launch the pdf document
			System.Diagnostics.Process.Start(@"..\..\" + fname);
		}
		private async void GenReport2(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			var controller = await metroWindow.ShowProgressAsync("Generating...", "Please wait while the system is generating the report.");
			controller.SetIndeterminate();
			GenReport2Phase2();
			// Close...
			await controller.CloseAsync();
		}
		private void GenReport2Phase2()
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

			page.Canvas.DrawString("PMS Transactions Report",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			350, 52);

			page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 52);

			page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 70), new PointF(530, 70));

			page.Canvas.DrawString("Transactions Report",
			new PdfFont(PdfFontFamily.TimesRoman, 14f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			210, 80);

			page.Canvas.DrawString(DateTime.Parse(MinDate.Text).ToString("MMM dd, yyyy") + " to " + DateTime.Parse(MaxDate.Text).ToString("MMM dd, yyyy"),
			new PdfFont(PdfFontFamily.TimesRoman, 14f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			180, 100);

			page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(170, 97), new PointF(360, 97));

			SaveToPng(BarChart, "chart2.png");

			logo = PdfImage.FromFile(@"chart2.png");
			_width = 490;
			height = 230;
			x = (page.Canvas.ClientSize.Width - _width) / 2;
			page.Canvas.DrawImage(logo, 20, 120, _width, height);

			page.Canvas.DrawString("Total Certificate Retrieval: " + TotalCertificateRetrieval.Content,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			60, 365);

			page.Canvas.DrawString("Baptismal: " + BaptismalCount.Content,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			60, 385);

			page.Canvas.DrawString("Confirmation: " + ConfirmationCount.Content,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			60, 405);

			page.Canvas.DrawString("Matrimonial: " + MatrimonialCount.Content,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			60, 425);

			page.Canvas.DrawString("Burial: " + BurialCount.Content,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			60, 445);

			page.Canvas.DrawString("Others: " + OtherCount.Content,
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			360, 365);

			DataTable dtNames = new DataTable();
			dtNames.Columns.Add("No", typeof(int));
			dtNames.Columns.Add("Type", typeof(string));
			dtNames.Columns.Add("Name on Record", typeof(string));
			dtNames.Columns.Add("Fee", typeof(string));
			dtNames.Columns.Add("Status", typeof(string));
			dtNames.Columns.Add("Placed On", typeof(string));
			dtNames.Columns.Add("Completed On", typeof(string));
			dtNames.Columns.Add("OR Number", typeof(string));

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				//TransactionItemsContainer.Items.Clear();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					if (RepType.Text == "All")
					{
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					}
					else if (RepType.Text == "Certificates")
					{
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Matrimonial Cert.' OR type = 'Burial Cert.' AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Matrimonial Cert.' OR type = 'Burial Cert.' AND status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					}
					else if (RepType.Text == "Scheduling")
					{
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type != 'Baptismal Cert.' OR type != 'Confirmation Cert.' OR type != 'Matrimonial Cert.' OR type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type != 'Baptismal Cert.' OR type != 'Confirmation Cert.' OR type != 'Matrimonial Cert.' OR type != 'Burial Cert.' AND status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					}
					else
					{
						if (RepStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = @type AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@type", RepType.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
						else
						{
							cmd.CommandText = "SELECT * FROM transactions WHERE type = @type AND status = @status AND tran_date > @min AND tran_date < @max ORDER BY tran_date DESC , tran_time DESC;";
							cmd.Parameters.AddWithValue("@type", RepType.Text);
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
							cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
							cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						}
					}
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					int temp = 1;
					while (db_reader.Read())
					{
						string recname = "";
						if (db_reader.GetString("target_id").Substring(0, 3) == "PMS")
						{
							recname = GetRecordName(db_reader.GetString("target_id"));
						}
						else
						{

						}
						string dateFinished = "";
						string timeFinished = "";
						if (db_reader.GetString("status") == "Unpaid")
						{
							dateFinished = " ";
							timeFinished = " ";
						}
						else
						{
							dateFinished = DateTime.Parse(db_reader.GetString("completion_date")).ToString("MMMM dd, yyyy");
							timeFinished = DateTime.Parse(db_reader.GetString("completion_time")).ToString("h:mm tt");
						}
						dtNames.Rows.Add(temp, db_reader.GetString("type"), recname, "PHP " + Convert.ToDouble(string.Format("{0:N3}", db_reader.GetDouble("fee"))), db_reader.GetString("status"), DateTime.Parse(db_reader.GetString("tran_date")).ToString("MMMM dd, yyyy") + " " + DateTime.Parse(db_reader.GetString("tran_time")).ToString("h:mm tt"), dateFinished + " " + timeFinished, db_reader.GetString("or_number"));
						temp++;
					}
					//dtNames.Rows.Add(" ", " ", " ", " ", " ", " ", " ");
					//close Connection
					conn.Close();
				}
				else
				{

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
			table.Columns[3].Width = width * 0.24f * width;
			table.Columns[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[4].Width = width * 0.24f * width;
			table.Columns[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[5].Width = width * 0.24f * width;
			table.Columns[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[6].Width = width * 0.24f * width;
			table.Columns[6].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[7].Width = width * 0.24f * width;
			table.Columns[7].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Draw(page, new PointF(10, 470));

			PdfPageBase page2 = pdfDoc.Pages.Add();
			page2.Canvas.DrawString("Prepared By: ",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 10);

			page2.Canvas.DrawString(pmsutil.GetEmpName(Application.Current.Resources["uid"].ToString()),
			new PdfFont(PdfFontFamily.TimesRoman, 12f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 50);

			page2.Canvas.DrawString("St. Raphael Parish - Administrator",
			new PdfFont(PdfFontFamily.TimesRoman, 12f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 70);

			string fname = "Transactions_Report-" + DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd") + "_to_" + DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd") + ".pdf";
			//save
			pdfDoc.SaveToFile(@"..\..\" + fname);
			//launch the pdf document
			System.Diagnostics.Process.Start(@"..\..\" + fname);
		}
		private void SyncRegisterSummary() {
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers;";
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						TotalReg.Content = "Total Registers: " + db_reader.GetString("COUNT(book_number)");
					}
					//close Connection
					conn.Close();
					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers WHERE status = 'Normal';";
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						NotArchived1.Content = "Not Archived: " + db_reader.GetString("COUNT(book_number)");
					}
					//close Connection
					conn.Close();
					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers WHERE status = 'Archived';";
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						Archived1.Content = "Archived: " + db_reader.GetString("COUNT(book_number)");
					}
					//close Connection
					conn.Close();
					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers WHERE book_type = 'Baptismal';";
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						BaptismalL.Content = "Baptismal: " + db_reader.GetString("COUNT(book_number)");
					}
					//close Connection
					conn.Close();
					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers WHERE book_type = 'Confirmation';";
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ConfirmationL.Content = "Confirmation: " + db_reader.GetString("COUNT(book_number)");
					}
					//close Connection
					conn.Close();
					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers WHERE book_type = 'Matrimonial';";
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						MatrimonialL.Content = "Matrimonial: " + db_reader.GetString("COUNT(book_number)");
					}
					//close Connection
					conn.Close();
					conn.Open();
					cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers WHERE book_type = 'Burial';";
					cmd.Prepare();
					db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						BurialL.Content = "Burial: " + db_reader.GetString("COUNT(book_number)");
					}
					//close Connection
					conn.Close();

					SeriesCollection2[0].Values.Clear();
					SeriesCollection2[0].Values.Add(new ObservableValue(CountBooks("Baptismal", "Normal")));
					SeriesCollection2[1].Values.Clear();
					SeriesCollection2[1].Values.Add(new ObservableValue(CountBooks("Confirmation", "Normal")));
					SeriesCollection2[2].Values.Clear();
					SeriesCollection2[2].Values.Add(new ObservableValue(CountBooks("Matrimonial", "Normal")));
					SeriesCollection2[3].Values.Clear();
					SeriesCollection2[3].Values.Add(new ObservableValue(CountBooks("Burial", "Normal")));
				}
			}
		}
		private int CountBooks(string type, string status) {
			int ret = 0;
			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT COUNT(book_number) FROM registers WHERE book_type = @type;";
					cmd.Parameters.AddWithValue("@type", type);
					cmd.Parameters.AddWithValue("@status", status);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						ret = db_reader.GetInt32("COUNT(book_number)");
					}
					//close Connection
					conn.Close();
				}
			}
			return ret;
		}
		private async void GenReport3(object sender, RoutedEventArgs e)
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			var controller = await metroWindow.ShowProgressAsync("Generating...", "Please wait while the system is generating the report.");
			controller.SetIndeterminate();
			GenReport3Phase2();
			// Close...
			await controller.CloseAsync();
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

			page.Canvas.DrawString("PMS Registers Report",
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			350, 52);

			page.Canvas.DrawString("Generated on: " + DateTime.Now.ToString("MMMM dd, yyyy hh:mm tt"),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			10, 52);

			page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(1, 70), new PointF(530, 70));

			page.Canvas.DrawString("Registers Report",
			new PdfFont(PdfFontFamily.TimesRoman, 14f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			230, 80);

			page.Canvas.DrawLine(new PdfPen(System.Drawing.Color.Black), new PointF(220, 97), new PointF(330, 97));

			SaveToPng(ChartEz, "chart3.png");

			logo = PdfImage.FromFile(@"chart3.png");
			_width = 400;
			height = 185;
			x = (page.Canvas.ClientSize.Width - _width) / 2;
			page.Canvas.DrawImage(logo, -30, 120, _width, height);

			page.Canvas.DrawString("Overview",
			new PdfFont(PdfFontFamily.TimesRoman, 14f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 110);

			page.Canvas.DrawString(TotalReg.Content.ToString(),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 140);
			page.Canvas.DrawString(NotArchived1.Content.ToString(),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 160);
			page.Canvas.DrawString(Archived1.Content.ToString(),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 180);

			page.Canvas.DrawString(BaptismalL.Content.ToString(),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 210);
			page.Canvas.DrawString(ConfirmationL.Content.ToString(),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 230);
			page.Canvas.DrawString(MatrimonialL.Content.ToString(),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 250);
			page.Canvas.DrawString(BurialL.Content.ToString(),
			new PdfFont(PdfFontFamily.TimesRoman, 12f),
			new PdfSolidBrush(System.Drawing.Color.Black),
			330, 270);

			DataTable dtNames = new DataTable();
			dtNames.Columns.Add("Type", typeof(string));
			dtNames.Columns.Add("Book #", typeof(string));
			dtNames.Columns.Add("Status", typeof(string));
			dtNames.Columns.Add("Record Count", typeof(string));
			dtNames.Columns.Add("Date Created", typeof(string));
			dtNames.Columns.Add("Access Frequency", typeof(string));

			dbman = new DBConnectionManager();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					if (RegType.Text == "All")
					{
						if (RegStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM registers ORDER BY book_type ASC;";
						}
						else
						{
							cmd.CommandText = "SELECT * FROM registers WHERE status = @status ORDER BY book_type ASC;";
							cmd.Parameters.AddWithValue("@status", RegStatus.Text);
						}
					}
					else
					{
						if (RegStatus.Text == "All")
						{
							cmd.CommandText = "SELECT * FROM registers WHERE book_type = @type ORDER BY book_type ASC;";
							cmd.Parameters.AddWithValue("@type", RegType.Text);
						}
						else
						{
							cmd.CommandText = "SELECT * FROM registers WHERE book_type = @type AND status = @status ORDER BY book_type ASC;";
							cmd.Parameters.AddWithValue("@status", RegStatus.Text);
							cmd.Parameters.AddWithValue("@type", RegType.Text);
						}
					}
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						string cdate = "";
						if (db_reader.GetString("creation_date") == "0000-00-00")
						{
							cdate = "";
						}
						else
						{
							cdate = DateTime.Parse(db_reader.GetString("creation_date")).ToString("MMMM dd, yyyy");
						}
						dtNames.Rows.Add(db_reader.GetString("book_type"), db_reader.GetString("book_number"), db_reader.GetString("status"), CountRecords(db_reader.GetInt32("book_number")), cdate, CheckFrequency(db_reader.GetInt32("book_number")));
					}
					//close Connection
					conn.Close();
				}
				else
				{

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
			table.Columns[3].Width = width * 0.24f * width;
			table.Columns[3].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[4].Width = width * 0.24f * width;
			table.Columns[4].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Columns[5].Width = width * 0.24f * width;
			table.Columns[5].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Draw(page, new PointF(10, 320));

			string fname = "Registers_Report-" + DateTime.Now.ToString("MMM_dd_yyyy") + ".pdf";
			//save
			pdfDoc.SaveToFile(@"..\..\" + fname);
			//launch the pdf document
			System.Diagnostics.Process.Start(@"..\..\" + fname);
		}
		private int CountT(int month, string type) {
			int ret = 0;

			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//This Month
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						if (type == "Others") {
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE tran_date > @min AND tran_date < @max AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.';";
						}
						else {
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE tran_date > @min AND tran_date < @max AND type = @type;";
							cmd.Parameters.AddWithValue("@type", type + " Cert.");
						}
						cmd.Parameters.AddWithValue("@min", cDate.ToString("yyyy-"+ month +"-01"));
						cmd.Parameters.AddWithValue("@max", cDate.ToString("yyyy-"+ month+1 +"-01"));
						
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
		private double CountA(int month)
		{
			double ret = 0;

			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//This Month
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@min", cDate.ToString("yyyy-" + month + "-01"));
						cmd.Parameters.AddWithValue("@max", cDate.ToString("yyyy-" + (month+1) + "-01"));
						//Console.WriteLine(cDate.ToString("yyyy-" + month + "-01") +" | "+ cDate.ToString("yyyy-" + month+1 + "-01"));

						cmd.Prepare();

						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								Console.WriteLine();
								ret += db_reader.GetDouble("fee");
							}
						}
					}
				}
			}

			return Convert.ToDouble(string.Format("{0:N3}", ret));
		}
		private void SaveToPng(FrameworkElement visual, string fileName)
		{
			var encoder = new PngBitmapEncoder();
			EncodeVisual(visual, fileName, encoder);
		}

		private static void EncodeVisual(FrameworkElement visual, string fileName, BitmapEncoder encoder)
		{
			var bitmap = new RenderTargetBitmap((int)visual.ActualWidth+40, (int)visual.ActualHeight, 96, 96, PixelFormats.Pbgra32);
			bitmap.Render(visual);
			var frame = BitmapFrame.Create(bitmap);
			encoder.Frames.Add(frame);
			using (var stream = File.Create(fileName)) encoder.Save(stream);
		}
		private void UpdatePieChart(object sender, EventArgs e)
		{
			if (PieReportType.SelectedIndex == 0) {
				SyncPieTransactions();
			}
			else {
				SyncPieAmounts();
			}
		}
		private void SyncPieTransactions()
		{
			YAxis.Title = "Count";

			int baptismalPaid = 0, baptismalUnpaid = 0, baptismalCancelled = 0;
			int confirmationPaid = 0, confirmationUnpaid = 0, confirmationCancelled = 0;
			int matrimonialPaid = 0, matrimonialUnpaid = 0, matrimonialCancelled = 0;
			int burialPaid = 0, burialUnpaid = 0, burialCancelled = 0;
			int othersPaid = 0, othersUnpaid = 0, othersCancelled = 0;

			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Baptismal Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								baptismalPaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Baptismal Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								baptismalUnpaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Baptismal Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								baptismalCancelled = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Confirmation Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								confirmationPaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Confirmation Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								confirmationUnpaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Confirmation Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								confirmationCancelled = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Matrimonial Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								matrimonialPaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Matrimonial Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								matrimonialUnpaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Matrimonial Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								matrimonialCancelled = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Burial Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								burialPaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Burial Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								burialUnpaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Burial Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								burialCancelled = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Others Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								othersPaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Others Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								othersUnpaid = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					//Others Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								othersCancelled = db_reader.GetInt32("COUNT(*)");
							}
						}
					}
					SeriesCollection3 = new SeriesCollection
					{
						new ColumnSeries
						{
							Title = "Baptismal",
							Values = new ChartValues<double> { baptismalPaid, baptismalUnpaid, baptismalCancelled}
						},
						new ColumnSeries
						{
							Title = "Confirmation",
							Values = new ChartValues<double> { confirmationPaid, confirmationUnpaid, confirmationCancelled}
						},
						new ColumnSeries
						{
							Title = "Matrimonial",
							Values = new ChartValues<double> { matrimonialPaid, matrimonialUnpaid, matrimonialCancelled}
						},
						new ColumnSeries
						{
							Title = "Burial",
							Values = new ChartValues<double> { burialPaid, burialUnpaid, burialCancelled}
						},
						new ColumnSeries
						{
							Title = "Others",
							Values = new ChartValues<double> { othersPaid, othersUnpaid, othersCancelled}
						}
					};
					BarChart.Series = SeriesCollection3;

					//Certificate Retrieval
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);
						var start = new DateTime(cDate.Year, 1, 1);
						var end = new DateTime(cDate.Year, 12, 31);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						if (RepStatus.SelectedIndex == 0)
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE (type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Matrimonial Cert.' OR type = 'Burial Cert.') AND tran_date > @min AND tran_date < @max;";
						}
						else
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND (type = 'Baptismal Cert.' OR type = 'Confirmation Cert.' OR type = 'Matrimonial Cert.' OR type = 'Burial Cert.') AND tran_date > @min AND tran_date < @max;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
						}
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
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
						if (RepStatus.SelectedIndex == 0)
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						}
						else
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
						}
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
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
						if (RepStatus.SelectedIndex == 0)
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Baptismal Cert.' AND tran_date > @min AND tran_date < @max;";
						}
						else
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = 'Baptismal Cert.' AND tran_date > @min AND tran_date < @max;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
						}
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
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
						if (RepStatus.SelectedIndex == 0)
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Confirmation Cert.' AND tran_date > @min AND tran_date < @max;";
						}
						else
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = 'Confirmation Cert.' AND tran_date > @min AND tran_date < @max;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
						}
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
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
						if (RepStatus.SelectedIndex == 0)
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Matrimonial Cert.' AND tran_date > @min AND tran_date < @max;";
						}
						else
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = 'Matrimonial Cert.' AND tran_date > @min AND tran_date < @max;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
						}
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
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
						if (RepStatus.SelectedIndex == 0)
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE type = 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						}
						else
						{
							cmd.CommandText = "SELECT COUNT(*) FROM transactions WHERE status = @status AND type = 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
							cmd.Parameters.AddWithValue("@status", RepStatus.Text);
						}
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
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
		private void SyncPieAmounts()
		{
			YAxis.Title = "Amount";
			double baptismalPaid = 0.0d;
			double baptismalUnpaid = 0.0d;
			double baptismalCancelled = 0.0d;
			double confirmationPaid = 0.0d;
			double confirmationUnpaid = 0.0d;
			double confirmationCancelled = 0.0d;
			double matrimonialPaid = 0.0d;
			double matrimonialUnpaid = 0.0d;
			double matrimonialCancelled = 0.0d;
			double burialPaid = 0.0d;
			double burialUnpaid = 0.0d;
			double burialCancelled = 0.0d;
			double othersPaid = 0.0d;
			double othersUnpaid = 0.0d;
			double othersCancelled = 0.0d;

			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					//Baptismal Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								while (db_reader.Read())
								{
									baptismalPaid += db_reader.GetDouble("fee");
								}
							}
						}
					}
					//Baptismal Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								baptismalUnpaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Baptismal Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Baptismal Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								baptismalCancelled += db_reader.GetDouble("fee");
							}
						}
					}
					//Confirmation Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								confirmationPaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Confirmation Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								confirmationUnpaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Confirmation Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Confirmation Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								confirmationCancelled += db_reader.GetDouble("fee");
							}
						}
					}
					//Matrimonial Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								matrimonialPaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Matrimonial Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								matrimonialUnpaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Matrimonial Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Matrimonial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								matrimonialCancelled += db_reader.GetDouble("fee");
							}
						}
					}
					//Burial Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								burialPaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Burial Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								burialUnpaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Burial Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type = @type AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@type", "Burial Cert.");
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								burialCancelled += db_reader.GetDouble("fee");
							}
						}
					}
					//Others Val Paid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Paid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								othersPaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Others Val Unpaid
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Unpaid");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								othersUnpaid += db_reader.GetDouble("fee");
							}
						}
					}
					//Others Val Cancelled
					using (MySqlConnection conn2 = new MySqlConnection(dbman.GetConnStr()))
					{
						string[] dt = pmsutil.GetServerDateTime().Split(null);
						DateTime cDate = Convert.ToDateTime(dt[0]);

						conn2.Open();
						MySqlCommand cmd = conn2.CreateCommand();
						cmd.CommandText = "SELECT fee FROM transactions WHERE status = @status AND type != 'Baptismal Cert.' AND type != 'Confirmation Cert.' AND type != 'Matrimonial Cert.' AND type != 'Burial Cert.' AND tran_date > @min AND tran_date < @max;";
						cmd.Parameters.AddWithValue("@status", "Cancelled");
						cmd.Parameters.AddWithValue("@min", DateTime.Parse(MinDate.Text).ToString("yyyy-MM-dd"));
						cmd.Parameters.AddWithValue("@max", DateTime.Parse(MaxDate.Text).ToString("yyyy-MM-dd"));
						cmd.Prepare();
						using (MySqlDataReader db_reader = cmd.ExecuteReader())
						{
							while (db_reader.Read())
							{
								othersCancelled += db_reader.GetDouble("fee");
							}
						}
					}
					SeriesCollection3 = new SeriesCollection
					{
						new ColumnSeries
						{
							Title = "Baptismal",
							Values = new ChartValues<double> { baptismalPaid, baptismalUnpaid, baptismalCancelled}
						},
						new ColumnSeries
						{
							Title = "Confirmation",
							Values = new ChartValues<double> { confirmationPaid, confirmationUnpaid, confirmationCancelled}
						},
						new ColumnSeries
						{
							Title = "Matrimonial",
							Values = new ChartValues<double> { matrimonialPaid, matrimonialUnpaid, matrimonialCancelled}
						},
						new ColumnSeries
						{
							Title = "Burial",
							Values = new ChartValues<double> { burialPaid, burialUnpaid, burialCancelled}
						},
						new ColumnSeries
						{
							Title = "Others",
							Values = new ChartValues<double> { othersPaid, othersUnpaid, othersCancelled}
						}
					};
					BarChart.Series = SeriesCollection3;

					if (RepStatus.SelectedIndex == 0)
					{
						//All
						TotalCertificateRetrieval.Content = "PHP " + string.Format("{0:N3}", (baptismalPaid + baptismalUnpaid + baptismalCancelled + confirmationPaid + confirmationUnpaid + confirmationCancelled + matrimonialPaid + matrimonialUnpaid + matrimonialCancelled + burialPaid + burialUnpaid + burialCancelled));
						BaptismalCount.Content = "PHP " + string.Format("{0:N3}", (baptismalPaid + baptismalUnpaid + baptismalCancelled));
						ConfirmationCount.Content = "PHP " + string.Format("{0:N3}", (confirmationPaid + confirmationUnpaid + confirmationCancelled));
						MatrimonialCount.Content = "PHP " + string.Format("{0:N3}", (matrimonialPaid + matrimonialUnpaid + matrimonialCancelled));
						BurialCount.Content = "PHP " + string.Format("{0:N3}", (burialPaid + burialUnpaid + burialCancelled));
						OtherCount.Content = "PHP " + string.Format("{0:N3}", (othersPaid + othersUnpaid + othersCancelled));
					}
					else if (RepStatus.SelectedIndex == 1)
					{
						//Paid
						TotalCertificateRetrieval.Content = "PHP " + string.Format("{0:N3}", (baptismalPaid + confirmationPaid + matrimonialPaid + burialPaid));
						BaptismalCount.Content = "PHP " + string.Format("{0:N3}", (baptismalPaid));
						ConfirmationCount.Content = "PHP " + string.Format("{0:N3}", (confirmationPaid));
						MatrimonialCount.Content = "PHP " + string.Format("{0:N3}", (matrimonialPaid));
						BurialCount.Content = "PHP " + string.Format("{0:N3}", (burialPaid));
						OtherCount.Content = "PHP " + string.Format("{0:N3}", (othersPaid));
					}
					else if (RepStatus.SelectedIndex == 2)
					{
						//Unpaid
						TotalCertificateRetrieval.Content = "PHP " + string.Format("{0:N3}", (baptismalUnpaid + confirmationUnpaid + matrimonialUnpaid + burialUnpaid));
						BaptismalCount.Content = "PHP " + string.Format("{0:N3}", (baptismalUnpaid));
						ConfirmationCount.Content = "PHP " + string.Format("{0:N3}", (confirmationUnpaid));
						MatrimonialCount.Content = "PHP " + string.Format("{0:N3}", (matrimonialUnpaid));
						BurialCount.Content = "PHP " + string.Format("{0:N3}", (burialUnpaid));
						OtherCount.Content = "PHP " + string.Format("{0:N3}", (othersUnpaid));
					}
					else if (RepStatus.SelectedIndex == 3)
					{
						//Cancelled
						TotalCertificateRetrieval.Content = "PHP " + string.Format("{0:N3}", (baptismalCancelled + confirmationCancelled + matrimonialCancelled + burialCancelled));
						BaptismalCount.Content = "PHP " + string.Format("{0:N3}", (baptismalCancelled));
						ConfirmationCount.Content = "PHP " + string.Format("{0:N3}", (confirmationCancelled));
						MatrimonialCount.Content = "PHP " + string.Format("{0:N3}", (matrimonialCancelled));
						BurialCount.Content = "PHP " + string.Format("{0:N3}", (burialCancelled));
						OtherCount.Content = "PHP " + string.Format("{0:N3}", (othersCancelled));
					}
				}
			}
		}

		private void UpdateX(object sender, EventArgs e)
		{
			SyncTransactions();

			if (PieReportType.SelectedIndex == 0)
			{
				SyncPieTransactions();
			}
			else
			{
				SyncPieAmounts();
			}
		}

		private void UpdateXX(object sender, EventArgs e)
		{
			SyncRegisters();
		}

		private void UpdateZ(object sender, RoutedEventArgs e)
		{
			SyncTransactions();

			if (PieReportType.SelectedIndex == 0)
			{
				SyncPieTransactions();
			}
			else
			{
				SyncPieAmounts();
			}
		}

		private void Preset_DropDownClosed(object sender, EventArgs e)
		{
			if (Preset.SelectedIndex == 0) {
				//Custom
			}
			else if (Preset.SelectedIndex == 1)
			{
				//Weekly
				DayOfWeek day = DateTime.Now.DayOfWeek;
				int days = day - DayOfWeek.Monday;
				DateTime start = DateTime.Now.AddDays(-days);
				DateTime end = start.AddDays(6);

				MinDate.SelectedDate = start;
				MaxDate.SelectedDate = end;
			}
			else if (Preset.SelectedIndex == 2)
			{
				//Monthly
				MinDate.SelectedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"));
				MaxDate.SelectedDate = DateTime.Parse(DateTime.Now.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"));
			}
			else if (Preset.SelectedIndex == 3)
			{
				//Yearly
				MinDate.SelectedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-01-01"));
				MaxDate.SelectedDate = DateTime.Parse(DateTime.Now.ToString("yyyy-01-01")).AddMonths(13).AddDays(-1);
			}
		}
	}
}