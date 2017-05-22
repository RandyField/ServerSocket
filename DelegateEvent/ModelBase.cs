using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelegateEvent
{
    /// <summary>
    /// Observer Pattern,Subject的抽象基类
    /// 规划subject所产生的事件，及提供触发事件的方法
    /// 1、声明委托
    /// 2、声明委托类型事件
    /// 3、提供触发事件的方法
    /// </summary>
    public abstract class ModelBase
    {
        /// <summary>
        /// 无参构造方法
        /// </summary>
        public ModelBase()
        {
 
        }

        /// <summary>
        /// 声明一个委托，用于代理一系列“无返回”及“不带参”的自定义方法
        /// </summary>
        public delegate void SubEventHandler();

        /// <summary>
        /// 声明一个绑定与上行所定义的委托的事件
        /// </summary>
        public event SubEventHandler SubEvent;

        /// <summary>
        /// 封装触发事件的方法
        /// 主要为了规范化及安全性，除Subject基类外，其派生类不直接触发委托事件
        /// </summary>
        protected void Notify()
        {
            if (this.SubEvent!=null)
            {
                this.SubEvent();
            }
        }
    }
}
