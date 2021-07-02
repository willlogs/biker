// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using System.Collections;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Modifier/TRS Path", ModuleName = "TRS Path", Description = "Transform,Rotate,Scale a Path")]
    [HelpURL(CurvySpline.DOCLINK + "cgtrspath")]
#pragma warning disable 618
    public class ModifierTRSPath : TRSModuleBase, IOnRequestPath
#pragma warning restore 618
    {
        [HideInInspector]
        [InputSlotInfo(typeof(CGPath), Name = "Path A", ModifiesData = true)]
        public CGModuleInputSlot InPath = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGPath))]
        public CGModuleOutputSlot OutPath = new CGModuleOutputSlot();



        #region ### Public Properties ###

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
                    var scaleLessMatrix = ApplyTrsOnShape(data);
                    for (int i = 0; i < data.Count; i++)
                        data.Direction[i] = scaleLessMatrix.MultiplyVector(data.Direction[i]);
                }
                result = new CGData[1] { data };
            }
            else
                result = null;

            return result;
        }
    }

    #endregion




}
