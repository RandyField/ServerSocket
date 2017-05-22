using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelegateEvent
{
    /// <summary>
    /// 此类为发布方，其继承于模型
    /// 其中包含（调用）了在模型中被封装好的触发委托事件的方法
    /// </summary>
    public class StationServer : ModelBase
    {
        /// <summary>
        /// 无参构造函数
        /// </summary>
        public StationServer()
        {
        }

        /// <summary>
        /// 定义中转站的行为
        /// 得到串口数据
        /// </summary>
        public void GetSerialPortData()
        {
            System.Console.WriteLine("得到串口传过来的数据");

            //调用了触发委托事件的方法.

            //通知委托开始执行观察者已订阅的方法.

            //从串口中得到数据
            this.Notify();
        }

        /// <summary>
        /// 得到游戏结束数据
        /// </summary>
        public void GetScreenGameData()
        {
            System.Console.WriteLine("得到屏幕端传过来的游戏结束数据");


            //调用了触发委托事件的方法.

            //通知委托开始执行观察者已订阅的方法.
            this.Notify();
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetP1GameData()
        {
            System.Console.WriteLine("得到P1端传过来选择游戏的数据");

            //调用了触发委托事件的方法.

            //通知委托开始执行观察者已订阅的方法.
            this.Notify();
        }

        /// <summary>
        /// 
        /// </summary>
        public void GetP2GameData()
        {
            System.Console.WriteLine("得到P2端传过来选择游戏的数据");

            //调用了触发委托事件的方法.

            //通知委托开始执行观察者已订阅的方法.
            this.Notify();
        }
    }
}
