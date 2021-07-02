// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using System.Collections;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.CurvyEditor.Generator;
using FluffyUnderware.Curvy.Controllers;
using FluffyUnderware.DevToolsEditor;

namespace FluffyUnderware.CurvyEditor
{

    public static class CurvyMenu
    {
        #region ### Tools Menu ###
        #endregion

        #region ### GameObject Menu ###

        [MenuItem("GameObject/Curvy/Spline", false, 0)]
        public static void CreateCurvySpline(MenuCommand cmd)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0 && cmd.context == selectedGameObjects[0])
                Selection.activeObject = null;
            CurvySpline spl=CreateCurvyObjectAsChild<CurvySpline>(cmd.context,"Spline");
            DTSelection.AddGameObjects(spl);
        }

        [MenuItem("GameObject/Curvy/UI Spline",false,1)]
        public static void CreateCurvyUISpline(MenuCommand cmd)
        {
            GameObject parent=cmd.context as GameObject;
            if (!parent || parent.GetComponentInParent<Canvas>() == null)
            {
                Canvas cv=GameObject.FindObjectOfType<Canvas>();
                if (cv)
                parent = cv.gameObject;
                else
                    parent = new GameObject("Canvas", typeof(Canvas));
            }

            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0 && cmd.context == selectedGameObjects[0])
                Selection.activeObject = null;

            CurvyUISpline uiSpline;
            {
                const string gameObjectName = "UI Spline";
                uiSpline = CurvyUISpline.CreateUISpline(gameObjectName);
                GameObjectUtility.SetParentAndAlign(uiSpline.gameObject, parent as GameObject);
                Undo.RegisterCreatedObjectUndo(uiSpline.gameObject, "Create " + gameObjectName);
            }
           
            DTSelection.AddGameObjects(uiSpline);
        }

        [MenuItem("GameObject/Curvy/Generator", false, 5)]
        public static void CreateCG(MenuCommand cmd)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0 && cmd.context == selectedGameObjects[0])
                Selection.activeObject = null;
            CurvyGenerator cg = CreateCurvyObjectAsChild<CurvyGenerator>(cmd.context, "Generator");
            DTSelection.AddGameObjects(cg);
        }
        [MenuItem("GameObject/Curvy/Controller/Spline", false, 10)]
        public static void CreateSplineController(MenuCommand cmd)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0 && cmd.context == selectedGameObjects[0])
                Selection.activeObject = null;
            SplineController ct = CreateCurvyObjectAsChild<SplineController>(cmd.context, "Controller");
            DTSelection.AddGameObjects(ct);
        }

        [MenuItem("GameObject/Curvy/Controller/CG Path", false, 12)]
        public static void CreatePathController(MenuCommand cmd)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0 && cmd.context == selectedGameObjects[0])
                Selection.activeObject = null;
            PathController ct = CreateCurvyObjectAsChild<PathController>(cmd.context, "Controller");
            DTSelection.AddGameObjects(ct);
        }

        [MenuItem("GameObject/Curvy/Controller/CG Volume", false, 13)]
        public static void CreateVolumeController(MenuCommand cmd)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0 && cmd.context == selectedGameObjects[0])
                Selection.activeObject = null;
            VolumeController ct = CreateCurvyObjectAsChild<VolumeController>(cmd.context, "Controller");
            DTSelection.AddGameObjects(ct);
        }

        [MenuItem("GameObject/Curvy/Controller/UI Text Spline", false, 14)]
        public static void CreateUITextSplineController(MenuCommand cmd)
        {
            GameObject[] selectedGameObjects = Selection.gameObjects;
            if (selectedGameObjects.Length > 0 && cmd.context == selectedGameObjects[0])
                Selection.activeObject = null;
            UITextSplineController ct = CreateCurvyObjectAsChild<UITextSplineController>(cmd.context, "Controller");
            DTSelection.AddGameObjects(ct);
        }

        #endregion

        #region ### Project window Create Menu ###

        [MenuItem("Assets/Create/Curvy/CG Module")]
        public static void CreatePCGModule()
        {
            ModuleWizard.Open();
        }

        [MenuItem("Assets/Create/Curvy/Shape")]
        public static void CreateShape()
        {
            ShapeWizard.Open();
        }

        #endregion

        public static T CreateCurvyObject<T>(Object parent, string name) where T : MonoBehaviour
        {
            GameObject go = parent as GameObject;
            if (go == null)
            {
                go = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(go, "Create " + name);
            }
            
            T obj = go.AddComponent<T>();
            Undo.RegisterCreatedObjectUndo(obj, "Create " + name);
  
            return obj;
        }

        public static T CreateCurvyObjectAsChild<T>(Object parent, string name) where T : MonoBehaviour
        {
            GameObject go = new GameObject(name);
            T obj = go.AddComponent<T>();
            GameObjectUtility.SetParentAndAlign(go, parent as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + name);

            return obj;
        }
    }
}
