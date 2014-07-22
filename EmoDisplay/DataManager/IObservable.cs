using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataManager
{
    /// <summary>
    /// 被观察者
    /// </summary>
    public abstract class IObservable
    {
        /// <summary>
        /// 观察者列表
        /// </summary>
        protected List<IObserver> ObserverList = new List<IObserver>();
        /// <summary>
        /// 注册观察者
        /// </summary>
        /// <param name="obs">观察者对象</param>
        public void RegistObserver(IObserver obs)
        {
            ObserverList.Add(obs);
        }
        /// <summary>
        /// 移除观察者
        /// </summary>
        /// <param name="obs">观察者对象</param>
        public void RemoveObserver(IObserver obs)
        {
            ObserverList.Remove(obs);
        }
        /// <summary>
        /// 更新所有注册的观察者
        /// </summary>
        public abstract void NotifyAll(Object obj);
    }
}
