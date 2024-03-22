
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using System.Threading;
using System.Threading.Tasks;

namespace com.vivo.codelibrary
{
    public static class Extended_GameObject
    {

#if UNITY_EDITOR

        /// <summary>
        /// 是否为预制体 或者 预制体实例
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsPrefabInstance(this UnityEngine.GameObject obj)
        {
            bool res = false;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res= IsPrefabInstanceRun(obj);
                }, null);
            }
            else
            {
                res = IsPrefabInstanceRun(obj);
            }
            return res;
        }

        static bool IsPrefabInstanceRun( UnityEngine.GameObject obj)
        {
            //prefab类型
            PrefabAssetType type = PrefabUtility.GetPrefabAssetType(obj);
            //实例类型
            PrefabInstanceStatus status = PrefabUtility.GetPrefabInstanceStatus(obj);
            // 是否为预制体实例判断
            if (type == PrefabAssetType.NotAPrefab || status == PrefabInstanceStatus.NotAPrefab)
            {
                return false;
            }
            return true;
        }

#endif

        /// <summary>
        /// 获取 p中 子节点上的 组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="p"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetComponetInChild<T>(this GameObject target, string path) where T : Component
        {
            T res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetComponetInChildRun<T>(target, path);
                }, null);
            }
            else
            {
                res = GetComponetInChildRun<T>(target, path);
            }
            return res;
        }

        static T GetComponetInChildRun<T>( GameObject target, string path) where T : Component
        {
            if (target == null || target.transform == null) return null;
            Transform transform = target.transform.Find(path);
            if (transform == null) return null;
            return transform.GetComponent<T>();
        }

        /// <summary>
        /// 节点路径
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static string GetGameObjectPath(this GameObject target)
        {
            string res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetGameObjectPathRun(target);
                }, null);
            }
            else
            {
                res = GetGameObjectPathRun(target);
            }
            return res;
        }

        static string GetGameObjectPathRun( GameObject target)
        {
            string path = target.name;
            Transform transform = target.transform;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = string.Format("{0}/{1}", transform.name, path);
            }
            return path;
        }

        /// <summary>
        /// 合并网格
        /// </summary>
        public static GameObject CombineMesh(this GameObject target)
        {
            GameObject res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = CombineMeshRun(target);
                }, null);
            }
            else
            {
                res = CombineMeshRun(target);
            }
            return res;
        }

        static GameObject CombineMeshRun( GameObject target)
        {
            MeshFilter[] meshFilters = target.GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[meshFilters.Length];
            int i = 0;
            while (i < meshFilters.Length)
            {
                combine[i].mesh = meshFilters[i].sharedMesh;
                combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
                MeshRenderer meshRenderer = meshFilters[i].gameObject.GetComponent<MeshRenderer>();
                meshRenderer.enabled = false;
                ++i;
            }
            GameObject newObj = new GameObject();
            newObj.name = "CombineMesh";
            newObj.transform.SetParent(target.transform, true);
            newObj.transform.localPosition = Vector3.zero;
            newObj.transform.localEulerAngles = Vector3.zero;
            newObj.transform.localScale = Vector3.one;
            MeshFilter mf = newObj.AddComponent<MeshFilter>();
            mf.sharedMesh = new Mesh();
            mf.sharedMesh.CombineMeshes(combine);
            newObj.AddComponent<MeshRenderer>();
            return newObj;
        }

        ///// <summary>
        ///// 模型尺寸(局部坐标)
        ///// </summary>
        ///// <param name="gameObject"></param>
        ///// <returns></returns>
        public static ModalSizeData GetModalSize(this UnityEngine.GameObject obj)
        {
            ModalSizeData res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetModalSizeRun(obj);
                }, null);
            }
            else
            {
                res = GetModalSizeRun(obj);
            }
            return res;
        }

        static ModalSizeData GetModalSizeRun( UnityEngine.GameObject obj)
        {
            if (obj == null)
            {
                return null;
            }
            Renderer[] renders = obj.GetComponentsInChildren<Renderer>(true);
            //Renderer seldRend = obj.GetComponent<Renderer>();
            //if (seldRend != null)
            //{
            //    System.Array.Resize<Renderer>(ref renders, renders.Length + 1);
            //    renders[renders.Length - 1] = seldRend;
            //}
            if (renders == null || renders.Length == 0)
            {
                ModalSizeData dataInv = new ModalSizeData();
                return dataInv;
            }
            Vector3 pos = obj.transform.position;
            Quaternion ori = obj.transform.rotation;
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);
            obj.transform.position = Vector3.zero;
            float xmax = float.MinValue;
            float ymax = float.MinValue;
            float zmax = float.MinValue;
            float xmin = float.MaxValue;
            float ymin = float.MaxValue;
            float zmin = float.MaxValue;
            for (int i = 0; i < renders.Length; i++)
            {
                Renderer render = renders[i];
                ParticleSystem part = render.gameObject.GetComponent<ParticleSystem>();
                if (part != null) continue;
                Vector3 vmax = render.bounds.max;
                Vector3 vmin = render.bounds.min;

                xmax = Mathf.Max(xmax, vmax.x);
                ymax = Mathf.Max(ymax, vmax.y);
                zmax = Mathf.Max(zmax, vmax.z);


                xmin = Mathf.Min(xmin, vmin.x);
                ymin = Mathf.Min(ymin, vmin.y);
                zmin = Mathf.Min(zmin, vmin.z);
            }

            Vector3 s = obj.transform.localScale;
            Vector3 c = new Vector3((xmax + xmin) * 0.5f, (ymax + ymin) * 0.5f, (zmax + zmin) * 0.5f) - obj.transform.position;

            ModalSizeData data = new ModalSizeData();
            data.centerNormal = new Vector3(c.x / s.x, c.y / s.y, c.z / s.z);
            data.sizeNormal = new Vector3((xmax - xmin) / s.x, (ymax - ymin) / s.y, (zmax - zmin) / s.z);

            obj.transform.rotation = ori;
            obj.transform.position = pos;
            return data;
        }

        /// <summary>
        /// 获得与目标的角度
        /// </summary>
        /// <returns></returns>
        public static float GetTargetAngle(this Transform self, Vector3 targetPos)
        {
            float res = 0;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetTargetAngleRun(self, targetPos);
                }, null);
            }
            else
            {
                res = GetTargetAngleRun(self, targetPos);
            }
            return res;
        }

        static float GetTargetAngleRun( Transform self, Vector3 targetPos)
        {
            Vector3 f = self.forward;
            Vector3 t = new Vector3(targetPos.x, self.position.y, targetPos.z) - self.position;
            return Vector3.Angle(f, t);
        }

        /// <summary>
        /// 获取所有的子物体
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="includeSelf">是否包含自身</param>
        /// <returns></returns>
        public static List<Transform> GetAllChild(this GameObject obj, bool includeSelf = true)
        {
            List<Transform> res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetAllChildRun(obj, includeSelf);
                }, null);
            }
            else
            {
                res = GetAllChildRun(obj, includeSelf);
            }
            return res;
        }

        static List<Transform> GetAllChildRun(GameObject obj, bool includeSelf = true)
        {
            List<Transform> res = new List<Transform>();
            GetAllChildPriv(obj.transform, ref res);
            return res;
        }

        /// <summary>
        /// 获得子物体
        /// </summary>
        /// <param name="t"></param>
        /// <param name="res"></param>
        static void GetAllChildPriv(Transform t, ref List<Transform> res)
        {
            res.Add(t);
            int count = t.childCount;
            if (count > 0)
            {
                for (int i = 0; i < count; ++i)
                {
                    Transform child = t.GetChild(i);
                    GetAllChildPriv(child, ref res);
                }
            }
        }

        static ComputeShader gameObjectPreviewShader;

        static ComputeShader GameObjectPreviewShader
        {
            get
            {
                if (gameObjectPreviewShader == null)
                {
                    gameObjectPreviewShader = Resources.Load<ComputeShader>("Extensions/Shader/GameObjectPreviewShader");
                }
                return gameObjectPreviewShader;
            }
        }

        /// <summary>
        /// 获取预览图象 (返回的Texture2D 需要手动进行销毁)
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="width">图片宽</param>
        /// <param name="height">图片高</param>
        /// 用法:GameObject obj.GetAssetPreview(this GameObject obj, int width, int height)
        /// <returns></returns>
        public static Texture2D GetAssetPreview(this GameObject obj, int width, int height, Color backColor)
        {
            Texture2D res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetAssetPreviewRun(obj, width, height, backColor);
                }, null);
            }
            else
            {
                res = GetAssetPreviewRun(obj, width, height, backColor);
            }
            return res;
        }

        struct OutputData
        {
            public UnityEngine.Vector4 color;
        }

        static bool IsPreviewRT(int size)
        {
            if (size == 16)
            {
                return true;
            }
            if (size == 32)
            {
                return true;
            }
            if (size == 64)
            {
                return true;
            }
            if (size == 128)
            {
                return true;
            }
            if (size == 256)
            {
                return true;
            }
            if (size == 512)
            {
                return true;
            }
            if (size == 1024)
            {
                return true;
            }
            if (size == 2048)
            {
                return true;
            }
            if (size == 4096)
            {
                return true;
            }
            return false;
        }

        static bool IsPreviewRT(int width, int height)
        {
            if (IsPreviewRT(width) && IsPreviewRT(height))
            {
                return true;
            }
            return false;
        }

        static int PreviewRTId(int width, int height)
        {
            if (IsPreviewRT(width) && IsPreviewRT(height))
            {
                return width* height;
            }
            return -1;
        }

        static Dictionary<int, RenderTexture> previewRenderTextures = new Dictionary<int, RenderTexture>();

        static RenderTexture GetRenderTexture(int width, int height)
        {
            int id = PreviewRTId(width, height);
            if (id>=0)
            {
                RenderTexture previewTexture = null;
                if (!previewRenderTextures.TryGetValue(id,out previewTexture))
                {
                    previewTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                    previewTexture.enableRandomWrite = true;
                    previewTexture.anisoLevel = 0;
                    previewTexture.filterMode = FilterMode.Point;
                    previewTexture.Create();
                    previewRenderTextures.Add(id, previewTexture);
                }
                return previewTexture;
            }
            else
            {
                RenderTexture previewTexture = null;
                previewTexture = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
                previewTexture.enableRandomWrite = true;
                previewTexture.anisoLevel = 0;
                previewTexture.filterMode = FilterMode.Point;
                previewTexture.Create();
                return previewTexture;
            }
        }

        static void DestroyRenderTexture(int width, int height, RenderTexture renderTexture)
        {
            if (!IsPreviewRT(width, height))
            {
                if (Application.isPlaying)
                {
                    GameObject.Destroy(renderTexture);
                }
                else
                {
                    GameObject.DestroyImmediate(renderTexture);
                }
            }
        }

        static Vector3 previewPosOffset = new Vector3(0,0,0);

        static Texture2D GetAssetPreviewRun( GameObject obj, int width, int height,Color backColor)
        {
            Camera previewCam = null;
            GameObject go = new GameObject("render camera");
            go.SetActive(false);
            previewCam = go.AddComponent<Camera>();
            previewCam.backgroundColor = new Color(0f, 0f, 0.5f, 1f);
            previewCam.clearFlags = CameraClearFlags.Color;
            previewCam.cameraType = CameraType.Preview;
            previewCam.nearClipPlane = 0.01f;

            RenderTexture previewTexture = GetRenderTexture(width, height);

            //
            previewCam.targetTexture = previewTexture;
            GameObject clone = GameObject.Instantiate(obj);
            clone.transform.localScale = Vector3.one;
            int count = clone.transform.childCount;
            for (int i = 0; i < count; ++i)
            {
                Transform c = clone.transform.GetChild(i);
                c.gameObject.SetActive(true);
            }
            clone.SetActive(true);
            Transform cloneTransform = clone.transform;
            cloneTransform.position = new Vector3(-5000, -5000, -5000)+ previewPosOffset;
            cloneTransform.eulerAngles = new Vector3(0, 0, 0);
            Bounds bounds = GetBounds(clone);
            Vector3 Min = bounds.min;
            Vector3 Max = bounds.max;
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = (Min + Max) / 2f;
            cube.transform.eulerAngles = Vector3.zero;
            float maxSize = float.MinValue;
            if (maxSize < (Max.x - Min.x))
            {
                maxSize = Max.x - Min.x;
            }
            if (maxSize < (Max.y - Min.y))
            {
                maxSize = Max.y - Min.y;
            }
            if (maxSize < (Max.z - Min.z))
            {
                maxSize = Max.z - Min.z;
            }
            cube.transform.localScale = Vector3.one * maxSize;
            cube.transform.SetParent(cloneTransform, true);
            cube.GetComponent<MeshRenderer>().enabled = false;
            //计算摄像机位置
            previewCam.transform.SetParent(cube.transform, true);
            previewCam.transform.localPosition = Vector3.zero;
            previewCam.transform.localEulerAngles = Vector3.zero;
            Vector3 localPos = new Vector3(0.7069294f, 0.670375f, 0.6315957f);
            previewCam.transform.localPosition = localPos;
            previewCam.transform.LookAt(cube.transform.position);

            Vector3 offset = previewCam.transform.position - clone.transform.position+Vector3.one;

            offset.y = 0;
            cloneTransform.position = cloneTransform.position + offset*2;
            previewPosOffset = previewPosOffset + offset;
            if (previewPosOffset.x>=1000 || previewPosOffset.z>=1000)
            {
                previewPosOffset = Vector3.zero;
            }

            if (SystemInfo.supportsComputeShaders)
            {
                int kernelInitHandle = GameObjectPreviewShader.FindKernel("InitRT");
                GameObjectPreviewShader.SetTexture(kernelInitHandle, "InitResult", previewTexture);
                GameObjectPreviewShader.Dispatch(kernelInitHandle, width / 8, height / 8, 1);
            }

            previewCam.Render();

            Texture2D tex2d = new Texture2D(width, height, TextureFormat.ARGB32, false);
            if (SystemInfo.supportsComputeShaders)
            {
                //
                int kernelHandle = GameObjectPreviewShader.FindKernel("CSMain");
                GameObjectPreviewShader.SetTexture(kernelHandle, "InputTexture", previewTexture);
                OutputData[] outputBuffer = new OutputData[width * height];
                ComputeBuffer cb = new ComputeBuffer(width * height, 16);
                cb.SetData(outputBuffer);
                GameObjectPreviewShader.SetBuffer(kernelHandle, "OutputBuffer", cb);
                GameObjectPreviewShader.SetInt("TextureWidth", width);
                GameObjectPreviewShader.SetInt("TextureHight", height);
                GameObjectPreviewShader.SetVector("BackColor", new Vector4(backColor.r, backColor.g, backColor.b, backColor.a));
                GameObjectPreviewShader.Dispatch(kernelHandle, width / 8, height / 8, 1);
                cb.GetData(outputBuffer);
                Color[] colors = new Color[outputBuffer.Length];
                for (int i = 0; i < colors.Length; ++i)
                {
                    Vector4 c = outputBuffer[i].color;
                    colors[i] = new Color(c.x, c.y, c.z, c.w);
                }
                tex2d.SetPixels(colors);
            }
            else
            {
                RenderTexture.active = previewTexture;
                tex2d.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            }

            tex2d.Apply();

            previewCam.targetTexture = null;
            RenderTexture.active = null;
            //if (Application.isPlaying)
            //{
            //    GameObject.Destroy(clone);
            //}
            //else
            //{
            //    GameObject.DestroyImmediate(clone);
            //}
            //DestroyRenderTexture(width, height, previewTexture);

            return tex2d;
        }

        /// <summary>
        /// 创建BoxCollider
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static BoxCollider CreateBoxCollider(this GameObject obj)
        {
            BoxCollider res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = CreateBoxColliderRun(obj);
                }, null);
            }
            else
            {
                res = CreateBoxColliderRun(obj);
            }
            return res;
        }

        static BoxCollider CreateBoxColliderRun(GameObject obj)
        {
            Bounds bounds = obj.GetBounds();
            if (bounds.size.x > 0 || bounds.size.y > 0 || bounds.size.z > 0)
            {
                BoxCollider box = obj.AddComponent<BoxCollider>();
                box.center = bounds.center - obj.transform.position;
                box.size = bounds.size;
                return box;
            }
            return null;
        }

        
        /// <summary>
        /// 获得某物体的bounds
        /// </summary>
        /// <param name="obj"></param>
        public static Bounds GetBounds(this GameObject obj, bool includeHide = true)
        {
            Bounds res= new Bounds ();
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetBoundsRun(obj, includeHide);
                }, null);
            }
            else
            {
                res = GetBoundsRun(obj, includeHide);
            }
            return res;
        }

        static Bounds GetBoundsRun( GameObject obj, bool includeHide = true)
        {
            Renderer[] renders = obj.GetComponentsInChildren<Renderer>(includeHide);
            Renderer seldRend = obj.GetComponent<Renderer>();
            if (seldRend != null)
            {
                System.Array.Resize<Renderer>(ref renders, renders.Length + 1);
                renders[renders.Length - 1] = seldRend;
            }
            Quaternion ori = obj.transform.rotation;
            obj.transform.rotation = Quaternion.Euler(0, 0, 0);
            float xmax = float.MinValue;
            float ymax = float.MinValue;
            float zmax = float.MinValue;
            float xmin = float.MaxValue;
            float ymin = float.MaxValue;
            float zmin = float.MaxValue;
            for (int i = 0; i < renders.Length; i++)
            {
                Renderer render = renders[i];
                ParticleSystem part = render.gameObject.GetComponent<ParticleSystem>();
                if (part != null) continue;
                Vector3 vmax = render.bounds.max;
                Vector3 vmin = render.bounds.min;

                xmax = Mathf.Max(xmax, vmax.x);
                ymax = Mathf.Max(ymax, vmax.y);
                zmax = Mathf.Max(zmax, vmax.z);

                xmin = Mathf.Min(xmin, vmin.x);
                ymin = Mathf.Min(ymin, vmin.y);
                zmin = Mathf.Min(zmin, vmin.z);
            }

            Vector3 s = obj.transform.localScale;
            Vector3 c = new Vector3((xmax + xmin) * 0.5f, (ymax + ymin) * 0.5f, (zmax + zmin) * 0.5f);
            obj.transform.rotation = ori;
            // return new Bounds(new Vector3(c.x / s.x, c.y / s.y, c.z / s.z), new Vector3((xmax - xmin) / s.x, (ymax - ymin) / s.y, (zmax - zmin) / s.z));
            return new Bounds(c, new Vector3((xmax - xmin), (ymax - ymin), (zmax - zmin)));
        }

        /// <summary>
        /// 获得物体的总缩放 会计算所有父级缩放
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Vector3 GetScale(this GameObject obj)
        {
            Vector3 res = new Vector3();
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetScaleRun(obj);
                }, null);
            }
            else
            {
                res = GetScaleRun(obj);
            }
            return res;
        }

        static Vector3 GetScaleRun( GameObject obj)
        {
            Vector3 sc = obj.transform.localScale;
            Transform parent = obj.transform.parent;
            while (parent != null)
            {
                Vector3 parentSc = parent.localScale;
                sc.x = parentSc.x * sc.x;
                sc.y = parentSc.y * sc.y;
                sc.z = parentSc.z * sc.z;
                parent = obj.transform.parent;
            }
            return sc;
        }

        /// <summary>
        /// 删除所有子物体
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static void ClearChilds(this GameObject obj)
        {
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    ClearChildsRun(obj);
                }, null);
            }
            else
            {
                ClearChildsRun(obj);
            }
        }

        static void ClearChildsRun(GameObject obj)
        {
            int num = obj.transform.childCount;
            for (int i = 0; i < obj.transform.childCount; ++i)
            {
                Transform tra = obj.transform.GetChild(0);
                if (Application.isPlaying)
                {
                    GameObject.Destroy(tra.gameObject);
                }
                else
                {
                    GameObject.DestroyImmediate(tra.gameObject);
                }
            }
        }

        /// <summary>
        /// 搜索子物体组件-GameObject版
        /// </summary>
        public static T GetComponent<T>(this GameObject go, string childName) where T : Component
        {
            T res = null;
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    res = GetComponentRun<T>(go, childName);
                }, null);
            }
            else
            {
                res = GetComponentRun<T>(go, childName);
            }
            return res;
        }

        static T GetComponentRun<T>(GameObject go, string childName) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.transform.Find(childName);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 深度获得子节点
        /// </summary>
        /// <param name="target"></param>
        /// <param name="childName"></param>
        /// <param name="res"></param>
        public static void FinChild(this GameObject go, string childName, ref Transform res, int maxLayerNum)
        {
            if (SynchronizationContext.Current != ThreadHelper.UnitySynchronizationContext)
            {
                Transform resTemp = null;
                ThreadHelper.UnitySynchronizationContext.Send((o) => {
                    Transform refRes = null;
                    FinChildRun(go, childName,ref refRes, maxLayerNum);
                    resTemp = refRes;
                }, null);
                res = resTemp;
            }
            else
            {
                FinChildRun(go, childName, ref res, maxLayerNum);
            }
        }

        static void FinChildRun( GameObject go, string childName, ref Transform res, int maxLayerNum)
        {
            Transform find = go.transform.Find(childName);
            if (find != null)
            {
                res = find;
                return;
            }
            if (maxLayerNum == 0)
            {
                return;
            }
            int curLayerNum = 0;
            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform c = go.transform.GetChild(i);
                FinChild(c.gameObject, childName, ref res, maxLayerNum, curLayerNum);
                if (res != null)
                {
                    return;
                }
            }
        }

        static void FinChild(this GameObject go, string childName, ref Transform res, int maxLayerNum, int curLayerNum)
        {
            curLayerNum = curLayerNum++;
            if (curLayerNum > maxLayerNum)
            {
                return;
            }
            Transform find = go.transform.Find(childName);
            if (find != null)
            {
                res = find;
                return;
            }
            int childCount = go.transform.childCount;
            for (int i = 0; i < childCount; ++i)
            {
                Transform c = go.transform.GetChild(i);
                FinChild(c.gameObject, childName, ref res, maxLayerNum, curLayerNum);
                if (res != null)
                {
                    return;
                }
            }
        }
    }

    public class ModalSizeData
    {
        /// <summary>
        /// 第一个box中心点(局部坐标)
        /// </summary>
        public Vector3 centerNormal;//
        /// <summary>
        /// 第一个box尺寸(局部坐标)
        /// </summary>
        public Vector3 sizeNormal;//
    }
}

