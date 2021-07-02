// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.Curvy.Generator.Modules;
using FluffyUnderware.DevToolsEditor;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CustomEditor(typeof(DebugRasterizedPath))]
    public class DebugRasterizedPathEditor : CGModuleEditor<DebugRasterizedPath>
    {

        protected override void OnEnable()
        {
            base.OnEnable();
            HasDebugVisuals = true;
            ShowDebugVisuals = true;
        }

        // Scene View GUI - Called only if the module is initialized and configured
        //public override void OnModuleSceneGUI() {}

        // Scene View Debug GUI - Called only when Show Debug Visuals is activated
        public override void OnModuleSceneDebugGUI()
        {
            if (Target.ShowNormals || Target.ShowOrientation)
            {
                CGPath path = Target.InPath.GetData<CGPath>();
                if(path)
                {
                    Color gizmoOrientationColor = CurvyGlobalManager.GizmoOrientationColor;
                    Color gizmoTangentColor = CurvySplineSegment.GizmoTangentColor;

                    if (Target.ShowOrientation)
                    {
                        DTHandles.PushHandlesColor(gizmoOrientationColor);

                        for (int i = 0; i < path.Count; i++)
                            Handles.DrawLine(path.Position[i], path.Position[i] + path.Direction[i] * 2);

                        DTHandles.PopHandlesColor();
                    }

                    if (Target.ShowNormals)
                    {
                        DTHandles.PushHandlesColor(gizmoTangentColor);

                        for (int i = 0; i < path.Count; i++)
                            Handles.DrawLine(path.Position[i], path.Position[i] + path.Normal[i] * 2);

                        DTHandles.PopHandlesColor();
                    }
                }
            }
        }

        public override void OnModuleDebugGUI()
        {
            CGPath path = Target.InPath.GetData<CGPath>();
            if (path)
            {
                EditorGUILayout.LabelField("VertexCount: " + path.Count.ToString());
            }
        }

        // Inspector Debug GUI - Called only when Show Debug Values is activated
        //public override void OnModuleDebugGUI() {}

        protected override void OnCustomInspectorGUI()
        {
            DebugVMeshEditor.CheckGeneratorDebugMode(Target);
            base.OnCustomInspectorGUI();
        }
    }
}