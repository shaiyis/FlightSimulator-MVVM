using System.ComponentModel;


namespace FlightSimulatorApp.Model.Interface
{
	public interface ISimulatorModel : INotifyPropertyChanged
	{
		void Connect(string Ip, int Port);
		void Disconnect();
		void Start();


		// The IP Of the Flight Server.
		string FlightServerIP { get; set; }

		// The Port of the Flight Server
		string FlightInfoPort { get; set; }            

		// Dashboard.

		double IndicatedHeadingDeg { set; get; }
		double GpsIndicatedVerticalSpeed { set; get; }
		double GpsIndicatedGroundSpeedKt { set; get; }
		double AirspeedIndicatorIndicatedSpeedKt  { set; get; }
		double GpsIndicatedAltitudeFt { set; get; }
		double AttitudeIndicatorInternalRollDeg { set; get; }
		double AttitudeIndicatorInternalPitchDeg { set; get; }
		double AltimeterIndicatedAltitudeFt { set; get; }


		// Map. 
		double Longitude { get; }
		double Latitude { get; }
		string Coordinates { get; }
		string Place { set; get; }



		// Set controls. 

		double Aileron { set; }
		double Elevator { set; }
		double Rudder { set; }
		double Throttle { set; }

		// Error. 
		string Error { set; get; }
	}

}

