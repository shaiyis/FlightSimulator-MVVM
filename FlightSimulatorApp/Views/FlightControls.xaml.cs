using System.Windows.Controls;


namespace FlightSimulatorApp.Views
{
    /// <summary>
    /// Interaction logic for FlightControls.xaml
    /// </summary>
    public partial class FlightControls : UserControl
    {
        public FlightControls()
        {
            InitializeComponent();
            // For the binding of the text boxes of Rudder and Elevator.
            RudderValueText.DataContext = MyJoystick;
            ElevatorValueText.DataContext = MyJoystick;

        }
    }
}
