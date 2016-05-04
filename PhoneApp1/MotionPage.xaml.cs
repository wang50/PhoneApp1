using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using Microsoft.Devices.Sensors;
using Microsoft.Xna.Framework;
using Microsoft.Phone.Net.NetworkInformation;

using System.IO;
using System.Net.Http;

namespace PhoneApp1
{
    public partial class MotionPage : PhoneApplicationPage
    {
        private Motion _motion;
        Boolean post = false;
        Boolean postNode = false;
        PostEventHub pb1;
        PostNode pn;
        int index = 0;
        int indexn = 0;

        public MotionPage()
        {
            InitializeComponent();
            pb1 = new PostEventHub();
            pn = new PostNode();        
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // 判断设备是否支 Motion API
            if (Motion.IsSupported)
            {
                lblMotionStatus.Text = "此设备支持 Motion API";
            }
            else
            {
                lblMotionStatus.Text = "此设备不支持 Motion API";

                btnStart.IsEnabled = false;
                btnStop.IsEnabled = false;
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (_motion == null)
            {
                // 实例化 Motion，注册相关事件
                _motion = new Motion();
                _motion.TimeBetweenUpdates = TimeSpan.FromMilliseconds(100);
                _motion.CurrentValueChanged += new EventHandler<SensorReadingEventArgs<MotionReading>>(_motion_CurrentValueChanged);

                lblTimeBetweenUpdates.Text = "TimeBetweenUpdates 设置为 100 毫秒，实际为 " + _motion.TimeBetweenUpdates.TotalMilliseconds.ToString() + " 毫秒";
            }

            try
            {
                // 打开 Motion 监测
                _motion.Start();
                lblMotionStatus.Text = "Motion 监测已打开";
            }
            catch (Exception ex)
            {
                lblMotionStatus.Text = "Motion 监测打开失败";
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (_motion != null)
            {
                // 关闭 Motion 监测
                _motion.Stop();
                lblMotionStatus.Text = "Motion 监测已关闭";
            }
        }

        async void _motion_CurrentValueChanged(object sender, SensorReadingEventArgs<MotionReading> e)
        {
            AttitudeReading attitude = e.SensorReading.Attitude;
            Vector3 deviceAcceleration = e.SensorReading.DeviceAcceleration;
            Vector3 deviceRotationRate = e.SensorReading.DeviceRotationRate;
            Vector3 gravity = e.SensorReading.Gravity;

            String showstr = "";
            // 在 UI 上显示相关参数
            showstr = "yaw: " + MathHelper.ToDegrees(attitude.Yaw).ToString("0.0");
            showstr += Environment.NewLine;
            showstr += "pitch: " + MathHelper.ToDegrees(attitude.Pitch).ToString("0.0");
            showstr += Environment.NewLine;
            showstr += "roll: " + MathHelper.ToDegrees(attitude.Roll).ToString("0.0");
            showstr += Environment.NewLine;
            showstr += "deviceAcceleration: \n" + deviceAcceleration.ToString();
            showstr += Environment.NewLine;
            showstr += "deviceRotationRate: \n" + deviceRotationRate.ToString();
            showstr += Environment.NewLine;
            showstr += "Gravity: \n" + gravity.ToString();
            showstr += Environment.NewLine;
            showstr += e.SensorReading.Timestamp;
            lblMsg.Dispatcher.BeginInvoke(() =>
            {
                lblMsg.Text = "MotionReading:\n" + showstr;
            });

            if (post)
            {
                String poststr = "{'yaw':" + MathHelper.ToDegrees(attitude.Yaw).ToString("0.0") + ",'pitch':" + MathHelper.ToDegrees(attitude.Pitch).ToString("0.0") + ",'roll':" + MathHelper.ToDegrees(attitude.Roll).ToString("0.0") +
                    ",'dacx':" + deviceAcceleration.X + ",'dacy':" + deviceAcceleration.Y + ",'dacz':" + deviceAcceleration.Z +
                    ",'drx':" + deviceRotationRate.X + ",'dry':" + deviceRotationRate.Y + ",'drz':" + deviceRotationRate.Z +
                    ",'gx':" + gravity.X + ",'gy':" + gravity.Y + ",'gz':" + gravity.Z + "}";
                string s = await pb1.sender(poststr);
                //string s = await pb1.sender("{ 'DeviceId':'dev-01', 'Temperature':" + (++index).ToString() + " }");
                PostMotionStatus.Dispatcher.BeginInvoke(() =>
                {
                    PostMotionStatus.Text = "Motion： " + s + "  No." + (++index).ToString() + " message";
                });
            }
            if (postNode)
            {
                String poststr = "{\"yaw\":" + attitude.Yaw + ",\"pitch\":" + attitude.Pitch + ",\"roll\":" + attitude.Roll +
                    ",\"dacx\":" + deviceAcceleration.X + ",\"dacy\":" + deviceAcceleration.Y + ",\"dacz\":" + deviceAcceleration.Z +
                    ",\"drx\":" + deviceRotationRate.X + ",\"dry\":" + deviceRotationRate.Y + ",\"drz\":" + deviceRotationRate.Z +
                    ",\"gx\":" + gravity.X + ",\"gy\":" + gravity.Y + ",\"gz\":" + gravity.Z + "}";
                //string s = senderr(poststr); //"{ \"DeviceId\":\"dev-01\", \"Temperature\":" + 4 + " }");
                string s = await pn.sender(poststr);
                PostNodeStatus.Dispatcher.BeginInvoke(() =>
                {
                    PostNodeStatus.Text = "Post Node： " + s + "  No." + (++indexn).ToString() + " message";
                });
            }
        }

        private void postStart_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceNetworkInformation.IsCellularDataEnabled || DeviceNetworkInformation.IsWiFiEnabled)
                post = true;
            else
                PostMotionStatus.Dispatcher.BeginInvoke(() =>
                {
                    PostMotionStatus.Text = "网络不可用，无法发送消息！";
                });
        }

        private void postStop_Click(object sender, RoutedEventArgs e)
        {
            post = false;
        }

        private void postNodeStart_Click(object sender, RoutedEventArgs e)
        {
            if (DeviceNetworkInformation.IsCellularDataEnabled || DeviceNetworkInformation.IsWiFiEnabled)
                postNode = true;
            else
                PostNodeStatus.Dispatcher.BeginInvoke(() =>
                {
                    PostNodeStatus.Text = "网络不可用，无法发送消息！";
                });
        }
        private void postNodeStop_Click(object sender, RoutedEventArgs e)
        {
            postNode = false;
        }

    }
}