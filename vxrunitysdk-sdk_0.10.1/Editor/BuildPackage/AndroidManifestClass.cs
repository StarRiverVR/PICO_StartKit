using com.vivo.openxr;
using System.Xml;
using UnityEditor.Android;
using UnityEditor;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;


namespace com.vivo.editor
{
    class AndroidManifestClass : IPostGenerateGradleAndroidProject
    {
        public void OnPostGenerateGradleAndroidProject(string path)
        {
#if UNITY_ANDROID
            ConfigAndroidManifest(BuildTargetGroup.Android, path);
#endif
        }

        public int callbackOrder { get { return 10000; } }

        public static void ConfigAndroidManifest(BuildTargetGroup targetGroup, string path)
        {
            bool isVivoXR = false;
            OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(targetGroup);
            foreach (OpenXRFeature feature in settings.GetFeatures<OpenXRFeature>())
            {
                if (feature.enabled)
                {
                    if (feature is VXRFeature)
                    {
                        isVivoXR = true;
                        break;
                    }
                }
            }
            if (isVivoXR)
            {
                ConfigAndroidManifest(path);
            }
        }

        static XmlNodeList FindTargetItemListByPath(XmlDocument doc, string pathName)
        {
            XmlNodeList findList = doc.SelectNodes("//" + pathName);
            return findList;
        }

        static XmlNode FindTargetItemByPath(XmlDocument doc, string pathName, string attributeName, string attributeValue)
        {
            XmlNodeList findList = FindTargetItemListByPath(doc, pathName);
            foreach (XmlNode grandsonNode in findList)
            {
                XmlAttributeCollection xmlAttributeCollection = grandsonNode.Attributes;
                XmlNode attributeNode = xmlAttributeCollection.GetNamedItem(attributeName);
                if (attributeNode != null && attributeNode.Value.CompareTo(attributeValue) == 0)
                {
                    return grandsonNode;
                }
            }
            return null;
        }

        static string FindTargetItemValueByPath(XmlDocument doc, string pathName, string attributeName)
        {
            XmlNodeList findList = FindTargetItemListByPath(doc, pathName);
            foreach (XmlNode grandsonNode in findList)
            {
                XmlAttributeCollection xmlAttributeCollection = grandsonNode.Attributes;
                XmlNode attributeNode = xmlAttributeCollection.GetNamedItem(attributeName);
                if (attributeNode != null)
                {
                    return attributeNode.Value;
                }
            }
            return null;
        }

        static void CreateItemByPath(XmlDocument doc, string pathName, string attributeName, string attributeValue, string url, bool createNew)
        {
            if (FindTargetItemByPath(doc, pathName, attributeName, attributeValue) == null)
            {
                XmlElement firstElement = null;
                XmlElement lastElement = null;
                string[] strs = pathName.Split('/');
                for (int i = 0; i < strs.Length; ++i)
                {
                    bool find = false;
                    XmlElement newElement = null;
                    string creatName = strs[i].Trim();
                    XmlNodeList xmlNodeList = doc.GetElementsByTagName(creatName);
                    if (xmlNodeList.Count == 0 || i == strs.Length - 1 || (i == strs.Length - 2 && createNew))
                    {
                        newElement = doc.CreateElement(creatName);
                    }
                    else
                    {
                        XmlNode xmlNode = xmlNodeList[xmlNodeList.Count - 1];
                        newElement = (XmlElement)xmlNode;
                        find = true;
                    }
                    if (firstElement == null)
                    {
                        firstElement = newElement;
                    }
                    if (lastElement != null && !find)
                    {
                        lastElement.AppendChild(newElement);
                    }
                    lastElement = newElement;
                }
                if (attributeName.Contains(":"))
                {
                    lastElement.SetAttribute(attributeName.Split(':')[1].Trim(), url, attributeValue);
                }
                else
                {
                    lastElement.SetAttribute(attributeName, attributeValue);
                }
                XmlNode root = doc.DocumentElement;
                root.AppendChild(firstElement);
            }
        }

        static void ConfigAndroidManifest(string path)
        {
            string originManifestPath = path + "/src/main/AndroidManifest.xml";
            //
            XmlDocument doc = new XmlDocument();
            doc.Load(originManifestPath);

            //
            string url = FindTargetItemValueByPath(doc, "manifest", "xmlns:android");
            XmlNode manifestNode = doc.SelectSingleNode("//manifest");
            XmlElement manifestElement = (XmlElement)manifestNode;
            manifestElement.SetAttribute("xmlns:android", url);

            //权限创建
            CreateItemByPath(doc, "uses-permission", "android:name", "android.permission.CAMERA", url, true);
            CreateItemByPath(doc, "uses-permission", "android:name", "android.permission.INTERNET", url, true);
            CreateItemByPath(doc, "uses-permission", "android:name", "com.qualcomm.qti.qxr.QXRServiceClientPermission", url, true);
            CreateItemByPath(doc, "uses-permission", "android:name", "org.khronos.openxr.permission.OPENXR", url, true);
            CreateItemByPath(doc, "uses-permission", "android:name", "org.khronos.openxr.permission.OPENXR_SYSTEM", url, true);
            //queries创建
            CreateItemByPath(doc, "queries/provider", "android:authorities", "org.khronos.openxr.runtime_broker;org.khronos.openxr.system_runtime_broker", url, true);
            CreateItemByPath(doc, "queries/intent/action", "android:name", "org.khronos.openxr.OpenXRRuntimeService", url, true);
            CreateItemByPath(doc, "queries/intent/action", "android:name", "org.khronos.openxr.OpenXRApiLayerService", url, true);
            //
            doc.Save(originManifestPath);
        }

    }

}