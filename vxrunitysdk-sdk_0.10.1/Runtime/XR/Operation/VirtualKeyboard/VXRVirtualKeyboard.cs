using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System;
using com.vivo.codelibrary;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.InputSystem;


namespace com.vivo.openxr
{
    public class VXRVirtualKeyboard : MonoBehaviour
    {
        /// <summary>
        /// 键盘位置类型枚举
        /// </summary>
        public enum KeyboardLocationType
        {
            /// <summary>
            /// 近场显示
            /// </summary>
            Near,
            /// <summary>
            /// 远场显示
            /// </summary>
            Far,
            /// <summary>
            /// 自定义移动位置
            /// </summary>
            Custom,
        }
        
        /// <summary>
        /// 键盘布局类型
        /// </summary>
        public enum VirtualKeyBoardLayout
        {
            /// <summary>
            /// 常规全键盘
            /// </summary>
            Text = 1,
            /// <summary>
            /// 数字键盘
            /// </summary>
            Number = 2,
        }

        public enum ControllerFlag
        {
            None=-1,
            LeftController=0,
            RightController=1
        }
        
        private struct VirtualKeyboardLocationInfo
        {
            public KeyboardLocationType LocationType;
            public Vector3 Position;
            public Quaternion Rotation;
            public Vector3 Scale;
        }

        private VXRPlugin.ImeType CurImeType = VXRPlugin.ImeType.Default;

        /// <summary>
        /// 推荐位置类型，让虚拟键盘以合适的尺寸显示在相应的位置
        /// </summary>
        [Tooltip("推荐位置类型，让虚拟键盘以合适的尺寸显示在相应的位置")]
        [SerializeField]
        [FormerlySerializedAs("LocationType")]
        private KeyboardLocationType _locationType = KeyboardLocationType.Near;

        /// <summary>
        /// 虚拟键盘响应的输入框列表，默认响应第一个，点击输入框时会切换当前响应的输入框对象
        /// </summary>
        [Tooltip("虚拟键盘响应的输入框列表，默认响应第一个，点击输入框时会切换当前响应的输入框对象")]
        [SerializeField]
        [FormerlySerializedAs("_inputFieldCommits")]
        private List<InputField>  _inputFieldCommits;
        
        /// <summary>
        /// 和键盘交互的射线发射源，需要指定一个z轴向前的物体来作为左控制器射线发射源
        /// </summary>
        [Tooltip("和键盘交互的射线发射源，需要指定一个z轴向前的物体来作为右控制器射线发射源")]
        public GameObject LeftZAxisGameObject;

        /// <summary>
        /// 和键盘交互的射线发射源，需要指定一个z轴向前的物体来作为左控制器射线发射源
        /// </summary>
        [Tooltip("和键盘交互的射线发射源，需要指定一个z轴向前的物体来作为右控制器射线发射源")]
        public GameObject RightZAxisGameObject;
        
        /// <summary>
        /// 射线选择时可被响应的层
        /// </summary>
        [Tooltip("射线选择时可被响应的层")]
        public LayerMask InteractionLayer;

        public const float k_rayMaxLength = 100;
        
        /// <summary>
        /// 左手柄触发射线点击的按键
        /// </summary>
        [Space(5)]
        [Header("Controller Trigger Action")]
        [Tooltip("左手柄触发射线点击的按键")]
        [SerializeField]
        private InputAction _leftControllerTriggerAction = null;
        
        /// <summary>
        /// 右手柄触发射线点击的按键
        /// </summary>
        [Tooltip("右手柄触发射线点击的按键")]
        [SerializeField]
        private InputAction _rightControllerTriggerAction = null;
        
       
        /// <summary>
        /// 左手柄控制器射线触发的响应按键
        /// </summary>
        [Space(5)]
        [Header("Left Controller Ray Start Pose")]
        [Tooltip("左手柄控制器射线触发的起点坐标")]
        public InputAction _leftRayPositionUpdateAction = null;
        
