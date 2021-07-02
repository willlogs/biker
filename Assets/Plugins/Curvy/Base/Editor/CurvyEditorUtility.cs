// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using FluffyUnderware.Curvy;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.DevTools;
using Application = UnityEngine.Application;

namespace FluffyUnderware.CurvyEditor
{

    public static class CurvyEditorUtility
    {
        [Obsolete]
        public static string HelpURL(string comp)
        {
            if (string.IsNullOrEmpty(comp))
                return string.Empty;
            return CurvySpline.DOCLINK + "legacy";
        }

        public static void SendBugReport()
        {
            string par = string.Format("@Operating System@={0}&@Unity Version@={1}&@Curvy Version@={2}", SystemInfo.operatingSystem, Application.unityVersion, CurvySpline.VERSION);
            Application.OpenURL(CurvySpline.WEBROOT + "bugreport?" + par.Replace(" ", "%20"));
        }

        public static void GenerateAssemblyDefinitions()
        {
            string parentDirectory = Directory.GetParent(GetCurvyRootPathAbsolute()).Parent.FullName;

            GenerateAssemblyDefinition(parentDirectory + "/" + @"LibTessDotNet/LibTessDotNet.asmdef",
            "{\n\t\"name\":\"LibTessDotNet\"\n}");
            GenerateAssemblyDefinition(parentDirectory + "/" + @"DevTools/FluffyUnderware.DevTools.asmdef",
            "{\n\t\"name\":\"FluffyUnderware.DevTools\"\n}");
            GenerateAssemblyDefinition(parentDirectory + "/" + @"DevTools/Editor/FlufyUnderware.DevTools.Editor.asmdef",
            "{\n\"name\":\"FluffyUnderware.DevTools.Editor\",\n\"references\":[\n\"FluffyUnderware.DevTools\"\n],\n\"includePlatforms\":[\n\"Editor\"\n],\n\"excludePlatforms\":[]\n}");
            GenerateAssemblyDefinition(parentDirectory + "/" + @"Curvy/ToolBuddy.Curvy.asmdef",
            "{\n\"name\":\"ToolBuddy.Curvy\",\n\"references\":[\n\"FluffyUnderware.DevTools\",\n\"LibTessDotNet\"\n],\n\"includePlatforms\":[],\n\"excludePlatforms\":[]\n}");
            GenerateAssemblyDefinition(parentDirectory + "/" + @"Curvy/Base/Editor/ToolBuddy.Curvy.Editor.asmdef",
            "{\n\"name\":\"ToolBuddy.Curvy.Editor\",\n\"references\":[\n\"ToolBuddy.Curvy\",\n\"FluffyUnderware.DevTools\",\n\"FluffyUnderware.DevTools.Editor\",\n\"LibTessDotNet\"\n],\n\"includePlatforms\":[\n\"Editor\"\n],\n\"excludePlatforms\":[]\n}");
            GenerateAssemblyDefinition(parentDirectory + "/" + @"Curvy Examples/ToolBuddy.Curvy.Examples.asmdef",
            "{\n\"name\":\"ToolBuddy.Curvy.Examples\",\n\"references\":[\n\"FluffyUnderware.DevTools\",\n\"ToolBuddy.Curvy\"\n],\n\"includePlatforms\":[],\n\"excludePlatforms\":[]\n}");
            GenerateAssemblyDefinition(parentDirectory + "/" + @"Curvy Examples/Scripts/Editor/ToolBuddy.Curvy.Examples.Editor.asmdef",
            "{\n\"name\":\"ToolBuddy.Curvy.Examples.Editor\",\n\"references\":[\n\"FluffyUnderware.DevTools\",\n\"FluffyUnderware.DevTools.Editor\",\n\"ToolBuddy.Curvy\",\n\"ToolBuddy.Curvy.Editor\",\n\"ToolBuddy.Curvy.Examples\"\n],\n\"includePlatforms\":[\n\"Editor\"\n],\n\"excludePlatforms\":[]\n}");

            AssetDatabase.Refresh();
        }

