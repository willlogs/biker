// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.ImportExport;
using FluffyUnderware.DevToolsEditor;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace FluffyUnderware.CurvyEditor
{
    /// <summary>
    /// A workaround to the Unity Json's class not being able to serialize top level arrays. Including such arrays in another object avoids the issue.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SerializableArray<T>
    {
        public T[] Array;
    }

    /// <summary>
    /// A window that allows exporting and importing splines as Json files
    /// </summary>
    public class ImportExportWizard : EditorWindow
    {
        /// <summary>
        /// Json version of the imported or exported splines
        /// </summary>
        private string serializedText = string.Empty;

        /// <summary>
        /// Limit used to avoid the "String too long for TextMeshGenerator. Cutting off characters." error.
        /// </summary>
        private const int MaxDisplayedSerializedTextLength = 15000;
        /// <summary>
        /// serializedText copy that iqs used in the UI display. Truncated if too long to avoid Unity error.
        /// </summary>
        private string displayedSerializedText = string.Empty;
        /// <summary>
        /// Defines if which coordinates should be read/written
        /// </summary>
        private CurvySerializationSpace coordinateSpace = CurvySerializationSpace.Global;

        private Vector2 scrollingPosition;

        private IDTInspectorNodeRenderer GUIRenderer;
        private DTGroupNode configurationGroup;
        private DTGroupNode actionsGroup;
        private DTGroupNode advancedActionsGroup;

        static public void Open()
        {
            ImportExportWizard win = GetWindow<ImportExportWizard>(true, "Import/Export splines");
            win.minSize = new Vector2(350, 340);
        }

        private void OnDisable()
        {
            DTSelection.OnSelectionChange -= Repaint;
        }

        private void OnEnable()
        {
            const string docLinkId = "import_export";

            GUIRenderer = new DTInspectorNodeDefaultRenderer();

            configurationGroup = new DTGroupNode("Configuration") { HelpURL = CurvySpline.DOCLINK + docLinkId };
            actionsGroup = new DTGroupNode("Actions") { HelpURL = CurvySpline.DOCLINK + docLinkId };
            advancedActionsGroup = new DTGroupNode("Advanced Actions") { HelpURL = CurvySpline.DOCLINK + docLinkId };

            DTSelection.OnSelectionChange += Repaint;
        }

        private void OnGUI()
        {
            List<CurvySpline> selectedSplines = Selection.GetFiltered(typeof(CurvySpline), SelectionMode.ExcludePrefab).Where(o => o != null).Select(o => (CurvySpline)o).ToList();

            //actions
            bool export = false;
            bool import = false;
            bool readFromSelection = false;
            bool writeToSelection = false;
            bool readFromFile = false;
            bool writeToFile = false;
            string editedString = null;

            //Display window and read user commands
            {

                GUI.skin.label.wordWrap = true;
                GUILayout.Label("This window allows you to import/export splines from/to JSON text.");

                DTInspectorNode.IsInsideInspector = false;

                //Configuration
                GUIRenderer.RenderSectionHeader(configurationGroup);
                if (configurationGroup.ContentVisible)
                {
                    coordinateSpace = (CurvySerializationSpace)EditorGUILayout.EnumPopup("Coordinate space to use", coordinateSpace, GUILayout.Width(280));
                }
                GUIRenderer.RenderSectionFooter(configurationGroup);


                //Actions
                GUIRenderer.RenderSectionHeader(actionsGroup);
                if (actionsGroup.ContentVisible)
                {
                    GUI.enabled = selectedSplines.Count > 0;
                    export = GUILayout.Button("Export selected spline(s)");

                    GUI.enabled = true;
                    import = GUILayout.Button("Import");
                }
                GUIRenderer.RenderSectionFooter(actionsGroup);


                //Advanced actions
                GUIRenderer.RenderSectionHeader(advancedActionsGroup);
                if (advancedActionsGroup.ContentVisible)
                {
                    GUI.enabled = selectedSplines.Count > 0;
                    readFromSelection = GUILayout.Button("Read selected spline(s)");

                    GUI.enabled = true;
                    readFromFile = GUILayout.Button("Read from file");

                    GUI.enabled = string.IsNullOrEmpty(serializedText) == false;
                    writeToSelection = GUILayout.Button("Write new spline(s)");

                    writeToFile = GUILayout.Button("Write to file");

                    bool textTooLong = IsDisplayedTextTruncated();
                    bool textCloseToBeTooLong = serializedText.Length >= MaxDisplayedSerializedTextLength * 0.9f;

                    GUI.enabled = true;

                    if (textTooLong)
                        EditorGUILayout.HelpBox(String.Format("Your text reached the limit of {0} characters. Current characters count is {1}. This limit is to avoid Unity's \"String too long for TextMeshGenerator\" error. Text will be truncated and it's edition disabled.", MaxDisplayedSerializedTextLength, serializedText.Length), MessageType.Warning);
                    else if (textCloseToBeTooLong)
                        EditorGUILayout.HelpBox(String.Format("Your text is close to reach the limit of {0} characters. Current characters count is {1}. This limit is to avoid Unity's \"String too long for TextMeshGenerator\" error. If the limit is reached, text will be truncated and it's edition disabled.", MaxDisplayedSerializedTextLength, serializedText.Length), MessageType.Warning);

                    scrollingPosition = EditorGUILayout.BeginScrollView(scrollingPosition, GUILayout.MaxHeight(position.height - 100));
                    EditorGUI.BeginChangeCheck();
                    GUI.enabled = textTooLong == false;
                    string modifiedString = EditorGUILayout.TextArea(displayedSerializedText, EditorStyles.textArea, GUILayout.ExpandHeight(true));
                    if (textTooLong == false && GUI.changed)
                        editedString = modifiedString;
                    EditorGUI.EndChangeCheck();
                    EditorGUILayout.EndScrollView();
                }
                GUIRenderer.RenderSectionFooter(advancedActionsGroup);
                GUILayout.Space(5);

                if (configurationGroup.NeedRepaint || actionsGroup.NeedRepaint || advancedActionsGroup.NeedRepaint)
                    Repaint();

                if (readFromFile || readFromSelection)
                    GUI.FocusControl(null); //Keeping the focus prevents the textfield from refreshing
            }

            if (export)
            {
                readFromSelection = true;
                writeToFile = true;
            }

            if (import)
            {
                readFromFile = true;
                writeToSelection = true;
            }

            ProcessCommands(selectedSplines, readFromSelection, readFromFile, editedString, writeToSelection, writeToFile);
        }


        private bool IsDisplayedTextTruncated()
        {
            return serializedText.Length >= MaxDisplayedSerializedTextLength;
        }

        private void ProcessCommands([NotNull]List<CurvySpline> selectedSplines, bool readFromSelection, bool readFromFile, [CanBeNull]string editedString, bool writeToSelection, bool writeToFile)
        {


            if (readFromSelection || readFromFile || editedString != null)
            {
                if (readFromSelection || readFromFile)
                {
                    string rawJson;
                    {
                        if (readFromSelection)
                        {
                            if (selectedSplines.Count > 0)
                            {
                                SerializedCurvySpline[] serializedSplines = selectedSplines.Select(s => new SerializedCurvySpline(s, coordinateSpace)).ToArray();
                                rawJson = JsonUtility.ToJson(new SerializableArray<SerializedCurvySpline> { Array = serializedSplines }, true);
                            }
                            else
                                throw new InvalidOperationException("Serialize Button should not be clickable when something other than splines is selected");
                        }
                        else
                        {
                            string fileToLoadFullName = EditorUtility.OpenFilePanel("Select file to load", Application.dataPath, "");
                            if (String.IsNullOrEmpty(fileToLoadFullName))//Happens when user cancel the file selecting window
                                rawJson = displayedSerializedText;
                            else
                                rawJson = File.ReadAllText(fileToLoadFullName);
                        }
                    }

                    serializedText = rawJson;
                }
                else
                    serializedText = editedString;

                displayedSerializedText = IsDisplayedTextTruncated()
                    ? serializedText.Substring(0, MaxDisplayedSerializedTextLength)
                    : serializedText;
            }

            if (writeToSelection)
            {
                
                SerializedCurvySpline[] serializedSplines;
                //The following deserializes the JSON text, but instead of doing with a simple and nice one line of code, it is done in a complex way. The reason to that is that JsonUtility doesn't handle default values for JSON fields.
                {
                    //First we deserialize the JSON in the sole goal to know how much elements there are in the arrays
                    SerializableArray<SerializedCurvySpline> serializableArray = JsonUtility.FromJson<SerializableArray<SerializedCurvySpline>>(serializedText);

                    //Knowing the number of array elements, we assign a new instance for each element. By creating the new instances ourselves, through the constructor, we have control on the default value of fields
                    for (int index = 0; index < serializableArray.Array.Length; index++)
                    {
                        int controlPointsCount = serializableArray.Array[index].ControlPoints.Length;

                        SerializedCurvySpline splineWithCorrectDefaultValue = new SerializedCurvySpline();
                        splineWithCorrectDefaultValue.ControlPoints = new SerializedCurvySplineSegment[controlPointsCount];
                        for (int controlPointIndex = 0; controlPointIndex < controlPointsCount; controlPointIndex++)
                        {
                            splineWithCorrectDefaultValue.ControlPoints[controlPointIndex] = new SerializedCurvySplineSegment();
                        }

                        serializableArray.Array[index] = splineWithCorrectDefaultValue;
                    }

                    //Then, through FromJsonOverwrite, we overwrite the fields that are existing in the JSON text
                    JsonUtility.FromJsonOverwrite(serializedText, serializableArray);

                    serializedSplines = serializableArray.Array;
                }
                

                foreach (SerializedCurvySpline spline in serializedSplines)
                {
                    CurvySpline deserializedSpline = CurvySpline.Create();
#if UNITY_EDITOR
                    Undo.RegisterCreatedObjectUndo(deserializedSpline.gameObject, "Deserialize");
#endif
                    deserializedSpline.transform.SetParent(Selection.activeTransform);
                    spline.WriteIntoSpline(deserializedSpline, coordinateSpace);
                }
            }
            else if (writeToFile)
            {
                string file = EditorUtility.SaveFilePanel("Save to...", Application.dataPath, String.Format("Splines_{0}.json", DateTime.Now.ToString("yyyy-MMMM-dd HH_mm")), "json");
                if (!string.IsNullOrEmpty(file))
                {
                    File.WriteAllText(file, serializedText);
                    AssetDatabase.Refresh();
                }
            }
        }
    }
}