        /// <summary>
        /// 左手柄控制器射线触发的响应按键
        /// </summary>
        [Tooltip("左手柄控制器射线的发射方向的四元数")]
        public InputAction _leftRayRotationUpdateAction = null;
        
     
        /// <summary>
        /// 右手柄控制器射线触发的响应按键
        /// </summary>
        [Space(5)]
        [Header("Right Controller Ray Start Pose")]
        [Tooltip("右手柄控制器射线触发的起点坐标")]
        public InputAction _rightRayUpdatePositionAction = null;
        
        /// <summary>
        /// 右手柄控制器射线触发的响应按键
        /// </summary>

        [Tooltip("右手柄控制器射线的发射方向的四元数")]
        public InputAction _rightRayUpdateRotationAction = null;
        
        private GameObject _moveWidget;
        private ImeManager _imeManager;
        private InputField _runtimeInputField;
        private Action<string> _commitTextCallBack;
        private Action _backSpaceCallBack;
        private Action _enterCallBack;
        private Action _showCallBack;
        private Action _hideCallBack;
        private Action _recordStartCallBack;
        private Action _recordEndCallback;
        private bool[] _isControllerTrigger;
        private bool _isShow = false;
        private VirtualKeyboardLocationInfo _locationInfo = new VirtualKeyboardLocationInfo();
        private bool _isMoving = false;
        private float _moveDistance = 0;
        private Transform _keyboardTransform;
        private ControllerFlag _currentMovingFlag;
        private Pose[] _rayStartPoses;
        private const int k_handleCount = 2;
        
        /// <summary>
        /// 当前响应键盘数据的输入框,可由外部调用动态加入一个输入框
        /// </summary>
        public InputField TextCommitField
        {
            get => _runtimeInputField;
            set
            {
                if (!_inputFieldCommits.Contains(value))
                {
                    _inputFieldCommits.Add(value);
                    AddInputNameClickEvent(value);
                }
                _runtimeInputField = value;               
            }
        }

        private void AddInputNameClickEvent(InputField textInput)
        {
            textInput.keyboardType = (TouchScreenKeyboardType)(-1);   
            var eventTrigger = textInput.gameObject.GetComponent<EventTrigger>();
            if (eventTrigger == null)
            {
                eventTrigger = textInput.gameObject.AddComponent<EventTrigger>();
            }
            UnityAction<BaseEventData> selectEvent = (BaseEventData data) => {

                OnInputFieldClicked(textInput);
            };
            EventTrigger.Entry onClick = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerClick
            };
            onClick.callback.AddListener(selectEvent);
            eventTrigger.triggers.Add(onClick);         
        }

        private void OnInputFieldClicked(InputField textInput)
        {
            
            if (_isShow && textInput == TextCommitField)
            {             
                return;
            }
            if (TextCommitField != textInput)
            {             
                TextCommitField = textInput;
                _locationType = _locationType == KeyboardLocationType.Custom ? KeyboardLocationType.Near : _locationType;
            }            
            UseRecommendLocation(_locationType);            
        }
        
        private void Awake()
        {
            _imeManager = ImeManager.Instance;

            for (int i = 0; i < _inputFieldCommits.Count; i++)
            {
                AddInputNameClickEvent(_inputFieldCommits[i]);
            }            
            AddCommitTextListener(OnCommitText);
            if (_inputFieldCommits.Count > 0)
            {
                TextCommitField = _inputFieldCommits[0];
            }            
        }
        
        private void OnEnable()
        {
            _leftControllerTriggerAction.Enable();
            _rightControllerTriggerAction.Enable();
            
            _leftRayPositionUpdateAction.Enable();
            _leftRayRotationUpdateAction.Enable();
            
            _rightRayUpdatePositionAction.Enable();
            _rightRayUpdateRotationAction.Enable();
            
            
            if (_leftControllerTriggerAction != null)
            {
                _leftControllerTriggerAction.started += OnLeftControllerTriggerActionStarted;
                _leftControllerTriggerAction.performed += OnLeftControllerTriggerActionPerformed;
                _leftControllerTriggerAction.canceled += OnLeftControllerTriggerActionCanceled;
            }

            if (_rightControllerTriggerAction != null)
            {
                _rightControllerTriggerAction.started += OnRightControllerTriggerActionStarted;
                _rightControllerTriggerAction.performed += OnRightControllerTriggerActionPerformed;
                _rightControllerTriggerAction.canceled += OnRightControllerTriggerActionCanceled;
            }
        }

