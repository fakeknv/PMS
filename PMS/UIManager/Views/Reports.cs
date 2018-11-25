using MySql.Data.MySqlClient;
using PMS.UIComponents;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PMS.UIManager.Views
{

	/// <summary>
	/// Interaction logic for Settings.xaml
	/// </summary>
	public partial class Reports : UserControl
	{
		public class Entries
		{
			public string ReceiptNo { get; set; }
			public string Name { get; set; }
			public float Amount { get; set; }
			public string EnvelopeNo { get; set; }
		}
		public class Books
		{
			public string BookNo { get; set; }
			public string BookType { get; set; }
			public int Pages { get; set; }
			public int Records { get; set; }
			public string Status { get; set; }
		}
		public Reports()
		{
			InitializeComponent();
			List<Entries> tmp = new List<Entries>();

			tmp.Add(new Entries() { ReceiptNo = 36651.ToString(), Name = "Paul Canriba", Amount = 500, EnvelopeNo = "November 16, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36652.ToString(), Name = "World Apostale of Fatima", Amount = 500, EnvelopeNo = "November 16, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36653.ToString(), Name = "Malayan Insurance Inc.", Amount = 1000, EnvelopeNo = "November 16, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36654.ToString(), Name = "Mr. and Mrs. Macario S. Sayson", Amount = 500, EnvelopeNo = "November 16, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36655.ToString(), Name = "Engr. & Mrs. Marsel R. Sayson", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36656.ToString(), Name = "Ms. & Mr. Bella Sayson", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36657.ToString(), Name = "Mr. and Mrs. Macario R. Sayson", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36658.ToString(), Name = "Mr. & Ms. Dominic P. Nunez", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36659.ToString(), Name = "Mr. & Ms. Eulogio Rodriguez Jr.", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36660.ToString(), Name = "Atty. & Mrs. Randy M. Padua", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36661.ToString(), Name = "Atty. & Mrs. Randy M. Padua", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36662.ToString(), Name = "A. R. Sayson & Associates, CPAS", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36663.ToString(), Name = "Engr. Fabian V. Aguilar", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36664.ToString(), Name = "Ms. Ave Florinda A. Buban", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36665.ToString(), Name = "Mr. & Mrs. Macerio S. Sayson", Amount = 900, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36666.ToString(), Name = "Danny Raguion", Amount = 300, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36667.ToString(), Name = "Danny Raguion", Amount = 700, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36668.ToString(), Name = "Normy delos Santos", Amount = 1000, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36669.ToString(), Name = "Mr. & Mrs. Endozo", Amount = 500, EnvelopeNo = "November 17, 2018" });
			tmp.Add(new Entries() { ReceiptNo = 36670.ToString(), Name = "PB Glenn & Mayet Barcelon", Amount = 500, EnvelopeNo = "November 17, 2018" });
			BillingDG.ItemsSource = tmp;

			List<Books> tmp2 = new List<Books>();
			tmp2.Add(new Books() { BookNo = 1.ToString(), BookType = "Baptismal", Pages = 50, Records = 503, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 2.ToString(), BookType = "Confirmation", Pages = 138, Records = 1385, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 3.ToString(), BookType = "Matrimonial", Pages = 1, Records = 10, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 4.ToString(), BookType = "Burial", Pages = 9, Records = 90, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 5.ToString(), BookType = "Confirmation", Pages = 200, Records = 1899, Status = "Archived" });
			tmp2.Add(new Books() { BookNo = 6.ToString(), BookType = "Burial", Pages = 1, Records = 10, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 7.ToString(), BookType = "Baptismal", Pages = 25, Records = 253, Status = "Archived" });
			tmp2.Add(new Books() { BookNo = 8.ToString(), BookType = "Confirmation", Pages = 60, Records = 1200, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 9.ToString(), BookType = "Matrimonial", Pages = 1, Records = 10, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 10.ToString(), BookType = "Matrimonial", Pages = 1, Records = 10, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 11.ToString(), BookType = "Matrimonial", Pages = 1, Records = 10, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 12.ToString(), BookType = "Matrimonial", Pages = 1, Records = 10, Status = "Normal" });
			tmp2.Add(new Books() { BookNo = 13.ToString(), BookType = "Matrimonial", Pages = 50, Records = 502, Status = "Archived" });
			ArchiveDG.ItemsSource = tmp2;
		}
	}
}