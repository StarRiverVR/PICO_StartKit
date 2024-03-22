using com.vivo.codelibrary;
using System;
using UnityEngine;
namespace com.vivo.openxr
{
    public class VXROverlay : MonoBehaviour
    {
        public enum OverlayShape
        {
            Quad = VXRPlugin.OverlayShape.Quad,
            Cylinder = VXRPlugin.OverlayShape.Cylinder,
        }
        
        public enum OverlayType
        {
            Underlay = VXRPlugin.OverlayType.Underlay,
            Overlay = VXRPlugin.OverlayType.Overlay
        }

        private struct SwapchainTexturesInfo
        {
            public int EyeId;
            public IntPtr[] SwapchainImgIds;
            public Texture[] SwapchianTextures;
        }
        
        static UInt32 s_curMaxLayerId = 0;
        public OverlayShape CurShape = OverlayShape.Quad;
        public OverlayType CurOverlayType = OverlayType.Overlay;
        public int CurOverlayDepth;        
        public bool IsExternalTexture = false;
        private int _externalSurfaceWidth = 1024;
        private int _externalSurfaceHeight = 1024;
        public Texture[] SourceTextures = new Texture[2];
        public bool[] SourceTexturesIsSRGB = new bool[2];
        public bool IsDynamic = false;
        public GameObject CameraRoot;
        private static Material _cubeMat;
        private VXRPlugin.OverlayLayerLayout _layout;
        private UInt32 _layerId { get; set; } = 0;
        private VXRPlugin.OverlayCreateParams _createParams;
        private VXRPlugin.OverlaySubmitParams _submitParams;
        private VXRPlugin.Vector3f _xrPosition = VXRPlugin.Vector3f.zero;
        private VXRPlugin.Quatf _xrQuaternion =VXRPlugin.Quatf.identity;
        private VXRPlugin.Vector3f _xrSize = VXRPlugin.Vector3f.zero;
        private int _eyesCount = 1;
        private SwapchainTexturesInfo[] _swapChainTextureInfos;
        private bool _isHeadLock;
        private Matrix4x4[] _textureMatrices = new Matrix4x4[2];
        private Vector3[] _layerScales = new Vector3[2];
        private Quaternion[] _layerRotations = new Quaternion[2];
        private Vector3[] _layerTranslations = new Vector3[2];
        private Quaternion[] _cameraRotations = new Quaternion[2];
        private Vector3[] _cameraTranslations = new Vector3[2];
        private Camera[] _overlayEyeCamera = new Camera[2];
        private UInt32 _swapChainImgCount = 0;
        private bool _isTextureChange = true;
        private bool _isGetSwapChainImg = false;
        [HideInInspector]
        public IntPtr AndroidSurfaceProject;
        public Action AndroidSurfaceProjectCreatedCallBack;
        private VXROverlayManager _overlayMgrIns;
        
        void Start()
        {
            _layerId = GetLayerID();
            VXRPlugin.CreateCompositionLayer(_layerId, (VXRPlugin.OverlayShape)CurShape);
             _createParams = new VXRPlugin.OverlayCreateParams()
            {
                LayerId = this._layerId,
                LayerType = VXRPlugin.OverlayType.Overlay,
                Layout = VXRPlugin.OverlayLayerLayout.Mono,
                Depth = 1,
                Format = 1,
                SampleCount = 0,
                Width = 0,
                Height = 0,
                MipCount = 0,
                //FaceCount = curShape == OverlayShape.Cubemap ? 6 : 1,
                FaceCount = 1,
                IsDynamic = this.IsDynamic ? 1 : 0,
                IsExternalTexture  = this.IsExternalTexture ? 1 : 0,
            };
            _submitParams = new VXRPlugin.OverlaySubmitParams();
            RefreshEyesCameras();
            for (int i = 0; i < SourceTextures.Length; ++i)
            {
                SourceTexturesIsSRGB[i] = TextureIsSRGB(SourceTextures[i]);
            }

            _overlayMgrIns = VXROverlayManager.Instance;
        }

        private void LateUpdate()
        {
            if (_isTextureChange)
            {
                ResetLayer();
                ReadyLayerParams();
                InitializeCompositionLayer();
                _isTextureChange = false;
            }
            if (CheckNeedTexture())
            {
                GetSwapChainTextures();
                RenderSwapChainTextures();
            }
            UpdateLayerSubmitParams();
            SubmitLayerParams();
        }
        
        private static UInt32 GetLayerID()
        {
            s_curMaxLayerId++;
            return VXROverlay.s_curMaxLayerId;
        }