        private void OnDisable()
        {
            if (_leftControllerTriggerAction != null)
            {
                _leftControllerTriggerAction.Disable();
                _leftControllerTriggerAction.started -= OnLeftControllerTriggerActionStarted;
                _leftControllerTriggerAction.performed -= OnLeftControllerTriggerActionPerformed;
                _leftControllerTriggerAction.canceled -= OnLeftControllerTriggerActionCanceled;
            }

            if (_rightControllerTriggerAction != null)
            {
                _rightControllerTriggerAction.Disable();
                _rightControllerTriggerAction.started -= OnRightControllerTriggerActionStarted;
                _rightControllerTriggerAction.performed -= OnRightControllerTriggerActionPerformed;
                _rightControllerTriggerAction.canceled -= OnRightControllerTriggerActionCanceled;
            }

            _leftRayPositionUpdateAction.Disable();
            _leftRayRotationUpdateAction.Disable();
            _rightRayUpdatePositionAction.Disable();
            _rightRayUpdateRotationAction.Disable();
        }
        
        void Start()
        {
            VLog.Info("ime component start");            
            _imeManager.CreateIme(CurImeType, this);
            _isControllerTrigger = new bool[k_handleCount];
            _rayStartPoses = new Pose[2];
            _rayStartPoses[0] = Pose.identity;
            _rayStartPoses[1] = Pose.identity;

            _currentMovingFlag = ControllerFlag.None;
            HideKeyBoard();
            _moveWidget = transform.Find("keyboard/move").gameObject;
            _keyboardTransform = transform.Find("keyboard");
            _moveWidget.SetActive(false);
        }
        
        void Update()
        {
            _imeManager.UpdateData();
            UpdateCommitText();
            SyncKeyboardLocation();
            UpdateKeyboardVisible();
        }
        
        private void UpdateCommitText()
        {
            VXRPlugin.ImeKey nCommit = (VXRPlugin.ImeKey)_imeManager.GetCommitCode();
            if (nCommit == VXRPlugin.ImeKey.KEYCODE_COMMIT)
            {
                string mStrCommit = _imeManager.GetCommitString();
                _commitTextCallBack?.Invoke(mStrCommit);
            }
            else if (nCommit != VXRPlugin.ImeKey.KEYCODE_UNKNOWN)
            {
                switch (nCommit)
                {
                    case VXRPlugin.ImeKey.KEYCODE_DEL:
                        if (TextCommitField != null)
                        {
                            String strText = TextCommitField.text;
                            TextCommitField.text = strText.Remove(strText.Length - 1);
                        }
                        _backSpaceCallBack?.Invoke();
                        break;
                    case VXRPlugin.ImeKey.KEYCODE_ENTER:

                        if (TextCommitField != null)
                        {
                            TextCommitField.text = TextCommitField.text + "\n";
                        }
                        _enterCallBack?.Invoke();
                        break;
                    case VXRPlugin.ImeKey.KEYCODE_VOICE_START:
                        _recordStartCallBack?.Invoke();
                        break;
                    case VXRPlugin.ImeKey.KEYCODE_VOICE_END:
                        _recordEndCallback?.Invoke();
                        break;
                }
            }
        }

        private void OnCommitText(string text)
        {
            if (TextCommitField == null)
            {
                return;
            }
            if (TextCommitField.isFocused && TextCommitField.caretPosition != TextCommitField.text.Length)
            {
                VLog.Warning("Virtual Keyboard expects an end of text caretPosition");
            }
            TextCommitField.SetTextWithoutNotify(TextCommitField.text + text);
            if (TextCommitField.isFocused && TextCommitField.caretPosition != TextCommitField.text.Length)
            {
                TextCommitField.caretPosition = TextCommitField.text.Length;
            }

            TextCommitField.onValueChanged.Invoke(TextCommitField.text);
        }

