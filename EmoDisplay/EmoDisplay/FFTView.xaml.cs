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
using EmoDisplay.Utils;

namespace EmoDisplay
{
    /// <summary>
    /// FFTView.xaml 的交互逻辑
    /// </summary>
    public partial class FFTView : UserControl, IObserver
    {
        // 设备
        IDevice device = null;
        // 通道名称
        IDevice.Channel[] Channels = null;
        //缓存数据队列
        private Queue<Object> DataQueue = new Queue<Object>();
        //页面刷新线程
        Thread UpdateThread = null;
        //UI代理
        delegate void MyDelegate(double[] data);
        ObservableDataSource<Point> dataSource = null;

        public FFTView()
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
            
            dataSource = new ObservableDataSource<Point>();
            
            plotter.AddLineGraph(dataSource, Colors.Red, 2, Channels[0].ToString());
            for (int i = 0; i < 128; i++)
            {
                Point p = new Point(i, 0);
                dataSource.AppendAsync(base.Dispatcher, p);
            }
        }

        public void Update(object obj)
        {
            DataQueue.Enqueue(obj);
        }

        private void OnUpdateView()
        {
            List<Object> data = new List<object>();
            MyDelegate mydelegate = new MyDelegate(setValue);

            while (true)
            {
                Object obj = getData();
                if (obj == null)
                {
                    Thread.Sleep(50);
                    continue;
                }
                else
                    data.Add(obj);
                if (data.Count < 8) continue;
                if (data.Count > 8) data.RemoveAt(0);
                
                int len = 0;
                CComplex[] src = MergeChannelData(data, Channels[0], ref len);
                CComplex[] dst = new CComplex[len];
                CFFT.FFT(src, dst, len);
                double[] fft_data = new double[len];
                for (int i = 0; i < len; i++)
                    fft_data[i] = Math.Sqrt(dst[i].Image * dst[i].Image + dst[i].Real * dst[i].Real);
                this.Dispatcher.Invoke(mydelegate, fft_data);
                Thread.Sleep(125);
            }
        }

        private Object getData()
        {
            Object obj = null;
            if (DataQueue.Count > 0) 
                obj = DataQueue.Dequeue();
            return obj;
        }

        private void setValue(double[] data)
        {
            for (int i = 1; i < data.Length; i++)
                dataSource.Collection[i] = new Point(i, data[i]);
        }

        private CComplex[] MergeChannelData(List<Object> data, IDevice.Channel channel, ref int len)
        {
            len = 0;
            Dictionary<IDevice.Channel, double[]> tm = null;

            for (int i = 0; i < data.Count; i++)
            {
                tm = (Dictionary<IDevice.Channel, double[]>)data[i];
                len += tm[Channels[0]].Length;
            }

            CComplex[] d = new CComplex[len];
            int tlen = 0;
            for (int i = 0; i < data.Count; i++)
            {
                tm = (Dictionary<IDevice.Channel, double[]>)data[i];
                double[] da = tm[channel];
                for (int j = 0; j < da.Length; j++)
                {
                    CComplex c = new CComplex();
                    c.Real = da[j];
                    c.Image = 0;
                    d[tlen + j] = c;
                }
                tlen += da.Length;
            }
            return d;
        }
    }
}
