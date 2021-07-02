// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using FluffyUnderware.Curvy.Generator;
using System.Collections.Generic;
using FluffyUnderware.DevToolsEditor.Extensions;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.CurvyEditor.Generator
{
    [CustomEditor(typeof(CurvyGenerator))]
    public class CurvyGeneratorEditor : CurvyEditorBase<CurvyGenerator>
    {
        protected override void OnCustomInspectorGUI()
        {
            GUILayout.Space(5);
            if (Target)
                EditorGUILayout.HelpBox("# of Modules: " + Target.Modules.Count.ToString(), MessageType.Info);
        }

        public override void OnInspectorGUI()
        {
            bool showInspector;
#if UNITY_2018_3_OR_NEWER // New prefab system
            showInspector = true; //With the new prefab system (Unity 2018.3) prefabs don't show inspector, and when opening prefab editor, its objects are of type PrefabAssetType.NotAPrefab, so no way to know if its from prefab or not?
#else
            showInspector = IsPrefab == false;
#endif

            if (showInspector == false)
            {
                EditorGUILayout.HelpBox("Curvy Generator Template", MessageType.Info);
            }
            else
            {
                GUILayout.BeginHorizontal(GUILayout.Height(24));
                if (GUILayout.Button(new GUIContent(CurvyStyles.OpenGraphTexture, "Edit Graph")))
                    CGGraph.Open(Target);

                if (GUILayout.Button(new GUIContent(CurvyStyles.DeleteTexture, "Clear Graph"), GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(true)) && EditorUtility.DisplayDialog("Clear", "Clear graph?", "Yes", "No"))
                    Target.Clear();
                GUILayout.EndHorizontal();

                base.OnInspectorGUI();
            }
        }
    }
}
