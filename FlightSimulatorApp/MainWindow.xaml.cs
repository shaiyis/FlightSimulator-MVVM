using FlightSimulatorApp.ViewModel;
using FlightSimulatorApp.Views;
using System.Windows;


namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
     
        public MainWindow()
        {
            InitializeComponent();

            // Data Context is the ViewModels that were instaniated at app start-up.
            Warning.DataContext = (Application.Current as App).ErrorViewModel;
            myDashBoard.DataContext = (Application.Current as App).DashVM;
            MyControls.DataContext = (Application.Current as App).ControlsViewModel;
            MapViewModel mapVm = (Application.Current as App).MapViewModel;
            GameMap.DataContext = mapVm;
            longitudeText.DataContext = mapVm;
            latitudeText.DataContext = mapVm;
            placeText.DataContext = mapVm;
            disconnectButton.IsEnabled = false;
            MyControls.IsEnabled = false;
            GameMap.IsEnabled = false;
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            // Open connection window - at first with default IP and Port, or if changed the changed values.
            ConnectWindow cW = new ConnectWindow()
            {
                DataContext = (Application.Current as App).ConnectViewModel
            };
            cW.Show();
            connectButton.IsEnabled = false;
            disconnectButton.IsEnabled = true;
            MyControls.IsEnabled = true;
            GameMap.IsEnabled = true;
        }

        private void DisconnectButton_Click(object sender, RoutedEventArgs e)
        {
            //move sliders to 0
            MyControls.MyThrottleSlider.MySlider.Value = 0;
            MyControls.MyAileronSlider.MySlider.Value = 0;

            // Disconnects from the server.
            (Application.Current as App).Model.Disconnect();
            disconnectButton.IsEnabled = false;
            connectButton.IsEnabled = true;
            MyControls.IsEnabled = false;
            GameMap.IsEnabled = false;
        }
    }
}
