using MahApps.Metro.SimpleChildWindow;
using System;
using MySql.Data.MySqlClient;
using System.Windows;
using System.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Spire.Pdf.Graphics;
using Spire.Pdf.Tables;
using Spire.Pdf;
using Humanizer;
using PMS.UIComponents;
using Spire.Doc;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for AddRequestWindow.xaml
	/// </summary>
	public partial class ConfirmPaymentWindow : ChildWindow
	{
		//MYSQL Related Stuff
		DBConnectionManager dbman;

		private PMSUtil pmsutil;

		private string tid;
		private string ornum;

		private DateTime cDate;
		private DateTime cTime;
		private string curDate;
		private string curTime;

		private Transactions trans1;
		private MySqlConnection conn;
		private MySqlConnection conn2;

		private System.Collections.IList _list;

		public ConfirmPaymentWindow(Transactions trans, System.Collections.IList items) {
			_list = items;
			trans1 = trans;
			pmsutil = new PMSUtil();
			InitializeComponent();

			GetInvoiceDetails2();

			ConfirmButton.Click += ConfirmPayment_Click2;
			CashTendered.ValueChanged += CashTendered_ValueChanged;
		}
		/// <summary>
		/// Creates the AddRequestForm Window and Initializes DB Param.
		/// </summary>
		public ConfirmPaymentWindow(Transactions trans, string transaction_id)
		{
			tid = transaction_id;
			trans1 = trans;
			pmsutil = new PMSUtil();
			InitializeComponent();

			GetInvoiceDetails(transaction_id);

			ConfirmButton.Click += ConfirmPayment_Click;
			CashTendered.ValueChanged += CashTendered_ValueChanged;
		}
		private void GetInvoiceDetails(string tid)
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @tid";
					cmd.Prepare();
					cmd.Parameters.AddWithValue("@tid", tid);
					using (MySqlDataReader db_reader = cmd.ExecuteReader())
					{
						while (db_reader.Read())
						{
							AmountToBePaid.Content = db_reader.GetString("fee");
						}
					}
				}
			}
		}
		private void GetInvoiceDetails2() {
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();

			double total = 0;
			for (int i = 0; i < _list.Count; i++)
			{
				Transaction ti = (Transaction)_list[i];
				total = total + ti.Fee;
			}
			AmountToBePaid.Content = total;
		}
		/// <summary>
		/// Inserts the request to the database.
		/// </summary>
		private int UpdateTransaction()
		{
			dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();
			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					string uid = Application.Current.Resources["uid"].ToString();
					string[] dt = pmsutil.GetServerDateTime().Split(null);
					cDate = Convert.ToDateTime(dt[0]);
					cTime = DateTime.Parse(dt[1] + " " + dt[2]);
					curDate = cDate.ToString("yyyy-MM-dd");
					curTime = cTime.ToString("HH:mm:ss");

					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText =
						"UPDATE transactions SET or_number = @or_number, status = @status, completion_date = @completion_date, completion_time = @completion_time, completed_by = @completed_by WHERE transaction_id = @transaction_id;";
					cmd.Parameters.AddWithValue("@transaction_id", tid);
					cmd.Parameters.AddWithValue("@status", "Paid");
					cmd.Parameters.AddWithValue("@completion_date", cDate);
					cmd.Parameters.AddWithValue("@completion_time", cTime);
					cmd.Parameters.AddWithValue("@completed_by", uid);
					cmd.Parameters.AddWithValue("@or_number", ornum);
					cmd.Prepare();
					int stat_code = cmd.ExecuteNonQuery();
					conn.Close();
					return stat_code;
				}
			}
			return 0;
		}
		private bool CheckInputs()
		{
			bool ret = true;

			var bc = new BrushConverter();

			if (string.IsNullOrWhiteSpace(CashTendered.Value.ToString()) || CashTendered.Value < 0 || CashTendered.Value < Double.Parse(AmountToBePaid.Content.ToString()))
			{
				CashTendered.ToolTip = "Cannot be less than zero/amount to pay!.";
				CashTendered.BorderBrush = Brushes.Red;
				CashTenderedIcon.BorderBrush = Brushes.Red;

				ret = false;
			}

			return ret;
		}
		private void ConfirmPayment_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			if (CheckInputs() == true) {
				//dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();

				int amnt = int.Parse(AmountToBePaid.Content.ToString());
				string OR = pmsutil.GenerateReceiptNum();

				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						MySqlCommand cmd = conn.CreateCommand();
						cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
						cmd.Parameters.AddWithValue("@transaction_id", tid);
						cmd.Prepare();
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							if (db_reader.GetString("status") == "Unpaid")
							{
								ornum = OR;
								using (conn2 = new MySqlConnection(dbman.GetConnStr()))
								{
									conn2.Open();
									MySqlCommand cmd2 = conn2.CreateCommand();
									cmd2.CommandText = "SELECT COUNT(*) FROM transactions WHERE or_number = @or_number;";
									cmd2.Parameters.AddWithValue("@or_number", OR);
									cmd2.Prepare();
									int counter = int.Parse(cmd2.ExecuteScalar().ToString());
									if (counter > 0)
									{
										InfoArea.Foreground = new SolidColorBrush(Colors.Red);
										InfoArea.Content = "Duplicate Receipt Number Detected!";
									}
									else
									{
										UpdateTransaction();
										trans1.SyncTransactions();
										MsgSuccess();
										this.Close();
									}
								}
							}
						}
					}
				}

				Document doc = new Document();
				doc.LoadFromFile("Data\\temp_receipt.docx");
				doc.Replace("date", DateTime.Now.ToString("MM/dd/yyyy"), true, true);
				doc.Replace("name", ReceivedFrom.Text, true, true);
				doc.Replace("wordsum", amnt.ToWords(), true, true);
				doc.Replace("amount", AmountToBePaid.Content.ToString(), true, true);
				doc.Replace("Total1", "P " + AmountToBePaid.Content.ToString(), true, true);
				doc.Replace("cashier", pmsutil.GetEmpName(Application.Current.Resources["uid"].ToString()), true, true);
				doc.Replace("123456", OR, true, true);

				////Transactions
				dbman = new DBConnectionManager();
				pmsutil = new PMSUtil();
				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					int count1 = 1;
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						MySqlCommand cmd = conn.CreateCommand();
						cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
						cmd.Parameters.AddWithValue("@transaction_id", tid);
						cmd.Prepare();
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							doc.Replace("Item" + count1, db_reader.GetString("type"), true, true);
							doc.Replace("Amount" + count1, db_reader.GetString("fee"), true, true);
							count1++;
						}
					}
				}

				string fname = "Receipt-No-" + OR + "-" + DateTime.Now.ToString("MMM_dd_yyyy") + ".pdf";
				doc.SaveToFile("Data\\print_receipt.docx", Spire.Doc.FileFormat.Docx);

				//Load Document
				Document document = new Document();
				document.LoadFromFile(@"Data\\print_receipt.docx");

				//Convert Word to PDF
				document.SaveToFile("Output\\" + fname, Spire.Doc.FileFormat.PDF);

				System.Diagnostics.Process.Start("Output\\" + fname);

			}
			else {

			}
		}
		private void ConfirmPayment_Click2(object sender, System.Windows.RoutedEventArgs e)
		{
			//dbman = new DBConnectionManager();
			pmsutil = new PMSUtil();

			int amnt = int.Parse(AmountToBePaid.Content.ToString());
			string OR = pmsutil.GenerateReceiptNum();

			using (conn = new MySqlConnection(dbman.GetConnStr()))
			{
				conn.Open();
				if (conn.State == ConnectionState.Open)
				{
					MySqlCommand cmd = conn.CreateCommand();
					cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
					cmd.Parameters.AddWithValue("@transaction_id", tid);
					cmd.Prepare();
					MySqlDataReader db_reader = cmd.ExecuteReader();
					while (db_reader.Read())
					{
						if (db_reader.GetString("status") == "Unpaid")
						{
							ornum = OR;
							using (conn2 = new MySqlConnection(dbman.GetConnStr()))
							{
								conn2.Open();
								MySqlCommand cmd2 = conn2.CreateCommand();
								cmd2.CommandText = "SELECT COUNT(*) FROM transactions WHERE or_number = @or_number;";
								cmd2.Parameters.AddWithValue("@or_number", OR);
								cmd2.Prepare();
								int counter = int.Parse(cmd2.ExecuteScalar().ToString());
								if (counter > 0)
								{
									InfoArea.Foreground = new SolidColorBrush(Colors.Red);
									InfoArea.Content = "Duplicate Receipt Number Detected!";
								}
								else
								{
									UpdateTransaction();
									trans1.SyncTransactions();
									MsgSuccess();
									this.Close();
								}
							}
						}
					}
				}
			}

			Document doc = new Document();
			doc.LoadFromFile("Data\\temp_receipt.docx");
			doc.Replace("date", DateTime.Now.ToString("MM/dd/yyyy"), true, true);
			doc.Replace("name", ReceivedFrom.Text, true, true);
			doc.Replace("wordsum", amnt.ToWords(), true, true);
			doc.Replace("amount", AmountToBePaid.Content.ToString(), true, true);
			doc.Replace("Total1", "P " + AmountToBePaid.Content.ToString(), true, true);
			doc.Replace("cashier", pmsutil.GetEmpName(Application.Current.Resources["uid"].ToString()), true, true);
			doc.Replace("123456", OR, true, true);
			
			int count1 = 1;
			for (int i = 0; i < _list.Count; i++)
			{
				Transaction ti = (Transaction)_list[i];

				using (conn = new MySqlConnection(dbman.GetConnStr()))
				{
					conn.Open();
					if (conn.State == ConnectionState.Open)
					{
						MySqlCommand cmd = conn.CreateCommand();
						cmd.CommandText = "SELECT * FROM transactions WHERE transaction_id = @transaction_id LIMIT 1;";
						cmd.Parameters.AddWithValue("@transaction_id", ti.TransactionID);
						cmd.Prepare();
						MySqlDataReader db_reader = cmd.ExecuteReader();
						while (db_reader.Read())
						{
							if (db_reader.GetString("status") == "Unpaid")
							{
								tid = ti.TransactionID;
								ornum = OR;

								doc.Replace("Item" + count1, db_reader.GetString("type"), true, true);
								doc.Replace("Amount" + count1, db_reader.GetString("fee"), true, true);
								count1++;

								UpdateTransaction();
							}
						}
					}
				}
			}
			for (int xtemp = 12; count1 <= xtemp; count1++) {
				doc.Replace("Item" + count1, "", true, true);
				doc.Replace("Amount" + count1, "", true, true);
			}

			string fname = "Receipt-No-" + OR + "-" + DateTime.Now.ToString("MMM_dd_yyyy") + ".pdf";
			doc.SaveToFile("Data\\print_receipt.docx", Spire.Doc.FileFormat.Docx);

			//Load Document
			Document document = new Document();
			document.LoadFromFile(@"Data\\print_receipt.docx");

			//Convert Word to PDF
			document.SaveToFile("Output\\" + fname, Spire.Doc.FileFormat.PDF);

			System.Diagnostics.Process.Start("Output\\" + fname);
			this.Close();
			trans1.SyncTransactions();
		}
		private void GenReceipt() {
			int amnt = int.Parse(AmountToBePaid.Content.ToString());
			string OR = pmsutil.GenerateReceiptNum();

			PdfDocument pdfDoc = new PdfDocument();
			PdfPageBase page = pdfDoc.Pages.Add();
			PdfStringFormat format1 = new PdfStringFormat(PdfTextAlignment.Center);

			page.Canvas.DrawString("ROMAN CATHOLIC BISHOP OF LEGAZPI, INC.",
			new PdfFont(PdfFontFamily.TimesRoman, 14f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black), 240, 0, format1);

			page.Canvas.DrawString("\n\nThe Chancery, Cathedral Compound,\nAlbay District. Legazpi City 4500\nTel. No. (052) 481-2178 . NON-VAT Reg.TIN No. 000-636-377-000",
			new PdfFont(PdfFontFamily.Helvetica, 10f),
			new PdfSolidBrush(System.Drawing.Color.Black), 240, 0, format1);

			page.Canvas.DrawString("OFFICIAL RECEIPT",
			new PdfFont(PdfFontFamily.TimesRoman, 18f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black), 40, 75);

			page.Canvas.DrawString("\nDate: _______________",
			new PdfFont(PdfFontFamily.Helvetica, 10f),
			new PdfSolidBrush(System.Drawing.Color.Black), 285, 70);

			page.Canvas.DrawString("Received From " + ReceivedFrom.Text +
									"\nwith TIN__________________and address at ___________________________" +
									"\nengaged in the business style of______________________________________" +
									"\nthe sum of__________________________________________________ pesos" +
									"\n(P___________) in parial/full payment of the following:",

			new PdfFont(PdfFontFamily.Helvetica, 10f),
			new PdfSolidBrush(System.Drawing.Color.Black), 40, 100);


			String[] data =
			{
				"For; ; ",
				" ;P; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				" ; ; ",
				"Total Sales; ; ",
				"Less: SC/PWD Discount; ; ",
				"Total Due; ; ",
				"Thank you        TOTAL; ; ",
			};
			String[][] dataSource = new String[data.Length][];
			for (int i = 0; i < data.Length; i++)
			{
				dataSource[i] = data[i].Split(';');
			}
			PdfTable table = new PdfTable();


			table.Style.CellPadding = 2;
			table.DataSource = dataSource;
			float width = page.Canvas.ClientSize.Width - (table.Columns.Count + 1);
			table.Columns[0].Width = width * 0.1f * width;
			table.Columns[0].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[1].Width = width * 0.02f * width;
			table.Columns[1].StringFormat = new PdfStringFormat(PdfTextAlignment.Left, PdfVerticalAlignment.Middle);
			table.Columns[2].Width = width * 0.03f * width;
			table.Columns[2].StringFormat = new PdfStringFormat(PdfTextAlignment.Center, PdfVerticalAlignment.Middle);
			table.Draw(page, new System.Drawing.PointF(40, 170));

			page.Canvas.DrawString("Form of Payment:                        __ Cash    __ Check\n" +
				"Check No.:_______________Bank:_______________\n" +
				"Sr. Citizen TIN\n" +
				"OSCA/PWD ID No.________________Signature_______________",
			new PdfFont(PdfFontFamily.Helvetica, 10f),
			new PdfSolidBrush(System.Drawing.Color.Black), 40, 420);

			page.Canvas.DrawString("No. 161455",
			new PdfFont(PdfFontFamily.TimesRoman, 18f, PdfFontStyle.Bold),
			new PdfSolidBrush(System.Drawing.Color.Black), 40, 475);

			page.Canvas.DrawString("by:",
			new PdfFont(PdfFontFamily.Helvetica, 10f),
			new PdfSolidBrush(System.Drawing.Color.Black), 200, 470); ;

			page.Canvas.DrawString("__________________________",
			new PdfFont(PdfFontFamily.Helvetica, 10f),
			new PdfSolidBrush(System.Drawing.Color.Black), 200, 500); ;

			page.Canvas.DrawString("Cashier",
			new PdfFont(PdfFontFamily.Helvetica, 10f, PdfFontStyle.Italic),
			new PdfSolidBrush(System.Drawing.Color.Black), 250, 510); ;

			string fname = "test.pdf";
			pdfDoc.SaveToFile(@"..\..\" + fname);
			//MessageBox.Show("yey");
			System.Diagnostics.Process.Start(@"..\..\" + fname);
		}
		private async void MsgSuccess()
		{
			var metroWindow = (Application.Current.MainWindow as MetroWindow);
			await metroWindow.ShowMessageAsync("Success!", "The selected transaction has been successfully confirmed.");
		}
		/// <summary>
		/// Closes the AddRequestForm Window.
		/// </summary>
		private void CancelButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}

		private void CashTendered_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
		{
			double? changeVal = CashTendered.Value - double.Parse(AmountToBePaid.Content.ToString());

			if (changeVal < 0) {
				changeVal = 0;
			}
			Change.Content = string.Format("{0:N2}", (changeVal));
		}
	}
}
