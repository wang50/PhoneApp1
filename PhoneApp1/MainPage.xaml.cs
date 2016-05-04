using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using PhoneApp1.Resources;

using System.Threading;
using Microsoft.Devices.Sensors;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Phone.Net.NetworkInformation;

namespace PhoneApp1
{
    public partial class MainPage : PhoneApplicationPage
    {
        PostEventHub pb;
        int indexacc = 0;
        int indexgyr = 0;
        Boolean post = false;
        Accelerometer accelerometer;
        Gyroscope gyroscope;

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

            pb = new PostEventHub();
            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar(); 

            if (!Accelerometer.IsSupported || !Gyroscope.IsSupported)
            {
                MessageBox.Show("device does not support accelerometer or gyroscope!");
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            hello.Text = "Hello, " + nameInput.Text + "!";
        }

        private void PostEvent_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceNetworkInformation.IsCellularDataEnabled || DeviceNetworkInformation.IsWiFiEnabled)
                post = true;
            else
                accstateOutput.Dispatcher.BeginInvoke(() =>
                {
                    accstateOutput.Text = "网络不可用，无法发送消息！";
                });
        }

        private void StopPostEvent_Click(object sender, RoutedEventArgs e)
        {
            post = false;
        }

        private void startButton_Click(object sender, RoutedEventArgs e)
        {
            if (accelerometer == null)
            {
                // Instantiate the Accelerometer.
                accelerometer = new Accelerometer();
                //accelerometer.TimeBetweenUpdates = TimeSpan.FromMilliseconds(200);
                accelerometer.CurrentValueChanged +=
                    new EventHandler<SensorReadingEventArgs<AccelerometerReading>>(accelerometer_CurrentValueChanged);
            }
            if (gyroscope == null)
            {
                // Instantiate the Accelerometer.
                gyroscope = new Gyroscope();
                // gyroscope.TimeBetweenUpdates = TimeSpan.FromMilliseconds(200);
                gyroscope.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<GyroscopeReading>>(gyroscope_CurrentValueChanged);
            }

            try
            {
                accOutput.Text = "starting accelerometer.";
                accelerometer.Start();
            }
            catch (InvalidOperationException ex)
            {
                accOutput.Text = "unable to start accelerometer.";
            }
            try
            {
                gyrOutput.Text = "starting gyroscope.";
                gyroscope.Start();
            }
            catch (InvalidOperationException ex)
            {
                gyrOutput.Text = "unable to start gyroscope.";
            }
        }

        async void accelerometer_CurrentValueChanged(object sender, SensorReadingEventArgs<AccelerometerReading> e)
        {
            Vector3 acceleration = e.SensorReading.Acceleration;
            String accstate = "";
            accstate += "aX: " + acceleration.X.ToString("0.00") + "\n";
            accstate += "aY: " + acceleration.Y.ToString("0.00") + "\n";
            accstate += "aZ: " + acceleration.Z.ToString("0.00") + "\n";
            accOutput.Dispatcher.BeginInvoke(() =>
            {
                accOutput.Text = "重力加速度:\n" + accstate;
            });

            await SendAccMsg(e.SensorReading);
        }
        async void gyroscope_CurrentValueChanged(object sender, SensorReadingEventArgs<GyroscopeReading> e)
        {
            Vector3 gyroscope = e.SensorReading.RotationRate;
            String gyrstate = "";
            gyrstate += "rX: " + gyroscope.X.ToString("0.00") + "\n";
            gyrstate += "rY: " + gyroscope.Y.ToString("0.00") + "\n";
            gyrstate += "rZ: " + gyroscope.Z.ToString("0.00") + "\n";
            gyrOutput.Dispatcher.BeginInvoke(() =>
            {
                gyrOutput.Text = "陀螺仪:\n" + gyrstate;
            });

            await SendGyrMsg(e.SensorReading);
        }
        private async Task SendAccMsg(AccelerometerReading accelerometerReading)
        {
            Vector3 acceleration = accelerometerReading.Acceleration;
            if (post)
            {
                string strr = "{'aX':" + acceleration.X.ToString("0.00") + ",'aY':" + acceleration.Y.ToString("0.00") + ",'aZ':" + acceleration.Z.ToString("0.00") + "}";
                string s = await pb.sender(strr);
                accstateOutput.Dispatcher.BeginInvoke(() =>
                {
                    accstateOutput.Text = "加速度： " + s + "  No." + (++indexacc).ToString() + " message";
                });
            }
        }

        private async Task SendGyrMsg(GyroscopeReading gyroscopeReading)
        {
            Vector3 gyroscope = gyroscopeReading.RotationRate;
            if (post)
            {
                string strr = "{'rX':" + gyroscope.X.ToString("0.00") + ",'rY':" + gyroscope.Y.ToString("0.00") + ",'rZ':" + gyroscope.Z.ToString("0.00") + "}";
                string s = await pb.sender(strr);
                gyrstateOutput.Dispatcher.BeginInvoke(() =>
                {
                    gyrstateOutput.Text = "陀螺仪： " + s + "  No." + (++indexgyr).ToString() + " message";
                });
            }
            // throw new NotImplementedException();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (accelerometer != null)
            {
                // Stop the accelerometer.
                accelerometer.Stop();
                accOutput.Text = "accelerometer stopped.";
            }
            if (accelerometer != null)
            {
                // Stop the accelerometer.
                gyroscope.Stop();
                gyrOutput.Text = "gyroscope stopped.";
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/MotionPage.xaml", UriKind.Relative));
        }
        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}