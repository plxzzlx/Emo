using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using DataManager;
using System.Threading;

namespace EmoDisplay
{
    /// <summary>
    /// LoadDeviceWnd.xaml 的交互逻辑
    /// </summary>
    public partial class LoadDeviceWnd : Window
    {
        DataManager.DataManager dm = null;
        IDevice device = null;
        List<Label> EmotList = new List<Label>();
        List<Label> GtecList = new List<Label>();
        Color[] StateColors = { Colors.Black,Colors.Red,Colors.Orange,Colors.Yellow,Colors.Lime};

        Thread StateThread = null;

        public LoadDeviceWnd()
        {
            InitializeComponent();
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            dm = DataManager.DataManager.GetInstance();
            EmotList.Add(Emotiv_AF3);
            EmotList.Add(Emotiv_AF4);
            EmotList.Add(Emotiv_F7);
            EmotList.Add(Emotiv_F3);
            EmotList.Add(Emotiv_FC5);
            EmotList.Add(Emotiv_T7);
            EmotList.Add(Emotiv_P7);
            EmotList.Add(Emotiv_O1);
            EmotList.Add(Emotiv_O2);
            EmotList.Add(Emotiv_P8);
            EmotList.Add(Emotiv_T8);
            EmotList.Add(Emotiv_FC6);
            EmotList.Add(Emotiv_F4);
            EmotList.Add(Emotiv_F8); 
            EmotList.Add(Emotiv_CMS); 
            EmotList.Add(Emotiv_DRL);

            GtecList.Add(Gtec_Fz);
            GtecList.Add(Gtec_Pz);
            GtecList.Add(Gtec_Oz);
            GtecList.Add(Gtec_Fp1);
            GtecList.Add(Gtec_Fp2);
            GtecList.Add(Gtec_F7);
            GtecList.Add(Gtec_F3);
            GtecList.Add(Gtec_F4);
            GtecList.Add(Gtec_F8);
            GtecList.Add(Gtec_C3);
            GtecList.Add(Gtec_C4);
            GtecList.Add(Gtec_P7);
            GtecList.Add(Gtec_P3);
            GtecList.Add(Gtec_P4);
            GtecList.Add(Gtec_P8);
            GtecList.Add(Gtec_EKG);
        }

        private void Radio_Gtec_Click(object sender, RoutedEventArgs e)
        {
            GtecGroupBox.Visibility = Visibility.Visible;
            EmotivGroupBox.Visibility = Visibility.Hidden;
        }

        private void Radio_Emotiv_Click(object sender, RoutedEventArgs e)
        {
            GtecGroupBox.Visibility = Visibility.Hidden;
            EmotivGroupBox.Visibility = Visibility.Visible;
        }

        private void LoadDevice_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Radio_Emotiv.IsChecked == true)
                {
                    dm.ConnectDevice("Emotiv");
                }
                else if (Radio_Gtec.IsChecked == true)
                {
                    dm.ConnectDevice("Gtec");
                }
                else
                {
                    MessageBox.Show("请选择一个设备！");
                    return ;
                }
                MessageBox.Show(dm.GetDevice().GetDeviceName()+"设备加载成功");
                Radio_Gtec.IsEnabled = false;
                Radio_Emotiv.IsEnabled = false;
                LoadDevice.IsEnabled = false;

                device = dm.GetDevice();
                DeviceNameLabel.Content = "设备名称："+device.GetDeviceName();
                ChannelNumLabel.Content = "通道数："+device.GetChNum();
                SampleRateLabel.Content = "采样率："+device.GetSampleRate();
                IDevice.Channel[] channels = device.GetChannels();
                for (int i = 0; i < channels.Length;i++ )
                {
                    ChannelListBox.Items.Add(channels[i].ToString());
                }
                DetectState.IsEnabled = true;
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());	
            }
        }

        private void DetectState_Click(object sender, RoutedEventArgs e)
        {
            if (StateThread == null)
            {
                StateThread = new Thread(new ThreadStart(DetecStateProcess));
                StateThread.Start();
                DetectState.Content = "停止检测";
            }
            else if (StateThread.IsAlive == true)
            {
                StateThread.Abort();
                DetectState.Content = "检测阻抗";
            }
        }

        private void DetecStateProcess()
        {
            IDevice.ConnectQuality[] CQs = new IDevice.ConnectQuality[device.GetChNum()];
            UpdateStateDelegate UpDelegate = new UpdateStateDelegate(UpdateState);
            while (true)
            {
                Thread.Sleep(1000);
                CQs = device.GetCQStates();
                if (CQs == null)
                    Console.WriteLine("No Conect!");
                else
                {
                    this.Dispatcher.Invoke(UpDelegate, CQs);
                }
            }
        }

        delegate void UpdateStateDelegate(IDevice.ConnectQuality[] CQs);

        void UpdateState(IDevice.ConnectQuality[] CQs)
        {
            if (device.GetDeviceName() == "Emotiv")
            {

                for (int i = 2; i < device.GetChNum()+2; i++)
                {
                    EmotList[i].Background = new SolidColorBrush(State2Color(CQs[i]));
                }
                EmotList[14].Background = new SolidColorBrush(State2Color(CQs[0]));
                EmotList[15].Background = new SolidColorBrush(State2Color(CQs[1]));
            }
            else if (device.GetDeviceName() == "Gtec")
            {
                for (int i = 0; i < 16;i++ )
                {
                    GtecList[i].Background = new SolidColorBrush(State2Color(CQs[i]));
                }
            }
        }

        Color State2Color(IDevice.ConnectQuality cq)
        {
            Color c;
            switch (cq)
            {
                case IDevice.ConnectQuality.EEG_CQ_NO_SIGNAL: c = StateColors[0]; break;
                case IDevice.ConnectQuality.EEG_CQ_VERY_BAD: c = StateColors[1]; break;
                case IDevice.ConnectQuality.EEG_CQ_POOR: c = StateColors[2]; break;
                case IDevice.ConnectQuality.EEG_CQ_FAIR: c = StateColors[3]; break;
                case IDevice.ConnectQuality.EEG_CQ_GOOD: c = StateColors[4]; break;
                default: c = StateColors[0]; break;
            }

            return c;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            StopThread();
        }

        private void StopThread()
        {
            if (StateThread != null && StateThread.IsAlive == true)
            {
                StateThread.Abort();
            }
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            StopThread();
            this.Close();
        }
    }
}