        #region Public Methods

        /// <summary>
        /// 添加键盘文本提交事件监听
        /// </summary>
        /// <param name="listener">键盘文本提交事件响应时需要回调的函数</param>
        public void AddCommitTextListener(Action<string> listener)
        {
            _commitTextCallBack += listener;
        }

        /// <summary>
        /// 移除键盘文本提交事件监听
        /// </summary>
        /// <param name="listener">键盘文本提交事件响应时需要回调的函数</param>
        public void RemoveCommitTextListener(Action<string> listener)
        {
            _commitTextCallBack -= listener;
        }

        /// <summary>
        /// 添加键盘退格键事件监听
        /// </summary>
        /// <param name="listener">键盘退格键事件响应时需要回调的函数</param>
        public void AddBackSpaceListener(Action listener)
        {
            _backSpaceCallBack += listener;
        }

        /// <summary>
        /// 移除键盘退格键事件监听
        /// </summary>
        /// <param name="listener">键盘退格键事件响应时需要回调的函数</param>
        public void RemoveBackSpaceListener(Action listener)
        {
            _backSpaceCallBack -= listener;
        }

        /// <summary>
        /// 添加键盘回车键事件监听
        /// </summary>
        /// <param name="listener">键盘回车键事件响应时需要回调的函数</param>
        public void AddEnterListener(Action listener)
        {
            _enterCallBack += listener;
        }

        /// <summary>
        /// 移除键盘回车键事件监听
        /// </summary>
        /// <param name="listener">键盘回车键事件响应时需要回调的函数</param>
        public void RemoveEnterListener(Action listener)
        {
            _enterCallBack -= listener;
        }

        /// <summary>
        /// 添加键盘显示事件监听
        /// </summary>
        /// <param name="listener">键盘显示事件响应时需要回调的函数</param>
        public void AddShowListener(Action listener)
        {
            _showCallBack += listener;
        }

        /// <summary>
        /// 移除键盘显示事件监听
        /// </summary>
        /// <param name="listener">键盘显示事件响应时需要回调的函数</param>
        public void RemoveShowListener(Action listener)
        {
            _showCallBack -= listener;
        }

        /// <summary>
        /// 添加键盘隐藏事件监听
        /// </summary>
        /// <param name="listener">键盘隐藏事件响应时需要回调的函数</param>
        public void AddHideListener(Action listener)
        {
            _hideCallBack += listener;
        }

        /// <summary>
        /// 移除键盘隐藏事件监听
        /// </summary>
        /// <param name="listener">键盘隐藏事件响应时需要回调的函数</param>
        public void RemoveHideListener(Action listener)
        {
            _hideCallBack -= listener;
        }

        /// <summary>
        /// 添加语音输入录音开始事件监听
        /// </summary>
        /// <param name="listener">键盘语音输入开始事件响应时需要回调的函数</param>
        public void AddRecordStartListener(Action listener)
        {
            _recordStartCallBack += listener;
        }

        /// <summary>
        /// 移除语音输入录音开始事件监听
        /// </summary>
        /// <param name="listener">键盘语音输入开始事件响应时需要回调的函数</param>
        public void RemoveRecordStartListener(Action listener)
        {
            _recordStartCallBack -= listener;
        }

        /// <summary>
        /// 添加语音输入录音结束事件监听
        /// </summary>
        /// <param name="listener">键盘语音输入结束事件响应时需要回调的函数</param>
        public void AddRecordEndListener(Action listener)
        {
            _recordEndCallback += listener;
        }

        /// <summary>
        /// 移除语音输入录音结束事件监听
        /// </summary>
        /// <param name="listener">键盘语音输入结束事件响应时需要回调的函数</param>
        public void RemoveRecordEndListener(Action listener)
        {
            _recordEndCallback -= listener;
        }

