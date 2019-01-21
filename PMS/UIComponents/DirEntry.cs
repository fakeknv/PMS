using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.UIComponents
{
	class DirEntry
	{
		public string DirectoryID { get; set; }
		public string RecordID { get; set; }
		public string Lot { get; set; }
		public string Plot { get; set; }
		public string FName { get; set; }
		public string PlaceOfInterment { get; set; }
		public string BurialDate { get; set; }
		public int Page { get; set; }
	}
}
