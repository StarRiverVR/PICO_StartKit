
using System.Collections.Generic;
using System;

namespace com.vivo.codelibrary
{

    /*
     *Dictionary<int, ReleaseOnceSkillBunchListShell> m_SkillBunchList = new Dictionary<int, ReleaseOnceSkillBunchListShell>(); // 一个技能列表
     *  protected float m_deltaTime;
        public void Update(float deltaTime)
        {
            m_deltaTime = deltaTime;
            m_SkillBunchList.ForeachValue(OnUpdateSkillBunch);
        }
        public void OnUpdateSkillBunch(ReleaseOnceSkillBunchListShell shell)
        {
            shell.Update(m_deltaTime);
        }
     */

    public static class Extended_Dictionary
    {
        /// <summary>
        /// 提供一个方法遍历所有项
        /// </summary>
        public static void Foreach<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TKey, TValue> action)
        {
            if (action == null) return;
            var enumerator = dic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action(enumerator.Current.Key, enumerator.Current.Value);
            }
        }

        /// <summary>
        /// 提供一个方法遍历所有key值
        /// </summary>
        public static void ForeachKey<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TKey> action)
        {
            if (action == null) return;
            var enumerator = dic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action(enumerator.Current.Key);
            }
        }

        /// <summary>
        /// 提供一个方法遍历所有value值
        /// </summary>
        public static void ForeachValue<TKey, TValue>(this Dictionary<TKey, TValue> dic, Action<TValue> action)
        {
            if (action == null) return;
            var enumerator = dic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                action(enumerator.Current.Value);
            }
        }

    }
}


