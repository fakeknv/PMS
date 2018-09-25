using System;
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
