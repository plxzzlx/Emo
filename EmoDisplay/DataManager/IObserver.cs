using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    /// <summary>
    /// 观察者
    /// </summary>
    public interface IObserver
    {
        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="obj">数据对象</param>
        void Update(Object obj);
    }
}
