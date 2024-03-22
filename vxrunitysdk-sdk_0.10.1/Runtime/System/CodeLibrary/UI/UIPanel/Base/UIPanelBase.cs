using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    /// <summary>
    /// MVC View 框架 UiPanel
    /// </summary>
    /// <typeparam name="TControl">控制中心</typeparam>
    /// <typeparam name="TModel">数据中心</typeparam>
    public class UIPanelBase2<TControl, TModel> : View<TControl, TModel> where TControl : Control<TControl> where TModel : Model<TModel, TControl>
    {
        #region//UIPanelBase公共数据和控制中心  针对于所有继承自UIPanelBase的

        /// <summary>
        /// 控制中心
        /// </summary>
        protected ControlBase<Control<UIPanelBaseControl>> publicControl;

        /// <summary>
        /// 数据中心  MVC
        /// </summary>
        protected UIPanelBaseModel publicModel;

        #endregion

        //专有数据和控制中心  针对于所有继承自UIPanelBase<TControl, TModel>的
        //base.control
        //base.model

        protected override void Awake()
        {
            base.Awake();
            BaseInit();
        }

        bool isBaseInit = false;

        void BaseInit()
        {
            if (isBaseInit)
            {
                return;
            }
            isBaseInit = true;

            publicControl = Control<UIPanelBaseControl>.Instance.ThisControl;
            publicModel = (UIPanelBaseModel)UIPanelBaseModel.Instance;

            //
            canvasGroup = transform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
            //注册监听，监控ExampleModel数据是否发生变动 公共
            publicControl.AddListen<UIPanelBaseModel.MSG>((int)UIPanelBaseModel.MSG.ShowAll, Show);
            publicControl.AddListen<UIPanelBaseModel.MSG>((int)UIPanelBaseModel.MSG.HideAll, Hide);
            //注册监听，监控ExampleModel数据是否发生变动 专有
            control.AddListen<UIPanelBase2<TControl, TModel>.MSG>((int)UIPanelBase2<TControl, TModel>.MSG.Show, Show);
            control.AddListen<UIPanelBase2<TControl, TModel>.MSG>((int)UIPanelBase2<TControl, TModel>.MSG.Hide, Hide);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            publicControl = null;
            publicModel = null;
            isBaseInit = false;
            canvasGroup = null;
            RemoveListens();
        }

        protected override void RemoveListens()
        {
            publicControl.RemoveListen<UIPanelBaseModel.MSG>((int)UIPanelBaseModel.MSG.ShowAll, Show);
            publicControl.RemoveListen<UIPanelBaseModel.MSG>((int)UIPanelBaseModel.MSG.HideAll, Hide);
            control.RemoveListen<UIPanelBase2<TControl, TModel>.MSG>((int)UIPanelBase2<TControl, TModel>.MSG.Show, Show);
            control.RemoveListen<UIPanelBase2<TControl, TModel>.MSG>((int)UIPanelBase2<TControl, TModel>.MSG.Hide, Hide);
        }

        protected CanvasGroup canvasGroup;

        public static void ShowAllPanel()
        {
            Control<UIPanelBaseControl>.Instance.ThisControl.Send<UIPanelBaseModel.MSG>((int)UIPanelBaseModel.MSG.ShowAll, true);
        }

        public static void HideAllPanel()
        {
            Control<UIPanelBaseControl>.Instance.ThisControl.Send<UIPanelBaseModel.MSG>((int)UIPanelBaseModel.MSG.HideAll, true);
        }

        public virtual void Show(params object[] objs)
        {
            BaseInit();
        }

        public virtual void Hide(params object[] objs)
        {
            BaseInit();
        }

        public enum MSG
        {
            Show,
            Hide,
        }
    }

    public class UIPanelBase : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;

        public virtual void Show()
        {
            InitCanvasGroup();
        }

        public virtual void Hide()
        {
            InitCanvasGroup();
        }

        bool isInitCanvasGroup = false;

        void InitCanvasGroup()
        {
            if (isInitCanvasGroup) return;
            isInitCanvasGroup = true;
            canvasGroup = transform.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            canvasGroup.alpha = 0;
        }
    }
}


