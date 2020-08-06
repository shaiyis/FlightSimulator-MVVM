using FlightSimulatorApp.Model.Interface;
using Microsoft.Maps.MapControl.WPF;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace FlightSimulatorApp.Model
{

    public class MySimulatorModel : ISimulatorModel
    {
        private TcpClient Client;
        volatile Boolean Stop;
        private readonly Queue<string> UpdateVariablesQueue = new Queue<string>();
        private string Message;
        private NetworkStream Stream;

        // Dashboard.

        private double IndicatedHeadingDegF;
        private double GpsIndicatedVerticalSpeedF;
        private double GpsIndicatedGroundSpeedKtF;
        private double AirspeedIndicatorIndicatedSpeedKtF;
        private double GpsIndicatedAltitudeFtF;
        private double AttitudeIndicatorInternalRollDegF;
        private double AttitudeIndicatorInternalPitchDegF;
        private double AltimeterIndicatedAltitudeFtF;

        private string PlaceF;

        private string PortF;
        private string IpF;

        private enum Status
        {
            NotSent,
            Sent,
            SentAndRecieved
        };

        private class Triplet<T1, T2, T3>
        {
            public Triplet(T1 first, T2 second, T3 third)
            {
                First = first;
                Second = second;
                Third = third;
            }
            public T1 First { get; set; }
            public T2 Second { get; set; }
            public T3 Third { get; set; }
        }

        public string FlightServerIP
        {
            get { return IpF; }
            set { IpF = value; }
        }

        public string FlightInfoPort
        {
            get { return PortF; }
            set
            {
                PortF = value;
                Connect(IpF, Convert.ToInt32(PortF));
                Start();
            }
        }



        // Map.

        private double LongitudeF;
        private double LatitudeF;
        private Point CoordinatePointF;
        private string CoordinatesF;

        //Error string.
        private string ErrorF;

        public MySimulatorModel()
        {
            this.Client = null;
            this.Stop = false;
            this.IpF = ConfigurationManager.AppSettings["Ip"];
            this.PortF = ConfigurationManager.AppSettings["Port"];
        }

        public void Connect(string ip, int port)
        {
            try
            {
                this.Error = "";
                Stop = false;
                this.Client = new TcpClient
                {
                    ReceiveTimeout = 10000
                };
                Client.Connect(ip, port);
                this.Stream = this.Client.GetStream();
            }
            catch
            {
                this.Error = "Connection Error";
            }
        }
        public void Disconnect()
        {
            this.Error = "";
            Stop = true;
            if (this.Stream != null)
            {
                this.Stream.Close();
                this.Stream = null;
            }
            if (this.Client != null)
            {
                this.Client.Dispose();
                this.Client.Close();
            }
        }
        public void WriteToServer(String message)
        {
            if (this.Stream == null)
            {
                this.Error = "First Server!!";
            }
            if (message != null && this.Stream != null)
            {
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
                this.Stream.Write(data, 0, data.Length);
            }
        }

        public string ReadFromServer()
        {
            bool end = false;
            if (this.Stream == null)
            {
                this.Error = "First Server!!";
                return null;
            }
            Byte[] data = new Byte[1024];
            StringBuilder response = new StringBuilder();
            while (!end)
            {
                Stream.Read(data, 0, data.Length);
                response.Append(Encoding.ASCII.GetString(data, 0, data.Length));
                for (int i = 0; i < 1024; i++)
                {
                    if (data[i] == '\n')
                    {
                        end = true;
                        break;
                    }
                }
            }
            return response.ToString();
        }

        public void Start()
        {
            // Reading.

            new Thread(delegate ()
            {
                // for every get of variable:
                //check if we didn't send the Message, or if we didn't get respond, or if all worked
                List<Triplet<string, string, Status>> varsAndStatus = new List<Triplet<string, string, Status>>
                {
                    new Triplet<string, string, Status>("IndicatedHeadingDegF",
                    "get /instrumentation/heading-indicator/indicated-heading-deg\n", Status.NotSent),
                    new Triplet<string, string, Status>("GpsIndicatedVerticalSpeedF",
                    "get /instrumentation/gps/indicated-vertical-speed\n", Status.NotSent),
                    new Triplet<string, string, Status>("GpsIndicatedGroundSpeedKtF",
                    "get /instrumentation/gps/indicated-ground-speed-kt\n", Status.NotSent),
                    new Triplet<string, string, Status>("AirspeedIndicatorIndicatedSpeedKtF",
                    "get /instrumentation/airspeed-indicator/indicated-speed-kt\n", Status.NotSent),
                    new Triplet<string, string, Status>("GpsIndicatedAltitudeFtF",
                    "get /instrumentation/gps/indicated-altitude-ft\n", Status.NotSent),
                    new Triplet<string, string, Status>("AttitudeIndicatorInternalRollDegF",
                    "get /instrumentation/attitude-indicator/internal-roll-deg\n", Status.NotSent),
                    new Triplet<string, string, Status>("AttitudeIndicatorInternalPitchDegF",
                    "get /instrumentation/attitude-indicator/internal-pitch-deg\n", Status.NotSent),
                    new Triplet<string, string, Status>("AltimeterIndicatedAltitudeFtF",
                    "get /instrumentation/altimeter/indicated-altitude-ft\n", Status.NotSent),
                    new Triplet<string, string, Status>("LongitudeF","get /position/longitude-deg\n", Status.NotSent),
                    new Triplet<string, string, Status>("LatitudeF","get /position/latitude-deg\n", Status.NotSent)
                };



                while (!Stop)
                {
                    try
                    {
                        string accepted = "";
                        foreach (Triplet<string,string,Status> triplet in varsAndStatus)
                        {
                            if (triplet.Third == Status.NotSent)
                            {
                                WriteToServer(triplet.Second);
                                triplet.Third = Status.Sent;
                            }
                            if (triplet.Third == Status.Sent)
                            {
                                accepted = ReadFromServer();
                                triplet.Third = Status.SentAndRecieved;
                                HandleMessage(accepted, triplet.First);
                            }
                        }
                        foreach (Triplet<string, string, Status> triplet in varsAndStatus)
                        {
                            triplet.Third = Status.NotSent;
                        }

                        while (this.GetQueueVariables().Count > 0)
                        {
                            Message = this.GetQueueVariables().Dequeue();
                            WriteToServer(Message);
                            Message = ReadFromServer();
                            Message = "";
                        }
                        Thread.Sleep(250);
                    }
                    catch (IOException e)
                    {
                        if (e.Message.Contains("time"))
                        {
                            this.Error = "Server is too\n slow.";
                        }
                        else if (e.Message.Contains("forcibly closed"))
                        {
                            this.Error = "Server is down,\n disconnect please.";
                        }
                        else
                        {
                            if (this.Stream != null)
                            {
                                this.Error = "Read/Write Err.";
                            }
                        }
                        if (this.Stream != null)
                        {
                            this.Stream.Flush();
                        }
                    }
                }
            }).Start();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged(string propName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


        private void HandleMessage(string accepted, string property)
        {
            if (accepted == null)
            {
                Stop = true;
                return;
            }
            if (Double.TryParse(accepted, out double acceptedValue))
            {
                if (acceptedValue > Double.MaxValue || acceptedValue < Double.MinValue)
                {
                    this.Error = "Invalid value";
                }
                else
                {
                    if (property == "IndicatedHeadingDegF")
                    {
                        IndicatedHeadingDeg = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "GpsIndicatedVerticalSpeedF")
                    {
                        GpsIndicatedVerticalSpeed = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "GpsIndicatedGroundSpeedKtF")
                    {
                        GpsIndicatedGroundSpeedKt = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "AirspeedIndicatorIndicatedSpeedKtF")
                    {
                        AirspeedIndicatorIndicatedSpeedKt = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "GpsIndicatedAltitudeFtF")
                    {
                        GpsIndicatedAltitudeFt = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "AttitudeIndicatorInternalRollDegF")
                    {
                        AttitudeIndicatorInternalRollDeg = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "AttitudeIndicatorInternalPitchDegF")
                    {
                        AttitudeIndicatorInternalPitchDeg = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "AltimeterIndicatedAltitudeFtF")
                    {
                        AltimeterIndicatedAltitudeFt = Math.Round(Double.Parse(accepted), 3);
                    }
                    else if (property == "LongitudeF")
                    {
                        Longitude = Double.Parse(accepted);
                    }
                    else if (property == "LatitudeF")
                    {
                        Latitude = Double.Parse(accepted);

                        Coordinates = Convert.ToString(LatitudeF + "," + LongitudeF);
                    }
                }
            }
            else if (accepted.StartsWith("ERR"))
            {
                if (property == "LongitudeF" || property == "LatitudeF")
                {
                    this.Error = "ERR in MAP";
                }
                else
                {
                    this.Error = "ERR in DashBoard";
                }
            }
            else
            {
                this.Error = "Value that isn't\n a double was sent";
            }
        }

        // Notify and set in the server communication.

        public string Error
        {
            set
            {
                this.ErrorF = value;
                NotifyPropertyChanged("Error");
            }
            get
            {
                return this.ErrorF;
            }
        }

        public string Place
        {
            set
            {
                if (this.PlaceF != value)
                {
                    this.PlaceF = value;
                    NotifyPropertyChanged("Place");
                }
            }
            get
            {
                return this.PlaceF;
            }
        }


        public string Coordinates
        {
            set
            {
                this.CoordinatesF = value;
                NotifyPropertyChanged("Coordinates");
            }
            get
            {
                return this.CoordinatesF;
            }
        }


        public Queue<string> GetQueueVariables()
        {
            return this.UpdateVariablesQueue;
        }

        public double Rudder
        {
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < -1)
                {
                    value = -1;
                }

                this.UpdateVariablesQueue.Enqueue("set /controls/flight/rudder " + value + "\n");
            }
        }
        public double Throttle
        {
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < 0)
                {
                    value = 0;
                }

                this.UpdateVariablesQueue.Enqueue("set /controls/engines/current-engine/throttle " + value + "\n");
            }
        }

        public double Elevator
        {
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < -1)
                {
                    value = -1;
                }

                this.UpdateVariablesQueue.Enqueue("set /controls/flight/elevator " + value + "\n");
            }
        }
        public double Aileron
        {
            set
            {
                if (value > 1)
                {
                    value = 1;
                }
                else if (value < -1)
                {
                    value = -1;
                }

                this.UpdateVariablesQueue.Enqueue("set /controls/flight/aileron " + value + "\n");
            }
        }


        public double IndicatedHeadingDeg
        {
            set
            {
                this.IndicatedHeadingDegF = value;
                NotifyPropertyChanged("IndicatedHeadingDeg");
            }
            get
            {
                return this.IndicatedHeadingDegF;
            }
        }
        public double GpsIndicatedVerticalSpeed
        {
            set
            {
                this.GpsIndicatedVerticalSpeedF = value;
                NotifyPropertyChanged("GpsIndicatedVerticalSpeed");
            }
            get
            {
                return this.GpsIndicatedVerticalSpeedF;
            }
        }

        public double GpsIndicatedGroundSpeedKt
        {
            set
            {
                this.GpsIndicatedGroundSpeedKtF = value;
                NotifyPropertyChanged("GpsIndicatedGroundSpeedKt");
            }
            get
            {
                return this.GpsIndicatedGroundSpeedKtF;
            }
        }
        public double AirspeedIndicatorIndicatedSpeedKt
        {
            set
            {
                this.AirspeedIndicatorIndicatedSpeedKtF = value;
                NotifyPropertyChanged("AirspeedIndicatorIndicatedSpeedKt");
            }
            get
            {
                return this.AirspeedIndicatorIndicatedSpeedKtF;
            }
        }
        public double GpsIndicatedAltitudeFt
        {
            set
            {
                this.GpsIndicatedAltitudeFtF = value;
                NotifyPropertyChanged("GpsIndicatedAltitudeFt");
            }
            get
            {
                return this.GpsIndicatedAltitudeFtF;
            }
        }
        public double AttitudeIndicatorInternalRollDeg
        {
            set
            {
                this.AttitudeIndicatorInternalRollDegF = value;
                NotifyPropertyChanged("AttitudeIndicatorInternalRollDeg");
            }
            get
            {
                return this.AttitudeIndicatorInternalRollDegF;
            }
        }
        public double AttitudeIndicatorInternalPitchDeg
        {
            set
            {
                this.AttitudeIndicatorInternalPitchDegF = value;
                NotifyPropertyChanged("AttitudeIndicatorInternalPitchDeg");
            }
            get
            {
                return this.AttitudeIndicatorInternalPitchDegF;
            }
        }
        public double AltimeterIndicatedAltitudeFt
        {
            set
            {
                this.AltimeterIndicatedAltitudeFtF = value;
                NotifyPropertyChanged("AltimeterIndicatedAltitudeFt");
            }
            get
            {
                return this.AltimeterIndicatedAltitudeFtF;
            }
        }
        public double Longitude
        {
            set
            {
                while (value > 180)
                {
                    value -= 360;
                }
                while (value < -180)
                {
                    value += 360;
                }
                this.LongitudeF = value;
                NotifyPropertyChanged("Longitude");
            }
            get
            {
                return this.LongitudeF;
            }
        }
        public double Latitude
        {
            set
            {
                if (value < -90)
                {
                    value = -90;
                }
                else if (value > 90)
                {
                    value = 90;
                }
                this.LatitudeF = value;
                NotifyPropertyChanged("Latitude");
                CoordinatePointF = new Point(this.Latitude, this.Longitude);
                DeterminePlace(CoordinatePointF);
            }
            get
            {
                return this.LatitudeF;
            }
        }

        private void DeterminePlace(Point coordinates)
        {
            double lat = coordinates.X;
            double lon = coordinates.Y;

            if ((lat > 20 && lat < 80) && (lon > -150 && lon <= -90))
            {
                this.Place = "We are in \n North America!";
            }
            else if ((lat > -60 && lat < 10) && (lon > -80 && lon < -35))
            {
                this.Place = "We are in \n South America!";
            }
            else if ((lat > -38 && lat < 30) && (lon > -20 && lon < 35))
            {
                this.Place = "We are in \n Africa!";
            }
            else if ((lat > 0 && lat < 80) && (lon > 30 && lon <= 180))
            {
                this.Place = "We are in \n Asia!";
            }
            else if ((lat > 40 && lat < 70) && (lon > -10 && lon < 30))
            {
                this.Place = "We are in \n Europe!";
            }
            else if ((lat > -50 && lat < -15) && (lon > 120 && lon < 150))
            {
                this.Place = "We are in \n Australia!";
            }
            else if ((lat >= -90 && lat < -70) && (lon >= -180 && lon <= 180))
            {
                this.Place = "We are in \n Antarctica!";
            }
            else
            {
                this.Place = "We are above the Ocean,\n don't fall :)";
            }
        }

    }
}
