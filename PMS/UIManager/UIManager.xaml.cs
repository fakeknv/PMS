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
			if (li.userID == null)
			{
				this.Hide();
				li.ShowDialog();
			}
			if (li.userID != null)
			{
				this.Show();
			}
			InitializeComponent();
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
