// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.CurvyEditor
{
    [CustomEditor(typeof(CurvySpline)), CanEditMultipleObjects]
    public class CurvySplineEditor : CurvyEditorBase<CurvySpline>
    {

        SerializedProperty tT;
        SerializedProperty tC;
        SerializedProperty tB;


        GUIStyle mUserValuesLabel;

        protected override void OnEnable()
        {
            base.OnEnable();

            tT = serializedObject.FindProperty("m_Tension");
            tC = serializedObject.FindProperty("m_Continuity");
            tB = serializedObject.FindProperty("m_Bias");
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override void OnSceneGUI()
        {
            // Bounds
            if (CurvyGlobalManager.Gizmos.HasFlag(CurvySplineGizmos.Bounds))
            {
                DTHandles.PushHandlesColor(new Color(0.3f, 0, 0));
                DTHandles.WireCubeCap(Target.Bounds.center, Target.Bounds.size);
                DTHandles.PopHandlesColor();
            }
            Handles.BeginGUI();

            mUserValuesLabel = new GUIStyle(EditorStyles.boldLabel);
            mUserValuesLabel.normal.textColor = Color.green;


            if (CurvyGlobalManager.Gizmos.HasFlag(CurvySplineGizmos.Labels))
            {
                GUIStyle stLabel = new GUIStyle(EditorStyles.boldLabel);
                stLabel.normal.textColor = Color.white;
                Handles.Label(Target.transform.position - new Vector3(-0.5f, 0.2f, 0), Target.name, stLabel);
                stLabel.normal.textColor = Color.red;
                foreach (CurvySplineSegment cp in Target.ControlPointsList)
                    Handles.Label(cp.transform.position + new Vector3(-0.5f, HandleUtility.GetHandleSize(cp.transform.position) * 0.35f, 0), cp.name, stLabel);
            }

            Handles.EndGUI();
            // Snap Transform
            if (Target && DT._UseSnapValuePrecision)
            {
                Target.transform.localPosition = DTMath.SnapPrecision(Target.transform.localPosition, 3);
                Target.transform.localEulerAngles = DTMath.SnapPrecision(Target.transform.localEulerAngles, 3);
            }
        }



        void TCBOptionsGUI()
        {
            if (Target.Interpolation == CurvyInterpolation.TCB)
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(new GUIContent("Set Catmul", "Set TCB to match Catmul Rom")))
                {
                    tT.floatValue = 0; tC.floatValue = 0; tB.floatValue = 0;
                }
                if (GUILayout.Button(new GUIContent("Set Cubic", "Set TCB to match Simple Cubic")))
                {
                    tT.floatValue = -1; tC.floatValue = 0; tB.floatValue = 0;
                }
                if (GUILayout.Button(new GUIContent("Set Linear", "Set TCB to match Linear")))
                {
                    tT.floatValue = 0; tC.floatValue = -1; tB.floatValue = 0;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        void ShowGizmoGUI()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShowGizmos"));
        }

        void CBCheck2DPlanar()
        {
            if (Target.RestrictTo2D && Target.IsPlanar(CurvyPlane.XY) == false)
            {
                EditorGUILayout.HelpBox("The spline isn't planar! Click the button below to correct this!", MessageType.Warning);
                if (GUILayout.Button("Make planar"))
                    Target.MakePlanar(CurvyPlane.XY);
            }
        }

        protected override void OnCustomInspectorGUI()
        {
            base.OnCustomInspectorGUI();
            GUILayout.Space(5);
            if (Target.ControlPointsList.Count == 0)
                EditorGUILayout.HelpBox("To add Control Points to your curve, please use the Toolbar in the SceneView window!", MessageType.Warning);
            if (Target.IsInitialized == false)
                EditorGUILayout.HelpBox("Spline is not initialized", MessageType.Info);
            else if (Target.Dirty)
                EditorGUILayout.HelpBox("Spline is dirty" , MessageType.Info);
            else
                EditorGUILayout.HelpBox("Control Points: " + Target.ControlPointCount.ToString() +
                                       "\nSegments: " + Target.Count.ToString() +
                                       "\nLength: " + Target.Length.ToString() +
                                       "\nCache Points: " + Target.CacheSize.ToString()
                                       , MessageType.Info);

        }



    }


}
