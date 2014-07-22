using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataManager
{
    public class DataManager : IObservable
    {
        #region 属性
        //设备
        IDevice device;
        //缓存队列
        private Queue<Object> DataQueue = new Queue<Object>();
        //线程控制符
        object Readflag = new object();
        private Semaphore TaskSemaphore = new Semaphore(0, 256);
        //生产者 - 读取数据
        Thread Producer = null;
        //消费者 - 使用数据
        Thread Consumer = null;
        //单例对象实例
        private static DataManager dm = null;
        #endregion

        #region 构造函数
        private DataManager()
        {
            Producer = new Thread(new ThreadStart(ProduceData));
            Consumer = new Thread(new ThreadStart(ComsumData));
        }
        // 获取Datamanager对象
        public static DataManager GetInstance()
        {
            if (dm == null)
                dm = new DataManager();
            return dm;
        }
        #endregion

        #region 成员函数
        public void ConnectDevice(String s)
        {
            if (device != null) return;
            if (s == "Emotiv")
                device = new EmoDevice();   
        }

        public void Start() 
        {
            if (device == null)
                throw new Exception("设备未连接！");
            if (Producer.IsAlive || Consumer.IsAlive)
                return;
            Producer.Start();
            Consumer.Start();
        }

        public void Stop()
        {
            if (Producer.IsAlive || Consumer.IsAlive)
            {
                Producer.Abort();
                Consumer.Abort();
            }
        }

        public IDevice GetDevice()
        {
            return device;
        }

        private void ComsumData() 
        {
            Object data = null;
            while (true)
            {
                TaskSemaphore.WaitOne();
                lock (Readflag)
                {
                    data = DataQueue.Dequeue();
                }
                NotifyAll(data);
                Thread.Sleep(125);
            }
        }

        private void ProduceData()
        {
            while (true)
            {
                Object data = GetData();
                if (data == null) continue;
                if (DataQueue.Count >= 255) continue;
                lock (Readflag)
                {
                    DataQueue.Enqueue(data);
                }
                Thread.Sleep(125);
                TaskSemaphore.Release(1);
            }
        }

        private Object GetData()
        {
            Object data = device.GetData();
            return data;
        }

        public int GetDataPackageCount()
        {
            return DataQueue.Count;
        }

        public override void NotifyAll(Object obj)
        {
            foreach (IObserver obs in ObserverList)
            {
                obs.Update(obj);
            }
        }
        #endregion
    }
}