        /// <summary>
        /// 显示虚拟键盘
        /// </summary>
        /// <param name="keyboardLayout">键盘显示布局类型，Text：常规全键盘、Number：数字键盘，默认为常规全键盘</param>
        public void ShowKeyBoard(VirtualKeyBoardLayout keyboardLayout = VirtualKeyBoardLayout.Text)
        {
            _imeManager.Show((VXRPlugin.VirtualKeyBoardLayout)keyboardLayout);            
        }

        /// <summary>
        /// 隐藏键盘
        /// </summary>
        public void HideKeyBoard()
        {
            _imeManager.Hide();            
        }

        /// <summary>
        /// 使用推荐的键盘显示位置
        /// </summary>
        /// <param name="locationType">推荐位置类型：Near:近场显示  Far:远场显示 Custom:自定义移动位置 </param>
        public void UseRecommendLocation(KeyboardLocationType locationType)
        {

            _locationType = locationType;
            switch (locationType)
            {
                case KeyboardLocationType.Near:
                    _locationInfo.Scale = new Vector3(0.02f, 0.02f, 0.02f);
                    _locationInfo.Position = Camera.main.transform.position + Camera.main.transform.rotation* Vector3.forward * 1 ; 
                    _locationInfo.Rotation = Camera.main.transform.rotation;                    
                    break;
                case KeyboardLocationType.Far:
                    _locationInfo.Scale = new Vector3(0.2f, 0.2f, 0.2f);
                    _locationInfo.Position = Camera.main.transform.position + Camera.main.transform.rotation * Vector3.forward * 10;
                    _locationInfo.Rotation = Camera.main.transform.rotation;
                    break;
                case KeyboardLocationType.Custom:
                    _locationInfo.Position = transform.position;
                    _locationInfo.Rotation = transform.rotation;
                    _locationInfo.Scale = transform.lossyScale;
                    break;
                default:
                    break;
            }
            _locationInfo.LocationType = locationType;
            _keyboardTransform.position = _locationInfo.Position;
            _keyboardTransform.rotation = _locationInfo.Rotation;
            _keyboardTransform.localScale = _locationInfo.Scale;
            ShowKeyBoard();
        }

        #endregion Public Methods

        private void OnLeftControllerTriggerActionStarted(InputAction.CallbackContext ctx)
        {
            _isControllerTrigger[(int)ControllerFlag.LeftController] = true;
            if (_imeManager != null) _imeManager.SetTriggerStatus((int)ControllerFlag.LeftController, true);
            VLog.Info("VirtualKeyboard  left Controller Started");
        }

        private void OnLeftControllerTriggerActionPerformed(InputAction.CallbackContext ctx)
        {
            _isControllerTrigger[(int)ControllerFlag.LeftController] = true;
            if (_imeManager != null) _imeManager.SetTriggerStatus((int)ControllerFlag.LeftController, true);
        }

        private void OnLeftControllerTriggerActionCanceled(InputAction.CallbackContext ctx)
        {
            _isControllerTrigger[(int)ControllerFlag.LeftController] = false;
            if (_imeManager != null) _imeManager.SetTriggerStatus((int)ControllerFlag.LeftController, false);
            VLog.Info("VirtualKeyboard  left Controller Canceled");
        }

        private void OnRightControllerTriggerActionStarted(InputAction.CallbackContext ctx)
        {
            _isControllerTrigger[(int)ControllerFlag.RightController] = true;
            if (_imeManager != null) _imeManager.SetTriggerStatus((int)ControllerFlag.RightController, true);
            
            VLog.Info("VirtualKeyboard  right Controller Started");
        }

        private void OnRightControllerTriggerActionPerformed(InputAction.CallbackContext ctx)
        {
            _isControllerTrigger[(int)ControllerFlag.RightController] = true;
            if (_imeManager != null) _imeManager.SetTriggerStatus((int)ControllerFlag.RightController, true);
        }

