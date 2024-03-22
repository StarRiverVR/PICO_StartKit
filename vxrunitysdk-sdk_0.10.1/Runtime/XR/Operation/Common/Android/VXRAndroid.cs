
#if USING_XR_UNITYXR && USING_XR_MANAGEMENT && UNITY_ANDROID

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.vivo.codelibrary;

namespace com.vivo.openxr
{
    public class VXRAndroid
    {
        /// <summary>
        /// 缓存类
        /// </summary>
        static Dictionary<string, AndroidJavaClass> _androidJavaClasss = new Dictionary<string, AndroidJavaClass>();

        /// <summary>
        /// 缓存类变量
        /// </summary>
        static Dictionary<string, Dictionary<string, AndroidJavaObject>> _androidJavaObjects = new Dictionary<string, Dictionary<string, AndroidJavaObject>>();

        /// <summary>
        /// 获取Java类。
        /// </summary>
        /// <param name="androidJavaClassName">Java类名</param>
        /// <returns>Java类在Untiy的实例</returns>
        public static AndroidJavaClass GetAndroidJavaClass(string androidJavaClassName)
        {
            if (!_androidJavaClasss.TryGetValue(androidJavaClassName, out AndroidJavaClass androidJavaClass))
            {
                androidJavaClass = new AndroidJavaClass(androidJavaClassName);
                if (androidJavaClass != null)
                {
                    _androidJavaClasss.Add(androidJavaClassName, androidJavaClass);
                }
            }
            return androidJavaClass;
        }


        /// <summary>
        /// 调用Java类的静态方法。
        /// 注意：调用此方法时，会缓存方法返回值。下次调用时直接使用缓存值。
        /// </summary>
        /// <param name="androidJavaClassName">Java类名</param>
        /// <param name="staticMethodName">静态方法名</param>
        /// <returns>静态方法返回参数</returns>
        public static AndroidJavaObject CallAndroidJavaObjectByAndroidJavaClass(string androidJavaClassName, string staticMethodName)
        {
            // 获取已经缓存的Java类静态方法返回值列表。
            if (!_androidJavaObjects.TryGetValue(androidJavaClassName, out var objects))
            {
                objects = new Dictionary<string, AndroidJavaObject>();
                _androidJavaObjects.Add(androidJavaClassName, objects);
            }

            // 在列表中查找是否存在指定静态方法的返回值。
            if (!objects.TryGetValue(staticMethodName, out AndroidJavaObject obj))
            {
                AndroidJavaClass androidJavaClass = GetAndroidJavaClass(androidJavaClassName);
                if (_androidJavaClasss!=null)
                {
                    // 调用静态方法
                    obj = androidJavaClass.CallStatic<AndroidJavaObject>(staticMethodName);
                    if (obj != null)
                    {
                        // 缓存返回值。
                        objects.Add(staticMethodName, obj);
                    }
                    else
                    {
                        VLog.Error($"androidJavaObject no return value , androidJavaClass={androidJavaClassName} ,staticFunction={staticMethodName}");
                    }
                }
                else
                {
                    VLog.Error($"androidJavaClass is null :{androidJavaClassName}");
                }
              
            }
            return obj;
        }

        /// <summary>
        /// 获取Java静态变量。
        /// </summary>
        /// <param name="androidJavaClassName">Java类名</param>
        /// <param name="staticVarName">变量名</param>
        /// <returns>Unity中的Java静态变量封装实例</returns>
        public static AndroidJavaObject GetAndroidJavaObjectByAndroidJavaClass(string androidJavaClassName, string staticVarName)
        {
            // 获取已经缓存的Java类静态方法返回值列表。
            if (!_androidJavaObjects.TryGetValue(androidJavaClassName, out var objects))
            {
                objects = new Dictionary<string, AndroidJavaObject>();
                _androidJavaObjects.Add(androidJavaClassName, objects);
            }

            if (!objects.TryGetValue(staticVarName, out AndroidJavaObject obj))
            {
                AndroidJavaClass androidJavaClass = GetAndroidJavaClass(androidJavaClassName);
                if (_androidJavaClasss != null)
                {
                    obj = androidJavaClass.GetStatic<AndroidJavaObject>(staticVarName);
                    if (obj != null)
                    {
                        objects.Add(staticVarName, obj);
                    }
                    else
                    {
                        VLog.Error($"androidJavaObject is null , androidJavaClass={androidJavaClassName} ,androidJavaObject={staticVarName}");
                    }
                }
                else
                {
                    VLog.Error($"androidJavaClass is null :{androidJavaClassName}");
                }
            }

            return obj;
        }

        /// <summary>
        /// 调用Java对象实例的成员方法（返回值不能是void）。
        /// Java对象实例可以通过静态方法<see cref="CallAndroidJavaObjectByAndroidJavaClass">或静态变量<see cref="GetAndroidJavaObjectByAndroidJavaClass">获取。
        /// </summary>
        /// <typeparam name="ReturnType">调用成员方法的返回值类型，不能为void。</typeparam>
        /// <param name="androidJavaObject">Java类实例</param>
        /// <param name="methodName">方法名</param>
        /// <param name="args">方法参数</param>
        /// <returns>方法返回值</returns>
        public static ReturnType Call<ReturnType>(AndroidJavaObject androidJavaObject, string methodName, params object[] args)
        {
            if (androidJavaObject == null)
            {
                VLog.Error("VivoOpenXRAndroid.Call Err . androidJavaObject is null .");
                return default(ReturnType);
            }
            ReturnType res = androidJavaObject.Call<ReturnType>(methodName, args);
            if (res == null)
            {
                VLog.Error("VivoOpenXRAndroid.Call Err . ReturnType is null .");
            }
            return res;
        }

        /// <summary>
        /// 调用Java对象实例的成员方法。
        /// Java对象实例可以通过静态方法<see cref="CallAndroidJavaObjectByAndroidJavaClass">或静态变量<see cref="GetAndroidJavaObjectByAndroidJavaClass">获取。
        /// </summary>
        /// <param name="androidJavaObject"></param>
        /// <param name="methodName">Java类实例</param>
        /// <param name="args">方法参数</param>
        public static void Call(AndroidJavaObject androidJavaObject, string methodName, params object[] args)
        {
            if (androidJavaObject == null)
            {
                VLog.Error("VivoOpenXRAndroid.Call Err . androidJavaObject is null .");
                return ;
            }
            androidJavaObject.Call(methodName, args);
        }
    }

}


#endif


