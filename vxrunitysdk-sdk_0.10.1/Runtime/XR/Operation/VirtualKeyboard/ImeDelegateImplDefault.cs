using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.OpenXR.Input;
using SGManagedPlugin;
using UnityEngine.XR;
using com.vivo.codelibrary;
using Pose = UnityEngine.Pose;

namespace com.vivo.openxr
{
    public class DefaultImeViewGather
    {
        private GameObject[] KbdViews;
        private bool _isActiveView = false;

        public DefaultImeViewGather(GameObject[] param)
        {
            KbdViews = param;
        }

        public void SetActive(bool bActive)
        {
            foreach (GameObject view in KbdViews)
            {
                view.SetActive(bActive);
            }
            _isActiveView = bActive;
        }

        public void SetActive(bool[] bActives)
        {
            bool show = false;
            foreach (GameObject view in KbdViews)
            {
                int blockType = GetBlockType(view.name);
                if (blockType == -1)
                {
                    continue;
                }
                view.SetActive(bActives[blockType]);
                show = show || bActives[blockType];
            }
            _isActiveView = show;
        }

        public bool IsActive()
        {
            return _isActiveView;
        }

        public bool FindName(string name)
        {
            foreach (GameObject view in KbdViews)
            {
                if (view.name == name)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetTexture(Texture2D[] texs)
        {
            foreach (GameObject view in KbdViews)
            {
                int blockType = GetBlockType(view.name);
                if (blockType == -1)
                {
                    continue;
                }
                VLog.Info("ime DelegateImpl_kbd SetTexture: blockType:" + blockType + " name:" + view.name);
                Renderer rend = view.GetComponent<Renderer>();                
                rend.material.mainTexture = texs[blockType];                
            }            
        }

        public int GetBlockType(string name)
        {
            int blockType = -1;
            if (name == "surfaceLeft")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_LEFT;
            }
            else if (name == "surfaceMiddle")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_MIDDLE;
            }
            else if (name == "surfaceRight")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_RIGHT;
            }
            else if (name == "surfaceNumber")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_NUMBER;
            }
            else if (name == "surfaceVoiceDefault")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_DEFAULT;
            }
            else if (name == "surfaceVoiceEditing")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_EDITING;
            }
            else if (name == "surfaceVoiceError")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_ERROR;
            }
            else if (name == "surfaceVoiceSymbol")
            {
                blockType = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_SYMBOL;
            }

            return blockType;
        }

        public void ResizeBlockSurface(VXRPlugin.ImeBlockType blockType, Vector2 vSize)
        {
            if (blockType < 0 || blockType >= VXRPlugin.ImeBlockType.TYPE_BLOCK_BUTT) return;
            GameObject view = KbdViews[(int)blockType];
            Transform transform = view.transform;
            transform.localScale = new Vector3(vSize.x / 330.0f, vSize.y / 222.0f, 1.0f);
        }
    }

    public class DefaultImeMouseTracker
    {
        private bool _downOld = false;
        private Vector2 _ptOld = new Vector2();
        private VXRPlugin.ImeMotionEventType _motionEvent;
        private const float _trackRadius = 20.0f;
        private long _timeDown;
        private bool _isLongPressed = false;
        private long _intervelLongPress = 100;

        public bool Track(Vector2 pt, bool bDown)
        {
            bool bRes = false;
            if (bDown)
            {
                if (_downOld)
                {
                    _motionEvent = VXRPlugin.ImeMotionEventType.ACTION_MOVE;
                    if (!_isLongPressed)
                    {
                        long timeDiff = DateTime.Now.Ticks - _timeDown;
                        if (timeDiff > _intervelLongPress)
                        {
                            _isLongPressed = true;
                            _motionEvent = VXRPlugin.ImeMotionEventType.ACTION_LONGPRESS;
                            bRes = true; //force sendmessage
                        }
                    }
                }
                else
                {
                    _motionEvent = VXRPlugin.ImeMotionEventType.ACTION_DOWN;
                    _timeDown = DateTime.Now.Ticks;
                    _isLongPressed = false;
                }
            }
            else
            {
                if (_downOld)
                {
                    _motionEvent = VXRPlugin.ImeMotionEventType.ACTION_UP;
                }
                else
                {
                    //_motionEvent = VXRPlugin.ImeMotionEventType.ACTION_HOVER_MOVE;
                    _motionEvent = VXRPlugin.ImeMotionEventType.ACTION_MOVE; //c++代码只识别move事件
                }
            }

            if (_downOld != bDown)
            {
                bRes = true;
            }
            else if (PointDist(_ptOld, pt) > _trackRadius)
            {
                bRes = true;
            }

            _downOld = bDown;

            if (bRes)
            {
                _ptOld = pt;
            }

            return bRes;
        }

        public bool TrackOuter()
        {
            bool bRes = false;
            if (_motionEvent != VXRPlugin.ImeMotionEventType.ACTION_OUTSIDE)
            {
                VXRPlugin.ImeMotionEventType eventOld = _motionEvent;
                _motionEvent = VXRPlugin.ImeMotionEventType.ACTION_OUTSIDE;
                bRes = true;
            }

            return bRes;
        }

        public Vector2 GetPoint()
        {
            return _ptOld;
        }

        public VXRPlugin.ImeMotionEventType GetEvent()
        {
            return _motionEvent;
        }

        private float PointDist(Vector2 ptNew, Vector2 ptOld)
        {
            return Math.Abs(ptNew[0] - ptOld[0]) + Math.Abs(ptNew[1] - ptOld[1]);
        }
    }

    public class ImeDelegateImplDefault : ImeDelegateBase
    {
        public GameObject[] KbdViews;
        public DefaultImeViewGather KbdViewGather;
        [SerializeField]
        private VoiceHandleBoard[] _handleBoards;
        private ImeManager _imeManager = ImeManager.Instance;
        private Vector2 _textureSize = new Vector2(780, 390);
        private Vector2 _ptKbd = new Vector2();
        private DefaultImeMouseTracker _mouseTracker = new DefaultImeMouseTracker();
        //--
        private Texture2D[] _kbdTextures = new Texture2D[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_BUTT];
        private bool[] _textureActives = { false, false, false, false, false, false, false, false };
        private static int _widthLeft = 74;    //normal keyboard
        private static int _widthMiddle = 826;
        private static int _widthRight = 318;
        private static int _normalHeight = 454;
        private static int _numberWidth = 375; //number keyboard
        private static int _numberHeight = 392;
        private static int _voiceDefaultWidth = 330; //audio keyboard default
        private static int _voiceDefaultHeight = 222;
        private static int _voiceEditingWidth = 330; //audio keyboard editing
        private static int _voiceEditingHeight = 510;
        private static int _voiceErrorWidth = 390; //audio keyboard error
        private static int _voiceErrorHeight = 218;
        private static int _voiceSymbolWidth = 816; //audio keyboard symbol
        private static int _voiceSymbolHeight = 454;
        private static List<Vector2> _dataSlices = new List<Vector2> { new Vector2(_widthLeft, _normalHeight),
            new Vector2(_widthMiddle, _normalHeight), new Vector2(_widthRight, _normalHeight),
            new Vector2(_numberWidth, _numberHeight),
            new Vector2(_voiceDefaultWidth, _voiceDefaultHeight), new Vector2(_voiceEditingWidth, _voiceEditingHeight),
            new Vector2(_voiceErrorWidth, _voiceErrorHeight), new Vector2(_voiceSymbolWidth, _voiceSymbolHeight)};
        private List<byte[]> _dataBlocks = new List<byte[]>();
        private VXRPlugin.ImeDeviceCommandType _lastCommand = VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_BUTT;
        private static long _intervalTime = 100;
        private static long _longPressIntervalTime = 50;
        private long _commandTimeStamp = (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
        private long _lastCommandTimeStamp = 0;
        private static int _locationIndex = 0;
        private static int _scaleIndex = 1;
        private static int mEulerIndex = 2;
        private static List<List<Vector3>> _locations = new List<List<Vector3>> {
            { new List<Vector3>{ new Vector3(0, -0.3f, -59.7f), new Vector3(0.01f, 0.01f, 0.01f), new Vector3(-35, 180, 0)} },
            { new List<Vector3>{ new Vector3(0, -10.0f, 0), new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0, 180, 0)} }
    };
        private int _curScene = -1;
        private VXRVirtualKeyboard _keyboard;
        private bool[] _isControllerTrigger;
        private Pose[] _rayStartPose;
        private int _currentControlerFlag = -1;
        //ImeDelegateBase
        public override void OnIMEShow(Vector2 vSize)
        {
            CreateTexture(vSize);    
        }

        public override void OnIMEHide()
        {
            VLog.Info("ime DelegateImpl_kbd OnIMEHide");
            // enactive
            for (int i = 0; i < (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_BUTT; ++i)
            {
                _textureActives[i] = false;
            }
            VLog.Info("ime DelegateImpl_kbd OnIMEHide1");
            KbdViewGather.SetActive(false);
            HideSubDevices();
            VLog.Info("ime DelegateImpl_kbd OnIMEHide2");
        }

        public override void OnIMEError(VXRPlugin.ImeError nType, string strErr)
        {
        }

        public override void OnAdjustLayout(Vector2 vSize, ref byte[] textureData)
        {
            VLog.Info("ime ImeDelegateImpl_kbd::OnAdjustLayout() w:" + (int)vSize.x + ", h" + (int)vSize.y);
            // judge block whether active or not
            _textureSize = vSize;
            UpdateBlockStatus(vSize);

            if (GetKeyBoardLayoutType(vSize) == (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_NORMAL)
            {
                List<int> slices = new List<int>();
                GetCurrentSlices(ref slices);
                Algorithm.CropBuffer(textureData, (int)_textureSize.x, (int)_textureSize.y, slices, ref _dataBlocks);
            }
            else if (GetKeyBoardLayoutType(vSize) == (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_NUMBER)
            {
                _dataBlocks[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_NUMBER] = textureData;
            }
            else if (GetKeyBoardLayoutType(vSize) == (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_VOICE)
            {
                _dataBlocks[(int)GetVoiceKbdBlockType(vSize)] = textureData;
            }

            for (int i = 0; i < (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_BUTT; i++)
            {
                VLog.Info("ime ImeManager LoadTextureData, 0 block=" + i + " act:" + _textureActives[i] + " size:" + vSize + " bufferSize:" + _dataBlocks[i].GetLength(0) + " slice:" + _dataSlices[i]);
                if (_textureActives[i])
                {
                    _kbdTextures[i].LoadRawTextureData(_dataBlocks[i]); //_dataBlocks[LEFT]
                    _kbdTextures[i].Apply();
                    VLog.Info("ime ImeManager LoadTextureData, block=" + i);
                }
            }

            KbdViewGather.SetActive(_textureActives);
            if (GetKeyBoardLayoutType(vSize) == (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_VOICE)
            {
                if (null != _imeManager && !_imeManager.IsRecording())
                {
                    VXRPlugin.ImeBlockType blockType = GetVoiceKbdBlockType(vSize);
                    VXRPlugin.HandlerPhase phase = VXRPlugin.HandlerPhase.HANDLER_PHASE_BUTT;
                    if (blockType == VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_DEFAULT)
                    {
                        phase = VXRPlugin.HandlerPhase.HANDLER_PHASE_DEFAULT;
                    }
                    else if (blockType == VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_EDITING)
                    {
                        phase = VXRPlugin.HandlerPhase.HANDLER_PHASE_EDITING;
                    }
                    if (phase == VXRPlugin.HandlerPhase.HANDLER_PHASE_DEFAULT || phase == VXRPlugin.HandlerPhase.HANDLER_PHASE_EDITING)
                    {
                        ShowSubDevices(phase);
                    }
                    else
                    {
                        HideSubDevices();
                    }
                }
                else
                {
                    HideSubDevices();
                }
            }
            else
            {
                HideSubDevices();
            }
        }

        public override bool OnImeIsShow()
        {
            if (KbdViewGather == null) return false;
            return KbdViewGather.IsActive();
        }

        public override void OnCommand(VXRPlugin.ImeDeviceCommandType command)
        {
            VLog.Info("ime ImeDelegateImpl_kbd:OnCommand=" + command);

            if (command == VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_BUTT)
            {
                _lastCommand = command;
                return;
            }

            // flow control
            _commandTimeStamp = (long)(DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds;
            if (_lastCommand == command)
            {
                if ((command == VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_DOWN ||
                    command == VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_LEFT ||
                    command == VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_RIGHT) &&
                    _commandTimeStamp - _lastCommandTimeStamp < _intervalTime)
                {
                    return;
                }
                else if (command == VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_UP &&
                    _commandTimeStamp - _lastCommandTimeStamp < _longPressIntervalTime)
                {
                    return;
                }
            }

            // diverse to ime service
            VLog.Info("ime ImeDelegateImpl_kbd:OnCommand= after flowcontrol " + command + " timestamp:" + _commandTimeStamp);
            // proc()
            if (null != _imeManager)
            {
                _imeManager.OnTouch(-1, -1, (VXRPlugin.ImeMotionEventType)((int)VXRPlugin.ImeMotionEventType.ACTION_HANDLER_JOYSTICK_BASE + (int)command));
            }
            _lastCommand = command;
            _lastCommandTimeStamp = _commandTimeStamp;
        }

        public override Transform GetTransform()
        {
            return transform;
        }

        public override void OnIMESetScene(VXRPlugin.ImeSceneType scene)
        {
            if (_curScene != -1 && _curScene == (int)scene)
            {
                return;
            }
            VLog.Info("ime ImeDelegateImpl_kbd:OnIMESetScene scene:" + scene);
            transform.localPosition = _locations[(int)scene][_locationIndex];
            transform.localScale = _locations[(int)scene][_scaleIndex];
            transform.localEulerAngles = _locations[(int)scene][mEulerIndex];
            _curScene = (int)scene;
        }

        //MonoBehaviour
        void Start()
        {
            this.name = "DelegateImpl_kbd"; //current game name
            _isControllerTrigger = new bool[2];
            KbdViewGather = new DefaultImeViewGather(KbdViews);
            _rayStartPose = new Pose[2];
            _rayStartPose[0] = Pose.identity;
            _rayStartPose[1] = Pose.identity;
            CreateTexture(_textureSize);
#if UNITY_EDITOR
#else
        OnIMEHide();
        //KbdViewGather.SetActive(false);
        _lastCommand = VXRPlugin.ImeDeviceCommandType.TYPE_DEVICE_JOYSTICK_COMMAND_BUTT;
#endif
        }

        public override void PlayVibrator(string type)
        {
            VLog.Info("ime ImeDelegateImpl_kbd:PlayVibrator:" + int.Parse(type));

            // note: to replace the implimentation of vibrator below with vendor's
            uint channel = 0;
            float amplitude = 0.2f;
            float duration = 0.5f;
            UnityEngine.XR.InputDevice leftdevice = DefaultImeDeviceInfo.GetDeviceByRole(InputDeviceRole.LeftHanded);
            VLog.Info("ime ImeDelegateImpl_kbd:PlayVibrator:ch:" + leftdevice.characteristics);
            if ((leftdevice.characteristics & InputDeviceCharacteristics.Left) != 0)
            {
                leftdevice.SendHapticImpulse(channel, amplitude, duration);
            }
            UnityEngine.XR.InputDevice rightdevice = DefaultImeDeviceInfo.GetDeviceByRole(InputDeviceRole.RightHanded);
            VLog.Info("ime ImeDelegateImpl_kbd:PlayVibrator:ch:" + rightdevice.characteristics);
            if ((rightdevice.characteristics & InputDeviceCharacteristics.Right) != 0)
            {
                rightdevice.SendHapticImpulse(channel, amplitude, duration);
            }
        }

        void Update()
        {
            CheckPlayerRigEvent();
            _imeManager.Draw();
        }
        
        private void ShowSubDevices(VXRPlugin.HandlerPhase phase)
        {
            foreach (var device in _handleBoards)
            {
              device.Show(phase);
            }
        }

        private void HideSubDevices()
        {
            foreach (var device in _handleBoards)
            {
              device.Hide();
            }
        }

        //other
        private void CreateTexture(Vector2 vSize)
        {
            if (_kbdTextures[0])
            {
                return;
            }
            // Create textures and data block
            for (int i = 0; i < _dataSlices.Count; i++)
            {
                //normal keyborad layout
                _kbdTextures[i] = new Texture2D((int)_dataSlices[i].x, (int)_dataSlices[i].y, TextureFormat.RGBA32, false);
                // Set point filtering just so we can see the pixels clearly
                _kbdTextures[i].filterMode = FilterMode.Trilinear;
                // Call Apply() so it's actually uploaded to the GPU
                _kbdTextures[i].Apply();
                //create data block memory
                _dataBlocks.Add(new byte[(int)_dataSlices[i].x * (int)_dataSlices[i].y * 4]);
            }

            VLog.Info(" ime DelegateImpl_kbd CreateTexture: texture created");

            // Set texture onto kbdview
            KbdViewGather.SetTexture(_kbdTextures);
        }

        private void DispatchMessageToAndroid(VXRPlugin.ImeMotionEventType type, Vector2 pt)
        {
            if (null != _imeManager)
            {
                _imeManager.OnTouch(pt.x, pt.y, type);
            }
        }

        private void LogEvent(string prefix, PointerEventData eventData)
        {
            VLog.Info(prefix + ": " + eventData.pointerCurrentRaycast.gameObject.name + " x=" + eventData.position.x +
                      ",y=" + eventData.position.y);
        }

        private void CheckMouseEvent()
        {
            if (Point2UV(Input.mousePosition, ref _ptKbd))
            {
                if (_mouseTracker.Track(_ptKbd, Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
                {
                    DispatchMessageToAndroid(_mouseTracker.GetEvent(), _mouseTracker.GetPoint());
                }
            }
            else if (_mouseTracker.TrackOuter())
            {
                DispatchMessageToAndroid(_mouseTracker.GetEvent(), _mouseTracker.GetPoint());
            }
        }

        private bool Point2UV(Vector3 ptScreen, ref Vector2 ptUV)
        {
            Ray ray = Camera.main.ScreenPointToRay(ptScreen);
            RaycastHit hitInfo;
            bool bRes = false;
            if (Physics.Raycast(ray, out hitInfo))
            {
                string name = hitInfo.collider.gameObject.name;
                if (KbdViewGather.FindName(name))
                {
                    GameObject kbd = hitInfo.collider.gameObject;
                    Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
                    Vector2 pixelUV = hitInfo.textureCoord;
                    Renderer rend = hitInfo.transform.GetComponent<Renderer>();
                    ptUV.x = pixelUV.x * _textureSize.x;
                    ptUV.y = (1 - pixelUV.y) * _textureSize.y;
                    //VLog.Info("ray click " + name + ": 3d point=" + vecKbd.ToString() + " uv=(" + pixelUV.x + "," + pixelUV.y + ") org=(" + ptUV.ToString() + ")" + " w=" + texSize.x + ",h=" + texSize.y);
                    bRes = true;
                }
            }

            return bRes;
        }

        private int GetBlockOffsetX(string name)
        {
            int offsetX = 0;
            int blockType = KbdViewGather.GetBlockType(name);
            if (blockType > 0 && blockType < (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_NUMBER)
            {
                for (int i = blockType - 1; i >= 0; --i)
                {
                    offsetX += (int)_dataSlices[i].x;
                }
            }
            return offsetX;
        }
        
        private void CheckPlayerRigEvent()
        {
            if (_isControllerTrigger[0]||_isControllerTrigger[1])
            {
                _currentControlerFlag = _isControllerTrigger[0] ? 0 : 1;
                if (TryGetRayPose(_currentControlerFlag))
                {
                    CheckHandleRayTrigger(_currentControlerFlag);
                }
                return;
            }
            for (int i = 0; i < 2; i++)
            {
                if (TryGetRayPose(i))
                {
                    CheckHandleRayTrigger(i);
                }
            }
        }

        private bool TryGetRayPose(int controllerFlag)
        {
            if (controllerFlag == 0)
            {
                if (_keyboard.LeftZAxisGameObject)
                {
                    _rayStartPose[0].position = _keyboard.LeftZAxisGameObject.transform.position;
                    _rayStartPose[0].rotation = _keyboard.LeftZAxisGameObject.transform.rotation;
                }
                else
                {
                    try
                    {
                        _rayStartPose[0].position =
                            _keyboard._leftRayPositionUpdateAction.ReadValue<Vector3>();
                        _rayStartPose[0].rotation =
                            _keyboard._leftRayRotationUpdateAction.ReadValue<Quaternion>();
                    }
                    catch (Exception e)
                    {
                        VLog.Warning("VirtualKeyboard not get left controller pointer position/rotation");
                        return false;
                    }
                }
            }
            else
            {
                if (_keyboard.RightZAxisGameObject)
                {
                    _rayStartPose[1].position = _keyboard.RightZAxisGameObject.transform.position;
                    _rayStartPose[1].rotation = _keyboard.RightZAxisGameObject.transform.rotation;
                }
                else
                {
                    try
                    {
                        _rayStartPose[1].position =
                            _keyboard._rightRayUpdatePositionAction.ReadValue<Vector3>();
                        _rayStartPose[1].rotation =
                            _keyboard._rightRayUpdateRotationAction.ReadValue<Quaternion>();
                    }
                    catch (Exception e)
                    {
                        VLog.Warning("VirtualKeyboard not get right controller pointer position/rotation");
                        return false;
                    }
                }
            }

            return true;
        }
        
        void CheckHandleRayTrigger(int controllerFlag)
        {
            Ray ray = new Ray(_rayStartPose[controllerFlag].position, _rayStartPose[controllerFlag].rotation*Vector3.forward);
            if (Point2UVNew(ray, ref _ptKbd))
            {
                if (_mouseTracker.Track(_ptKbd, _isControllerTrigger[controllerFlag]))
                {
                    VLog.Info("VirtualKeyboard Point2UVNew --- isTrigger: " + _isControllerTrigger[controllerFlag] + " --- _ptKbd:" + _ptKbd + "--- GetEvent() "
                              + _mouseTracker.GetEvent() + " --- GetPoint() " + _mouseTracker.GetPoint());
                    DispatchMessageToAndroid(_mouseTracker.GetEvent(), _ptKbd); //_mouseTracker.GetPoint()
                }
            }
            else if (_mouseTracker.TrackOuter())
            {
                DispatchMessageToAndroid(_mouseTracker.GetEvent(), _mouseTracker.GetPoint());
            }
        }

        private bool Point2UVNew(Ray ray, ref Vector2 ptUV)
        {
            RaycastHit hitInfo;
            bool bRes = false;
            if (Physics.Raycast(ray, out hitInfo,VXRVirtualKeyboard.k_rayMaxLength,_keyboard.InteractionLayer))
            {
                string name = hitInfo.collider.gameObject.name;
                //VLog.Info("Point2UVNew: name=" + name);
                if (KbdViewGather.FindName(name))
                {
                    GameObject kbd = hitInfo.collider.gameObject;
                    Vector3 vecKbd = kbd.transform.InverseTransformPoint(hitInfo.point);
                    Vector2 pixelUV = hitInfo.textureCoord;
                    Renderer rend = hitInfo.transform.GetComponent<Renderer>();
                    int blockType = KbdViewGather.GetBlockType(name);
                    if (blockType == -1)
                    {
                        return false;
                    }
                    int width = (int)_dataSlices[blockType].x;
                    int height = (int)_dataSlices[blockType].y;
                    ptUV.x = pixelUV.x * width + GetBlockOffsetX(name);
                    ptUV.y = (1 - pixelUV.y) * height;
                    //VLog.Info("Point2UVNew: name=" + name + " width=" + width + "height=" + height + " pixelUV.x=" + pixelUV.x + " pixelUV.y=" + pixelUV.y);
                    bRes = true;
                }
            }

            return bRes;
        }

        VXRPlugin.ImeBlockType GetVoiceKbdBlockType(Vector2 vSize)
        {
            for (int i = (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_DEFAULT; i <= (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_VOICE_SYMBOL; i++)
            {
                if (vSize == _dataSlices[i]) return (VXRPlugin.ImeBlockType)i;
            }
            return VXRPlugin.ImeBlockType.TYPE_BLOCK_BUTT;
        }

        bool IsVoiceKeyboardLayout(Vector2 vSize)
        {
            return GetVoiceKbdBlockType(vSize) != VXRPlugin.ImeBlockType.TYPE_BLOCK_BUTT;
        }

        int GetKeyBoardLayoutType(Vector2 vSize)
        {
            VLog.Info("ime GetKeyBoardLayoutType vSize:" + vSize + " " + _dataSlices[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_NUMBER].y + " is:" + IsVoiceKeyboardLayout(vSize));
            if ((int)vSize.y == (int)_dataSlices[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_NUMBER].y)
            {
                return (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_NUMBER;
            }
            else if (IsVoiceKeyboardLayout(vSize))
            {
                return (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_VOICE;
            }
            else
            {
                return (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_NORMAL;
            }
        }

        void UpdateBlockStatus(Vector2 vSize)
        {
            int width = (int)vSize.x;
            int height = (int)vSize.y;

            // enactive
            for (int i = 0; i < (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_BUTT; ++i)
            {
                _textureActives[i] = false;
            }

            int keyboardType = GetKeyBoardLayoutType(vSize);
            if (keyboardType == (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_NORMAL)
            {
                if (width == (int)_dataSlices[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_LEFT].x + (int)_dataSlices[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_MIDDLE].x)
                {
                    _textureActives[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_LEFT] = true;
                    _textureActives[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_MIDDLE] = true;
                }
                else if (width >= (int)_dataSlices[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_LEFT].x + (int)_dataSlices[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_MIDDLE].x
                  + (int)_dataSlices[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_RIGHT].x)
                {
                    _textureActives[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_LEFT] = true;
                    _textureActives[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_MIDDLE] = true;
                    _textureActives[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_RIGHT] = true;
                }
            }
            else if (keyboardType == (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_NUMBER)
            {
                _textureActives[(int)VXRPlugin.ImeBlockType.TYPE_BLOCK_NUMBER] = true;
            }
            else if (keyboardType == (int)VXRPlugin.ImeKeyboardLayoutType.TYPE_KEYBOARD_LAYOUT_VOICE)
            {
                _textureActives[(int)GetVoiceKbdBlockType(vSize)] = true;
            }
        }

        void GetCurrentSlices(ref List<int> slices)
        {
            for (int i = 0; i <= (int)VXRPlugin.ImeBlockType.TYPE_BLOCK_RIGHT; ++i)
            {
                if (_textureActives[i])
                {
                    slices.Add((int)_dataSlices[i].x);
                }
            }
        }

        public override void SetTriggerStatus(int controllerFlag, bool isTrigger)
        {
            _isControllerTrigger[controllerFlag] = isTrigger;
        }

        public override void OnIMECommit(string strCommit)
        {
            throw new NotImplementedException();
        }

        public override void OnIMEKey(VXRPlugin.ImeKey key)
        {
            throw new NotImplementedException();
        }

        public override void SetKeyVirtualBoard(VXRVirtualKeyboard keyboard)
        {
            _keyboard = keyboard;
        }
    }
}