        private void ResetLayer()
        {
            DestroyRenderTexture();
            VXRPlugin.ResetCompositionLayer(_layerId, (VXRPlugin.OverlayShape)CurShape);
        }

        private bool CheckNeedTexture()
        {
            if (IsExternalTexture)
            {
                if (AndroidSurfaceProject == IntPtr.Zero)
                {
                    VXRPlugin.AcquireCompositionLayerSwapchainImage(_layerId);
                    AndroidSurfaceProject = VXRPlugin.GetAndroidSurfaceObj(_layerId);
                    if (AndroidSurfaceProject != IntPtr.Zero)
                    {
                        _overlayMgrIns.StartIssueUpdate();
                        VLog.Info("vxroverlay获取到AndroidSurfaceObject");
                    }
                    if (AndroidSurfaceProject != IntPtr.Zero && AndroidSurfaceProjectCreatedCallBack != null )
                    {
                        AndroidSurfaceProjectCreatedCallBack();
                    }
                }
                return false;
            }
            return true;
        }

        private void ReadyLayerParams()
        {
            int mipCount = 0;
            int textureWidth = 0;
            int textureHeight = 0;
            if (!IsExternalTexture)
            {
                var tex2D = SourceTextures[0] as Texture2D;
                if (tex2D != null && (tex2D.format == TextureFormat.RGBAHalf || tex2D.format == TextureFormat.RGBAFloat))
                {
                    mipCount = tex2D.mipmapCount;
                }
                var texCube = SourceTextures[0] as Cubemap;
                if (texCube != null && (texCube.format == TextureFormat.RGBAHalf || texCube.format == TextureFormat.RGBAFloat))
                {
                    mipCount = texCube.mipmapCount;
                }
                _layout = SourceTextures[0] == SourceTextures[1] ? VXRPlugin.OverlayLayerLayout.Mono : VXRPlugin.OverlayLayerLayout.Stereo;
                _eyesCount = _layout == VXRPlugin.OverlayLayerLayout.Mono ? 1 : 2;
                textureWidth = SourceTextures[0].width;
                textureHeight = SourceTextures[0].height;
            }
            else
            {                
                _layout = VXRPlugin.OverlayLayerLayout.Mono;
                _eyesCount = _layout == VXRPlugin.OverlayLayerLayout.Mono ? 1 : 2;
                textureWidth = _externalSurfaceWidth;
                textureHeight = _externalSurfaceHeight;                
            }
            _createParams.LayerType = (VXRPlugin.OverlayType)CurOverlayType;
            _createParams.Layout = _layout;
            _createParams.Depth = this.CurOverlayDepth;
            _createParams.Format = (UInt64)VXRPlugin.OverlayLayerFormat.GL_SRGB8_ALPHA8;
            _createParams.SampleCount = 1;
            _createParams.Width = (UInt32)textureWidth;
            _createParams.Height = (UInt32)textureHeight;
            _createParams.MipCount = (UInt32)mipCount;            
        }
        
        private void InitializeCompositionLayer()
        {
            VXRPlugin.InitializeCompositionLayer(_createParams);
            if (IsExternalTexture)
            {
                _overlayMgrIns.SendCrateAndroidSurfaceEvent();
            }
        }

        private void GetSwapChainTextures()
        {
            if (_isGetSwapChainImg) { return; }
            _swapChainImgCount = VXRPlugin.GetCompositionLayerSwapchainImgCount(_layerId);
            if (_swapChainImgCount == 0)
            {
                return;
            }
            _swapChainTextureInfos = new SwapchainTexturesInfo[_eyesCount];
            for (int eyeId = 0; eyeId < _eyesCount; eyeId++)
            {
                _swapChainTextureInfos[eyeId].EyeId = eyeId;
                _swapChainTextureInfos[eyeId].SwapchainImgIds = new IntPtr[_swapChainImgCount];
                _swapChainTextureInfos[eyeId].SwapchianTextures = new Texture[_swapChainImgCount];
                for (int imgIndex = 0; imgIndex < _swapChainImgCount; imgIndex++)
                {
                    IntPtr imgPtr = (IntPtr)VXRPlugin.GetCompositionLayerSwapchainTexturePtr(_layerId, eyeId, imgIndex);
                    Texture texture;
                    //if (curShape == OverlayShape.Cubemap)
                    //{
                    //    texture = Cubemap.CreateExternalTexture((int)_createparams.Width, TextureFormat.RGBA32, false, imgPtr);
                    //}
                    //else
                    //{
                    texture = Texture2D.CreateExternalTexture((int)_createParams.Width, (int)_createParams.Height, TextureFormat.ARGB32, false, true, imgPtr);
                    //}
                    _swapChainTextureInfos[eyeId].SwapchainImgIds[imgIndex] = imgPtr;
                    _swapChainTextureInfos[eyeId].SwapchianTextures[imgIndex] = texture;
                }
            }
            _isGetSwapChainImg = true;
        }

