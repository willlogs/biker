// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy.Generator;
using System.Collections.Generic;
using FluffyUnderware.Curvy.Generator.Modules;
using System.Linq;
using FluffyUnderware.DevToolsEditor;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CustomEditor(typeof(BuildVolumeMesh))]
    public class BuildVolumeMeshEditor : CGModuleEditor<BuildVolumeMesh>
    {
        bool showAddButton;
        int matcount;

        public override void OnModuleDebugGUI()
        {
            CGVMesh vmesh = Target.OutVMesh.GetData<CGVMesh>();
            if (vmesh)
            {
                EditorGUILayout.LabelField("Vertices: " + vmesh.Count.ToString());
                EditorGUILayout.LabelField("Triangles: " + vmesh.TriangleCount.ToString());
                EditorGUILayout.LabelField("SubMeshes: " + vmesh.SubMeshes.Length.ToString());
            }
        }

        protected override void OnReadNodes()
        {
            ensureMaterialTabs();
        }

        void ensureMaterialTabs()
        {
            DTGroupNode tabbar = Node.FindTabBarAt("Default");
            
            if (tabbar == null)
                return;

            tabbar.MaxItemsPerRow = 4;
            for (int i = 0; i < Target.MaterialCount; i++)
            {
                string tabName = string.Format("Mat {0}", i);
                if (tabbar.Count <= i + 1)
                    tabbar.AddTab(tabName, OnRenderTab);
                else
                {
                    tabbar[i + 1].Name = tabName;
                    tabbar[i + 1].GUIContent.text = tabName;
                }
            }
            while (tabbar.Count > Target.MaterialCount+1)
                tabbar[tabbar.Count - 1].Delete();
            matcount = Target.MaterialCount;
        }

        void OnRenderTab(DTInspectorNode node)
        {
            int idx = node.Index-1;
            
            if (idx >= 0 && idx < Target.MaterialCount)
            {
                CGMaterialSettingsEx mat = Target.MaterialSetttings[idx];
                EditorGUI.BeginChangeCheck();
                mat.MaterialID = EditorGUILayout.IntField(new GUIContent("Material ID", "As defined in the Control Points"), mat.MaterialID);
                mat.SwapUV = EditorGUILayout.Toggle("Swap UV", mat.SwapUV);
                bool b = mat.KeepAspect != CGKeepAspectMode.Off;
                b=EditorGUILayout.Toggle(new GUIContent("Keep Aspect","Keep proportional texel size?"),b);
                mat.KeepAspect = (b) ? CGKeepAspectMode.ScaleV : CGKeepAspectMode.Off;
                mat.UVOffset = EditorGUILayout.Vector2Field("UV Offset", mat.UVOffset);
                mat.UVScale = EditorGUILayout.Vector2Field("UV Scale", mat.UVScale);
                
                Target.SetMaterial(idx, EditorGUILayout.ObjectField("Material", Target.GetMaterial(idx), typeof(Material), true) as Material);
                if (Target.MaterialCount > 1 && GUILayout.Button("Remove"))
                {
                    Target.RemoveMaterial(idx);
                    node.Delete();
                    ensureMaterialTabs();
                    GUIUtility.ExitGUI();
                }
                if (EditorGUI.EndChangeCheck())
                {
                    Target.Dirty = true;
                    EditorUtility.SetDirty(Target);
                }
            }
        }

        void CBAddMaterial()
        {
            if (DTGUI.IsLayout)
                showAddButton = Node.FindTabBarAt("Default").SelectedIndex == 0;
            if (showAddButton)
            {
                if (GUILayout.Button("Add Material Group"))
                {
                    Target.AddMaterial();
                    ensureMaterialTabs();
                    GUIUtility.ExitGUI();
                }
            }
            
        }

        protected override void OnCustomInspectorGUI()
        {
            if (matcount != Target.MaterialCount)
                ensureMaterialTabs();
        }
    }
}
