using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public class GameCoroutineBehaviour : MonoSingleton<GameCoroutineBehaviour>
    {
        protected override void AwakeFun()
        {
            GameObject.DontDestroyOnLoad(gameObject);
            gameObject.name = "GameCoroutineBehaviour";
#if !UNITY_EDITOR
            gameObject.hideFlags = HideFlags.HideAndDontSave;
#endif
        }
    }
}


