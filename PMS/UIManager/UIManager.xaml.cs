using System;
using System.Windows;
using MahApps.Metro.Controls;

namespace PMS.UIManager
{
    /// <summary>
    /// Interaction logic for Registrar.xaml
    /// </summary>
    public partial class UIManager : MetroWindow
    {
        public UIManager()
        {
			Login li = new Login();
			if (Application.Current.Resources["uid"] == null)
			{
				this.Hide();
				li.ShowDialog();
			}
			if (Application.Current.Resources["uid"] != null)
			{
				//WindowStartupLocation = System.Windows.WindowStartupLocation.CenterScreen;
				this.Top = 10;
				this.Left = 80;
				this.Show();
			}
			InitializeComponent();
			InitPriv();
		}
		private void InitPriv() {
			HamburgerMenuItemCollection itemCollection = HamburgerMenuControl.ItemsSource as HamburgerMenuItemCollection;
			
			if (Convert.ToInt32(Application.Current.Resources["priv"]) == 1) {
				//Admin
				itemCollection.Remove(Transactions);
				itemCollection.Remove(Registers);
				itemCollection.Remove(Directory);
				itemCollection.Remove(Appointments);
				itemCollection.Remove(Search);
			}
			else if (Convert.ToInt32(Application.Current.Resources["priv"]) == 2)
			{
				//Secretary
				itemCollection.Remove(Transactions);
				itemCollection.Remove(Registers);
				itemCollection.Remove(Directory);
				itemCollection.Remove(Archives);
				itemCollection.Remove(Reports);
				itemCollection.Remove(Accounts);
				itemCollection.Remove(Priests);
				itemCollection.Remove(Timeslots);
				itemCollection.Remove(AppointmentTypes);
				itemCollection.Remove(Search);
				itemCollection.Remove(Settings);
			}
			else if (Convert.ToInt32(Application.Current.Resources["priv"]) == 3)
			{
				//Cashier
				itemCollection.Remove(Registers);
				itemCollection.Remove(Directory);
				itemCollection.Remove(Archives);
				itemCollection.Remove(Reports);
				itemCollection.Remove(Accounts);
				itemCollection.Remove(Priests);
				itemCollection.Remove(Timeslots);
				itemCollection.Remove(AppointmentTypes);
				itemCollection.Remove(Search);
				itemCollection.Remove(Settings);
			}
			else if (Convert.ToInt32(Application.Current.Resources["priv"]) == 4)
			{
				//Registrar
				itemCollection.Remove(Transactions);
				itemCollection.Remove(Registers);
				itemCollection.Remove(Directory);
				itemCollection.Remove(Archives);
				itemCollection.Remove(Reports);
				itemCollection.Remove(Accounts);
				itemCollection.Remove(Priests);
				itemCollection.Remove(Timeslots);
				itemCollection.Remove(AppointmentTypes);
				itemCollection.Remove(Settings);
			}
		}
        private void HamburgerMenuControl_OnItemClick(object sender, ItemClickEventArgs e)
        {
            // set the content
            this.HamburgerMenuControl.Content = e.ClickedItem;
            // close the pane
            this.HamburgerMenuControl.IsPaneOpen = false;
        }
    }
}
