using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace com.vivo.codelibrary
{

    public class UIPanelManager
    {
        static Transform _dynamicScreenCanvas;

        /// <summary>
        /// 2D动态节点
        /// </summary>
        public static Transform DynamicScreenCanvas
        {
            get
            {
                if (_dynamicScreenCanvas == null)
                {
                    _dynamicScreenCanvas = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/Canvas/DynamicScreenCanvas")).transform;
                    _dynamicScreenCanvas.GetComponent<CanvasScaler>().enabled = false;
                    _dynamicScreenCanvas.GetComponent<GraphicRaycaster>().enabled = true;
                }
                return _dynamicScreenCanvas;
            }
        }

        static Transform uiPanelRoot;

        /// <summary>
        /// 界面根节点
        /// </summary>
        public static Transform UIPanelRoot
        {
            get
            {
                if (uiPanelRoot == null)
                {
                    uiPanelRoot = GameObject.Find("Canvas").transform;
                    if (uiPanelRoot == null)
                    {
                        uiPanelRoot = CreateCanvas().transform;
                    }
                }
                return uiPanelRoot;
            }
        }

        /// <summary>
        /// 创建父物体
        /// </summary>
        /// <param name="objName"></param>
        /// <param name="followUIEnum"></param>
        /// <returns></returns>
        static Canvas CreateCanvas()
        {
            GameObject obj = new GameObject("ControllerUiRoot");
            Canvas c = obj.gameObject.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.pixelPerfect = false;
            c.sortingOrder = 1000;
            c.targetDisplay = 0;
            CanvasScaler cs = obj.gameObject.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
            cs.scaleFactor = 1;
            cs.referencePixelsPerUnit = 100;
            GraphicRaycaster gr = obj.gameObject.AddComponent<GraphicRaycaster>();
            gr.ignoreReversedGraphics = true;
            gr.blockingObjects = GraphicRaycaster.BlockingObjects.None;
            return c;
        }

        static Dictionary<string, UIPanelBase> panels = new Dictionary<string, UIPanelBase>();

        /// <summary>
        /// 显示界面
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static UIPanelBase ShowPanel(string str)
        {
            UIPanelBase findData = null;
            bool find = false;
            if (panels.TryGetValue(str, out findData))
            {
                find = true;
            }
            if (findData != null)
            {
                findData.Show();
                return findData;
            }
            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(str));
            obj.transform.SetParent(UIPanelRoot, false);
            UIPanelBase newData = obj.GetComponent<UIPanelBase>();
            if (newData != null)
            {
                newData.Show();
                if (find)
                {
                    panels[str] = newData;
                }
                else
                {
                    panels.Add(str, newData);
                }
                return newData;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 隐藏界面
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static void HidePanel(string str)
        {
            UIPanelBase findData = null;
            if (panels.TryGetValue(str, out findData))
            {
                if (findData != null)
                {
                    findData.Hide();
                }
                else
                {
                    panels.Remove(str);
                }
            }
        }

        /// <summary>
        /// 查找面板实例
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static UIPanelBase FindPanel(string str)
        {
            UIPanelBase findData = null;
            panels.TryGetValue(str, out findData);
            return findData;
        }

        #region//飘字

        static FloatUIPanel floatUIPanel;

        /// <summary>
        /// 显示飘字提示
        /// </summary>
        /// <param name="_str">飘字内容</param>
        /// <param name="_pos">飘字出生位置 世界坐标</param>
        public static void ShowFloatText(string _str, Vector3 _pos)
        {
            if (floatUIPanel == null)
            {
                floatUIPanel = (FloatUIPanel)ShowPanel(UIPanelPath.FloatUIPanel);
            }
            floatUIPanel.Show();
            FreshFloatUIPanelIndex();
            floatUIPanel.ShowTip(_str, _pos);
        }

        /// <summary>
        /// 显示飘字提示 随机屏幕位置(1：上 2：下 3：左 4：右  5:中)
        /// </summary>
        /// <param name="_str"></param>
        public static void ShowFloatText(string _str)
        {
            ShowFloatText(_str, UnityEngine.Random.Range(1, 6));
        }

        /// <summary>
        /// 显示飘字提示
        /// </summary>
        /// <param name="_str">飘字内容</param>
        /// <param name="_posMod">飘字出生屏幕位置 1：上 2：下 3：左 4：右  5:中</param>
        public static void ShowFloatText(string _str, int _posMod)
        {
            if (floatUIPanel == null)
            {
                floatUIPanel = (FloatUIPanel)ShowPanel(UIPanelPath.FloatUIPanel);
            }
            FreshFloatUIPanelIndex();
            floatUIPanel.ShowTip(_str, _posMod);
        }

        /// <summary>
        /// 显示飘字提示
        /// </summary>
        /// <param name="_str">飘字内容</param>
        /// <param name="_worldTarget">飘字跟随的世界物体目标</param>
        public static void ShowFloatText(string _str, Transform _worldTarget)
        {
            if (floatUIPanel == null)
            {
                floatUIPanel = (FloatUIPanel)ShowPanel(UIPanelPath.FloatUIPanel);
            }
            FreshFloatUIPanelIndex();
            floatUIPanel.ShowTip(_str, _worldTarget);
        }

        static void FreshFloatUIPanelIndex()
        {
            if (floatUIPanel == null) return;
            UIPanelBase loadingPanel = FindPanel(UIPanelPath.LoadingPanel);
            int panelIndex = 10000;
            if (loadingPanel != null)
            {
                panelIndex = loadingPanel.transform.GetSiblingIndex();
            }
            floatUIPanel.transform.SetSiblingIndex(panelIndex);
            if (loadingPanel != null)
            {
                ((LoadingPanel)loadingPanel).SupperShow();
            }
        }

        #endregion

        #region//对话框

        static DialogPanel dialogPanel;

        /// <summary>
        /// 显示对话框
        /// </summary>
        /// <param name="_str">内容</param>
        /// <param name="yesButtonName">确定按钮明</param>
        /// <param name="_yesAction">确定按钮点击回调</param>
        /// <param name="_noButtonName">取消按钮明</param>
        /// <param name="_noAction">取消按钮点击回调</param>
        public static void ShowDialog(string _str, string _yesButtonName = "确定", System.Action _yesAction = null,
            string _noButtonName = "取消", System.Action _noAction = null)
        {
            if (dialogPanel == null)
            {
                dialogPanel = (DialogPanel)ShowPanel(UIPanelPath.DialogPanel);
            }
            dialogPanel.Show();
            dialogPanel.Dialog(_str, _yesButtonName, _yesAction, _noButtonName, _noAction);
        }

        #endregion
    }
}

