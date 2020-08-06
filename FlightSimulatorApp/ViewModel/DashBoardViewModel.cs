using System;
using System.ComponentModel;
using FlightSimulatorApp.Model.Interface;

namespace FlightSimulatorApp.ViewModel
{
	public class DashBoardViewModel : INotifyPropertyChanged
	{
		private readonly ISimulatorModel SimulatorModel;

		public DashBoardViewModel(ISimulatorModel simulatorModel)
		{
			this.SimulatorModel = simulatorModel;
			SimulatorModel.PropertyChanged += delegate (Object sender, PropertyChangedEventArgs e)
			{
				NotifyPropertyChanged("VM" + e.PropertyName);
			};
		}
		
		public double VMIndicatedHeadingDeg
		{
			get{ return SimulatorModel.IndicatedHeadingDeg; }
		}
		public double VMGpsIndicatedVerticalSpeed
		{
			get { return SimulatorModel.GpsIndicatedVerticalSpeed; }
		}
		public double VMGpsIndicatedGroundSpeedKt
		{
			get { return SimulatorModel.GpsIndicatedGroundSpeedKt; }
		}
		public double VMAirspeedIndicatorIndicatedSpeedKt
		{
			get { return SimulatorModel.AirspeedIndicatorIndicatedSpeedKt; }
		}
		public double VMGpsIndicatedAltitudeFt
		{
			get { return SimulatorModel.GpsIndicatedAltitudeFt; }
		}
		public double VMAttitudeIndicatorInternalRollDeg
		{
			get { return SimulatorModel.AttitudeIndicatorInternalRollDeg; }
		}
		public double VMAttitudeIndicatorInternalPitchDeg
		{
			get { return SimulatorModel.AttitudeIndicatorInternalPitchDeg; }
		}
		public double VMAltimeterIndicatedAltitudeFt
		{
			get { return SimulatorModel.AltimeterIndicatedAltitudeFt; }
		}
		

		public event PropertyChangedEventHandler PropertyChanged;

		public void NotifyPropertyChanged(string propName)
		{
			this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
		}

	}

}

