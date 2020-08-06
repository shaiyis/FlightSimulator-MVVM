using FlightSimulatorApp.Model.Interface;
using System.ComponentModel;


namespace FlightSimulatorApp.ViewModel
{
    public class ConnectWindowViewModel : INotifyPropertyChanged
    {
        private readonly ISimulatorModel Model;

        public ConnectWindowViewModel(ISimulatorModel model)
        {
            Model = model;
        }


        public string VMFlightServerIP
        {
            get { return Model.FlightServerIP; }
            set { Model.FlightServerIP = value; }
        }

        public string VMFlightInfoPort
        {
            get { return Model.FlightInfoPort; }
            set { Model.FlightInfoPort = value; }
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
