using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;


namespace FlightSimulatorApp.Views
{
    /// <summary>
    /// Interaction logic for Joystick.xaml.
    /// </summary>
    public partial class Joystick : UserControl
    {
        // Looked at an open-source on https://github.com/shakram02/XamlVirtualJoystick/blob/master/WpfCustomControls/OnScreenJoystick.xaml.cs .
        public static readonly DependencyProperty XProperty =
            DependencyProperty.Register("XVal", typeof(double), typeof(Joystick), null);


        public static readonly DependencyProperty YProperty =
            DependencyProperty.Register("YVal", typeof(double), typeof(Joystick), null);

        public double XVal
        {
            get { return Convert.ToDouble(GetValue(XProperty)); }
            set { SetValue(XProperty, value); }
        }

        public double YVal
        {
            get { return Convert.ToDouble(GetValue(YProperty)); }
            set { SetValue(YProperty, value); }
        }

        public delegate void EmptyJoystickEventHandler(Joystick sender);

        public event EmptyJoystickEventHandler Released;

        public event EmptyJoystickEventHandler Captured;

        private Point StartPos;
        private double CanvasWidth, CanvasHeight;
        private readonly Storyboard CenterKnob;
        private const double MaxValX = 170;
        private const double MinValX = -170;
        private const double MaxValY = 170;
        private const double MinValY = -170;



        public Joystick()
        {
            InitializeComponent();

            Knob.MouseLeftButtonDown += Knob_MouseLeftButtonDown;
            Knob.MouseLeftButtonUp += Knob_MouseLeftButtonUp;
            Knob.MouseMove += Knob_MouseMove;
            
            CenterKnob = Knob.Resources["CenterKnob"] as Storyboard;
        }

        private void Knob_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            StartPos = e.GetPosition(Base);
            CanvasWidth = Base.ActualWidth;
            CanvasHeight = Base.ActualHeight;
            Captured?.Invoke(this);
            Knob.CaptureMouse();

            CenterKnob.Stop();
        }

        private void Knob_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Knob.IsMouseCaptured)
                return;

            Point newPos = e.GetPosition(Base);

            Point deltaPos = new Point(newPos.X - StartPos.X, newPos.Y - StartPos.Y);

            // If the distance is bigger than the size of the joystic it doesn't move.

            double distance = Math.Round(Math.Sqrt(deltaPos.X * deltaPos.X + deltaPos.Y * deltaPos.Y));
            if (distance >= CanvasWidth / 2 || distance >= CanvasHeight / 2)
                return;

            // Normalize X and y in [-1,1].

            YVal = -1 * (2 * ((deltaPos.Y + MaxValY) / (MaxValY - MinValY)) - 1);
            XVal = 2 * ((deltaPos.X + MaxValX) / (MaxValX - MinValX)) - 1;

            knobPosition.X = deltaPos.X;
            knobPosition.Y = deltaPos.Y;
        }

        private void Knob_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Knob.ReleaseMouseCapture();
            CenterKnob.Begin();
        }

        private void CenterKnob_Completed(object sender, EventArgs e)
        {
            XVal = YVal = 0;
            Released?.Invoke(this);
        }
    }
}
