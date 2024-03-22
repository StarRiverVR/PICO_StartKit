using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.vivo.codelibrary
{
    public class UIPanelBaseModel : Model<UIPanelBaseModel, UIPanelBaseControl>
    {
        public override void Initialization()
        {
            base.Initialization();
        }

        public enum MSG
        {
            ShowAll,
            HideAll,
        }
    }
}


