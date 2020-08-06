using FlightSimulatorApp.Model.Interface;
using System;
using System.ComponentModel;

namespace FlightSimulatorApp.ViewModel
{
	public class MapViewModel : INotifyPropertyChanged
    {
        private readonly ISimulatorModel SimulatorModel;

        public MapViewModel (ISimulatorModel simulatorModel)
        {
            this.SimulatorModel = simulatorModel;
            SimulatorModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
            {
                NotifyPropertyChanged("VM" + e.PropertyName);
            };
        }

        public string VMCoordinates
        {
            get
            {
                return SimulatorModel.Coordinates;
            }
        }

        public double VMLongitude

        {
            get
            {
                return SimulatorModel.Longitude;
            }
        }

        public double VMLatitude
        {
            get
            {
                return SimulatorModel.Latitude;
            }
        }
        public string VMPlace
        {
            get
            {
                return SimulatorModel.Place;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

    }
}
