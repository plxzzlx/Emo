using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DataManager
{
    public class DataWriter : IObserver
    {
        //文件名
        String FileName = null;
        //文件路径
        String FilePath = null;
        //文件全名
        String FullFileName = null;
        String TagFileName = null;
        //Header
        String Header = null;
        //TextWriter 
        TextWriter Writer = null;
        //Tag Writer
        TextWriter TagWriter = null;

        DataManager dm = null;
        IDevice.Channel[] Channels = null;

        public int SimulusCode { get; set; }
        public int SourceTime { get; set; }

        int index = 0;

        public DataWriter(String FilePath, String FileName)
        {
            this.FilePath = FilePath; this.FileName = FileName;
            CreateWriter();
        }
        public DataWriter(String FullFileName)
        {
            FileName = System.IO.Path.GetFileName(FullFileName);
            FilePath = System.IO.Path.GetDirectoryName(FullFileName);
            CreateWriter();
        }
        private void CreateWriter()
        {
            if (Writer != null) return;

            if (FileName == null || FilePath == null)
                throw new Exception("未设置文件名和文件路径！");
            dm = DataManager.GetInstance();
            Channels = dm.GetDevice().GetChannels();
            SimulusCode = 0;
            SourceTime = 0;
            FullFileName = FilePath + "\\" + FileName;
            TagFileName = FilePath + "\\Tag-" + FileName;
        }
        private void WriteHeader()
        {
            Writer = new StreamWriter(FullFileName, true);
            for (int i = 0; i < Channels.Length; i++)
            {
                Header += Channels[i].ToString() + ",";
            }
            Writer.WriteLine(Header);
            Writer.Close();
        }
        public void Update(object obj)
        {
            if (obj == null) return;

            Writer = new StreamWriter(FullFileName, true);
            TagWriter = new StreamWriter(TagFileName, true);
            Dictionary<IDevice.Channel, double[]> Data = (Dictionary<IDevice.Channel, double[]>)obj;
            int len = Data[Channels[0]].Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < Data.Count; j++)
                    Writer.Write(Data[Channels[j]][i] + ",");
                Writer.WriteLine("");
                TagWriter.WriteLine(SimulusCode + " " + SourceTime);
                index++;
                SourceTime = 0;
                SimulusCode = 0;
            }

            Writer.Close();
            TagWriter.Close();
        }
    }
}
