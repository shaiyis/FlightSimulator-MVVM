using FlightSimulatorApp.Model.Interface;
using System;
using System.ComponentModel;


namespace FlightSimulatorApp.ViewModel
{
    public class ErrorViewModel : INotifyPropertyChanged
    {
        private readonly ISimulatorModel SimulatorModel;

        public ErrorViewModel (ISimulatorModel simulatorModel)
        {
            this.SimulatorModel = simulatorModel;
            SimulatorModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM" + e.PropertyName);
            };
        }

        public string VMError
        {
            get
            {
                return SimulatorModel.Error;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
