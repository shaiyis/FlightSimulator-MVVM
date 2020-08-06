using System;
using System.Windows;
using System.Windows.Controls;


namespace FlightSimulatorApp.Views
{
    /// <summary>
    /// Interaction logic for ThrottleSlider.xaml
    /// </summary>
    public partial class ThrottleSlider : UserControl
    {
        public ThrottleSlider()
        {
            InitializeComponent();
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Show value of the Throttle.
            double val = Convert.ToDouble(e.NewValue);
            string msg = String.Format("Throttle: {0}", Math.Round(val, 2));
            this.TheValue.Text = msg;
            ThrottleValue = val;
        }

        public double ThrottleValue
        {
            get { return (double)GetValue(ThrottleValueProperty); }
            set
            {
                SetValue(ThrottleValueProperty, value);
            }
        }

        public static readonly DependencyProperty ThrottleValueProperty =
            DependencyProperty.Register("ThrottleValue", typeof(double), typeof(ThrottleSlider));
    }
}
