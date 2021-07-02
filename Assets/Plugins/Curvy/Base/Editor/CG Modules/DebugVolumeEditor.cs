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
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CustomEditor(typeof(DebugVolume))]
    public class DebugVolumeEditor : CGModuleEditor<DebugVolume>
    {

        protected override void OnEnable()
        {
            base.OnEnable();
            HasDebugVisuals = true;
            ShowDebugVisuals = true;
        }

        protected override void OnReadNodes()
        {
            base.OnReadNodes();
            Node.FindTabBarAt("Default").AddTab("Cross Groups",OnCrossGroupTab);
        }

        public override void OnModuleSceneDebugGUI()
        {
            base.OnModuleSceneDebugGUI();
            CGVolume vol = Target.InData.GetData<CGVolume>();
            if (vol)
            {
                Handles.matrix = Target.Generator.transform.localToWorldMatrix;

                if (Target.ShowPathSamples)
                {
                    CGEditorUtility.SceneGUIPlot(vol.Position, 0.1f, Target.PathColor);
                    if (Target.ShowIndex)
                    {
                        string[] labels = Enumerable.Range(0, vol.Count).Select(i => i.ToString()).ToArray();
                        CGEditorUtility.SceneGUILabels(vol.Position, labels, Color.black,Vector2.zero);
                    }

                    if (Target.ShowNormals)
                    {
                        DTHandles.PushHandlesColor(Target.NormalColor);

                        for (int i = 0; i < vol.Count; i++)
                            Handles.DrawLine(vol.Position[i], vol.Position[i] + vol.Normal[i] * 2);

                        DTHandles.PopHandlesColor();
                    }
                }
                if (Target.ShowCrossSamples)
                {
                    int vtLo=Target.LimitCross.From*vol.CrossSize;
                    int vtHi = vtLo + vol.CrossSize;
                    if (!Target.LimitCross.SimpleValue)
                    {
                        vtLo = Target.LimitCross.Low * vol.CrossSize;
                        vtHi = (Target.LimitCross.High + 1) * vol.CrossSize;
                    }

                    vtLo = Mathf.Clamp(vtLo, 0, vol.VertexCount);
                    vtHi = Mathf.Clamp(vtHi, vtLo, vol.VertexCount);
                    Vector3[] range = vol.Vertex.SubArray(vtLo, vtHi - vtLo);
                    CGEditorUtility.SceneGUIPlot(range, 0.1f, Color.white);
                    
                    if (Target.ShowIndex)
                    {
                        string[] labels = Enumerable.Range(vtLo, vtHi).Select(i => i.ToString()).ToArray();
                        CGEditorUtility.SceneGUILabels(range, labels, Color.black,Vector2.zero);
                    }

                    if (Target.ShowMap)
                    {
                        string[] labels = Enumerable.Range(vtLo, vtHi).Select(i => DTMath.SnapPrecision(vol.CrossMap[i],3).ToString()).ToArray();
                        CGEditorUtility.SceneGUILabels(range, labels, new Color(1, 0, 1), new Vector2(10, 20));
                    }

                    if (Target.ShowNormals)
                    {
                        DTHandles.PushHandlesColor(Target.NormalColor);

                        for (int i = vtLo; i < vtHi; i++)
                            Handles.DrawLine(vol.Vertex[i], vol.Vertex[i] + vol.VertexNormal[i] * 2);

                        DTHandles.PopHandlesColor();
                    }
                }
                if (Target.Interpolate)
                {
                    Vector3 pos;
                    Vector3 tan;
                    Vector3 up;
                    vol.InterpolateVolume(Target.InterpolatePathF,Target.InterpolateCrossF, out pos, out tan, out up);
#if UNITY_5_6_OR_NEWER
                    Handles.ConeHandleCap(0, pos, Quaternion.LookRotation(up, tan), 1f,EventType.Repaint);
#else
                    Handles.ConeCap(0, pos, Quaternion.LookRotation(up,tan), 1f);
#endif
                }
                Handles.matrix = Matrix4x4.identity;
            }
        }

        public override void OnModuleDebugGUI()
        {
            CGVolume vol = Target.InData.GetData<CGVolume>();
            if (vol)
            {
                EditorGUILayout.LabelField("VertexCount: " + vol.VertexCount.ToString());
            }
        }

        void OnCrossGroupTab(DTInspectorNode node)
        {
            CGVolume vol = Target.InData.GetData<CGVolume>();
            if (vol)
            {
                GUILayout.Label("MaterialGroup.Patch: (MaterialID) Patch Details", EditorStyles.boldLabel);
                for (int i = 0; i < vol.CrossMaterialGroups.Count; i++)
                {
                    for (int p = 0; p < vol.CrossMaterialGroups[i].Patches.Count; p++)
                        GUILayout.Label(string.Format("{0}.{1}: (Mat:{2}) {3}", i, p, vol.CrossMaterialGroups[i].MaterialID,vol.CrossMaterialGroups[i].Patches[p].ToString()));
                }
            }
        }

        protected override void OnCustomInspectorGUI()
        {
            DebugVMeshEditor.CheckGeneratorDebugMode(Target);
            base.OnCustomInspectorGUI();
        }

    }
}
