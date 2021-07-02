// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using FluffyUnderware.DevTools;
using System.Collections.Generic;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Input/Game Objects",ModuleName="Input GameObjects", Description="")]
    [HelpURL(CurvySpline.DOCLINK + "cginputgameobject")]
    public class InputGameObject : CGModule
    {

        [HideInInspector]
        [OutputSlotInfo(typeof(CGGameObject),Array=true)]
        public CGModuleOutputSlot OutGameObject = new CGModuleOutputSlot();

        #region ### Serialized Fields ###

        [ArrayEx]
        [SerializeField]
        List<CGGameObjectProperties> m_GameObjects = new List<CGGameObjectProperties>();
        
        #endregion

        #region ### Public Properties ###

        public List<CGGameObjectProperties> GameObjects
        {
            get { return m_GameObjects; }
        }

        public bool SupportsIPE
        {
            get { return false; }
        }
        
        #endregion

        #region ### Private Fields & Properties ###
        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */


#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Dirty = true;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            GameObjects.Clear();
            Dirty = true;
        }

        /*! \endcond */
        #endregion

        #region ### Public Methods ###

        public override void Refresh()
        {
            base.Refresh();
            //OutVMesh
            if (OutGameObject.IsLinked)
            {
                CGGameObject[] data = new CGGameObject[GameObjects.Count];
                int total = 0;
                for (int i = 0; i < GameObjects.Count; i++)
                {
                    if (GameObjects[i] != null)
                    {
                        data[total++] = new CGGameObject(GameObjects[i]);
                    }
                }
                System.Array.Resize(ref data, total);
                OutGameObject.SetData(data);
            }
        }

        public override void OnTemplateCreated()
        {
            base.OnTemplateCreated();
            GameObjects.Clear();
        }
        
        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */


        /*! \endcond */
        #endregion


	
    }
}
