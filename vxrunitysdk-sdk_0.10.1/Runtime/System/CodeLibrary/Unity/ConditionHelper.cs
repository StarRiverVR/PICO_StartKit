using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 条件比较
    /// </summary>
    public class ConditionHelper
    {
        static Dictionary<Type, SimplePool<ConditionBase>> pools = new Dictionary<Type, SimplePool<ConditionBase>>();

        /// <summary>
        /// 获得比较条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T Spawn<T>() where T: ConditionBase
        {
            lock (pools)
            {
                SimplePool<ConditionBase> pool;
                if (!pools.TryGetValue(typeof(T), out pool))
                {
                    pool = new SimplePool<ConditionBase>();
                    pools.Add(typeof(T), pool);
                }

                ConditionBase data = pool.Spawn();
                return (T)data;
            }
        }

        /// <summary>
        /// 回收比较条件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        public static void Recycle<T>(T t) where T : ConditionBase
        {
            lock (pools)
            {
                SimplePool<ConditionBase> pool;
                if (!pools.TryGetValue(typeof(T), out pool))
                {
                    pool = new SimplePool<ConditionBase>();
                    pools.Add(typeof(T), pool);
                }
                pool.Recycle(t);
            }    
        }
    }

    /// <summary>
    /// 分级条件
    /// </summary>
    public class ConditionBase : ISimplePoolData
    {
        public virtual void Recycle()
        {

        }

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public virtual bool IsCondition()
        {
            return false;
        }

        bool isUsed = false;

        public bool IsUsed
        {
            get
            {
                return isUsed;
            }
        }

        void ISimplePoolData.PutIn()
        {
            isUsed = false;
        }

        void ISimplePoolData.PutOut()
        {
            isUsed = true;
        }

        bool disposed = false;

        public bool Disposed
        {
            get
            {
                return disposed;
            }
        }

        void IDisposable.Dispose()
        {
            disposed = true;
        }
    }

    public enum ConditionType
    {
        Equal,
        UnEqual,
        Less,
        LEqual,
        Greater,
        GEqual,
    }

    public enum ConditionRel
    {
        /// <summary>
        /// 与
        /// </summary>
        And,
        /// <summary>
        /// 或
        /// </summary>
        Or,
    }

    /// <summary>
    /// 条件比较 float 
    /// </summary>
    public class ConditionFloat : ConditionBase
    {
        public void SetCondition(float minConditionValue, ConditionType minConditionType, float maxConditionValue, ConditionType maxConditionType, float conditionValueMid)
        {
            this.minConditionValue = minConditionValue;
            this.maxConditionValue = maxConditionValue;
            this.minConditionType = minConditionType;
            this.maxConditionType = maxConditionType;
            this.conditionValueMid = conditionValueMid;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionFloat Spawn()
        {
            return ConditionHelper.Spawn<ConditionFloat>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionFloat>(this);
        }

        float minConditionValue;

        ConditionType minConditionType;

        float maxConditionValue;

        ConditionType maxConditionType;

        float conditionValueMid;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            bool min = false;
            switch (minConditionType)
            {
                case ConditionType.Equal:
                    {
                        min = conditionValueMid == minConditionValue;
                    }
                    break;
                case ConditionType.UnEqual:
                    {
                        min = conditionValueMid != minConditionValue;
                    }
                    break;
                case ConditionType.Less:
                    {
                        min = conditionValueMid < minConditionValue;
                    }
                    break;
                case ConditionType.LEqual:
                    {
                        min = conditionValueMid <= minConditionValue;
                    }
                    break;
                case ConditionType.Greater:
                    {
                        min = conditionValueMid > minConditionValue;
                    }
                    break;
                case ConditionType.GEqual:
                    {
                        min = conditionValueMid >= minConditionValue;
                    }
                    break;
            }
            bool max = false;
            switch (maxConditionType)
            {
                case ConditionType.Equal:
                    {
                        max = conditionValueMid == maxConditionValue;
                    }
                    break;
                case ConditionType.UnEqual:
                    {
                        max = conditionValueMid != maxConditionValue;
                    }
                    break;
                case ConditionType.Less:
                    {
                        max = conditionValueMid < maxConditionValue;
                    }
                    break;
                case ConditionType.LEqual:
                    {
                        max = conditionValueMid <= maxConditionValue;
                    }
                    break;
                case ConditionType.Greater:
                    {
                        max = conditionValueMid > maxConditionValue;
                    }
                    break;
                case ConditionType.GEqual:
                    {
                        max = conditionValueMid >= maxConditionValue;
                    }
                    break;
            }
            return min & max;
        }
    }

    /// <summary>
    /// 条件比较 long
    /// </summary>
    public class ConditionLong : ConditionBase
    {
        public void SetCondition(long minConditionValue, ConditionType minConditionType, long maxConditionValue, ConditionType maxConditionType, long conditionValueMid)
        {
            this.minConditionValue = minConditionValue;
            this.maxConditionValue = maxConditionValue;
            this.minConditionType = minConditionType;
            this.maxConditionType = maxConditionType;
            this.conditionValueMid = conditionValueMid;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionLong Spawn()
        {
            return ConditionHelper.Spawn<ConditionLong>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionLong>(this);
        }

        long minConditionValue;

        ConditionType minConditionType;

        long maxConditionValue;

        ConditionType maxConditionType;

        long conditionValueMid;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            bool min = false;
            switch (minConditionType)
            {
                case ConditionType.Equal:
                    {
                        min = conditionValueMid == minConditionValue;
                    }
                    break;
                case ConditionType.UnEqual:
                    {
                        min = conditionValueMid != minConditionValue;
                    }
                    break;
                case ConditionType.Less:
                    {
                        min = conditionValueMid < minConditionValue;
                    }
                    break;
                case ConditionType.LEqual:
                    {
                        min = conditionValueMid <= minConditionValue;
                    }
                    break;
                case ConditionType.Greater:
                    {
                        min = conditionValueMid > minConditionValue;
                    }
                    break;
                case ConditionType.GEqual:
                    {
                        min = conditionValueMid >= minConditionValue;
                    }
                    break;
            }
            bool max = false;
            switch (maxConditionType)
            {
                case ConditionType.Equal:
                    {
                        max = conditionValueMid == maxConditionValue;
                    }
                    break;
                case ConditionType.UnEqual:
                    {
                        max = conditionValueMid != maxConditionValue;
                    }
                    break;
                case ConditionType.Less:
                    {
                        max = conditionValueMid < maxConditionValue;
                    }
                    break;
                case ConditionType.LEqual:
                    {
                        max = conditionValueMid <= maxConditionValue;
                    }
                    break;
                case ConditionType.Greater:
                    {
                        max = conditionValueMid > maxConditionValue;
                    }
                    break;
                case ConditionType.GEqual:
                    {
                        max = conditionValueMid >= maxConditionValue;
                    }
                    break;
            }
            return min & max;
        }
    }

    /// <summary>
    /// 条件比较 Bool
    /// </summary>
    public class ConditionBool : ConditionBase
    {

        public void SetCondition(bool conditionValueRight, ConditionType conditionType, bool conditionValueLeft)
        {
            this.conditionValueRight = conditionValueRight;
            this.conditionType = conditionType;
            this.conditionValueLeft = conditionValueLeft;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionBool Spawn()
        {
            return ConditionHelper.Spawn<ConditionBool>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionBool>(this);
        }

        bool conditionValueRight;

        ConditionType conditionType;

        bool conditionValueLeft;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            switch (conditionType)
            {
                case ConditionType.Equal:
                case ConditionType.GEqual:
                case ConditionType.LEqual:
                    {
                        return conditionValueLeft == conditionValueRight;
                    }
                default:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
            }
        }
    }

    /// <summary>
    /// 条件比较 Color
    /// </summary>
    public class ConditionColor : ConditionBase
    {
        public void SetCondition(UnityEngine.Color conditionValueRight, ConditionType conditionType, UnityEngine.Color conditionValueLeft)
        {
            this.conditionValueRight = conditionValueRight;
            this.conditionType = conditionType;
            this.conditionValueLeft = conditionValueLeft;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionColor Spawn()
        {
            return ConditionHelper.Spawn<ConditionColor>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionColor>(this);
        }

        UnityEngine.Color conditionValueRight;

        ConditionType conditionType;

        UnityEngine.Color conditionValueLeft;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            switch (conditionType)
            {
                case ConditionType.Equal:
                    {
                        return conditionValueLeft == conditionValueRight;
                    }
                case ConditionType.GEqual:
                    {
                        return conditionValueLeft == conditionValueRight || 
                            (conditionValueLeft.r * conditionValueLeft.r + conditionValueLeft.g * conditionValueLeft.g + conditionValueLeft.b * conditionValueLeft.b + conditionValueLeft.a * conditionValueLeft.a) > (conditionValueRight.r * conditionValueRight.r + conditionValueRight.g * conditionValueRight.g + conditionValueRight.b * conditionValueRight.b + conditionValueRight.a * conditionValueRight.a);
                    }
                case ConditionType.LEqual:
                    {
                        return conditionValueLeft == conditionValueRight ||
                            (conditionValueLeft.r * conditionValueLeft.r + conditionValueLeft.g * conditionValueLeft.g + conditionValueLeft.b * conditionValueLeft.b + conditionValueLeft.a * conditionValueLeft.a) < (conditionValueRight.r * conditionValueRight.r + conditionValueRight.g * conditionValueRight.g + conditionValueRight.b * conditionValueRight.b + conditionValueRight.a * conditionValueRight.a);
                    }
                case ConditionType.Greater:
                    {
                        return (conditionValueLeft.r * conditionValueLeft.r + conditionValueLeft.g * conditionValueLeft.g + conditionValueLeft.b * conditionValueLeft.b + conditionValueLeft.a * conditionValueLeft.a) > (conditionValueRight.r * conditionValueRight.r + conditionValueRight.g * conditionValueRight.g + conditionValueRight.b * conditionValueRight.b + conditionValueRight.a * conditionValueRight.a);
                    }
                case ConditionType.Less:
                    {
                        return (conditionValueLeft.r * conditionValueLeft.r + conditionValueLeft.g * conditionValueLeft.g + conditionValueLeft.b * conditionValueLeft.b + conditionValueLeft.a * conditionValueLeft.a) > (conditionValueRight.r * conditionValueRight.r + conditionValueRight.g * conditionValueRight.g + conditionValueRight.b * conditionValueRight.b + conditionValueRight.a * conditionValueRight.a);
                    }
                case ConditionType.UnEqual:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
                default:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
            }
        }
    }

    /// <summary>
    /// 条件比较 Vector2
    /// </summary>
    public class ConditionVector2 : ConditionBase
    {
        public void SetCondition(UnityEngine.Vector2 conditionValueRight, ConditionType conditionType, UnityEngine.Vector2 conditionValueLeft)
        {
            this.conditionValueRight = conditionValueRight;
            this.conditionType = conditionType;
            this.conditionValueLeft = conditionValueLeft;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionVector2 Spawn()
        {
            return ConditionHelper.Spawn<ConditionVector2>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionVector2>(this);
        }

        UnityEngine.Vector2 conditionValueRight;

        ConditionType conditionType;

        UnityEngine.Vector2 conditionValueLeft;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            switch (conditionType)
            {
                case ConditionType.Equal:
                    {
                        return conditionValueLeft == conditionValueRight;
                    }
                case ConditionType.GEqual:
                    {
                        return conditionValueLeft == conditionValueRight ||
                            (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y ) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y);
                    }
                case ConditionType.LEqual:
                    {
                        return conditionValueLeft == conditionValueRight ||
                            (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y ) < (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y);
                    }
                case ConditionType.Greater:
                    {
                        return (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y ) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y);
                    }
                case ConditionType.Less:
                    {
                        return (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y ) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y);
                    }
                case ConditionType.UnEqual:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
                default:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
            }
        }
    }

    /// <summary>
    /// 条件比较 Vector3
    /// </summary>
    public class ConditionVector3 : ConditionBase
    {
        public void SetCondition(UnityEngine.Vector3 conditionValueRight, ConditionType conditionType, UnityEngine.Vector3 conditionValueLeft)
        {
            this.conditionValueRight = conditionValueRight;
            this.conditionType = conditionType;
            this.conditionValueLeft = conditionValueLeft;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionVector3 Spawn()
        {
            return ConditionHelper.Spawn<ConditionVector3>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionVector3>(this);
        }

        UnityEngine.Vector3 conditionValueRight;

        ConditionType conditionType;

        UnityEngine.Vector3 conditionValueLeft;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            switch (conditionType)
            {
                case ConditionType.Equal:
                    {
                        return conditionValueLeft == conditionValueRight;
                    }
                case ConditionType.GEqual:
                    {
                        return conditionValueLeft == conditionValueRight ||
                            (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z);
                    }
                case ConditionType.LEqual:
                    {
                        return conditionValueLeft == conditionValueRight ||
                            (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z ) < (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z );
                    }
                case ConditionType.Greater:
                    {
                        return (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z ) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z );
                    }
                case ConditionType.Less:
                    {
                        return (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z);
                    }
                case ConditionType.UnEqual:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
                default:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
            }
        }
    }

    /// <summary>
    /// 条件比较 Vector4
    /// </summary>
    public class ConditionVector4 : ConditionBase
    {
        public void SetCondition(UnityEngine.Vector4 conditionValueRight, ConditionType conditionType, UnityEngine.Vector4 conditionValueLeft)
        {
            this.conditionValueRight = conditionValueRight;
            this.conditionType = conditionType;
            this.conditionValueLeft = conditionValueLeft;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionVector4 Spawn()
        {
            return ConditionHelper.Spawn<ConditionVector4>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionVector4>(this);
        }

        UnityEngine.Vector4 conditionValueRight;

        ConditionType conditionType;

        UnityEngine.Vector4 conditionValueLeft;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            switch (conditionType)
            {
                case ConditionType.Equal:
                    {
                        return conditionValueLeft == conditionValueRight;
                    }
                case ConditionType.GEqual:
                    {
                        return conditionValueLeft == conditionValueRight ||
                            (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z + conditionValueLeft.w * conditionValueLeft.w) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z + conditionValueRight.w * conditionValueRight.w);
                    }
                case ConditionType.LEqual:
                    {
                        return conditionValueLeft == conditionValueRight ||
                            (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z + conditionValueLeft.w * conditionValueLeft.w) < (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z + conditionValueRight.w * conditionValueRight.w);
                    }
                case ConditionType.Greater:
                    {
                        return (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z + conditionValueLeft.w * conditionValueLeft.w) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z + conditionValueRight.w * conditionValueRight.w);
                    }
                case ConditionType.Less:
                    {
                        return (conditionValueLeft.x * conditionValueLeft.x + conditionValueLeft.y * conditionValueLeft.y + conditionValueLeft.z * conditionValueLeft.z + conditionValueLeft.w * conditionValueLeft.w) > (conditionValueRight.x * conditionValueRight.x + conditionValueRight.y * conditionValueRight.y + conditionValueRight.z * conditionValueRight.z + conditionValueRight.w * conditionValueRight.w);
                    }
                case ConditionType.UnEqual:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
                default:
                    {
                        return conditionValueLeft != conditionValueRight;
                    }
            }
        }
    }

    /// <summary>
    /// 条件比较 String
    /// </summary>
    public class ConditionString : ConditionBase
    {
        public void SetCondition(string conditionValueRight, ConditionType conditionType, string conditionValueLeft)
        {
            this.conditionValueRight = conditionValueRight;
            this.conditionType = conditionType;
            this.conditionValueLeft = conditionValueLeft;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionString Spawn()
        {
            return ConditionHelper.Spawn<ConditionString>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionString>(this);
        }

        string conditionValueRight;

        ConditionType conditionType;

        string conditionValueLeft;

        /// <summary>
        /// 条件是否达成
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public override bool IsCondition()
        {
            switch (conditionType)
            {
                case ConditionType.Equal:
                case ConditionType.GEqual:
                case ConditionType.LEqual:
                    {
                        if (conditionValueRight==null)
                        {
                            if (conditionValueLeft==null)
                            {
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            if (conditionValueLeft==null)
                            {
                                return false;
                            }
                            if (conditionValueLeft.CompareTo(conditionValueRight) !=0)
                            {
                                return false;
                            }
                            return true;
                        }
                    }
                default:
                    {
                        if (conditionValueRight == null)
                        {
                            if (conditionValueLeft==null)
                            {
                                return false;
                            }
                            return true;
                        }
                        else
                        {
                            if (conditionValueLeft == null)
                            {
                                return true;
                            }
                            if (conditionValueLeft.CompareTo(conditionValueRight) != 0)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
            }
        }
    }

    /// <summary>
    /// 条件比较 ConditionBase List
    /// </summary>
    public class ConditionList : ConditionBase
    {
        public void SetCondition(List<ConditionBase> conditionList, ConditionRel relation)
        {
            this.conditionList = conditionList;
            this.relation = relation;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionList Spawn()
        {
            return ConditionHelper.Spawn<ConditionList>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionList>(this);
        }

        List<ConditionBase>  conditionList;

        ConditionRel relation;

        public override bool IsCondition()
        {
            switch (relation)
            {
                case ConditionRel.And:
                    {
                        for (int i=0;i< conditionList.Count;++i)
                        {
                            if (!conditionList[i].IsCondition())
                            {
                                return false;
                            }
                        }
                        return true;
                    }
                case ConditionRel.Or:
                    {
                        for (int i = 0; i < conditionList.Count; ++i)
                        {
                            if (conditionList[i].IsCondition())
                            {
                                return true;
                            }
                        }
                        return false;
                    }
            }
            return false;
        }
    }

    /// <summary>
    /// 条件比较 ConditionBase
    /// </summary>
    public class ConditionRelation : ConditionBase
    {
        public void SetCondition(ConditionBase conditionLeft, ConditionBase conditionRight, ConditionRel relation)
        {
            this.conditionLeft = conditionLeft;
            this.conditionRight = conditionRight;
            this.relation = relation;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <returns></returns>
        public static ConditionRelation Spawn()
        {
            return ConditionHelper.Spawn<ConditionRelation>();
        }

        /// <summary>
        /// 回收
        /// </summary>
        public override void Recycle()
        {
            ConditionHelper.Recycle<ConditionRelation>(this);
        }

        ConditionBase conditionLeft;

        ConditionBase conditionRight;

        ConditionRel relation;

        public override bool IsCondition()
        {
            bool blA = conditionLeft.IsCondition();
            bool blB = conditionLeft.IsCondition();
            switch (relation)
            {
                case ConditionRel.And:
                    {
                        return blA && blB;
                    }
                case ConditionRel.Or:
                    {
                        return blA || blB;
                    }
            }
            return false;
        }

    }
}


