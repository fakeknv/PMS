using PMS.UIComponents;
using System.Windows.Controls;
using System.Windows.Media;

namespace PMS.UIManager.Views
{
	/// <summary>
	/// Interaction logic for Registers.xaml
	/// </summary>
	public partial class Registers : UserControl
	{
		public Registers()
		{
			InitializeComponent();
			for (int i=0; i<10; i++) { 
			RegisterItem ri = new RegisterItem();
			test1.Items.Add(ri);
			}
		}

		private void CreateRequestButton_Click(object sender, System.Windows.RoutedEventArgs e)
		{

		}
	}
}