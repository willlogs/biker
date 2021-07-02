// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.Curvy.Generator.Modules;
using System.Linq;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CustomEditor(typeof(DebugVMesh))]
    public class DebugVMeshEditor : CGModuleEditor<DebugVMesh>
    {

        protected override void OnEnable()
        {
            base.OnEnable();
            HasDebugVisuals = true;
            ShowDebugVisuals = true;
        }

        public override void OnModuleSceneDebugGUI()
        {
            CGVMesh vmesh = Target.InData.GetData<CGVMesh>();
            if (vmesh)
            {
                Handles.matrix = Target.Generator.transform.localToWorldMatrix;
                if (Target.ShowVertices)
                    CGEditorUtility.SceneGUIPlot(vmesh.Vertex, 0.1f, Color.gray);

                if (Target.ShowVertexID)
                {
                    string[] labels = Enumerable.Range(0, vmesh.Count).Select(i => i.ToString()).ToArray();
                    CGEditorUtility.SceneGUILabels(vmesh.Vertex, labels, Color.black,Vector2.zero);
                }
                if (Target.ShowUV && vmesh.HasUV)
                {
                    string[] labels = Enumerable.Range(0, vmesh.UV.Length - 1).Select(i => string.Format("({0:0.##},{1:0.##})", vmesh.UV[i].x, vmesh.UV[i].y)).ToArray();
                    CGEditorUtility.SceneGUILabels(vmesh.Vertex, labels, Color.black,Vector2.zero);
                }
                Handles.matrix = Matrix4x4.identity;
            }
        }

        public override void OnModuleDebugGUI()
        {
            CGVMesh vmesh = Target.InData.GetData<CGVMesh>();
            if (vmesh)
            {
                EditorGUILayout.LabelField("VertexCount: " + vmesh.Count.ToString());
            }
        }

        protected override void OnCustomInspectorGUI()
        {
            CheckGeneratorDebugMode(Target);
            base.OnCustomInspectorGUI();
        }

        /// <summary>
        /// Clears all existing UI messages and adds one if the generator debug mode is not active
        /// </summary>
        internal static void CheckGeneratorDebugMode(CGModule module)
        {
            module.UIMessages.Clear();
            if (module.Generator.ShowDebug == false)
                module.UIMessages.Add("To display the debug information, activate the Generator's Debug mode either via its toolbar, or its inspector");
        }
    }
}
