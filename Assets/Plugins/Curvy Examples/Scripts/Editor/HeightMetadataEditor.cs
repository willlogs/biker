// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.Curvy.Examples;

namespace FluffyUnderware.Curvy.ExamplesEditor
{
    
    [CustomEditor(typeof(HeightMetadata))]
    public class HeightMetadataEditor : DTEditor<HeightMetadata>
    {
        
        [DrawGizmo(GizmoType.Active|GizmoType.NonSelected|GizmoType.InSelectionHierarchy)]
        static void GizmoDrawer(HeightMetadata data, GizmoType context)
        {
            if (CurvyGlobalManager.ShowMetadataGizmo && data.Spline.ShowGizmos)
            {
                Vector3 p = data.ControlPoint.transform.position;
                p.y += HandleUtility.GetHandleSize(data.ControlPoint.transform.position) * 0.3f;
                Handles.Label(p, data.MetaDataValue.ToString(), DTStyles.BackdropHtmlLabel);
            }
        }
    }
}
