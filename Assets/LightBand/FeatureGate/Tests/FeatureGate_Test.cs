using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


public class FeatureGate_Test
{
    // A Test behaves as an ordinary method
    [Test]
    public void FeatureGate_Generic_Test()
    {
        // Use the Assert class to test conditions  
        var fg = FeatureGateCenter.Instance;

        string textOut = "";
        Action<Feature<bool>> va = (x) =>
        {
            textOut = x.Name;
        };
        fg.Register<bool>(va);

        string textIn = "你好吗？";
        fg.AddFeatureGate<bool>(
            textIn,
            () => true,
            (value) => { Debug.Log("ssssss" + value); }
        );
        Assert.That(textOut, Is.EqualTo("你好吗？"));
        
    }

    [Test]
    public void FeatureGate_Object_Test()
    {
        // Use the Assert class to test conditions  
        var fg = FeatureGateCenter.Instance;

        string textOut = "";
        Action<Feature<bool>> va = (x) =>
        {
            textOut = x.Name;
        };
        fg.Register<bool>(va);

        string textIn = "你好吗？";
        fg.AddFeatureGate(
            textIn,
            () => true,
            (value) => { Debug.Log("ssssss" + value); }
        );
        Assert.That(textOut, Is.EqualTo("你好吗？"));
    }
     [Test]
    public void FeatureGate_ReTriggerAll_Test()
    {
        var fg = FeatureGateCenter.Instance;
        string textOut = "Test";

        string textIn = "你好吗？";
        fg.AddFeatureGate(
            textIn,
            () => true,
            (value) => { Debug.Log("ssssss" + value); }
        );
        Assert.That(textOut, Is.EqualTo("Test"));

        Action<Feature<bool>> va = (x) =>
        {
            textOut = x.Name;
        };
        fg.Register<bool>(va);
        fg.ReTriggerAll();
        Assert.That(textOut, Is.EqualTo("你好吗？"));
    }

    // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // `yield return null;` to skip a frame.
    [UnityTest]
    public IEnumerator FeatureGate_TestWithEnumeratorPasses()
    {
        // Use the Assert class to test conditions.
        // Use yield to skip a frame.
        yield return null;
    }
}
