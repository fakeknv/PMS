using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PMS.UIComponents
{
    /// <summary>
    /// Interaction logic for Day.xaml
    /// </summary>
    public partial class Day : UserControl
    {
		private DateTime date;
		private string notes;
		private bool enabled;
		private bool isTargetMonth;
		private bool isToday;

		public bool IsToday
		{
			get { return isToday; }
			set
			{
				isToday = value;
			}
		}

		public bool IsTargetMonth
		{
			get { return isTargetMonth; }
			set
			{
				isTargetMonth = value;
			}
		}

		public bool Enabled
		{
			get { return enabled; }
			set
			{
				enabled = value;
			}
		}

		public string Notes
		{
			get { return notes; }
			set
			{
				notes = value;
			}
		}

		public DateTime Date
		{
			get { return date; }
  			set {
				DayHolder.Content = value.ToString("dd");
				DateVal.Content = value.ToString("MM/dd/yyyy");
			}
		}
		public Day()
        {
            InitializeComponent();
        }

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(this.DateVal.Content.ToString());
		}
	}
}
