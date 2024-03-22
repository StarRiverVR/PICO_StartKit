#if UNITY_WIN || UNITY_EDITOR

using System.IO;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor.SceneManagement;
using System.Threading.Tasks;
using UnityEngine;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices;
using System.Text;

namespace com.vivo.codelibrary
{
    public class WindowsHelper
    {

        #region//控制台窗口类

        const int STD_INPUT_HANDLE = -10;
        const uint ENABLE_PROCESSED_INPUT = 0x0001; //Ctrl+c暂停
        const uint ENABLE_LINE_INPUT = 0x0002; //仅当读取回车符时
        const uint ENABLE_ECHO_INPUT = 0x0004; //快速编辑 函数读取的字符在键入到控制台时，将被写入到活动屏幕缓冲区 。 只有同时启用了 ENABLE_LINE_INPUT 模式时，才能使用此模式。
        const uint ENABLE_WINDOW_INPUT = 0x0008; //更改控制台屏幕缓冲区大小的用户交互将记录到控制台的输入缓冲区中。
        const uint ENABLE_MOUSE_INPUT = 0x0010; //如果鼠标指针位于控制台窗口的边框内并且窗口具有键盘焦点，则通过移动鼠标和按下按钮生成的鼠标事件会放置在输入缓冲区中
        const uint ENABLE_INSERT_MODE = 0x0020; //如果启用，在控制台窗口中输入的文本将插入到当前光标位置，并且不会覆盖该位置后面的所有文本。 如果禁用，则将覆盖后面的所有文本。
        const uint ENABLE_QUICK_EDIT_MODE = 0x0040; //用户可通过此标志使用鼠标选择和编辑文本。 

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr GetStdHandle(int hConsoleHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint mode);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint mode);

        public static void DisbleQuickEditMode()
        {
            try
            {
                IntPtr hStdin = GetStdHandle(STD_INPUT_HANDLE);
                uint mode;
                GetConsoleMode(hStdin, out mode);
                mode &= ~ENABLE_QUICK_EDIT_MODE;//移除快速编辑模式
                mode &= ~ENABLE_INSERT_MODE;      //移除插入模式
                mode &= ~ENABLE_PROCESSED_INPUT;      //移除Ctrl+c
                SetConsoleMode(hStdin, mode);
            }
            catch (System.Exception ex)
            {
                VLog.Error(ex.Message);
            }
        }

        [DllImport("user32.dll", EntryPoint = "GetSystemMenu")]
        extern static IntPtr GetSystemMenu(IntPtr hWnd, IntPtr bRevert);

