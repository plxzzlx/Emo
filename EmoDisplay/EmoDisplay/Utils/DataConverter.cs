using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emotiv;

namespace EmoDisplay.Utils
{
    public class DataConverter
    {
        //通道数
        public static int Ch_Num = 14;
        //通道名称
        public static readonly EdkDll.EE_DataChannel_t[] Channels = { 
               EdkDll.EE_DataChannel_t.AF3, EdkDll.EE_DataChannel_t.F7,
               EdkDll.EE_DataChannel_t.F3,  EdkDll.EE_DataChannel_t.FC5,
               EdkDll.EE_DataChannel_t.T7,  EdkDll.EE_DataChannel_t.P7,
               EdkDll.EE_DataChannel_t.O1,  EdkDll.EE_DataChannel_t.O2,
               EdkDll.EE_DataChannel_t.P8,  EdkDll.EE_DataChannel_t.T8,
               EdkDll.EE_DataChannel_t.FC6, EdkDll.EE_DataChannel_t.F4,
               EdkDll.EE_DataChannel_t.F8,  EdkDll.EE_DataChannel_t.AF4};
       
        //EEG数据格式 转化为 Matrix格式
        public static Matrix Dic2Matrix(Dictionary<EdkDll.EE_DataChannel_t, double[]> data)
        {

            //数据长度
            int Ch_Len = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;
            
            Matrix m = new Matrix(Ch_Num, Ch_Len);
            
            for (int i = 0; i < Ch_Len; i++)
            {
                for (int j = 0; j < Ch_Num; j++)
                {
                    m[j, i] = new Complex(data[Channels[j]][i],0);
                }
            }
            
            return null;
        }

        public static CComplex[][] Dic2CComplex(Dictionary<EdkDll.EE_DataChannel_t, double[]> data)
        {
            int len = data[EdkDll.EE_DataChannel_t.TIMESTAMP].Length;
            CComplex[][] matrix = new CComplex[Ch_Num][];
            for (int i = 0; i < Ch_Num;i++ )
            {
                matrix[i] = new CComplex[len];
                for (int j = 0; i < len;j++ )
                {
                    matrix[i][j].Real = data[Channels[i]][j];
                    matrix[i][j].Image = 0;
                }
            }
            
            return matrix;
        }

        //Complex对象 转化为 CComplex对象
        public static CComplex Complex2CComplex(Complex c)
        {
            CComplex cc = new CComplex(c.Re, c.Im);
            return cc;
        }

        //CComplex对象 转化为 Complex对象
        public static Complex Complex2CComplex(CComplex cc)
        {
            Complex c = new Complex(cc.Real, cc.Image);
            return c;
        }
    }
}
