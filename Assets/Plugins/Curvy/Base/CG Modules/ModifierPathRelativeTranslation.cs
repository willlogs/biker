// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    /// <summary>
    /// Translates a path relatively to it's direction, instead of relatively to the world as does the TRS Path module.
    /// </summary>
    [ModuleInfo("Modifier/Path Relative Translation", ModuleName = "Path Relative Translation", Description = "Translates a path relatively to it's direction, instead of relatively to the world as does the TRS Path module.")]
    [HelpURL(CurvySpline.DOCLINK + "cgpathrelativetranslation")]
#pragma warning disable 618
    public class ModifierPathRelativeTranslation : CGModule, IOnRequestPath
#pragma warning restore 618
    {
        [HideInInspector]
        [InputSlotInfo(typeof(CGPath), Name = "Path A", ModifiesData = true)]
        public CGModuleInputSlot InPath = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGPath))]
        public CGModuleOutputSlot OutPath = new CGModuleOutputSlot();


        #region ### Serialized Fields ###

        /// <summary>
        /// The translation amount
        /// </summary>
        [SerializeField]
        [Tooltip("The translation amount")]
        private float lateralTranslation;
        #endregion

        #region ### Public Properties ###

        /// <summary>
        /// The translation amount
        /// </summary>
        public float LateralTranslation
        {
            get { return lateralTranslation; }
            set
            {
                if (lateralTranslation != value)
                {
                    lateralTranslation = value;
                    Dirty = true;
                }
            }
        }

        public bool PathIsClosed
        {
            get
            {
                return (IsConfigured) ? InPath.SourceSlot().PathProvider.PathIsClosed : false;
            }
        }

        #endregion

        #region ### IOnRequestProcessing ###

        public CGData[] OnSlotDataRequest(CGModuleInputSlot requestedBy, CGModuleOutputSlot requestedSlot, params CGDataRequestParameter[] requests)
        {
            CGData[] result;
            if (requestedSlot == OutPath)
            {
                CGPath data = InPath.GetData<CGPath>(requests);
                if (data)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        Vector3 translation = Vector3.Cross(data.Normal[i], data.Direction[i]) * lateralTranslation;
                        data.Position[i].x = data.Position[i].x + translation.x;
                        data.Position[i].y = data.Position[i].y + translation.y;
                        data.Position[i].z = data.Position[i].z + translation.z;
                    }
                }
                result = new CGData[1] { data };
            }
            else
                result = null;

            return result;
        }

        #endregion


        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        protected override void OnEnable()
        {
            base.OnEnable();
            Properties.MinWidth = 250;
            Properties.LabelWidth = 165;
        }

        public override void Reset()
        {
            base.Reset();
            LateralTranslation = 0;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            LateralTranslation = lateralTranslation;
            Dirty = true;
        }
#endif

        /*! \endcond */
        #endregion
    }
}
