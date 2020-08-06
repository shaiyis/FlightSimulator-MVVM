using FlightSimulatorApp.Model.Interface;


namespace FlightSimulatorApp.ViewModel
{
	public class ControlsViewModel
	{
		private readonly ISimulatorModel SimulatorModel;

		public ControlsViewModel (ISimulatorModel simulatorModel)
		{
			SimulatorModel = simulatorModel;
		}
        public double VMAileron
        {
            set { SimulatorModel.Aileron = value; }
        }
        public double VMElevator
        {
            set { SimulatorModel.Elevator = value; }
        }
        public double VMRudder
        {
            set { SimulatorModel.Rudder = value; }
        } 
        public double VMThrottle
        {
            set { SimulatorModel.Throttle = value; }
        }
    }
}

