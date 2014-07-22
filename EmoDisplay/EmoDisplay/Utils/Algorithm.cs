using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EmoDisplay.Utils
{
    /// <summary>
    /// FFT 的摘要说明。
    /// </summary>
    public class CFFT
    {
        /////////////////////////////////////////// 
        // Function name : BitReverse 
        // Description : 二进制倒序操作 
        // Return type : int 
        // Argument : int src 待倒读的数 
        // Argument : int size 二进制位数 
        static int BitReverse(int src, int size)
        {
            int tmp = src;
            int des = 0;
            for (int i = size - 1; i >= 0; i--)
            {
                des = ((tmp & 0x1) << i) | des;
                tmp = tmp >> 1;
            }
            return des;
        }

        /*
        Parameter:
        sequ:	point a sequance that f(x)
        r:		多少级蝶形运算
        Function:
        the procedure change the sequance by this:
            // 101 -> 101
            // 110 -> 011
            // 1010 -> 0101
        f(5) -> f(5)
        f(6) -> f(3)
        f(10) -> f(5)
        and so on.
        */
        static void BitReOrder(CComplex[] sequ, int r)
        {
            CComplex dTemp;
            int x;
            for (int i = 0; i < (1 << r); ++i)
            {
                x = BitReverse(i, r);
                if (x > i)
                {
                    dTemp = sequ[i];
                    sequ[i] = sequ[x];
                    sequ[x] = dTemp;
                }
            }
        }

        public static void IFFT(CComplex[] F, CComplex[] f, int N)
        {
            int i;
            CComplex[] T = new CComplex[N];

            for (i = 0; i < N; ++i)
            {
                //复制数据到T,并取共轭
                T[i] = new CComplex(F[i].Real, -F[i].Image);
            }
            //以共轭数据进行FFT变换
            FFT(T, f, N);

            for (i = 0; i < N; ++i)
            {
                //取共轭，并乘以N
                f[i] = new CComplex(f[i].Real, -f[i].Image);
                f[i] *= new CComplex(N, 0);
            }
        }

        public static void FFT(CComplex[] f, CComplex[] F, int N)
        {
            int i = 0;
            for (i = 0; i < N; ++i)
            {
                //复制数据到F
                F[i] = new CComplex(f[i].Real, f[i].Image);
            }

            //计算需要多少级蝶形运算

            int r = (int)Math.Log(N + 1, 2); //Log2(N+1)

            //进行倒位序运算
            BitReOrder(F, r);

            //计算Wr因子, 加权系数 
            CComplex[] W = new CComplex[N / 2];
            double angle;
            for (i = 0; i < N / 2; i++)
            {
                angle = -i * Math.PI * 2 / N;
                W[i] = new CComplex(Math.Cos(angle), Math.Sin(angle));
                //W[i] = complex<double>(0, exp(-i*2*PI/N));
            }

            //采用蝶形算法进行快速傅立叶变换
            int DFTn;//第DFTn级
            int k;//分级有多少个蝶形运算
            int d;//蝶形运算的偏移
            int p;//index

            //complex<double> *X,*X1,*X2;
            //X1 = new complex<double>[N]; 
            //X2 = new complex<double>[N]; 
            CComplex X1 = new CComplex(), X2 = new CComplex();
            for (DFTn = 0; DFTn < r; DFTn++)
            {//第几级蝶形运算
                for (k = 0; k < (1 << (r - DFTn - 1)); ++k)
                {//当前列所有的蝶形进行运算
                    p = 2 * k * (1 << DFTn);
                    for (d = 0; d < (1 << DFTn); ++d)
                    {//相邻蝶形运算
                        //p = k * (1 << (k + 1)) + d;
                        X1 = F[p + d];
                        X2 = F[p + d + (1 << DFTn)];
                        F[p + d] = X1 + X2 * W[d * (1 << (r - DFTn - 1))];
                        F[p + d + (1 << DFTn)] = X1 - X2 * W[d * (1 << (r - DFTn - 1))];
                    }
                }
            }
            //BitReOrder(F,r);
            CComplex complexN = new CComplex(N);
            for (i = 0; i < N; ++i)
                F[i] /= complexN;

        }
    }

    class Hilbert
    {
        // Hilbert 函数 希尔伯特变换
        // Xr,Xi,Hr,Hi必须是一维列向量，且预先分配好相应的空间 向量长度均相同
        // 矩阵Sr - 源数据的实部向量
        // 矩阵Si - 源数据的虚部向量
        // 矩阵Hr - 目标向量的实部向量
        // 矩阵Hi - 目标向量的虚部向量
        // len    - 向量长度
        public CComplex[] Convert(CComplex[] cc)
        {
            int len = cc.Length;
            CComplex[] clist = new CComplex[len];
            CComplex[] hlist = new CComplex[len];
            int[] h = new int[len];
            
            h[0] = 1;
            for (int i = 1; i < len / 2; i++)
                h[i] = 2;
            if (2 * (len / 2) == len)
                h[len / 2] = 1;
            else
                h[len / 2 - 1] = 1;

            CFFT cfft = new CFFT();
            
            CFFT.FFT(cc, clist, len);
            for (int i = 1; i < len; i++)
                Console.WriteLine(hlist[i].Real + " - " + hlist[i].Image + "i");

            for (int i = 1; i < len/2; i++)
            {
                clist[i].Real = clist[i].Real * h[i];
                clist[i].Image = clist[i].Image * h[i];
            }
            CFFT.IFFT(clist, hlist, len);

            return hlist;
        }
    }

    public class Util
    {
        /// <summary>
        /// 求数组前Len个元素的最大值
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="N">前N个元素</param>
        /// <returns>最大值</returns>
        public static double Max(double[] array, int N)
        {
            if (N > array.Length)
                throw new Exception("数组长度小于查找长度！");

            double max = double.MinValue;
            for (int i = 0; i < N; i++)
            {
                if (array[i] > max)
                {
                    max = array[i];
                }
            }
            return max;
        }
        /// <summary>
        /// 求数组第N~M个元素的最大值
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="N">第N个元素</param>
        /// <param name="M">第M个元素</param>
        /// <returns>最大值</returns>
        public static double Max(double[] array, int N,int M,ref int index)
        {
            if (N>=M)
                 throw new Exception("数组下标错误！");
            if (N > array.Length)
                throw new Exception("数组长度小于查找长度！");

            double max = double.MinValue;
            for (int i = N; i < M; i++)
            {
                if (array[i] > max)
                {
                    index = i;
                    max = array[i];
                }
            }
            return max;
        }
        
        /// <summary>
        /// 求数组最大值
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>最大值</returns>
        public static double Max(double []array)
        {
            return Max(array, array.Length);
        }

        /// <summary>
        /// 求数组前N个数的最小值
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="N">前N个数</param>
        /// <returns>最小值</returns>
        public static double Min(double[] array,int N) 
        {
            if (array.Length < N)
                throw new Exception("数组长度小于查找长度！");
            double min = Double.MaxValue;
            for (int i = 0; i < N; i++)
            {
                if (array[i] < min)
                    min = array[i];
            }
            return min; 
        }
        /// <summary>
        /// 求数组最小值
        /// </summary>
        /// <param name="array">数组array</param>
        /// <returns>最小值</returns>
        public static double Min(double[] array) 
        {
            return Min(array, array.Length); 
        }
        /// <summary>
        /// 求数组前N个元素和
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="N">前N个元素</param>
        /// <returns>总和</returns>
        public static double Sum(double[] array,int N) 
        {
            if (array.Length < N)
                throw new Exception("数组长度小于查找长度！");
            double sum = 0;
            for (int i = 0; i < N; i++)
            {
                sum += array[i];
            }
            return sum; 
        }
        /// <summary>
        /// 求数组各个元素总和
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>总和</returns>
        public static double Sum(double[] array)
        {
            return Sum(array, array.Length);
        }
        /// <summary>
        /// 求数组前N个元素的均值
        /// </summary>
        /// <param name="array">数组</param>
        /// <param name="N">前N个元素</param>
        /// <returns>均值</returns>
        public static double Ave(double[] array,int N) 
        {
            if (array.Length < N)
                throw new Exception("数组长度小于查找长度！");
            double aver = 0;
            for (int i = 0; i < N; i++)
                aver += array[i];
            aver = aver / N;
            return aver; 
        }
        /// <summary>
        /// 求数组各个元素的均值
        /// </summary>
        /// <param name="array">数组</param>
        /// <returns>均值</returns>
        public static double Ave(double[] array)
        {
            return Ave(array, array.Length);
        }
    }
}
