namespace: com.vivo.codelibrary

Debug：
	DebugLog.Log("text",false); //bool是否向外写日志 （LogError，LogWarning，LogException，LogFormat）

本地文件读写：
 	//Read -    serializ,text,byte[]
    FileReadHelper.Instance.ReadText(filePath, (path, readResult, obj)=> {
        DebugLog.LogFormat("文件路径:{0} 内容:{1}",false, path, readResult);
    }, System.Text.Encoding.UTF8);

    FileReadHelper.Instance.ReadTextAsyn(filePath, (path, readResult, obj) =>
    {
        DebugLog.LogFormat("文件路径:{0} 内容:{1}", false, path, readResult);
    }, (isSucceed, path, sendData) =>
    {
        DebugLog.LogFormat("是否读取成功:{0} 文件路径:{1} 参数传递:{2}", false, isSucceed, path, sendData);
    }, null, System.Text.Encoding.UTF8);

    //Write -   serializ,text
    FileWriteHelper.Instance.Write(filePath, "写出内容...txt", true);

    FileWriteHelper.Instance.WriteAsyn(filePath, "异步写出内容...txt", true, (path, sendData) => {
        DebugLog.LogFormat("文件路径:{0} 参数传递:{1}", false, path, sendData);
    },null);
	
	//Delete
    FileDeleteHelper.Instance.DeleteFile(filePath);
    FileDeleteHelper.Instance.DeleteFileAsynchronous(filePath,(isSucceed, sendData) => {
        DebugLog.LogFormat("是否删除成功:{0} 参数传递:{1}", false, isSucceed, sendData);
    },null);

	//Copy
    FileCopyHelper.Instance.CopyFile(filePathA, filePathB);
    FileCopyHelper.Instance.CopyFileAsynchronous(filePathA, filePathB, (isSucceed, sendData) => {
		DebugLog.LogFormat("是否删除成功:{0} 参数传递:{1}", false, isSucceed, sendData);
    },null);
	
	
读写路径：
	FilePath.LocalStorageDir //本地存储目录
	FilePath.TemporaryCachePath //临时缓存目录
	FilePath.DebugLogDir //Log记录目录
	
	
消息中心:
    //常驻消息中心 （场景跳转不会清空）
    InformationManager.Instance.GameInformationCenter.AddListen<TestMSG>((int)TestMSG.MsgA,(object[] objs) => {
        DebugLog.LogFormat("收到消息:{0}",false, objs[0]);
    });
    InformationManager.Instance.GameInformationCenter.Send<TestMSG>((int)TestMSG.MsgA,true,"abc");

    //场景消息中心 （场景跳转会清空）
    InformationManager.Instance.SceneInformationCenter.AddListen<TestMSG>((int)TestMSG.MsgA, (object[] objs) => {
        DebugLog.LogFormat("收到消息:{0}", false, objs[0]);
    });
    InformationManager.Instance.SceneInformationCenter.Send<TestMSG>((int)TestMSG.MsgA, true, "abc");
	
	
内存池：
	//内存池 List [string,object,byte[],byte,int,long,Texture2D,Collider]
    List<string> getData = ListPool.Instance.GetOneStringList();
    ListPool.Instance.PutBackOneStringList(getData);

    //内存池 Dictionary [<string,string>,<string,object>]
    Dictionary<string, string> getData = DictionaryPool.Instance.GetOneStringStringDic();
    DictionaryPool.Instance.PutBackOneStringStringDic(getData);

    //内存池 StringBuilder
    StringBuilder getData = StringBuilderPool.Instance.GetOneStringBuilder(16);
    StringBuilderPool.Instance.PutBackOneStringBuilder(getData);
	
	//内存池 byte[]
	static ByteArrayPool byteArrayPool = new ByteArrayPool();
    byte[] getData = byteArrayPool.Spawn(64);
    byteArrayPool.Recycle(getData);
	
	//内存池 继承:MonoBehaviour,ISimplePoolData
	static SimpleMonoPool<NewBehaviourScriptTest> monoPool = new SimpleMonoPool<NewBehaviourScriptTest>(gameObject);
    NewBehaviourScriptTest getData = monoPool.Spawn();
    monoPool.Recycle(getData);
	
	
MVC框架 （UI）：
	案例-ExampleControl.cs , ExampleModel.cs , ExampleView.cs
	
	
单例：
	//Class
	public class CustemClass : CSingleton<CustemClass>
	
	//MonoBehaviour
	[MonoSingletonCreate(MonoSingletonCreateType.OnGameStart运行时创建，true是否长久保留，“GameObject Name”)]
	public class CustemMonoBehaviour : MonoSingleton<CustemMonoBehaviour>
协程 [Editor and Runtime]：
	GameCoroutine.Instance.StartCoroutine(CoroutineFun);
	

Update管理：Update,FixedUpdate,LateUpdate
	InformationManager.Instance.GameInformationCenter.AddListen<MonoUpdateMsg>((int)MonoUpdateMsg.Update, MUpdate);
	InformationManager.Instance.GameInformationCenter.RemoveListen<MonoUpdateMsg>((int)MonoUpdateMsg.Update, MUpdate);
	void MUpdate(object[] objs)
	
	
延迟执行 [Editor]：
	DelayFunHelper.DelayRun(Action action, Action<object[]> actionObjs, object[] objs, double delay)
	

时间：
	TimeHelper.cs
	
	
序列化与反序列化：
	Serializ.cs
	
HTTP下载器：[AssetBundle,Audio,File,FileSave,Texture(png)] [AssetBundleLoader.cs,AudioLoader.cs,FileHttpLoad.cs,FileSaveLoader.cs,TextureHttpLoader.cs]
	List<TextureHttpLoadeData> datas = TextureHttpLoader.GetOneList();
    TextureHttpLoadeData data = TextureHttpLoader.GetOneData(url, 0, readable);
    datas.Add(data);
    TextureHttpLoader textureHttpLoader = new TextureHttpLoader(datas, (texture2D, textureHttpLoadData) => {
        //String.Format("TextureErr={0} \nLoadErr={1} \nCallBackErr={2}", textureHttpLoadData.TextureErr, textureHttpLoadData.LoadErr, textureHttpLoadData.CallBackErr)
    }, (bl, obj, textureHttpLoader) => {

    }, null, null, reLoadCount);
	
	

