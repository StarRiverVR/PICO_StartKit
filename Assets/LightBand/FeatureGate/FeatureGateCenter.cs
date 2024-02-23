using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public enum FeatureCategory{
    Common,
    Scene,
    
}
public class Feature
{
    public string Name;
    public int Order;
    public Type type;

    public FeatureCategory Category = FeatureCategory.Common;

    
}
public class Feature<T> : Feature
{
    public Func<T> GetValue;
    public T MaxValue;
    public Action<T> SetValue;
}

public class FeatureMethod : Feature<MethodInfo>
{
    public MethodInfo MethodInfo;
    public GMCommandAttribute GMCommandAttribute;
}

public class FeatureInt : Feature<Int32> { 
};
public class FeatureFloat : Feature<float> {
 };
public class FeatureBool : Feature<bool> { };
public class FeatureString : Feature<string> { };

public class FeatureGateCenter
{
    private static FeatureGateCenter _instance = null;

    public static FeatureGateCenter Instance
    {
        get
        {
            FeatureGateCenter instance;
            if (null == _instance)
            {
                instance = new FeatureGateCenter();
                if (null == _instance) _instance = instance;
            }

            return _instance;
        }
    }
    private int m_Order;
    Dictionary<string, List<Delegate>> m_CallbackDic = new Dictionary<string, List<Delegate>>();

    // public Dictionary<string, Feature
    private FeatureGateCenter()
    {
        this.m_Order = 0;
    }
    public List<Feature> featureList = new List<Feature>();

    public Feature AddFeatureGate<T>(string text, T value, T maxValue, Action<T> setValue)
    {
        return this.AddFeatureGate<T>(text, () => value, maxValue, setValue);
    }

    public Feature AddFeatureGate<T>(string text, T value, Action<T> setValue)
    {
        return this.AddFeatureGate<T>(text, () => value, default(T), setValue);
    }

    public Feature AddFeatureGate<T>(string text, Func<T> getValue, Action<T> setValue){
        return this.AddFeatureGate<T>(text, getValue, default(T), setValue);
    }
    public Feature AddFeatureGate<T>(string text, Func<T> getValue, T maxValue, Action<T> setValue)
    {
        Debug.Log("FeatureGateCenter " + "AddFeatureGate " + text);
        var newFeatureGate = new Feature<T>
        {
            Name = text,
            Order = this.m_Order++,
            GetValue = getValue,
            SetValue = setValue,
            type = typeof(T),
            MaxValue = maxValue
        };

        return this.AddFeatureGate<T>(newFeatureGate);
    }

    public Feature AddFeatureGate(MethodInfo methodInfo, GMCommandAttribute gmCommandAttribute)
    {
        Debug.Log("FeatureGateCenter " + "AddFeatureGate " + gmCommandAttribute.Command);
        var newFeatureGate = new FeatureMethod
        {
            Name = gmCommandAttribute.Command,
            Order = this.m_Order++,
            MethodInfo = methodInfo,
            GMCommandAttribute = gmCommandAttribute,
            type = typeof(MethodInfo),
        };

        return this.AddFeatureGate(newFeatureGate);
    }

    public Feature AddFeatureGate<T>(Feature<T> feature)
    {
        this.featureList.Add(feature);
        
        this.TriggerFeatureEvent<T>(feature);
        return feature;
    }

    public Feature AddFeatureGate(FeatureMethod feature)
    {
        this.featureList.Add(feature);
        
        this.TriggerFeatureEvent(feature);
        return feature;
    }

    public void Register<T>(Action<Feature<T>> callback)
    {
        var key = typeof(T).ToString();
        if(!this.m_CallbackDic.ContainsKey(key)) this.m_CallbackDic[key] = new List<Delegate>();

        this.m_CallbackDic[key].Add(callback);
    }

    public void Register(Action<FeatureMethod> callback)
    {
        var key = typeof(MethodInfo).ToString();
        if(!this.m_CallbackDic.ContainsKey(key)) this.m_CallbackDic[key] = new List<Delegate>();

        this.m_CallbackDic[key].Add(callback);
    }

    public void UnRegister<T>(Action<Feature<T>> callback)
    {
        var key = typeof(T).ToString();
        if(!this.m_CallbackDic.ContainsKey(key)) 
            return;
        
        this.m_CallbackDic[key].Remove(callback);
    }

    public void UnRegister(Action<FeatureMethod> callback)
    {
        var key = typeof(MethodInfo).ToString();
        if(!this.m_CallbackDic.ContainsKey(key)) 
            return;
        
        this.m_CallbackDic[key].Remove(callback);
    }

    void TriggerFeatureEvent<T>(Feature<T> feature)
    {

        if(!this.m_CallbackDic.ContainsKey(typeof(T).ToString())) return;
        
        if (this.m_CallbackDic[typeof(T).ToString()].Count == 0)
        {
            Debug.LogWarning("没有对应的回调函数" + typeof(T).ToString() + "\n 也许FeatureGate不支持该类型");
            return;
        }

        this.m_CallbackDic[typeof(T).ToString()].ForEach(x=> (x as Action<Feature<T>>)(feature));
    }

    void TriggerFeatureEvent(FeatureMethod feature)
    {

        if(!this.m_CallbackDic.ContainsKey(typeof(MethodInfo).ToString())) return;
        
        if (this.m_CallbackDic[typeof(MethodInfo).ToString()].Count == 0)
        {
            Debug.LogWarning("没有对应的回调函数" + typeof(MethodInfo).ToString() + "\n 也许FeatureGate不支持该类型");
            return;
        }

        this.m_CallbackDic[typeof(MethodInfo).ToString()].ForEach(x=> (x as Action<FeatureMethod>)(feature));
    }

    public void ReTriggerAll()
    {
        foreach (var feature in this.featureList)
        {
            if(feature == null) continue;
            
            if (feature.type == typeof(bool))
                this.TriggerFeatureEvent((Feature<bool>)feature);

            if (feature.type == typeof(string))
                this.TriggerFeatureEvent((Feature<string>)feature);

            if (feature.type == typeof(int))
                this.TriggerFeatureEvent((Feature<int>)feature);

            if (feature.type == typeof(float))
                this.TriggerFeatureEvent((Feature<float>)feature);

            if (feature.type == typeof(MethodInfo))
                this.TriggerFeatureEvent((FeatureMethod)feature);
        }
    }
}
