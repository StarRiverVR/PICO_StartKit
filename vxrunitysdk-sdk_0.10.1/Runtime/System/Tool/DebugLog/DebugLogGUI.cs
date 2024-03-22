using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine.UI;

namespace com.vivo.codelibrary
{
    public class DebugLogGUI : MonoBehaviour
    {
        static int guiMaxCount = 512;

        static List<IDebugLogData> guiLogCombinList = new List<IDebugLogData>();

        static List<LogData_Log> guiLogList = new List<LogData_Log>();

        static List<LogData_Warning> guiLogWarningList = new List<LogData_Warning>();

        static List<LogData_Error> guiLogErrorList = new List<LogData_Error>();

        static bool isFresh = false;

        static object listLock = new object();

        public static SimplePool<LogData_Log> LogPool = new SimplePool<LogData_Log>();

        public static SimplePool<LogData_Warning> WarningPool = new SimplePool<LogData_Warning>();

        public static SimplePool<LogData_Error> ErrorPool = new SimplePool<LogData_Error>();

        static void ClearGUIMoreLog()
        {
            lock (listLock)
            {
                while (guiLogCombinList.Count >= guiMaxCount*3)
                {
                    IDebugLogData logData = guiLogCombinList[0];
                    guiLogCombinList.RemoveAt(0);
                }
                while (guiLogList.Count >= guiMaxCount)
                {
                    LogData_Log logData = guiLogList[0];
                    guiLogList.RemoveAt(0);
                    LogPool.Recycle(logData);
                }
                while (guiLogWarningList.Count >= guiMaxCount)
                {
                    LogData_Warning logData = guiLogWarningList[0];
                    guiLogWarningList.RemoveAt(0);
                    WarningPool.Recycle(logData);
                }
                while (guiLogErrorList.Count >= guiMaxCount)
                {
                    LogData_Error logData = guiLogErrorList[0];
                    guiLogErrorList.RemoveAt(0);
                    ErrorPool.Recycle(logData);
                }
            }
        }

