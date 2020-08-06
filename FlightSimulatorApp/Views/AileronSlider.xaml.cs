using System;
using System.Windows;
using System.Windows.Controls;

namespace FlightSimulatorApp.Views
{
    /// <summary>
    /// Interaction logic for AileronSlider.xaml
    /// </summary>
    public partial class AileronSlider : UserControl
    {
        public AileronSlider()
        {
            InitializeComponent();
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            // Show value of the Aileron.
            double val = Convert.ToDouble(e.NewValue);
            string msg = String.Format("Aileron: {0}",Math.Round(val,2));
            this.TheValue.Text = msg;
            AileronValue = val;
        }

        public double AileronValue
        {
            get { return (double)GetValue(AileronValueProperty); }
            set
            {
                SetValue(AileronValueProperty, value);
            }
        }

        public static readonly DependencyProperty AileronValueProperty =
            DependencyProperty.Register("AileronValue", typeof(double), typeof(AileronSlider));
    }
}

