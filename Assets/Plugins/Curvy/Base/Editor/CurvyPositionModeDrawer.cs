// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using FluffyUnderware.Curvy;
using UnityEngine;
using UnityEditor;

namespace FluffyUnderware.CurvyEditor.Controllers
{
    [CustomPropertyDrawer(typeof(CurvyPositionMode))]
    public class CurvyPositionModeDrawer : PropertyDrawer
    {
        readonly GUIContent[] options = new[] { new GUIContent("Relative", "Position is expressed as a fraction of a spline: 0 meaning the spline start, 1 meaning the spline end."), new GUIContent("Absolute", "Position is expressed as world units") };
        readonly GUIStyle guiStyle = EditorStyles.popup;

        override public void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            property.intValue = EditorGUI.Popup(position, label, property.intValue, options, guiStyle);
            EditorGUI.EndProperty();
        }
    }
}
