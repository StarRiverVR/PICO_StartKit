
using System;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 延迟执行
    /// </summary>
    public class DelayFunHelper
    {
        public static void DelayRun(Action action, Action<object[]> actionObjs, object[] objs, double delay)
        {
            DelayFunHelper delayFunHelper = new DelayFunHelper(action, actionObjs, objs, delay);
            delayFunHelper.Run();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="actionObjs"></param>
        /// <param name="objs"></param>
        /// <param name="delay">秒</param>
        DelayFunHelper(Action action, Action<object[]> actionObjs, object[] objs, double delay)
        {
            Action = action;
            ActionObjs = actionObjs;
            Objs = objs;
            Delay = delay;
        }

        /// <summary>
        /// 需要执行的逻辑
        /// </summary>
        Action Action;

        /// <summary>
        /// 需要执行的逻辑
        /// </summary>
        Action<object[]> ActionObjs;

        /// <summary>
        /// 需要执行的逻辑参数
        /// </summary>
        object[] Objs;

        /// <summary>
        /// 延迟执行时间
        /// </summary>
        double Delay = 0.1f;

        /// <summary>
        /// 执行方法
        /// </summary>
        void Run()
        {
            System.Func<Task> func = async () =>
            {
                await Task.Delay(System.TimeSpan.FromSeconds(Delay));
                if (ThreadHelper.UnitySynchronizationContext != System.Threading.SynchronizationContext.Current)
                {
                    ThreadHelper.UnitySynchronizationContext.Send((o) => {
                        if (Action != null)
                        {
                            Action();
                        }
                        if (ActionObjs != null)
                        {
                            ActionObjs(Objs);
                        }
                    }, null);
                }
                else
                {
                    if (Action != null)
                    {
                        Action();
                    }
                    if (ActionObjs != null)
                    {
                        ActionObjs(Objs);
                    }
                }
            };
            func();
        }
    }
}