        private Material _overlayMat;

        private Material OverlayMat
        {
            get
            {
                if (_overlayMat == null)
                {
                    _overlayMat = new Material(Shader.Find("Vivo/Hide/OverlayConvert"));
                }

                return _overlayMat;
            }
        }

        private bool TextureIsSRGB(Texture tex)
        {
            if (tex != null && tex.graphicsFormat.ToString().Contains("SRGB"))
            {
                return true;
            }

            return false;
        }

        private void RenderSwapChainTextures()
        {
            if (!_isGetSwapChainImg)
            {
                return;
            }
            VXRPlugin.AcquireCompositionLayerSwapchainImage(_layerId);
            for (int eyeId = 0; eyeId < _swapChainTextureInfos.Length; eyeId++)
            {
                //if (curShape == OverlayShape.Cubemap && null == SourceTextures[eyeId] as Cubemap)
                //{
                //    return;
                //}                
                UInt32 acquireIndex = VXRPlugin.GetSwapchainAcquireIndex(_layerId, eyeId);
                Texture[] swapImgs = _swapChainTextureInfos[eyeId].SwapchianTextures;
                Texture swapchainTexture = swapImgs[acquireIndex];
                if (swapchainTexture == null)
                {
                    continue;
                }
                for (int k = 0; k < _createParams.FaceCount; k++)
                {
                    RenderTextureDescriptor rtDes = new RenderTextureDescriptor((int)_createParams.Width, (int)_createParams.Height, RenderTextureFormat.ARGB32, 0);

                    rtDes.msaaSamples = (int)_createParams.SampleCount;
                    rtDes.useMipMap = true;
                    rtDes.autoGenerateMips = false;
                    rtDes.sRGB = true;
                    RenderTexture renderTexture = RenderTexture.GetTemporary(rtDes);
                    if (!renderTexture.IsCreated())
                    {
                        renderTexture.Create();
                    }
                    renderTexture.DiscardContents();

                    if (SourceTexturesIsSRGB[eyeId])
                    {
                        OverlayMat.EnableKeyword(string.Intern("_NEEDLINEAR_TO_SRGB_CONVERSION"));
                    }
                    else
                    {
                        OverlayMat.DisableKeyword(string.Intern("_NEEDLINEAR_TO_SRGB_CONVERSION"));
                    }
                    Graphics.Blit(SourceTextures[eyeId], renderTexture, OverlayMat);
                    Graphics.CopyTexture(renderTexture, 0, 0, swapchainTexture, k, 0);
                    RenderTexture.ReleaseTemporary(renderTexture);
                }
            }
        }

        /// <summary>
        /// 设置左右眼的纹理
        /// </summary>
        /// <param name="leftEyeTexture"></param>
        /// <param name="rightEyeTexture"></param>
        public void SetTextures(Texture leftEyeTexture, Texture rightEyeTexture)
        {
            if (!IsDynamic)
            {                
                VLog.Warning("当前合成层为静态纹理模式，不能动态修改显示纹理！");
                return;
            }
            if (leftEyeTexture != null && (leftEyeTexture.width != SourceTextures[0].width || leftEyeTexture.height != SourceTextures[0].height || leftEyeTexture.graphicsFormat != leftEyeTexture.graphicsFormat))
            {
                _isGetSwapChainImg = false;
                _isTextureChange = true;
                SourceTextures[0] = leftEyeTexture;
                SourceTexturesIsSRGB[0] = TextureIsSRGB(SourceTextures[0]);
            }
            if (rightEyeTexture != null && (rightEyeTexture.width != SourceTextures[1].width || rightEyeTexture.height != SourceTextures[1].height || rightEyeTexture.graphicsFormat != rightEyeTexture.graphicsFormat))
            {
                _isGetSwapChainImg = false;
                _isTextureChange = true;
                SourceTextures[1] = rightEyeTexture;
                SourceTexturesIsSRGB[1] = TextureIsSRGB(SourceTextures[1]);
            }
        }

