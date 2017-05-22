using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelegateEvent
{
    /// <summary>
    /// 此类为Observer Pattern中具体observer，其继承于观察者基类
    /// 其中覆盖了观察者基类规划好的方法，实现响应的具体行为
    /// </summary>
    public class ObserverScreen : ObserverBase
    {
        public ObserverScreen(ModelBase childModel)
            : base(childModel)
        {
            
        }
        /// <summary>
        /// 覆盖该类观察者需要做出的具体响应行为Resoponse
        /// 此行为已在观察者基类中注册与委托时间，由委托时间调度执行，不需要直接调用
        /// </summary>
        public override void Resoponse()
        {
            System.Console.WriteLine("发送单车数据到玩家1");
            System.Console.WriteLine("发送单车数据到玩家2");
        }
    }
}
