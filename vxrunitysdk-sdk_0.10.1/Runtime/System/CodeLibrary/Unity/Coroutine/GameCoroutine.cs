
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// 协程控制器 可以在编辑器状态下使用 编辑器状态下使用协程内只能使用 yield return null; 其他yield效果与 null一样会立刻返回
    /// </summary>
    public class GameCoroutine:CSingleton<GameCoroutine>, IDisposable
    {
#if UNITY_EDITOR

        bool isEditorStart = false;

        void EditorStart(IEnumerator iterator)
        {
            iEnumerators.Push(iterator);
            if (isEditorStart) return;
            isEditorStart = true;
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
        }

        void EditorClose()
        {
            if (!isEditorStart) return;
            isEditorStart = false;
            EditorApplication.update -= EditorUpdate;
        }

        void EditorUpdate()
        {
            IEnumerator iEnumerator = iEnumerators.Peek();
            if (!iEnumerator.MoveNext())
            {
                iEnumerators.Pop();
                if (iEnumerators.Count == 0)
                {
                    EditorClose();
                }
            }
        }

        Stack<IEnumerator> iEnumerators = new Stack<IEnumerator>();

#endif
        /// <summary>
        /// 开启协程
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        public IEnumerator StartCoroutine(IEnumerator iterator)
        {
            if (iterator == null) return null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((obj)=> {
                    ErrLockData errLockDataA = null;
                    if (ErrLock.ErrLockOpen)
                    {
                        errLockDataA = ErrLock.LockStart("GameCoroutine.cs-->StartCoroutine-->65");
                    }
                    IEnumerator ie = (IEnumerator)obj;
                    RunCoroutine(ie);
                    if (ErrLock.ErrLockOpen)
                    {
                        ErrLock.LockEnd(errLockDataA);
                    }
                }, iterator);
            }
            else
            {
                RunCoroutine(iterator);
            }
            return iterator;
        }

        void RunCoroutine(IEnumerator iterator)
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                EditorStart(iterator);
            }
            else
            {
                GameCoroutineBehaviour.Instance.StartCoroutine(iterator);
            }
#else
            GameCoroutineBehaviour.Instance.StartCoroutine(iterator);
#endif
        }

        public void Dispose()
        {

        }
    }
}


