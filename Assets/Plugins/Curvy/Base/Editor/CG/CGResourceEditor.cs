// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy.Generator;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevToolsEditor;

namespace FluffyUnderware.CurvyEditor.Generator
{

    public static class CGResourceEditorHandler
    {
        static Dictionary<string, System.Type> Editors = new Dictionary<string, System.Type>();

        internal static CGResourceEditor GetEditor(string resourceName, Component resource)
        {
            if (Editors.Count == 0)
                loadEditors();
            if (Editors.ContainsKey(resourceName))
            {
                return (CGResourceEditor)System.Activator.CreateInstance(Editors[resourceName], (object)resource);
            }
            return null;
        }

        static void loadEditors()
        {
            Editors.Clear();
            Type[] types = TypeExt.GetLoadedTypes();
            foreach (Type T in types)
            {
                object[] at = T.GetCustomAttributes(typeof(ResourceEditorAttribute), true);
                if (at.Length > 0)
                    Editors.Add(((ResourceEditorAttribute)at[0]).ResourceName, T);
            }
        }
    }

    public class CGResourceEditor
    {
        protected Component Resource { get; private set; }

        public CGResourceEditor() { }

        public CGResourceEditor(Component resource)
        {
            Resource = resource;
        }

        /// <summary>
        /// Resource GUI
        /// </summary>
        /// <returns>true if changes were made</returns>
        public virtual bool OnGUI() { return false; }

        public static implicit operator bool(CGResourceEditor a)
        {
            return !object.ReferenceEquals(a, null);
        }
    }

    [ResourceEditor("Mesh")]
    public class CGMeshResourceGUI : CGResourceEditor
    {

        public CGMeshResourceGUI(Component resource) : base(resource)
        {
        }


    }

    [ResourceEditor("Spline")]
    public class CGSplineResourceGUI : CGResourceEditor
    {

        public CGSplineResourceGUI(Component resource)
            : base(resource)
        {
        }
    }

    [ResourceEditor("Shape")]
    public class CGShapeResourceGUI : CGResourceEditor
    {
        CurvyShape2D mCurrentShape;

        string[] mMenuNames;
        int mSelection;
        bool mFreeform;

        public CGShapeResourceGUI(Component resource) : base(resource)
        {
            mCurrentShape = resource.GetComponent<CurvyShape2D>();
            mMenuNames = CurvyShape.GetShapesMenuNames((mCurrentShape) ? mCurrentShape.GetType() : null, out mSelection, true).ToArray();
            mFreeform = (mCurrentShape == null);
        }

        public override bool OnGUI()
        {
            bool dirty = false;

            bool b = GUILayout.Toggle(mFreeform, "Freeform");
            if (b != mFreeform)
            {
                if (b)
                {
                    mCurrentShape.Spline.ShowGizmos = true;
                    mCurrentShape.Delete();
                    mCurrentShape = null;
                    mFreeform = b;

                }
                else if (EditorUtility.DisplayDialog("Warning", "The current shape will be irreversible replaced. Are you sure?", "Ok", "Cancel"))
                {
                    mFreeform = b;
                    mCurrentShape = (CurvyShape2D)Resource.gameObject.AddComponent(CurvyShape.GetShapeType(mMenuNames[mSelection]));
                    mCurrentShape.Dirty = true;
                    mCurrentShape.Spline.ShowGizmos = false;
                }
            }
            if (!mFreeform)
            {
                int sel = EditorGUILayout.Popup(mSelection, mMenuNames);
                if (sel != mSelection)
                {
                    mSelection = sel;
                    dirty = true;
                    if (mCurrentShape)
                        mCurrentShape.Delete();
                    mCurrentShape = (CurvyShape2D)Resource.gameObject.AddComponent(CurvyShape.GetShapeType(mMenuNames[mSelection]));
                    mCurrentShape.Dirty = true;

                }
                if (mCurrentShape)
                {
#if UNITY_2017_4_OR_NEWER
                    using (SerializedObject so = new SerializedObject(mCurrentShape))
#else
                    var so = new SerializedObject(mCurrentShape);
#endif
                    {
                        SerializedProperty prop = so.GetIterator();

                        bool enterChildren = true;

                        while (prop.NextVisible(enterChildren))
                        {
                            switch (prop.name)
                            {
                                case "m_Script":
                                case "InspectorFoldout":
                                case "m_Plane":
                                    //case "m_Persistent":
                                    break;
                                default:
                                    EditorGUILayout.PropertyField(prop);
                                    break;
                            }
                            enterChildren = false;
                        }
                        dirty = dirty || so.ApplyModifiedProperties();
                    }
                }
            }

            return dirty;
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ResourceEditorAttribute : System.Attribute
    {
        public readonly string ResourceName;

        public ResourceEditorAttribute(string resName)
        {
            ResourceName = resName;
        }
    }


}
