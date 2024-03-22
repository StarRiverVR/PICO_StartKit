using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace com.vivo.codelibrary
{
    public abstract class View<TControl, TModel> : MonoBehaviour where TControl : Control<TControl> where TModel : Model<TModel, TControl>
    {
        /// <summary>
        /// 销毁监听
        /// </summary>
        protected abstract void RemoveListens();

        #region//MVC公共数据和控制中心 针对于所有继承自View的

        /// <summary>
        /// 控制中心
        /// </summary>
        protected ControlBase<Control<MVCControl>> mvcControl;

        /// <summary>
        /// 数据中心  MVC
        /// </summary>
        protected MVCModel mvcModel;

        #endregion

        #region//专有数据和控制中心  针对于所有继承自View<TControl, TModel>的

        /// <summary>
        /// 控制中心
        /// </summary>
        protected ControlBase<Control<TControl>> control;

        /// <summary>
        /// 数据中心
        /// </summary>
        protected TModel model;

        #endregion

        protected Transform _transform;

        protected RectTransform _rectTransform;

        protected GameObject _gameObject;

        protected virtual void Awake()
        {
            ViewInit();
        }

        protected virtual void OnDestroy()
        {
            isViewInit = false;
            _transform = null;
            control = null;
            model = null;
            mvcControl = null;
            mvcModel = null;
            _gameObject = null;
            _rectTransform = null;
            RemoveListens();
        }

        bool isViewInit = false;

        void ViewInit()
        {
            if (isViewInit)
            {
                return;
            }
            isViewInit = true;

            _transform = transform;
            _gameObject = gameObject;
            _rectTransform = gameObject.GetComponent<RectTransform>();

            mvcControl = Control<MVCControl>.Instance.ThisControl;
            mvcModel = (MVCModel)MVCModel.Instance;

            control = Control<TControl>.Instance.ThisControl;
            model = (TModel)(Model<TModel, TControl>.Instance);
        }
    }

}



