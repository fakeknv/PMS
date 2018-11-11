using MahApps.Metro.SimpleChildWindow;
using MySql.Data.MySqlClient;
using System.Data;

namespace PMS.UIManager.Views.ChildWindows
{
	/// <summary>
	/// Interaction logic for ViewRemarksWindow.xaml
	/// </summary>
	public partial class ViewRemarksWindow : ChildWindow
	{
		//MYSQL
		private DBConnectionManager dbman;

		public ViewRemarksWindow(string recordID)
        {
            InitializeComponent();

			dbman = new DBConnectionManager();

			if (dbman.DBConnect().State == ConnectionState.Open)
			{
				MySqlCommand cmd = dbman.DBConnect().CreateCommand();
				cmd.CommandText = "SELECT * FROM baptismal_records, matrimonial_records, confirmation_records, burial_records WHERE burial_records.record_id = @record_id OR baptismal_records.record_id = @record_id OR matrimonial_records.record_id = @record_id OR confirmation_records.record_id = @record_id LIMIT 1;";
				cmd.Parameters.AddWithValue("@record_id", recordID);
				cmd.Prepare();
				MySqlDataReader db_reader = cmd.ExecuteReader();
				while (db_reader.Read())
				{
					if (db_reader.GetString("remarks") == "---") {

					}
					else {
						RemarksContainer.Text = db_reader.GetString("remarks");
					}
				}
				//close Connection
				dbman.DBClose();
			}
			else
			{

			}
		}

		private void Close_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			this.Close();
		}
	}
}
