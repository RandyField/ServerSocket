using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DelegateEvent
{
    /// <summary>
    /// Observer Pattern,Observer的抽象基类
    /// 规划Observer订阅行为
    /// 
    /// 具体实施过程：
    /// 1、指定Observer所观察的对象(即发布方)。（通过构造器传递）
    /// 2、规划Observer自身需要做出相应的方法列表
    /// 3、注册需要委托执行的方法。（通过构造器实现）
    /// </summary>
    public abstract class ObserverBase
    {
        /// <summary>
        /// 规划Observer的一种行为（方法），所有派生于该Observer base class 的具体Observer
        /// 通过覆盖该方法来实现作出响应的行为
        /// </summary>
        public abstract void Resoponse();
 


        /// <summary>
        /// 构造时通过传入模型对象，把Observer与模型关联，并完成订阅
        /// 在此确定需要观察的模型对象
        /// </summary>
        /// <param name="childModel">需要观察的对象</param>
        public ObserverBase(ModelBase childModel)
        {
            //订阅
            //把观察这行为注册于委托事件
            childModel.SubEvent += new ModelBase.SubEventHandler(Resoponse);
        }
    }
}
