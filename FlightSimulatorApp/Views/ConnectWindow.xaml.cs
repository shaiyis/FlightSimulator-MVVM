using System.Windows;
using System.Windows.Controls;


namespace FlightSimulatorApp.Views
{
    /// <summary>
    /// Interaction logic for ConnectWindow.xaml
    /// </summary>
    public partial class ConnectWindow : Window
    {
       

        public ConnectWindow () 
        {
            InitializeComponent();
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Update IP and Port when connect button is clicked.
            IpText.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            PortText.GetBindingExpression(TextBox.TextProperty).UpdateSource();
            this.Close();
        }
    }
}
