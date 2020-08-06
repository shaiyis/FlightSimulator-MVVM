using FlightSimulatorApp.Model;
using FlightSimulatorApp.Model.Interface;
using FlightSimulatorApp.ViewModel;
using System.Windows;

namespace FlightSimulatorApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
      
        public DashBoardViewModel DashVM { get; internal set; }
        public ControlsViewModel ControlsViewModel { get; internal set; }
        public MapViewModel MapViewModel { get; internal set; }
        public ErrorViewModel ErrorViewModel { get; internal set; }
        public ISimulatorModel Model { get; internal set; }
        public ConnectWindowViewModel ConnectViewModel { get; internal set; }

        void App_Startup(object sender, StartupEventArgs e)
        {
            Model = new MySimulatorModel();
            DashVM = new DashBoardViewModel(Model);
            MapViewModel = new MapViewModel(Model);
            ControlsViewModel = new ControlsViewModel(Model);
            ConnectViewModel = new ConnectWindowViewModel(Model);
            ErrorViewModel = new ErrorViewModel(Model);
        }


    }
}