        [DllImport("user32.dll", EntryPoint = "RemoveMenu")]
        extern static IntPtr RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);

        static void DisbleClosebtn()
        {
            IntPtr windowHandle = FindWindow(null, "控制台标题");
            IntPtr closeMenu = GetSystemMenu(windowHandle, IntPtr.Zero);
            uint SC_CLOSE = 0xF060;
            RemoveMenu(closeMenu, SC_CLOSE, 0x0);
        }

        protected static void CloseConsole(object sender, ConsoleCancelEventArgs e)
        {
            Environment.Exit(0);
        }

        #endregion

        #region//资源管理器

        /// <summary>
        /// 打开文件 多选
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="fileter"></param>
        public static void SelectFileMutiSelect(Action<List<string>> callBack, string fileter = "所有文件(*.*)\0*.*", string title = null)
        {
            List<string> res = new List<string>();
            try
            {
                int lenght = 4096;
                string fileNames= new string(new char[lenght]);
                //
                OpenFileName openFileName = new OpenFileName();
                openFileName.structSize = Marshal.SizeOf(openFileName);
                openFileName.filter = fileter;
                openFileName.file = fileNames;
                openFileName.maxFile = openFileName.file.Length;
                openFileName.fileTitle = new string(new char[64]);
                openFileName.maxFileTitle = openFileName.fileTitle.Length;
                openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
                if (!string.IsNullOrEmpty(title))
                {
                    openFileName.title = title + " " + fileter;
                }
                else
                {
                    openFileName.title = "选择文件" + fileter;
                }
                //openFileName.defExt = "FBX";
                //0x00080000 | ==  OFN_EXPLORER |对于旧风格对话框，目录 和文件字符串是被空格分隔的，函数为带有空格的文件名使用短文件名
                //OFN_EXPLORER |OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR    
                openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;

                if (LocalDialog.GetFile(openFileName))
                {
                    if (!string.IsNullOrEmpty(openFileName.file)) {
                        List<string> tempList = new List<string>();
                        List<char> tempChars = new List<char>();
                        for (int i=0;i< lenght;++i)
                        {
                            if (fileNames[i]!='\0')
                            {
                                tempChars.Add(fileNames[i]);
                            }
                            else
                            {
                                if (i!=(lenght-1))
                                {
                                    tempList.Add(new string(tempChars.ToArray()));
                                    tempChars = new List<char>();
                                    if (fileNames[i+1]== '\0')
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    tempList.Add(new string(tempChars.ToArray()));
                                }
                            }
                        }
                        if (tempList.Count>1)
                        {
                            string filePathDir= tempList[0]+"/";
                            for (int i=1;i< tempList.Count;++i)
                            {
                                string filePath = filePathDir + tempList[i];
                                filePath= filePath.Replace("\\", "/");
                                res.Add(filePath);
                            }
                        }
                        else
                        {
                            string filePath = tempList[0].Replace("\\","/");
                            res.Add(filePath);
                        }
                        callBack?.Invoke(res);
                    }
                    else
                    {
                        callBack?.Invoke(res);
                    }
                }
                else
                {
                    callBack?.Invoke(res);
                }
            }
            catch (Exception ex)
            {
                VLog.Exception(ex);
                callBack?.Invoke(res);
            }
        }

        /// <summary>
        /// 打开文件 单选
        /// </summary>
        /// <param name="callBack">获得文件回调</param>
        /// <param name="fileter">文件类型 "Excel文件(*.xlsx)\0*.xlsx;*.xlsx" </param>
        public static void SelectFile(Action<string> callBack, string fileter = "所有文件(*.*)\0*.*",string title=null)
        {
            try
            {
                OpenFileName openFileName = new OpenFileName();
                openFileName.structSize = Marshal.SizeOf(openFileName);
                openFileName.filter = fileter;
                openFileName.file = new string(new char[1024]);
                openFileName.maxFile = openFileName.file.Length;
                openFileName.fileTitle = new string(new char[64]);
                openFileName.maxFileTitle = openFileName.fileTitle.Length;
                openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
                if (!string.IsNullOrEmpty(title))
                {
                    openFileName.title = title+ " " + fileter;
                }
                else
                {
                    openFileName.title = "选择文件" + fileter;
                }
                //openFileName.defExt = "FBX";
                //0x00080000 | ==  OFN_EXPLORER |对于旧风格对话框，目录 和文件字符串是被空格分隔的，函数为带有空格的文件名使用短文件名
                //OFN_EXPLORER |OFN_FILEMUSTEXIST|OFN_PATHMUSTEXIST| OFN_ALLOWMULTISELECT|OFN_NOCHANGEDIR    
                openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

                if (LocalDialog.GetFile(openFileName))
                {
                    string filePath = openFileName.file;
                    if (File.Exists(filePath))
                    {
                        callBack?.Invoke(filePath);
                        return;
                    }
                    else
                    {
                        callBack?.Invoke(null);
                    }
                }
                else
                {
                    callBack?.Invoke(null);
                }
            }
            catch (Exception ex)
            {
                VLog.Exception(ex);
                callBack?.Invoke(null);
            }
        }

        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="title"></param>
        public static void SelectFolder(Action<string> callBack, string title = "请选择文件夹")
        {
            try
            {
                OpenDialogDir ofn2 = new OpenDialogDir();
                ofn2.pszDisplayName = new string(new char[2048]);
                ofn2.lpszTitle = title; // 标题  
                ofn2.ulFlags = 0x00000040; // 新的样式,带编辑框  
                IntPtr pidlPtr = LocalDialog.GetFolder(ofn2);

                char[] charArray = new char[2048];

                for (int i = 0; i < 2048; i++)
                {
                    charArray[i] = '\0';
                }
                LocalDialog.GetPathFormIDList(pidlPtr, charArray);
                string res = new string(charArray);
                res = res.Substring(0, res.IndexOf('\0'));
                if (Directory.Exists(res))
                {
                    callBack?.Invoke(res);
                }
                else
                {
                    callBack?.Invoke(null);
                }

            }
            catch (Exception ex)
            {
                VLog.Exception(ex);
                callBack?.Invoke(null);
            }
        }

        /// <summary>
        /// 文件另存为
        /// </summary>
        /// <param name="callBack"></param>
        /// <param name="fileter">文件类型 FBX文件(*.fbx)\0*.fbx\0</param>
        public static void SaveFile(Action<string> callBack, string fileter = "所有文件(*.*)\0*.*", string title = null)
        {
            try
            {
                OpenFileName openFileName = new OpenFileName();
                openFileName.structSize = Marshal.SizeOf(openFileName);
                openFileName.filter = fileter;
                openFileName.file = new string(new char[1024]);
                openFileName.maxFile = openFileName.file.Length;
                openFileName.fileTitle = new string(new char[64]);
                openFileName.maxFileTitle = openFileName.fileTitle.Length;
                openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径
                if (!string.IsNullOrEmpty(title))
                {
                    openFileName.title = title + " " + fileter;
                }
                else
                {
                    openFileName.title = "另存为" + fileter;
                }

                //openFileName.defExt = "FBX";
                //openFileName.flags = 0x00001000 | 0x00001000 | 0x00000800 | 0x00000200 | 0x00000008;
                openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;

                if (LocalDialog.SaveFile(openFileName))
                {
                    string filePath = openFileName.file;
                    if (!filePath.Contains('.'))
                    {
                        int index = fileter.IndexOf('*');
                        fileter = fileter.Substring(index+1+1, fileter.Length- index-1-1);
                        index = fileter.IndexOf(')');
                        fileter = fileter.Substring(0, index);
                        if (fileter.CompareTo("*") !=0)
                        {
                            filePath = filePath + "." + fileter;
                        }
                    }
                    callBack?.Invoke(filePath);
                }
                else
                {
                    callBack?.Invoke(null);
                }
            }
            catch (Exception ex)
            {
                VLog.Exception(ex);
                callBack?.Invoke(null);
            }
        }

        /// <summary>
        /// 打开目录
        /// </summary>
        /// <param name="path">将要打开的文件目录</param>
        public static void OpenFolder(string path)
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public class OpenFileName
        {
            public int structSize = 0;       //结构的内存大小
            public IntPtr dlgOwner = IntPtr.Zero;       //设置对话框的句柄
            public IntPtr instance = IntPtr.Zero;       //根据flags标志的设置，确定instance是谁的句柄，不设置则忽略
            public string filter = null;         //调取文件的过滤方式
            public string customFilter = null;  //一个静态缓冲区 用来保存用户选择的筛选器模式
            public int maxCustFilter = 0;     //缓冲区的大小
            public int filterIndex = 0;                 //指向的缓冲区包含定义过滤器的字符串对
            public string file = null;                  //存储调取文件路径
            public int maxFile = 0;                     //存储调取文件路径的最大长度 至少256
            public string fileTitle = null;             //调取的文件名带拓展名
            public int maxFileTitle = 0;                //调取文件名最大长度
            public string initialDir = null;            //最初目录
            public string title = null;                 //打开窗口的名字
            public int flags = 0;                       //初始化对话框的一组位标志  参数类型和作用查阅官方API
            public short fileOffset = 0;                //文件名前的长度
            public short fileExtension = 0;             //拓展名前的长度
            public string defExt = null;                //默认的拓展名
            public IntPtr custData = IntPtr.Zero;       //传递给lpfnHook成员标识的钩子子程的应用程序定义的数据
            public IntPtr hook = IntPtr.Zero;           //指向钩子的指针。除非Flags成员包含OFN_ENABLEHOOK标志，否则该成员将被忽略。
            public string templateName = null;          //模块中由hInstance成员标识的对话框模板资源的名称
            public IntPtr reservedPtr = IntPtr.Zero;
            public int reservedInt = 0;
            public int flagsEx = 0;                     //可用于初始化对话框的一组位标志

        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        internal class OpenDialogDir
        {
            public IntPtr hwndOwner = IntPtr.Zero;
            public IntPtr pidlRoot = IntPtr.Zero;
            public String pszDisplayName = null;
            public String lpszTitle = null;
            public UInt32 ulFlags = 0;
            public IntPtr lpfn = IntPtr.Zero;
            public IntPtr lParam = IntPtr.Zero;
            public int iImage = 0;
        }

        internal class LocalDialog
        {
            //链接指定系统函数       打开文件对话框
            [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool GetOpenFileName([In, Out] OpenFileName ofn);

            //链接指定系统函数        另存为对话框
            [DllImport("Comdlg32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            public static extern bool GetSaveFileName([In, Out] OpenFileName ofn);

            [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            private static extern IntPtr SHBrowseForFolder([In, Out] OpenDialogDir ofn);

            [DllImport("shell32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
            private static extern bool SHGetPathFromIDList([In] IntPtr pidl, [In, Out] char[] fileName);

            /// <summary>
            /// 获取文件
            /// </summary>
            /// <param name="ofn"></param>
            /// <returns></returns>
            public static bool GetFile([In, Out] OpenFileName ofn)
            {
                return GetOpenFileName(ofn);
            }

            /// <summary>
            /// 另存为
            /// </summary>
            /// <param name="ofn"></param>
            /// <returns></returns>
            public static bool SaveFile([In, Out] OpenFileName ofn)
            {
                return GetSaveFileName(ofn);
            }

            /// <summary>
            ///  获取文件夹路径
            /// </summary>
            public static IntPtr GetFolder([In, Out] OpenDialogDir ofn)
            {
                return SHBrowseForFolder(ofn);
            }

            /// <summary>
            /// 获取文件夹路径
            /// </summary>
            public static bool GetPathFormIDList([In] IntPtr pidl, [In, Out] char[] fileName)
            {
                return SHGetPathFromIDList(pidl, fileName);
            }
        }

        #endregion

        #region//进程

        public delegate bool WNDENUMPROC(IntPtr hwnd, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool EnumWindows(WNDENUMPROC lpEnumFunc, uint lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, ref uint lpdwProcessId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowThreadProcessId(System.IntPtr handle, out int processId);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int nMaxCount);

        [DllImport("User32.dll", SetLastError = true, ThrowOnUnmappableChar = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(IntPtr handle, String message, String title, int type);

        [DllImport("kernel32.dll")]
        public static extern void SetLastError(uint dwErrCode);


        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);


        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);


        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        /// 获得Unity窗口句柄
        /// </summary>
        /// <param name="containsWindName"></param>
        /// <returns></returns>
        public static IntPtr FindWindByWinName(string containsWindName)
        {
            try
            {
                IntPtr ptrWnd = IntPtr.Zero;
                uint pid = (uint)Process.GetCurrentProcess().Id;  // 当前进程 ID
                bool bResult = EnumWindows(new WNDENUMPROC(delegate (IntPtr hwnd, uint lParam)
                {
                    uint id = 0;

                    if (GetParent(hwnd) == IntPtr.Zero)
                    {
                        GetWindowThreadProcessId(hwnd, ref id);
                        if (id == lParam)    // 找到进程对应的主窗口句柄
                        {
                            int length = GetWindowTextLength(hwnd);
                            StringBuilder windowName = new StringBuilder(length + 1);
                            GetWindowText(hwnd, windowName, windowName.Capacity);
                            if (windowName.ToString().Contains(containsWindName))
                            {
                                ptrWnd = hwnd;   // 把句柄缓存起来
                                SetLastError(0);    // 设置无错误
                                return false;   // 返回 false 以终止枚举窗口
                            }
                        }
                    }

                    return true;

                }), pid);

                return (!bResult && Marshal.GetLastWin32Error() == 0) ? ptrWnd : IntPtr.Zero;
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
                return IntPtr.Zero;
            }
        }

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(System.IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentWindow, IntPtr previousChildWindow, string windowClass, string windowTitle);

        [DllImport("user32.dll")]
        public static extern bool EnumWindows(EnumWindowsProc enumProc, System.IntPtr lParam);

        public delegate bool EnumWindowsProc(System.IntPtr hWnd, System.IntPtr lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern bool SetWindowPos(System.IntPtr hwnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        /// <summary>
        /// 设置窗口放大缩小
        /// </summary>
        /// <param name="containsWindName"> 窗口命中包含字段: "Unity 2020.3.7f1"</param>
        /// <param name="windowType"></param>
        public static void SetWindMin(string containsWindName, WindowType windowType)
        {
            try
            {
                IntPtr unityWinHandle = FindWindByWinName(containsWindName);
                if (unityWinHandle != IntPtr.Zero)
                {
                    ShowWindow(unityWinHandle, 2);
                }
            }
            catch (System.Exception ex)
            {
                VLog.Exception(ex);
            }
        }

        public enum WindowType
        {
            /// <summary>
            /// 还原 
            /// </summary>
            ShowRestore = 1,

            /// <summary>
            /// 最小化
            /// </summary>
            ShowMinimized = 2,

            /// <summary>
            /// 最大化
            /// </summary>
            ShowMaximized = 3,
        }

        static public IntPtr[] GetProcessWindows(int processId)
        {
            List<IntPtr> output = new List<IntPtr>();
            IntPtr winPtr = IntPtr.Zero;
            do
            {
                winPtr = FindWindowEx(IntPtr.Zero, winPtr, null, null);
                int id;
                GetWindowThreadProcessId(winPtr, out id);
                if (id == processId)
                    output.Add(winPtr);
            } while (winPtr != IntPtr.Zero);

            return output.ToArray();
        }

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
            public int Width { get { return Right - Left; } }
            public int Height { get { return Bottom - Top; } }

            public override string ToString()
            {
                return "(l: " + Left + ", r: " + Right + ", t: " + Top + ", b: " + Bottom + ")";
            }
        }

        public static bool GetProcessRect(System.Diagnostics.Process process, ref Rect rect)
        {
            IntPtr[] winPtrs = GetProcessWindows(process.Id);

            for (int i = 0; i < winPtrs.Length; i++)
            {
                bool gotRect = GetWindowRect(winPtrs[i], ref rect);
                if (gotRect && (rect.Left != 0 && rect.Top != 0))
                    return true;
            }
            return false;
        }

        public static void SetWindowPosition(int x, int y, int sizeX = 0, int sizeY = 0)
        {
            System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess();
            process.Refresh();

            EnumWindows(delegate (System.IntPtr wnd, System.IntPtr param)
            {
                int id;
                GetWindowThreadProcessId(wnd, out id);
                if (id == process.Id)
                {
                    SetWindowPos(wnd, 0, x, y, sizeX, sizeY, sizeX * sizeY == 0 ? 1 : 0);
                    return false;
                }

                return true;
            }, System.IntPtr.Zero);
        }

        /// <summary>
        /// 杀死Unity进程 主线程调用
        /// true:全部杀死
        /// false:未全部杀死
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static bool KillProcess(string processName)
        {
            try
            {
                Process[] unityProcessArray = Process.GetProcessesByName(processName);
                for (int i = 0; i < unityProcessArray.Length; ++i)
                {
                    Process unityProcess = unityProcessArray[i];
                    //DebugLog(string.Format("[Process Start Kill]={0} [processId]={1}", processName, unityProcess.Id));
                    //unityProcess.StandardOutput.ReadToEnd();
                    unityProcess.Kill();
                    unityProcess.WaitForExit();
                    VLog.Info(string.Format("[Process Killed]={0} [processId]={1}", processName, unityProcess.Id));
                }
                return true;
            }
            catch (System.Exception e)
            {
                VLog.Info(string.Format("Process Kill fail:{0} ", e.Message));
                return false;
            }
        }

        /// <summary>
        /// 进程句柄数量
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static int GetProcessHandles(string processName)
        {
            Process p = FindProcessByName(processName);
            if (p != null)
            {
                return p.HandleCount;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 进程获取  ͨ        
        /// </summary>
        /// <param name="processName"></param>
        /// <returns></returns>
        public static System.Diagnostics.Process FindProcessByName(string processName)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in processes)
            {
                if (p.ProcessName.CompareTo(processName) == 0)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        /// 进程获取 ͨ  ID
        /// </summary>
        /// <param name="processId"></param>
        /// <returns></returns>
        public static System.Diagnostics.Process FindProcessById(int processId)
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in processes)
            {
                if (p.Id == processId)
                {
                    return p;
                }
            }
            return null;
        }

        /// <summary>
        ///  打印所有进程
        /// </summary>
        public static void DebugLogAllProcess()
        {
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process process in processes)
            {
                DebugLogProcess(process);
            }
        }

        /// <summary>
        /// 打印当前进程   
        /// </summary>
        public static void DebugLogCurrentProcess()
        {
            Process process = Process.GetCurrentProcess();
            DebugLogProcess(process);
        }

        /// <summary>
        ///  打印当前进程中的所有线程   
        /// </summary>
        public static void DebugLogCurrentProcessThreads()
        {
            Process process = Process.GetCurrentProcess();
            DebugAllThreadInProcess(process);
        }

        /// <summary>
        /// 打印进程  
        /// </summary>
        /// <param name="process"></param>
        public static void DebugLogProcess(System.Diagnostics.Process process)
        {
            VLog.Info(string.Format("[ProcessName]={0} [Handle]={1} [HandleCount]={2} [Id]={3} [MachineName]={4} [PagedMemorySize64]={5} [PagedSystemMemorySize64]={6} [PeakPagedMemorySize64]={7} [PeakVirtualMemorySize64]={8} [PeakWorkingSet64]={9},[PrivateMemorySize64]={10} [VirtualMemorySize64]={11} [ThreadsCount]={12}",
                process.ProcessName, process.Handle, process.HandleCount, process.Id, process.MachineName, process.PagedMemorySize64, process.PagedSystemMemorySize64,
                process.PeakPagedMemorySize64, process.PeakVirtualMemorySize64, process.PeakWorkingSet64, process.PrivateMemorySize64, process.VirtualMemorySize64, process.Threads.Count));
        }

        /// <summary>
        ///  打印进程中的所有线程
        /// </summary>
        /// <param name="process"></param>
        public static void DebugAllThreadInProcess(System.Diagnostics.Process process)
        {
            System.Diagnostics.ProcessThreadCollection threads = process.Threads;
            foreach (System.Threading.Thread th in threads)
            {
                DebugThread(th);
            }
        }

        /// <summary>
        /// 打印线程
        /// </summary>
        /// <param name="thread"></param>
        public static void DebugThread(System.Threading.Thread thread)
        {
            VLog.Info(string.Format("[ThreadName]={0} [IsAlive]={1} [IsBackground]={2} [IsThreadPoolThread]={3} [ManagedThreadId]={4} [ThreadState]={5}", thread.Name, thread.IsAlive, thread.IsBackground, thread.IsThreadPoolThread,
                thread.ManagedThreadId, thread.ThreadState));
        }

        #endregion

    }

}

#endif
