using com.vivo.openxr;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Video;

public class OverlayVideoDemo : MonoBehaviour
{


    public VideoPlayer UnityPlayer;
    public string MovieName;

    private string _moviePath;
    private IntPtr AndroidSurfaceObj;   
    public VXROverlay OverlayCmp;
    private static AndroidJavaObject mediaPlayer = null;
    private static int duration = 0;

    private string videoPath {
        get {
            return Application.persistentDataPath;
        }
    }
    
    void Start()
    {
        OverlayCmp = OverlayCmp == null ? GetComponent<VXROverlay>() : OverlayCmp;
        OverlayCmp.AndroidSurfaceProjectCreatedCallBack = GetSurfaceObjCallback;
        //UnityPlayer.url = Application.streamingAssetsPath + "/" + MovieName;        
    }

    void GetSurfaceObjCallback()
    {       
        mediaPlayer = new AndroidJavaObject("android/media/MediaPlayer");     
        if (mediaPlayer == null)
        {
            Debug.Log("播放器创建失败");
            return;
        }        
        AndroidSurfaceObj = OverlayCmp.AndroidSurfaceProject;        
        IntPtr setSurfaceMethodId = AndroidJNI.GetMethodID(mediaPlayer.GetRawClass(), "setSurface", "(Landroid/view/Surface;)V");
        jvalue[] parms = new jvalue[1];
        parms[0] = new jvalue();
        parms[0].l = AndroidSurfaceObj;
        AndroidJNI.CallVoidMethod(mediaPlayer.GetRawObject(), setSurfaceMethodId, parms);        
        StartCoroutine(PlayerVideo());
    }

    IEnumerator PlayerVideo()
    {       
        yield return null;        
        mediaPlayer.Call("reset");        
        mediaPlayer.Call("setDataSource", GetMovePath());        
        mediaPlayer.Call("prepare");        
        mediaPlayer.Call("setLooping", true);        
        mediaPlayer.Call("start");        
        duration = mediaPlayer.Call<int>("getDuration");
        Debug.Log("播放器播放成功，视频时长：" + duration);
        UnityPlayer.Play();
    }

    string GetMovePath()
    {
        DirectoryInfo moviesFolder = new DirectoryInfo(videoPath);        
        foreach (FileInfo nextFile in moviesFolder.GetFiles())
        {                        
            if (nextFile.FullName.Contains(MovieName))
            {
                _moviePath = nextFile.FullName;
            }            
        }
        Debug.Log("播放器视频路径：" + _moviePath);
        return _moviePath; 
    }  
}
