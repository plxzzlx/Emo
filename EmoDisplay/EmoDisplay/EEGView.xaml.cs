using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using Microsoft.Research.DynamicDataDisplay.DataSources;
using Microsoft.Research.DynamicDataDisplay;
using DataManager;

namespace EmoDisplay
{
    /// <summary>
    /// EEGView.xaml 的交互逻辑
    /// </summary>
    public partial class EEGView : UserControl, IObserver
    {
        #region 属性
        // View中显示的总点数
        int index = 0;
        // View中单通道的Point总数
        int TotalLen = 128;
        // 设备
        IDevice device = null;
        // 通道名称
        IDevice.Channel[] Channels = null;
        //EEG View 曲线坐标点集
        List<ObservableDataSource<Point>> dataSourceList = new List<ObservableDataSource<Point>>();
        //缓存数据队列
        private Queue<Object> DataQueue = new Queue<Object>();
        //通道颜色
        Color[] Channel_Colors = {   Colors.Red,        Colors.Blue,
                                     Colors.DarkOrange, Colors.DeepSkyBlue,
                                     Colors.GreenYellow,Colors.LightCyan,
                                     Colors.LimeGreen,  Colors.MediumTurquoise,
                                     Colors.Tomato,     Colors.PaleVioletRed,
                                     Colors.Purple,     Colors.SaddleBrown,
                                     Colors.SlateGray,  Colors.Yellow,
                                     Colors.Chocolate,  Colors.Cyan};

        //页面刷新线程
        Thread UpdateThread = null;
        //UI代理
        delegate void MyDelegate(double[] data);
        #endregion 

        #region 成员函数
        public EEGView()
        {
            InitializeComponent();
            UpdateThread = new Thread(new ThreadStart(OnUpdateView));
        }

        public void Start()
        {
            if (!UpdateThread.IsAlive)
                UpdateThread.Start();
        }

        public void Stop()
        {
            if (UpdateThread != null && UpdateThread.IsAlive)
                UpdateThread.Abort();
        }

        public void SetDevice(IDevice device)
        {
            this.device = device;
            Channels = device.GetChannels();
            for (int i = 0; i < device.GetChNum(); i++)
            {
                ObservableDataSource<Point> dataSource = new ObservableDataSource<Point>();
                plotter.AddLineGraph(dataSource, Channel_Colors[i], 2, Channels[i].ToString());
                dataSourceList.Add(dataSource);
            }
        }

        public void Update(object obj)
        {
            DataQueue.Enqueue(obj);
        }

        private void OnUpdateView()
        {
            MyDelegate mydelegate = new MyDelegate(setValue);
            double[] max = new double[14];
            double[] min = new double[14];
            double[] NewData = new double[Channels.Length];
            while (true)
            {
                Dictionary<IDevice.Channel, double[]> data = getData();
                if (data == null)
                {
                    Thread.Sleep(50);
                    continue;
                }
                int len = data[Channels[0]].Length;
                int step = len / 5 > 0 ? len / 5 : 1;

                for (int i = 0; i < len; i += step)
                {
                    for (int j = 0; j < Channels.Length; j++)
                    {
                        NewData[j] = data[Channels[j]][i];
                    }
                    this.Dispatcher.Invoke(mydelegate, NewData);
                    index++;
                }
                Thread.Sleep(25);
            }

        }

        private Dictionary<IDevice.Channel, double[]> getData()
        {
            Dictionary<IDevice.Channel, double[]> d = null;
            if (DataQueue.Count > 0)
            {
                d = (Dictionary<IDevice.Channel, double[]>)DataQueue.Dequeue();
            }
            return d;
        }

        private void setValue(double[] data)
        {
            for (int channel = 0; channel < Channels.Length; channel++)
            {
                ObservableDataSource<Point> dataSource = dataSourceList[channel];
                bool IsMore = dataSource.Collection.Count >= TotalLen;
                if (IsMore)
                {
                    int local = index % TotalLen;
                    dataSource.Collection[local] = new Point(local, data[channel]);
                }
                else
                {
                    double c = data[channel];
                    Point p = new Point(index, c);
                    dataSource.AppendAsync(base.Dispatcher, p);
                }
            }
        }
        #endregion
    }
}
