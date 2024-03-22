using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.openxr
{
    public sealed partial class VXRPlugin
    {
        /// <summary>
        /// 注视点等级
        /// </summary>
        [SerializeField]
        public enum FoveationLevel
        {
            None = 0,
            Low = 1,
            Medinum = 2,
            High = 3,
        }
        /// <summary>
        /// 动态注视点等级
        /// </summary>
        [SerializeField]
        public enum FoveationDynamic
        {
            Disabled = 0,
            LevelEnabled = 1,
        }
    }
}
