using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelegateEvent
{
    public class ObserverP1 : ObserverBase
    {
        public ObserverP1(ModelBase childModel)
            : base(childModel)
        {
            
        }
        /// <summary>
        /// 覆盖该类观察者需要做出的具体响应行为Resoponse
        /// 此行为已在观察者基类中注册与委托时间，由委托时间调度执行，不需要直接调用
        /// </summary>
        public override void Resoponse()
        {
            System.Console.WriteLine("发送玩家1数据到大屏程序");
        }
    }
}
