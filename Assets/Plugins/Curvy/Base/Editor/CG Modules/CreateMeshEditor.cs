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
using FluffyUnderware.DevToolsEditor;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CustomEditor(typeof(CreateMesh))]
    public class CreateMeshEditor : CGModuleEditor<CreateMesh>
    {
        public override void OnModuleDebugGUI()
        {
            base.OnModuleDebugGUI();
            if (Target){
                EditorGUILayout.LabelField("Meshes: "+Target.MeshCount.ToString());
                EditorGUILayout.LabelField("Vertices: " + Target.VertexCount.ToString());
            }
        }

        protected override void OnReadNodes()
        {
            base.OnReadNodes();
            Node.FindTabBarAt("Default").AddTab("Export", onExportTab);
        }

        void onExportTab(DTInspectorNode node) 
        {
            GUI.enabled = Target.MeshCount > 0;
            if (GUILayout.Button("Save To Scene"))
                Target.SaveToScene();
            if (GUILayout.Button("Save Mesh Asset(s)"))
            {
                string file = EditorUtility.SaveFilePanelInProject("Save Assets", Target.ModuleName, "mesh", "Save Mesh(es) as").Replace(".mesh","");
                if (!string.IsNullOrEmpty(file))
                {
                    List<Component> res;
                    List<string> names;
                    Target.GetManagedResources(out res, out names);
                    for (int i = 0; i < res.Count; i++)
                    {
                        string resFile = string.Format("{0}{1}.asset", file, i);
                        AssetDatabase.DeleteAsset(resFile);
                        AssetDatabase.CreateAsset(Instantiate(res[i].GetComponent<MeshFilter>().sharedMesh), resFile);
                    }
                    AssetDatabase.Refresh();
                }
            }
            GUI.enabled = true;
        }

    }
}