        private void UpdateLayerSubmitParams()
        {
            if (transform == null || !gameObject.activeSelf || _overlayEyeCamera[0] == null || _overlayEyeCamera[0] == null)
            {
                return;
            }
            for (int i = 0; i < _textureMatrices.Length; i++)
            {
                if (transform is RectTransform uiTransform)
                {
                    var rect = uiTransform.rect;
                    var lossyScale = transform.lossyScale;
                    _layerScales[i] = new Vector3(rect.width * lossyScale.x,
                        rect.height * lossyScale.y, 1);
                    _layerTranslations[i] = uiTransform.TransformPoint(rect.center);
                }
                else
                {
                    _layerScales[i] = transform.lossyScale;
                    _layerTranslations[i] = transform.position;
                }
                _layerRotations[i] = transform.rotation;
                _cameraRotations[i] = _overlayEyeCamera[i].transform.rotation;
                _cameraTranslations[i] = _overlayEyeCamera[i].transform.position;
            }
            if (_isHeadLock)
            {
                _xrQuaternion.x = transform.localRotation.x;
                _xrQuaternion.y = transform.localRotation.y;
                _xrQuaternion.z = -transform.localRotation.z;
                _xrQuaternion.w = -transform.localRotation.w;
                _xrPosition.x = transform.localRotation.x;
                _xrPosition.y = transform.localRotation.y;
                _xrPosition.z = -transform.localRotation.z;
            }
            else
            {
                _xrQuaternion.x = _layerRotations[0].x;
                _xrQuaternion.y = _layerRotations[0].y;
                _xrQuaternion.z = -_layerRotations[0].z;
                _xrQuaternion.w = -_layerRotations[0].w;
                _xrPosition.x = _layerTranslations[0].x;
                _xrPosition.y = _layerTranslations[0].y;
                _xrPosition.z = -_layerTranslations[0].z;
            }
            _xrSize.x = transform.localScale.x;
            _xrSize.y = transform.localScale.y;
            _xrSize.z = transform.localScale.z;
        }

        private void SubmitLayerParams()
        {
            _submitParams.OverLayerType = (VXRPlugin.OverlayType)CurOverlayType;
            _submitParams.OverlayDepth = CurOverlayDepth;
            _submitParams.Position = _xrPosition;
            _submitParams.Quaternion = _xrQuaternion;
            _submitParams.Size = _xrSize;
            VXRPlugin.SubmitCompositionLayerParams(_layerId, _submitParams);
        }

        private void RefreshEyesCameras()
        {
            _overlayEyeCamera[0] = Camera.main;
            _overlayEyeCamera[1] = Camera.main;
            Camera[] eyeCamera = new Camera[3];
            Camera[] cam = CameraRoot.GetComponentsInChildren<Camera>();
            for (int i = 0; i < cam.Length; i++)
            {
                if (cam[i].stereoTargetEye == StereoTargetEyeMask.Both && cam[i] == Camera.main)
                {
                    eyeCamera[0] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Left)
                {
                    eyeCamera[1] = cam[i];
                }
                else if (cam[i].stereoTargetEye == StereoTargetEyeMask.Right)
                {
                    eyeCamera[2] = cam[i];
                }
            }
            if (eyeCamera[0] != null && eyeCamera[0].enabled)
            {
                _overlayEyeCamera[0] = eyeCamera[0];
                _overlayEyeCamera[1] = eyeCamera[0];
            }
            else if (cam[1] != null && cam[2] != null)
            {
                _overlayEyeCamera[0] = eyeCamera[1];
                _overlayEyeCamera[1] = eyeCamera[2];
            }
        }

        private void DestroyRenderTexture()
        {
            if (_swapChainTextureInfos == null)
            {
                return;
            }
            for (int i = 0; i < _swapChainTextureInfos.Length; i++)
            {
                _swapChainTextureInfos[i].EyeId = 0;

                for (int j = 0; j < _swapChainImgCount; j++)
                {
                    _swapChainTextureInfos[i].SwapchainImgIds[j] = IntPtr.Zero;
                    DestroyImmediate(_swapChainTextureInfos[i].SwapchianTextures[j]);
                }
            }
            _swapChainTextureInfos = null;
        }

        void OnEnable()
        {
            _isGetSwapChainImg = false;
            _isTextureChange = true;
        }

        void OnDisable()
        {
            ResetLayer();
        }

        void OnDestroy()
        {
            DestroyRenderTexture();
            VXRPlugin.DestroyCompositionLayer(_layerId);
        }        
    }

}