        private void OnRightControllerTriggerActionCanceled(InputAction.CallbackContext ctx)
        {
            _isControllerTrigger[(int)ControllerFlag.RightController] = false;
            if (_imeManager != null) _imeManager.SetTriggerStatus((int)ControllerFlag.RightController, false);
            VLog.Info("VirtualKeyboard  right Controller Canceled");
        }

        private void SyncKeyboardLocation()
        {
            if (!_isMoving)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (TryGetRayPose(i))
                    {
                        HandleCheckFirstLocationMove(i);
                    }
                }
            }
            else
            {
                if (TryGetRayPose((int)_currentMovingFlag))
                {
                    HandleSyncKeyboardLocation();
                }
            }
            
            if (_currentMovingFlag == (int)ControllerFlag.LeftController &&
                !_isControllerTrigger[(int)ControllerFlag.LeftController] ||
                _currentMovingFlag == ControllerFlag.RightController &&
                !_isControllerTrigger[(int)ControllerFlag.RightController])
            {
                _currentMovingFlag = ControllerFlag.None;
                _isMoving = false;
            }
        }

        private void HandleCheckFirstLocationMove(int controllerFlag)
        {
            if (!_isMoving && _currentMovingFlag == ControllerFlag.None && _isControllerTrigger[controllerFlag])
            {
                Ray ray = new Ray(_rayStartPoses[controllerFlag].position,
                    _rayStartPoses[controllerFlag].rotation * Vector3.forward);
                RaycastHit hitInfo;
                if (Physics.Raycast(ray, out hitInfo, k_rayMaxLength, InteractionLayer))
                {
                    if (hitInfo.collider.gameObject == _moveWidget)
                    {
                        _currentMovingFlag = (ControllerFlag)controllerFlag;
                        _isMoving = true;
                        _locationType = KeyboardLocationType.Custom;
                        _moveDistance = Vector3.Distance(_moveWidget.transform.position,
                            _rayStartPoses[controllerFlag].position);
                        VLog.Info($"VirtualKeyboard Ray Move Start Flag {controllerFlag}");
                    }
                }
            }
        }

        private bool TryGetRayPose(int controllerFlag)
        {
            if (controllerFlag == 0)
            {
                if (LeftZAxisGameObject)
                {
                    _rayStartPoses[0].position = LeftZAxisGameObject.transform.position;
                    _rayStartPoses[0].rotation = LeftZAxisGameObject.transform.rotation;
                }
                else
                {
                    try
                    {
                        _rayStartPoses[0].position = _leftRayPositionUpdateAction.ReadValue<Vector3>();
                        _rayStartPoses[0].rotation = _leftRayRotationUpdateAction.ReadValue<Quaternion>();
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
                if (RightZAxisGameObject)
                {
                    _rayStartPoses[1].position = RightZAxisGameObject.transform.position;
                    _rayStartPoses[1].rotation = RightZAxisGameObject.transform.rotation;
                }
                else
                {
                    try
                    {
                        _rayStartPoses[1].position = _rightRayUpdatePositionAction.ReadValue<Vector3>();
                        _rayStartPoses[1].rotation = _rightRayUpdateRotationAction.ReadValue<Quaternion>();
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

        private void HandleSyncKeyboardLocation()
        {
            _keyboardTransform.position = _rayStartPoses[(int)_currentMovingFlag].position +
                                          _moveDistance * (_rayStartPoses[(int)_currentMovingFlag].rotation *
                                                           Vector3.forward) + (_keyboardTransform.position -
                                                                               _moveWidget.transform.position);
            _keyboardTransform.rotation = _rayStartPoses[(int)_currentMovingFlag].rotation;   
        }
        
        private void UpdateKeyboardVisible()
        {
            bool isShow = _imeManager.IsShow();
            if (_isShow != isShow)
            {
                _moveWidget.SetActive(isShow);
                _imeManager.SetKeyboardVisible(isShow);
                _isShow = isShow;
                if (isShow)
                {
                    _showCallBack?.Invoke();
                }
                else
                { 
                    _hideCallBack?.Invoke();
                }
            }
        }
    }
}
