using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Threading;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// c#程序集工具
    /// </summary>
    public class CLR
    {
        /// <summary>
        /// 是否为泛型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsT(Type type)
        {
            return type.IsGenericType;
        }

        /// <summary>
        /// 判断类型是否为 UnityEngine.MonoBehaviour
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsMonoBehaviour(Type type)
        {
            return IsTargetType(type, typeof(UnityEngine.MonoBehaviour));
        }

        /// <summary>
        /// 判断类型是否为 UnityEngine.Behaviour
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsBehaviour(Type type)
        {
            return IsTargetType(type, typeof(UnityEngine.Behaviour));
        }

        /// <summary>
        /// 判断类型是否为 UnityEngine.Component
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsComponent(Type type)
        {
            return IsTargetType(type, typeof(UnityEngine.Component));
        }

        /// <summary>
        /// 判断类型是否为 UnityEngine.Object
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsUnityEngineObject(Type type)
        {
            return IsTargetType(type, typeof(UnityEngine.Object));
        }

        /// <summary>
        /// 判断类型type是否是targetType类型
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        static bool IsTargetType(Type type, Type targetType)
        {
            if (type == null || targetType == null) return false;
            if (type == targetType) { return true; }
            if (type.BaseType != null && type.BaseType != type)
            {
                return IsTargetType(type.BaseType, targetType);
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获得类型中的方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="returnType">返回类型</param>
        /// <param name="methodName"></param>
        /// <param name="paramsTypes">参数类型</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, Type returnType, string methodName,params Type[] paramsTypes)
        {
            BindingFlags bindingAttr = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public;
            return GetMethod(type, methodName, bindingAttr, returnType, paramsTypes);
        }

        /// <summary>
        /// 获得类型中的方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingAttr"></param>
        /// <param name="returnType">返回类型</param>
        /// <param name="paramsTypes">参数类型</param>
        /// <returns></returns>
        public static MethodInfo GetMethod(Type type, string methodName, BindingFlags bindingAttr, Type returnType, params Type[] paramsTypes)
        {
            System.Reflection.MethodInfo[] methodInfos = type.GetMethods(bindingAttr);
            for (int i = 0; i < methodInfos.Length; i++)
            {
                System.Reflection.MethodInfo methodInfo = methodInfos[i];
                if (methodInfo.Name.CompareTo(methodName) == 0)
                {
                    if (returnType!=null)
                    {
                        if (methodInfo.ReturnType!= returnType)
                        {
                            continue;
                        }
                    }
                    if (paramsTypes!=null && paramsTypes.Length>0)
                    {
                        System.Reflection.ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                        if (parameterInfos==null || parameterInfos.Length!= paramsTypes.Length)
                        {
                            continue;
                        }
                        for (int j=0;j< paramsTypes.Length;++j)
                        {
                            if (paramsTypes[j]!= parameterInfos[j].ParameterType)
                            {
                                continue;
                            }
                        }
                    }
                    return methodInfo;
                }
            }
            return null;
        }


        //public static void GetMethodDDDD(Type type, string methodName)
        //{
        //     GetMethodXXXXX(type, methodName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.DeclaredOnly | System.Reflection.BindingFlags.Public);
        //}
        //public static void GetMethodXXXXX(Type type, string methodName, BindingFlags bindingAttr)
        //{
        //    System.Reflection.MethodInfo[] methodInfos = type.GetMethods(bindingAttr);
        //    for (int i = 0; i < methodInfos.Length; i++)
        //    {
        //        if (methodInfos[i].Name.CompareTo(methodName) == 0)
        //        {
        //            ParameterInfo[] fff = methodInfos[i].GetParameters();
        //            UnityEngine.Debug.Log(methodInfos[i].ReturnType);
        //        }
        //    }
        //}

        /// <summary>
        /// 获得目标中字段的值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="fieldName"></param>
        /// <returns></returns>
        public static object GetTypeFieldValue(object obj, string fieldName)
        {
            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName);
            if (fieldInfo == null) return null;
            return fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// 根据匹配度最高的构造函数创建类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type t, params object[] args)
        {
            if (t.IsGenericType)
            {
                //泛型定义
                return default(T);
            }
            T obj = (T)System.Activator.CreateInstance(t, args);
            return obj;
        }

        /// <summary>
        /// 使用默认构造函数创建类型的实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public static T CreateInstance<T>(Type t)
        {
            if (t.IsGenericType)
            {
                //泛型定义
                return default(T);
            }
            T obj = (T)System.Activator.CreateInstance(t);
            return obj;
        }

        /// <summary>
        /// 获得当前c#、正在运行的程序管理
        /// </summary>
        /// <returns></returns>
        public static string GetCLRCallingDomainName()
        {
            return AppDomain.CurrentDomain.FriendlyName;
        }

        /// <summary>
        /// 获得目标程序集
        /// </summary>
        /// <param name="dllManager"></param>
        /// <returns></returns>
        public static Assembly[] GetAssemblys(DLLManager dllManager)
        {
            if (dllManager.AppDomain != null)
            {
                return GetAssemblys(dllManager.AppDomain);
            }
            return null;
        }

        /// <summary>
        /// 获得目标程序集
        /// </summary>
        /// <param name="appDomain"></param>
        /// <returns></returns>
        public static Assembly[] GetAssemblys(AppDomain appDomain)
        {
            return appDomain.GetAssemblies();
        }

        /// <summary>
        /// 从程序集管理器中获得对应类的类型
        /// </summary>
        /// <param name="dllManager">程序集管理器</param>
        /// <param name="attributeType">需要具备的属性的类型</param>
        /// <param name="interfaceType">需要继承的接口类型</param>
        /// <param name="list">查询结果</param>
        /// <param name="bIgnoreAbstract">是否忽视抽象类</param>
        /// <param name="bInheritAttribute">继承属性</param>
        public static void GetTypes(DLLManager dllManager, Type attributeType, Type interfaceType, ref List<Type> list, bool bIgnoreAbstract = true,
            bool bInheritAttribute = false)
        {
            Dictionary<string, DLLObject>.Enumerator enumerator = dllManager.Dlls.GetEnumerator();
            while (enumerator.MoveNext())
            {
                GetTypes(enumerator.Current.Value.Assembly, attributeType, interfaceType, ref list, bIgnoreAbstract, bInheritAttribute);
            }
        }

        /// <summary>
        /// 从类型所在的程序集中获得对应类的类型
        /// </summary>
        /// <param name="Type">类型</param>
        /// <param name="attributeType">需要具备的属性的类型</param>
        /// <param name="interfaceType">需要继承的接口类型</param>
        /// <param name="list">查询结果</param>
        /// <param name="bIgnoreAbstract">是否忽视抽象类</param>
        /// <param name="bInheritAttribute">继承属性</param>
        public static void GetTypes(Type basetype, Type attributeType, Type interfaceType, ref List<Type> list, bool bIgnoreAbstract = true,
            bool bInheritAttribute = false)
        {
            GetTypes(basetype.Assembly, attributeType, interfaceType, ref list, bIgnoreAbstract, bInheritAttribute);
        }

        /// <summary>
        /// 从程序集中获得对应类的类型
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="attributeType">需要具备的属性的类型</param>
        /// <param name="interfaceType">需要继承的接口类型</param>
        /// <param name="list">查询结果</param>
        /// <param name="bIgnoreAbstract">是否忽视抽象类</param>
        /// <param name="bInheritAttribute">继承属性</param>
        public static void GetTypes(Assembly assembly, Type attributeType, Type interfaceType, ref List<Type> list, bool bIgnoreAbstract = true,
            bool bInheritAttribute = false)
        {
            if (assembly == null) return;
            Type[] type = assembly.GetTypes();
            for (int i = 0, listCount = type.Length; i < listCount; ++i)
            {
                Type t = type[i];
                if (interfaceType == null || interfaceType.IsAssignableFrom(t))
                {
                    if (!bIgnoreAbstract || (bIgnoreAbstract && !t.IsAbstract))
                    {
                        if (attributeType!=null)
                        {
                            if (t.GetCustomAttributes(attributeType, bInheritAttribute).Length > 0)
                            {
                                if (list == null)
                                {
                                    list = new List<Type>();
                                }
                                list.Add(t);
                            }
                        }
                        else
                        {
                            if (list == null)
                            {
                                list = new List<Type>();
                            }
                            list.Add(t);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获得所有子类
        /// </summary>
        /// <param name="parentType"></param>
        /// <param name="list"></param>
        /// <param name="top">true：只获取第一层直接子类</param>
        public static void GetAllChildType(Type parentType, ref List<Type> list, bool top=true)
        {
            list.Clear();
            Assembly assembly = parentType.Assembly;
            Type[] types = assembly.GetTypes();
            for (int i=0,listCount= types.Length;i< listCount;++i)
            {
                Type t = types[i];
                Type baseType = t.BaseType;
                if (baseType!=null)
                {
                    if (top)
                    {
                        if (baseType == parentType || (baseType.Name.CompareTo(parentType.Name) == 0 && baseType.Namespace.CompareTo(parentType.Namespace) == 0))
                        {
                            list.Add(t);
                        }
                    }
                    else
                    {
                        while (baseType!=null)
                        {
                            if (baseType == parentType || (baseType.Name.CompareTo(parentType.Name) == 0 && baseType.Namespace.CompareTo(parentType.Namespace) == 0))
                            {
                                list.Add(t);
                                break;
                            }
                            baseType = baseType.BaseType;
                        }
                    }
                }
                
            }
        }

        /// <summary>
        /// dll 集合管理
        /// 在卸载的时候 会同时卸载其中的dll程序集
        /// </summary>
        public class DLLManager
        {
            public DLLManager(string appDomainName)
            {
                appDomain = AppDomain.CreateDomain(appDomainName);
            }

            /// <summary>
            /// 程序集运行管理器
            /// </summary>
            AppDomain appDomain;

            public AppDomain AppDomain
            {
                get
                {
                    return appDomain;
                }
                set
                {
                    appDomain = value;
                }
            }

            /// <summary>
            ///  key=dll路径 ,value=DLLObject
            /// </summary>
            Dictionary<string, DLLObject> dlls = new Dictionary<string, DLLObject>();

            /// <summary>
            /// 包含的程序集 key=dll路径 ,value=DLLObject
            /// </summary>
            public Dictionary<string, DLLObject> Dlls
            {
                get
                {
                    return dlls;
                }
            }

            /// <summary>
            /// 加载dll
            /// </summary>
            /// <param name="dllFilePath"></param>
            /// <returns></returns>
            public DLLObject LoadDLL(string dllFilePath)
            {
                DLLObject res;
                if (dlls.TryGetValue(dllFilePath, out res))
                {
                    return res;
                }
                res = new DLLObject(dllFilePath, this);
                if (!res.IsNull)
                {
#if UNITY_2018_1_OR_NEWER
                    appDomain.ExecuteAssemblyByName(res.AssemblyName);
#else
                appDomain.ExecuteAssemblyByName(res.AssemblyName, null);
#endif
                    dlls.Add(dllFilePath, res);
                }
                return res;
            }

            /// <summary>
            /// 卸载管理器及下的dll程序集
            /// </summary>
            public void Unload()
            {
                if (appDomain != null)
                {
                    AppDomain.Unload(appDomain);
                }
            }

        }

        /// <summary>
        /// dll程序集
        /// </summary>
        public class DLLObject : MarshalByRefObject
        {
            DLLManager dllManager;

            /// <summary>
            /// 程序集
            /// </summary>
            Assembly assembly;

            /// <summary>
            /// 程序集
            /// </summary>
            public Assembly Assembly
            {
                get
                {
                    return assembly;
                }
            }

            /// <summary>
            /// 是否为空
            /// </summary>
            public bool IsNull
            {
                get
                {
                    if (assembly == null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            /// <summary>
            /// 程序集名
            /// </summary>
            public AssemblyName AssemblyName
            {
                get
                {
                    if (assembly != null)
                    {
                        return assembly.GetName();
                    }
                    return null;
                }
            }

            /// <summary>
            /// 传入 .dll 路径加载指定的dll 文件
            /// </summary>
            /// <param name="dllFilePath"></param>
            public DLLObject(string dllFilePath, DLLManager dllManager)
            {
                assembly = Assembly.LoadFile(dllFilePath);
                this.dllManager = dllManager;
            }

            /// <summary>
            /// 加载DLL 受限于 cs 垃圾回收机制 此处无法卸载 dll
            /// 在卸载appDomain时 ，处于appDomain中的assembly会被卸载
            /// </summary>
            /// <param name="dllFilePath"></param>
            public void LoadDll(string dllFilePath)
            {
                assembly = Assembly.LoadFile(dllFilePath);
            }

            /// <summary>
            /// 执行dll中的类中的方法
            /// </summary>
            /// <param name="fullClassName">类的全名 TestDll.Class1</param>
            /// <param name="methodName">函数的名 </param>
            /// <param name="args">函数的参数</param>
            /// <returns></returns>
            public bool Invoke(string fullClassName, string methodName, params System.Object[] args)
            {
                if (assembly == null)
                {
                    VLog.Error("DLLObject.cs Invoke assembly null !");
                    return false;
                }
                Type tp = assembly.GetType(fullClassName);
                if (tp == null)
                {
                    VLog.Error("DLLObject.cs Invoke fullClassName not find !");
                    return false;
                }
                MethodInfo method = tp.GetMethod(methodName);
                if (method == null)
                {
                    VLog.Error("DLLObject.cs Invoke method not find !");
                    return false;
                }
                System.Object obj = Activator.CreateInstance(tp);
                method.Invoke(obj, args);
                return true;
            }

        }

    }
}




