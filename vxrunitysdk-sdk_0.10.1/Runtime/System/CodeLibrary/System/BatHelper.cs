using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using UnityEngine;
using System.IO;

namespace com.vivo.codelibrary
{
    public class BatHelper
    {
        public static void RunBat(string batFilePath)
        {
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                Process proc = null;
                try
                {
                    proc = new Process();
                    proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(batFilePath);
                    proc.StartInfo.FileName = Path.GetFileName(batFilePath);
                    proc.StartInfo.Arguments = string.Format("10");
                    //proc.StartInfo.CreateNoWindow = true;
                    proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;//这里设置DOS窗口不显示，经实践可行
                    proc.Start();
                    proc.WaitForExit();
                }
                catch (Exception ex)
                {
                    UnityEngine.Debug.LogError(string.Format("Exception Occurred :{0},{1}", ex.Message, ex.StackTrace.ToString()));
                }

            }
        }
    }
}