        private static void GenerateAssemblyDefinition(string filePath, string fileContent)
        {
            DirectoryInfo directory = Directory.GetParent(filePath);
            if (Directory.Exists(directory.FullName) == false)
                EditorUtility.DisplayDialog("Missing directory",
                    String.Format("Could not find the directory '{0}', file generation will be skipped", directory.FullName), "Continue");
            else if (!File.Exists(filePath) || EditorUtility.DisplayDialog("Replace File?", String.Format("The file '{0}' already exists! Replace it?", filePath), "Yes", "No"))
                using (StreamWriter streamWriter = File.CreateText(filePath))
                {
                    streamWriter.WriteLine(fileContent);
                }
        }


        /// <summary>
        /// Converts a path/file relative to Curvy's root path to the real path, e.g. "ReadMe.txt" gives "Curvy/ReadMe.txt"
        /// </summary>
        /// <param name="relativePath">a path/file inside the Curvy package, WITHOUT the leading Curvy</param>
        /// <returns>the real path, relative to Assets</returns>
        public static string GetPackagePath(string relativePath)
        {
            return GetCurvyRootPath() + relativePath.TrimStart('/', '\\');
        }
        /// <summary>
        /// Converts a path/file relative to Curvy's root path to the real absolute path
        /// </summary>
        /// <param name="relativePath">a path/file inside the Curvy package, WITHOUT the leading Curvy</param>
        /// <returns>the absolute system path</returns>
        public static string GetPackagePathAbsolute(string relativePath)
        {
            return Application.dataPath + "/" + GetPackagePath(relativePath);
        }

        /// <summary>
        /// Gets the Curvy folder relative path, e.g. "Plugins/Curvy/" by default
        /// </summary>
        /// <returns></returns>
        public static string GetCurvyRootPath()
        {
            // Quick check for the regular path
            if (File.Exists(Application.dataPath + "/Plugins/Curvy/Base/CurvySpline.cs"))
                return "Plugins/Curvy/";

            // Still no luck? Do a project search
            string[] guid = AssetDatabase.FindAssets("curvyspline");
            if (guid.Length == 0)
            {
                DTLog.LogError("[Curvy] Unable to locate CurvySpline.cs in the project! Is the Curvy package fully imported?");
                return null;
            }
            else
                return AssetDatabase.GUIDToAssetPath(guid[0]).TrimStart("Assets/").TrimEnd("Base/CurvySpline.cs");
        }

        /// <summary>
        /// Gets the Curvy folder absolute path, i.e. Application.dataPath+"/"+CurvyEditorUtility.GetCurvyRootPath()
        /// </summary>
        /// <returns></returns>
        public static string GetCurvyRootPathAbsolute()
        {
            return Application.dataPath + "/" + GetCurvyRootPath();
        }
    }

    public static class CurvyGUI
    {

        #region ### GUI Controls ###

        public static bool Foldout(ref bool state, string text) { return Foldout(ref state, new GUIContent(text), null); }
        public static bool Foldout(ref bool state, string text, string helpURL) { return Foldout(ref state, new GUIContent(text), helpURL); }

        public static bool Foldout(ref bool state, GUIContent content, string helpURL, bool hierarchyMode = true)
        {
            Rect controlRect = GUILayoutUtility.GetRect(content, CurvyStyles.Foldout);
            bool isInsideInspector = DTInspectorNode.IsInsideInspector;
            int xOffset = isInsideInspector ? 12 : -2;
            controlRect.x -= xOffset;
            controlRect.width += (isInsideInspector ? 0 : 1);

            int indentLevel = DTInspectorNodeDefaultRenderer.RenderHeader(controlRect, xOffset, helpURL, content, ref state);

            EditorGUI.indentLevel = indentLevel;

            return state;
        }

        #endregion

    }
}


