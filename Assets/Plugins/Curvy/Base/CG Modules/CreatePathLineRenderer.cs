// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using FluffyUnderware.DevTools;
using FluffyUnderware.Curvy.Utils;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Create/Path Line Renderer", ModuleName = "Create Path Line Renderer", Description = "Feeds a Line Renderer with a Path")]
    [HelpURL(CurvySpline.DOCLINK + "cgcreatepathlinerenderer")]
    public class CreatePathLineRenderer : CGModule
    {

        [HideInInspector]
        [InputSlotInfo(typeof(CGPath), DisplayName = "Rasterized Path")]
        public CGModuleInputSlot InPath = new CGModuleInputSlot();

        #region ### Serialized Fields ###

        public LineRenderer LineRenderer
        {
            get
            {
                if (mLineRenderer == null)
                    mLineRenderer = GetComponent<LineRenderer>();
                return mLineRenderer;
            }
        }

        #endregion

        LineRenderer mLineRenderer;

        #region ### Unity Callbacks ###
        /*! \cond UNITY */
        protected override void Awake()
        {
            base.Awake();
            createLR();
        }
        /*! \endcond */
        #endregion

        #region ### Module Overrides ###

        public override void Refresh()
        {
            base.Refresh();
            CGPath path = InPath.GetData<CGPath>();
#if UNITY_5_6_OR_NEWER
            if (path != null)
            {
                LineRenderer.positionCount = path.Position.Length;
                LineRenderer.SetPositions(path.Position);
            }
            else
                LineRenderer.positionCount = 0;
#else
            if (path != null)
            {
                LineRenderer.numPositions = path.Position.Length;
                for (int v = 0; v < path.Position.Length; v++)
                    LineRenderer.SetPosition(v, path.Position[v]);
            }
            else
                LineRenderer.numPositions = 0;
#endif

        }

        // Called when a module's state changes (Link added/removed, Active toggles etc..)
        //public override void OnStateChange()
        //{
        //    base.OnStateChange();
        //}

        // Called after a module was copied to a template
        //public override void OnTemplateCreated() 
        //{
        //	base.OnTemplateCreated();
        //}


        #endregion

        void createLR()
        {
            if (LineRenderer == null)
            {
                mLineRenderer = gameObject.AddComponent<LineRenderer>();
                mLineRenderer.useWorldSpace = false;
#if UNITY_5_6_OR_NEWER
                mLineRenderer.textureMode = LineTextureMode.Tile;
#endif
                mLineRenderer.sharedMaterial = CurvyUtility.GetDefaultMaterial();
            }
        }

    }
}
