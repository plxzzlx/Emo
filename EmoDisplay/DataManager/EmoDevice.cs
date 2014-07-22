using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emotiv;

namespace DataManager
{
    class EmoDevice : IDevice
    {
        public static readonly String DEV_NAME = "Emotiv";
        //通道数
        public static readonly int CHANNEL_NUM = 14;
        //采样率
        public static readonly int SAMPLE_RATE = 128;
        //采集数据频率 （每秒采集的次数）
        public static readonly int READ_FREQUENT = 8;
        //统一格式的通道
        public static readonly Channel[] Channels = 
        {
            IDevice.Channel.AF3, IDevice.Channel.F7,
            IDevice.Channel.F3,  IDevice.Channel.FC5,
            IDevice.Channel.T7,  IDevice.Channel.P7,
            IDevice.Channel.O1,  IDevice.Channel.O2,
            IDevice.Channel.P8,  IDevice.Channel.T8,
            IDevice.Channel.FC6, IDevice.Channel.F4,
            IDevice.Channel.F8,  IDevice.Channel.AF4   
        };
        //Emotiv通道
        private static readonly EdkDll.EE_DataChannel_t[] EmoChannels = 
        { 
            EdkDll.EE_DataChannel_t.AF3, EdkDll.EE_DataChannel_t.F7,
            EdkDll.EE_DataChannel_t.F3,  EdkDll.EE_DataChannel_t.FC5,
            EdkDll.EE_DataChannel_t.T7,  EdkDll.EE_DataChannel_t.P7,
            EdkDll.EE_DataChannel_t.O1,  EdkDll.EE_DataChannel_t.O2,
            EdkDll.EE_DataChannel_t.P8,  EdkDll.EE_DataChannel_t.T8,
            EdkDll.EE_DataChannel_t.FC6, EdkDll.EE_DataChannel_t.F4,
            EdkDll.EE_DataChannel_t.F8,  EdkDll.EE_DataChannel_t.AF4
        };

        EdkDll.EE_EEG_ContactQuality_t[] Edk_CQs = null;
        IDevice.ConnectQuality[] CQs = null;
        //Emotiv Headset 
        EmoEngine engine;
        int userID = -1;

        public EmoDevice()
        {
            engine = EmoEngine.Instance;
            engine.UserAdded += new EmoEngine.UserAddedEventHandler(engine_UserAdded_Event);
            engine.EmoEngineEmoStateUpdated += new EmoEngine.EmoEngineEmoStateUpdatedEventHandler(engine_EmoEngineEmoStateUpdated);
            engine.Connect();

            //14个电极 + 4个参考电极
            CQs = new IDevice.ConnectQuality[CHANNEL_NUM + 4];
        }

        void engine_EmoEngineEmoStateUpdated(object sender, EmoStateUpdatedEventArgs e)
        {
            EmoState es = e.emoState;
            int num = es.GetNumContactQualityChannels();
            Edk_CQs = es.GetContactQualityFromAllChannels();
        }

        void engine_UserAdded_Event(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("User Added Event has occured");
            // record the user 
            userID = (int)e.userId;

            // enable data aquisition for this user.
            engine.DataAcquisitionEnable((uint)userID, true);

            // ask for up to 1 second of buffered data
            engine.EE_DataSetBufferSizeInSec(1);
        }

        override public Channel[] GetChannels()
        {
            return Channels;
        }

        override public int GetChNum()
        {
            return Channels.Length;
        }

        override public object GetData()
        {
            engine.ProcessEvents();
            if ((int)userID == -1) return null;
            Dictionary<EdkDll.EE_DataChannel_t, double[]> data = engine.GetData((uint)userID);
            if (data == null) return null;
            Dictionary<IDevice.Channel, double[]> rebuildData = (Dictionary<IDevice.Channel, double[]>)RebuildData(data);

            return rebuildData;
        }

        override public int GetSampleRate()
        {
            return SAMPLE_RATE;
        }

        override public Object RebuildData(Object obj)
        {
            Dictionary<EdkDll.EE_DataChannel_t, double[]> data =
                (Dictionary<EdkDll.EE_DataChannel_t, double[]>)obj;

            Dictionary<IDevice.Channel, double[]> rebuildData = new Dictionary<IDevice.Channel, double[]>();

            for (int i = 0; i < CHANNEL_NUM; i++)
            {
                double[] d = data[EmoChannels[i]];
                rebuildData.Add(Channels[i], d);
            }

            return rebuildData;
        }

        override public string GetDeviceName()
        {
            return DEV_NAME;
        }

        public override IDevice.ConnectQuality[] GetCQStates()
        {
            engine.ProcessEvents(1000);
            if (Edk_CQs == null)
                return null;
            for (int i = 0; i < CHANNEL_NUM + 4; i++)
            {
                CQs[i] = TransCQ(Edk_CQs[i]);
            }
            return CQs;
        }

        private IDevice.ConnectQuality TransCQ(EdkDll.EE_EEG_ContactQuality_t state)
        {
            IDevice.ConnectQuality cq;
            switch (state)
            {
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_FAIR: cq = IDevice.ConnectQuality.EEG_CQ_FAIR; break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_GOOD: cq = IDevice.ConnectQuality.EEG_CQ_GOOD; break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_NO_SIGNAL: cq = IDevice.ConnectQuality.EEG_CQ_NO_SIGNAL; break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_POOR: cq = IDevice.ConnectQuality.EEG_CQ_POOR; break;
                case EdkDll.EE_EEG_ContactQuality_t.EEG_CQ_VERY_BAD: cq = IDevice.ConnectQuality.EEG_CQ_VERY_BAD; break;
                default: cq = IDevice.ConnectQuality.EEG_CQ_NO_SIGNAL; break;
            }
            return cq;
        }
    }
}
