using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UIComponents
{
	class Transaction
	{
		public int No { get; set; }
		public string TransactionID { get; set; }
		public string Type { get; set; }
		public string Name { get; set; }
		public double Fee { get; set; }
		public string Status { get; set; }
		public string ORNumber { get; set; }
		public string DatePlaced { get; set; }
		public string TimePlaced { get; set; }
		public string DateConfirmed { get; set; }
		public string TimeConfirmed { get; set; }
		public int Page { get; set; }
	}
}
