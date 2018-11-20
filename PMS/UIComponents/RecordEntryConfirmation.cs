using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UIComponents
{
	class RecordEntryConfirmation
	{
		public string RecordID { get; set; }
		public int EntryNumber { get; set; }
		public string ConfirmationYear { get; set; }
		public string ConfirmationDate { get; set; }
		public string FullName { get; set; }
		public int Age { get; set; }
		public string Parish { get; set; }
		public string Province { get; set; }
		public string PlaceOfBaptism { get; set; }
		public string Parent1 { get; set; }
		public string Parent2 { get; set; }
		public string Sponsor1 { get; set; }
		public string Sponsor2 { get; set; }
		public float Stipend { get; set; }
		public string Minister { get; set; }
	}
}