        public static void AddInGuiLogList(IDebugLogData logData)
        {
            lock (listLock)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogGUI.cs-->71-->AddInGuiLogList");
                }
                ClearGUIMoreLog();
                guiLogCombinList.Add(logData);
                switch (logData.LogState)
                {
                    case LogState.Log:
                        {
                            guiLogList.Add((LogData_Log)logData);
                        }
                        break;
                    case LogState.WarningLog:
                        {
                            guiLogWarningList.Add((LogData_Warning)logData);
                        }
                        break;
                    case LogState.ErrLog:
                        {
                            guiLogErrorList.Add((LogData_Error)logData);
                        }
                        break;
                }
                isFresh = true;
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }
     
        }

        private void Awake()
        {
            GameObject.DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            Init();
        }

        GameObject debugLogPanel;

        Toggle debugLogToggle;

        bool isInit = false;

        void Init()
        {
            if (isInit) return;
            isInit = true;
            debugLogPanel = transform.Find("DebugLogPanel").gameObject;
            debugLogToggle= transform.Find("DebugLogToggle").GetComponent<Toggle>();
            debugLogToggle.isOn = false;
            debugLogToggle.onValueChanged.AddListener(DebugLogToggleChange);
            debugLogPanel.SetActive(false);
            ipInputField = transform.Find("DebugLogPanel/SocketBG/IP InputField").GetComponent<InputField>();
            socketInputField = transform.Find("DebugLogPanel/SocketBG/Port InputField").GetComponent<InputField>();
            clientButton = transform.Find("DebugLogPanel/SocketBG/Button").GetComponent<Button>();
            clientButton.onClick.AddListener(ClientButtonClick);

            allButton = transform.Find("DebugLogPanel/AllButton").GetComponent<Button>();
            allButton.onClick.AddListener(AllButtonClick);
            logButton = transform.Find("DebugLogPanel/LogButton").GetComponent<Button>();
            logButton.onClick.AddListener(LogButtonClick);
            logWarningButton = transform.Find("DebugLogPanel/LogWarningButton").GetComponent<Button>();
            logWarningButton.onClick.AddListener(LogWarningButtonClick);
            logErrorButton = transform.Find("DebugLogPanel/LogErrorButton").GetComponent<Button>();
            logErrorButton.onClick.AddListener(LogErrorButtonClick);

            itemRectTransform = transform.Find("DebugLogPanel/Scroll View/Viewport/Item").GetComponent<RectTransform>();
            itemRoot = transform.Find("DebugLogPanel/Scroll View/Viewport/Content").GetComponent<RectTransform>();
            itemRectTransform.anchorMin = new Vector2(0,1);
            itemRectTransform.anchorMax = new Vector2(0, 1);
            itemRectTransform.gameObject.SetActive(false);
            itemRectTransform.sizeDelta = new Vector2(Screen.width-80, itemRectTransform.sizeDelta.y);

            textShow = transform.Find("DebugLogPanel/TextShow").gameObject;
            textShow.gameObject.SetActive(false);
            textShowCloseButton = transform.Find("DebugLogPanel/TextShow/BG/CloseButton").GetComponent<Button>();
            textShowCloseButton.onClick.AddListener(TextShowClose);
            textShowText= transform.Find("DebugLogPanel/TextShow/BG/Scroll View/Viewport/Content/Text").GetComponent<Text>();
        }

        InputField ipInputField;

        InputField socketInputField;

        Button clientButton;

        void ClientButtonClick() {
            string ip = ipInputField.text.Trim();
            int port = int.Parse(socketInputField.text.Trim());
            clientIsOpen=DebugLogClient.Open(ip, port, ClientRestartConnect, ClientCloseCallBack);
        }

        bool clientIsOpen = false;

        void ClientRestartConnect(bool bl)
        {
            VLog.Info($"连接重连:{bl}");
        }

        void ClientCloseCallBack()
        {
            VLog.Info("连接关闭");
        }

        float curTime = 0;

        private void Update()
        {
            curTime = curTime + Time.deltaTime;
            if (curTime>=0.1f)
            {
                curTime = 0;
                lock (listLock)
                {
                    if (isFresh)
                    {
                        isFresh = false;
                        Fresh();
                    }
                }
            }
        }

        List<IDebugLogData> curList = new List<IDebugLogData>();

        List<DebugLogGUIItem> showlist = new List<DebugLogGUIItem>();

        void Fresh()
        {
            lock (listLock)
            {
                ErrLockData errLockData = null;
                if (ErrLock.ErrLockOpen)
                {
                    errLockData = ErrLock.LockStart("DebugLogGUI.cs-->202-->Fresh");
                }
                curList.Clear();
                switch (freshType)
                {
                    case FreshType.All:
                        {
                            for (int i = 0; i < guiLogCombinList.Count; ++i)
                            {
                                curList.Add((IDebugLogData)guiLogCombinList[i]);
                            }
                        }
                        break;
                    case FreshType.Log:
                        {
                            for (int i=0;i< guiLogList.Count;++i)
                            {
                                curList.Add((IDebugLogData)guiLogList[i]);
                            }
                        }
                        break;
                    case FreshType.LogWarning:
                        {
                            for (int i = 0; i < guiLogWarningList.Count; ++i)
                            {
                                curList.Add((IDebugLogData)guiLogWarningList[i]);
                            }
                        }
                        break;
                    case FreshType.LogError:
                        {
                            for (int i = 0; i < guiLogErrorList.Count; ++i)
                            {
                                curList.Add((IDebugLogData)guiLogErrorList[i]);
                            }
                        }
                        break;
                }

                int count = Math.Max(curList.Count, showlist.Count);
                for (int i = 0; i < count; ++i)
                {
                    if (i < curList.Count)
                    {
                        Type t = curList[i].GetType();
                        Color textColor;
                        if (t == typeof(LogData_Log))
                        {
                            textColor = Color.white;
                        }
                        else if (t == typeof(LogData_Warning))
                        {
                            textColor = Color.yellow;
                        }
                        else if (t == typeof(LogData_Error))
                        {
                            textColor = Color.red;
                        }
                        else
                        {
                            textColor = Color.white;
                        }
                        if (i < showlist.Count)
                        {
                            DebugLogGUIItem item = showlist[i];
                            item.Show(curList[i].SB.ToString(), ItemClick, textColor);
                        }
                        else
                        {
                            DebugLogGUIItem item = GetOneItem(curList[i].SB.ToString(), textColor);
                            showlist.Add(item);
                        }
                    }
                    else
                    {
                        while (showlist.Count>i)
                        {
                            DebugLogGUIItem item = showlist[i];
                            PutBackOne(item);
                            showlist.Remove(item);
                        }
                        break;
                    }
                }
                if (ErrLock.ErrLockOpen)
                {
                    ErrLock.LockEnd(errLockData);
                }
            }

        }

        void ItemClick(string str, Color textColor)
        {
            TextShow(str, textColor);
        }

        GameObject textShow;

        Text textShowText;

        Button textShowCloseButton;

        void TextShow(string str,Color textColor)
        {
            textShowText.color = textColor;
            textShowText.text = str; 
            textShow.SetActive(true);
        }

        void TextShowClose()
        {
            textShow.SetActive(false);
        }

        void DebugLogToggleChange(bool bl)
        {
            debugLogPanel.SetActive(bl);
            if (debugLogPanel.activeSelf)
            {
                Fresh();
            }
            else
            {
                TextShowClose();
            }
        }

        RectTransform itemRectTransform;
        RectTransform itemRoot;
        Button allButton;
        Button logButton;
        Button logWarningButton;
        Button logErrorButton;
        FreshType freshType;

        void AllButtonClick()
        {
            freshType = FreshType.All;
            Fresh();
        }

        void LogButtonClick()
        {
            freshType = FreshType.Log;
            Fresh();
        }

        void LogWarningButtonClick()
        {
            freshType = FreshType.LogWarning;
            Fresh();
        }

        void LogErrorButtonClick()
        {
            freshType = FreshType.LogError;
            Fresh();
        }

        enum FreshType
        {
            All,
            Log,
            LogWarning,
            LogError,
        }

        List<DebugLogGUIItem> itemPool = new List<DebugLogGUIItem>();

        DebugLogGUIItem GetOneItem(string str, Color textColor)
        {
            if (itemPool.Count>0)
            {
                DebugLogGUIItem item = itemPool[0];
                itemPool.RemoveAt(0);
                item.gameObject.SetActive(true);
                item.transform.SetSiblingIndex(int.MaxValue);
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(itemRectTransform.sizeDelta.x, item.GetComponent<RectTransform>().sizeDelta.y);
                item.Show(str, ItemClick, textColor);
                return item;
            }
            else
            {
                GameObject obj = Instantiate(itemRectTransform.gameObject);
                DebugLogGUIItem item = obj.GetComponent<DebugLogGUIItem>();
                item.transform.SetParent(itemRoot, true);
                item.gameObject.SetActive(true);
                item.GetComponent<RectTransform>().sizeDelta = new Vector2(itemRectTransform.sizeDelta.x, item.GetComponent<RectTransform>().sizeDelta.y);
                item.Show(str, ItemClick, textColor);
                return item;
            }
        }

        void PutBackOne(DebugLogGUIItem item)
        {
            item.gameObject.SetActive(false);
            itemPool.Add(item);
        }
    }
}

