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

namespace PMS.UIManager.Views.ChildViews
{
    /// <summary>
    /// Interaction logic for ViewRecordEntries.xaml
    /// </summary>
    public partial class ViewRecordEntries : UserControl
    {
        public ViewRecordEntries(int bookNum)
        {
            InitializeComponent();
		}

		private void BackButton_Click(object sender, RoutedEventArgs e)
		{
			this.Content = new Registers();
		}
	}
}
