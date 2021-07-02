// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using UnityEditor;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevToolsEditor.Extensions;
using FluffyUnderware.Curvy.Generator;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.DevToolsEditor;
using UnityEditor.AnimatedValues;
using FluffyUnderware.DevTools;
using System.Reflection;

namespace FluffyUnderware.CurvyEditor.Generator
{
    public class CGGraph : EditorWindow
    {

        #region ### Static Properties ###

        public static CGModuleEditorBase InPlaceEditTarget;
        public static CGModuleEditorBase InPlaceEditInitiatedBy;

        /// <summary>
        /// Initiates an IPE session or terminates it
        /// </summary>
        public static void SetIPE(IExternalInput target = null, CGModuleEditorBase initiatedBy = null)
        {

            if (InPlaceEditTarget != null)
                InPlaceEditTarget.EndIPE();

            InPlaceEditInitiatedBy = initiatedBy;

            if (target != null)
            {
                InPlaceEditTarget = initiatedBy.Graph.GetModuleEditor((CGModule)target);

                if (SceneView.currentDrawingSceneView)
                    SceneView.currentDrawingSceneView.Focus();

                SyncIPE();
                InPlaceEditTarget.BeginIPE();
            }
        }

        /// <summary>
        /// Sets IPE target's TRS
        /// </summary>
        public static void SyncIPE()
        {
            if (InPlaceEditInitiatedBy != null && InPlaceEditTarget != null)
            {
                Vector3 pos;
                Quaternion rot;
                Vector3 scl;
                InPlaceEditInitiatedBy.OnIPEGetTRS(out pos, out rot, out scl);
                InPlaceEditTarget.OnIPESetTRS(pos, rot, scl);
            }
        }


        #endregion

        public CurvyGenerator Generator;
        public Dictionary<CGModule, CGModuleEditorBase> ModuleEditors = new Dictionary<CGModule, CGModuleEditorBase>();
        public Dictionary<System.Type, Color> TypeColors = new Dictionary<System.Type, Color>();


        List<CGModule> mModules;
        public List<CGModule> Modules
        {
            get
            {
                if (mModules == null)
                    mModules = new List<CGModule>(Generator.Modules.ToArray());
                return mModules;
            }
            set
            {
                mModules = value;
            }
        }

        internal CanvasState Canvas;
        public CanvasSelection Sel;
        internal CanvasUI UI;
        // Statusbar
        public DTStatusbar StatusBar = new DTStatusbar();
        int mStatusbarHeight = 20;
        int mModuleCount;
        bool mDoRepaint;
        /// <summary>
        /// True if the user clicked on the Reorder button
        /// </summary>
        bool mDoReorder;
        AnimBool mShowDebug = new AnimBool();

        Event EV { get { return Event.current; } }
        public bool LMB { get { return EV.type == EventType.MouseDown && EV.button == 0; } }
        public bool RMB { get { return EV.type == EventType.MouseDown && EV.button == 1; } }




        CGModule editTitleModule;

        void OnSelectionChange()
        {
            CurvyGenerator gen = null;
            List<CGModule> mod = DTSelection.GetAllAs<CGModule>();
            if (mod.Count > 0)
                gen = mod[0].Generator;
            if (gen == null)
                gen = DTSelection.GetAs<CurvyGenerator>();
            if (gen != null && (Generator == null || gen != Generator))
            {
                Initialize(gen);
                Repaint();
            }
            else
                if (mod.Count > 0 && CurvyProject.Instance.CGSynchronizeSelection)
            {
                Sel.Select(mod);
                Canvas.FocusSelection();
                Repaint();
            }


        }

        internal static CGGraph Open(CurvyGenerator generator)
        {
            generator.Initialize(true);
            CGGraph win = EditorWindow.GetWindow<CGGraph>("", true);
            win.Initialize(generator);
            win.wantsMouseMove = true;
            win.autoRepaintOnSceneChange = true;
            return win;
        }

        public void Initialize(CurvyGenerator generator)
        {
            destroyEditors();
            if (generator)
            {
                mShowDebug.speed = 3;
                mShowDebug.value = generator.ShowDebug;
                mShowDebug.valueChanged.RemoveAllListeners();
                mShowDebug.valueChanged.AddListener(Repaint);
#if UNITY_5_0 || UNITY_4_6
                title=generator.name;
#else
                titleContent.text = generator.name;
#endif
                Generator = generator;
                Generator.ArrangeModules();
                Sel.Clear();
                Show();
                if (Generator.Modules.Count == 0)
                    StatusBar.SetInfo("Welcome to the Curvy Generator! Right click or drag a CurvySpline on the canvas to get started!", "", 10);
                else
                    StatusBar.SetMessage(Generator.Modules.Count.ToString() + " modules loaded!", "", MessageType.None, 3);
            }
        }

        void OnDestroy()
        {
            destroyEditors();
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        void OnDisable()
        {
            SceneView.
#if UNITY_2019_1_OR_NEWER
duringSceneGui
#else
onSceneGUIDelegate
#endif
 -= OnSceneGUI;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged -= onPauseStateChanged;
#else
            EditorApplication.playmodeStateChanged -= OnStateChanged;
#endif
            SetIPE();
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        void OnEnable()
        {
            Canvas = new CanvasState(this);
            UI = new CanvasUI(this);
            Sel = new CanvasSelection(this);
            loadTypeColors();
            DTSceneView.
#if UNITY_2019_1_OR_NEWER
duringSceneGui
#else
onSceneGUIDelegate
#endif
 -= OnSceneGUI;
            SceneView.
#if UNITY_2019_1_OR_NEWER
duringSceneGui
#else
onSceneGUIDelegate
#endif
 += OnSceneGUI;
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged -= onPauseStateChanged;
#else
            EditorApplication.playmodeStateChanged -= OnStateChanged;
#endif
#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            EditorApplication.pauseStateChanged += onPauseStateChanged;
#else
            EditorApplication.playmodeStateChanged += OnStateChanged;
#endif
            //EditorApplication.update -= onUpdate;
            //EditorApplication.update += onUpdate;   
            autoRepaintOnSceneChange = true;
            wantsMouseMove = true;
            Undo.undoRedoPerformed -= OnUndoRedo;
            Undo.undoRedoPerformed += OnUndoRedo;
        }

        void OnUndoRedo()
        {
            if (Generator)
            {
                if (mModuleCount != Generator.GetComponentsInChildren<CGModule>().Length)
                {
                    Generator.Initialize(true);
                    Generator.Initialize(false);
                    Initialize(Generator);
                }
            }
        }

        void onUpdate()
        {
            /* THIS CAUSES NullRefException when rendering ReorderableList's:
            if (EditorApplication.isCompiling)
            {
                var eds = new List<CGModuleEditorBase>(ModuleEditors.Values);
                for (int i = eds.Count - 1; i >= 0; i--)
                    Editor.DestroyImmediate(eds[i]);
                ModuleEditors.Clear();
            }*/
        }

#if UNITY_2017_2_OR_NEWER
        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            OnStateChanged();
        }

        void onPauseStateChanged(PauseState state)
        {
            OnStateChanged();
        }
#endif

        void OnStateChanged()
        {
            destroyEditors();
            if (!Generator && Selection.activeGameObject)
            {
                Initialize(Selection.activeGameObject.GetComponent<CurvyGenerator>());
                Repaint();
            }
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (!Generator)
                return;
            for (int i = 0; i < Modules.Count; i++)
            {
                CGModule mod = Modules[i];
                if (mod != null && mod.IsInitialized && mod.IsConfigured && mod.Active)
                {
                    CGModuleEditorBase ed = GetModuleEditor(mod);
                    ed.OnModuleSceneGUI();
                    if (Generator.ShowDebug && ed.ShowDebugVisuals)
                        ed.OnModuleSceneDebugGUI();
                }
            }
        }

        Vector2 deltaAccu;


        void OnGUI()
        {

            mDoRepaint = false;
            if (!Generator)
                return;
            if (!Generator.IsInitialized)
            {
                Generator.Initialize();
            }


            Modules = new List<CGModule>(Generator.Modules.ToArray());
            mModuleCount = Modules.Count; // store count to be checked in window GUI

            if (!Application.isPlaying && !Generator.IsInitialized)
                Repaint();

            DrawToolbar();
            Canvas.SetClientRect(0, GUILayoutUtility.GetLastRect().yMax, 0, mStatusbarHeight);


            // Scrollable Canvas
            if (Canvas.Scroll.isAnimating)
                GUI.BeginScrollView(Canvas.ClientRect, Canvas.Scroll.value, Canvas.CanvasRect);
            else
                Canvas.Scroll.value = GUI.BeginScrollView(Canvas.ClientRect, Canvas.Scroll.value, Canvas.CanvasRect);


            // render background
            DTGUI.PushColor(Color.black.SkinAwareColor(true));
            GUI.Box(Canvas.ViewPort, "");
            DTGUI.PopColor();

#if CURVY_DEBUG
            
                GUILayout.BeginArea(Canvas.ViewPort);
                GUILayout.Label("Canvas ClientRect: " + Canvas.ClientRect);
                GUILayout.Label("Canvas Rect: " + Canvas.CanvasRect);
                GUILayout.Label("Canvas Scroll: " + Canvas.Scroll);
                GUILayout.Label("Canvas ViewPort: " + Canvas.ViewPort);

                GUILayout.Label("Mouse: " + EV.mousePosition);
                GUILayout.Label("IsWindowDrag: " + Canvas.IsWindowDrag);
                GUILayout.Label("IsSelectionDrag: " + Canvas.IsSelectionRectDrag);
                GUILayout.Label("IsLinkDrag: " + Canvas.IsLinkDrag);
                GUILayout.Label("IsCanvasDrag: " + Canvas.IsCanvasDrag);
                GUILayout.Label("IsModuleDrag: " + Canvas.IsModuleDrag);
                GUILayout.Label("MouseOverModule: " + Canvas.MouseOverModule);
                GUILayout.Label("MouseOverCanvas: " + Canvas.IsMouseOverCanvas);
                GUILayout.Label("SelectedLink: " + Sel.SelectedLink);
                GUILayout.Label("Selected Module: " + Sel.SelectedModule);
                GUILayout.EndArea();
#endif

            if (CurvyProject.Instance.CGShowHelp)
            {
                Rect r = new Rect(Canvas.ViewPort);
                r.x = r.width - 210;
                r.y = 10;
                r.width = 200;
                r.height = 200;

                GUILayout.BeginArea(r, GUI.skin.box);
                GUI.Label(new Rect(10, 5, 200, 20), "<b>General</b>", DTStyles.HtmlLabel);
                GUI.Label(new Rect(20, 25, 50, 20), "<b>RMB</b>", DTStyles.HtmlLabel);
                GUI.Label(new Rect(70, 25, 150, 20), "Context Menu", DTStyles.HtmlLabel);
                GUI.Label(new Rect(20, 40, 150, 40), "<b>MMB/\nSpace</b>", DTStyles.HtmlLabel);
                GUI.Label(new Rect(70, 40, 150, 20), "Drag Canvas", DTStyles.HtmlLabel);
                GUI.Label(new Rect(20, 70, 150, 20), "<b>Alt</b>", DTStyles.HtmlLabel);
                GUI.Label(new Rect(70, 70, 150, 20), "Hold to snap to grid", DTStyles.HtmlLabel);
                GUI.Label(new Rect(20, 85, 150, 20), "<b>Del</b>", DTStyles.HtmlLabel);
                GUI.Label(new Rect(70, 85, 150, 20), "Delete selection", DTStyles.HtmlLabel);


                GUI.Label(new Rect(10, 110, 200, 20), "<b>Add Modules</b>", DTStyles.HtmlLabel);
                GUI.Label(new Rect(20, 130, 180, 40), "Hold <b>Ctrl</b> while releasing a\nlink to create & connect", DTStyles.HtmlLabel);
                GUI.Label(new Rect(20, 160, 180, 40), "Drag & Drop splines to create\nPath module", DTStyles.HtmlLabel);



                GUILayout.EndArea();
            }

            DrawLinks();

            // Init and early catch some events
            Canvas.BeginGUI();

            DrawModules();

            Canvas.EndGUI();

            // Draw Selection

            DTGUI.PushBackgroundColor(Color.white);//.SkinAwareColor());
            foreach (CGModule mod in Sel.SelectedModules)
            {
                Rect selectionHighlightRectangle = mod.Properties.Dimensions.ScaleBy(2);
                if (DTEditorUtility.UsesNewEditorUI)
#pragma warning disable 162
                {
                    selectionHighlightRectangle.x -= 1;
                    selectionHighlightRectangle.y -= 1;
                    selectionHighlightRectangle.width += 2;
                    selectionHighlightRectangle.height += 1;
                }
#pragma warning restore 162
                GUI.Box(selectionHighlightRectangle, "", CurvyStyles.RoundRectangle);
            }
            DTGUI.PopBackgroundColor();

            // Keep dragged Module in view and handle multiselection move
            if (Canvas.IsModuleDrag && !DTGUI.IsLayout)
            {
                Vector2 delta = EV.delta;
                deltaAccu += EV.delta;
                if (EV.alt)
                {
                    delta = deltaAccu.Snap(CurvyProject.Instance.CGGraphSnapping);
                    if (delta == deltaAccu)
                        delta = Vector2.zero;
                }
                if (Sel.SelectedModules.Count > 1)
                {
                    foreach (CGModule mod in Sel.SelectedModules)
                    {
                        mod.Properties.Dimensions.position += delta;
                    }
                    if (!EV.alt || delta != Vector2.zero)
                        deltaAccu = Vector2.zero;
                }
                CGModule m = (Canvas.MouseOverModule) ? Canvas.MouseOverModule : Sel.SelectedModule;
                if (m)
                    GUI.ScrollTowards(m.Properties.Dimensions, 0.8f);
            }

            // Linking in progress?
            if (Canvas.IsLinkDrag)
            {
                Texture2D linkstyle = (Canvas.LinkDragFrom.OnRequestModule != null) ? CurvyStyles.RequestLineTexture : CurvyStyles.LineTexture;
                Handles.DrawBezier(Canvas.LinkDragFrom.Origin, EV.mousePosition, Canvas.LinkDragFrom.Origin + new Vector2(40, 0), EV.mousePosition + new Vector2(-40, 0), Color.white, linkstyle, 2);
            }

            GUI.EndScrollView(true);

            // Selection
            if (Canvas.IsSelectionRectDrag)
                DrawSelectionRect();

            // Statusbar
            DrawStatusbar();

            // IPE
            SyncIPE();

            mDoRepaint = mDoRepaint || Canvas.IsCanvasDrag || Canvas.IsLinkDrag || Canvas.IsSelectionRectDrag || EV.type == EventType.MouseMove || mShowDebug.isAnimating || Canvas.Scroll.isAnimating;


            // Disable Title edit mode?
            if (editTitleModule != null)
            {
                if ((EV.isKey && (EV.keyCode == KeyCode.Escape || EV.keyCode == KeyCode.Return)) ||
                    Sel.SelectedModule != editTitleModule
                    )
                {
                    editTitleModule = null;
                    //GUI.FocusControl("");
                    mDoRepaint = true;
                }
            }

            if (mDoReorder)
                Generator.ReorderModules();
            if (mDoRepaint)
                Repaint();
        }



        void DrawModules()
        {
            //TODO at some point this method should be reworked to distinguish between what should be called when Event.current.type == EventType.Layout and what should be called otherwise
            const int refreshHighlightSize = 9;

            //When modules are culled, they are not rendered (duh) and thus their height is not updated. This is ok as long as the height is constant. When there is some module expanding/collapsing, the height should change. In those cases, we disable the culling (for all modules for implementation simplicity sake, could be optimized) so the height is updated, so that the modules reordering code can work based on the actual height, and not the preculling one, which was leading to bad reordering results.
            bool animationIsHappening = mShowDebug.isAnimating || Modules.Exists(m => m.Properties.Expanded.isAnimating);

            CGModule curSel = Sel.SelectedModule;
            // Begin drawing Module Windows
            BeginWindows();
            for (int i = 0; i < Modules.Count; i++)
            {
                CGModule mod = Modules[i];
                if (mod != null)
                {
                    mod.Properties.Dimensions.width = Mathf.Max(mod.Properties.Dimensions.width, mod.Properties.MinWidth);

                    //This is based on the condition at which mod.Properties.Expanded.target is modified in OnModuleWindowCB
                    bool autoModuleDetailsWillMakeModuleAnimate = DTGUI.IsLayout && CurvyProject.Instance.CGAutoModuleDetails && mod.Properties.Expanded.target != (mod == curSel);

                    // Render title
                    string title = mod.ModuleName;
                    if (!mod.IsConfigured)
                        title = string.Format("<color={0}>{1}</color>", new Color(1, 0.2f, 0.2f).SkinAwareColor().ToHtml(), mod.ModuleName);
                    //"<color=#ff3333>" + mod.ModuleName + "</color>"; 
                    else if (mod is IOnRequestProcessing)
                        title = string.Format("<color={0}>{1}</color>", CurvyStyles.IOnRequestProcessingTitleColor.SkinAwareColor().ToHtml(), mod.ModuleName);

#if CURVY_DEBUG
                    title = mod.UniqueID + ":" + title;
#endif

                    // the actual window
                    Vector2 oldPos = mod.Properties.Dimensions.position;

                    bool shouldDraw;
                    {
                        //Ok, shit gets complicated here. The idea was to not draw modules that are out of the screen, but for reasons I don't fully grasp, the height can be 0, which fucks up the boundaries test.
                        //The height is set to 0 in the line just before calling GUILayout.Window, with the apparent goal for that method to update the height, but this update does not happen all the time. It happens when the OnGUI method is called following an Event of type Repaint, but does not happen when is called following an Event of type Layout.
                        //And if you remove the code setting height to 0, the height of the module is not updated correctly
                        if (mod.Properties.Dimensions.height == 0 || animationIsHappening || autoModuleDetailsWillMakeModuleAnimate)
                            shouldDraw = true;
                        else
                        {
                            Rect testedBoundaries = mod.Properties.Dimensions.ScaleBy(refreshHighlightSize);
                            shouldDraw = Canvas.ViewPort.Contains(testedBoundaries.min) || Canvas.ViewPort.Overlaps(testedBoundaries);
                        }
                    }

                    Rect newWindowRect;
                    if (shouldDraw)
                    {
                        mod.Properties.Dimensions.height = 0; // will be set by GUILayout.Window
                        newWindowRect = GUILayout.Window(i, mod.Properties.Dimensions, OnModuleWindowCB, title, CurvyStyles.ModuleWindow);
                    }
                    else
                    {
                        UpdateLinks(mod);
                        newWindowRect = mod.Properties.Dimensions;
                    }

                    if (!Application.isPlaying && oldPos != newWindowRect.position)

#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                        EditorApplication.MarkSceneDirty();
#else
                        UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
#endif

                    if (Sel.SelectedModules.Count > 1) // Multi-Module move in OnGUI()
                    {
                        mod.Properties.Dimensions = newWindowRect.SetPosition(oldPos);
                    }
                    else
                    {
                        if (EV.alt && Canvas.IsModuleDrag && Sel.SelectedModule == mod)
                            newWindowRect.position = newWindowRect.position.Snap(CurvyProject.Instance.CGGraphSnapping);
                        mod.Properties.Dimensions = newWindowRect;
                    }



                    // Debugging
                    double lastUpdateDelta = (System.DateTime.Now - mod.DEBUG_LastUpdateTime).TotalMilliseconds;
                    if (lastUpdateDelta < 1500)
                    {
                        float alpha = Mathf.SmoothStep(1, 0, (float)lastUpdateDelta / 1500f);
                        DTGUI.PushBackgroundColor(new Color(0, 1, 0, alpha));
                        GUI.Box(mod.Properties.Dimensions.ScaleBy(refreshHighlightSize), "", CurvyStyles.GlowBox);
                        DTGUI.PopBackgroundColor();
                        Repaint();
                    }
                    // Track Window Movement for Viewport calculation
                    Canvas.ViewPortRegisterWindow(mod);
                }
            }
            EndWindows();
            if (Sel.SelectedModule != curSel)
                Canvas.FocusSelection();

        }

        void DrawSelectionRect()
        {
            Vector2 p = Canvas.SelectionRectStart;
            Vector2 p2 = Canvas.ViewPortMousePosition;
            Vector3[] verts = new Vector3[4]
            {
                new Vector3(p.x,p.y,0),
                new Vector3(p2.x,p.y,0),
                new Vector3(p2.x,p2.y,0),
                new Vector3(p.x,p2.y,0)
            };
            Handles.DrawSolidRectangleWithOutline(verts, new Color(.5f, .5f, .5f, 0.1f), Color.white);
        }

        void OnModuleWindowCB(int id)
        {
            // something happened in the meantime?
            if (id >= Modules.Count || mModuleCount != Modules.Count)
                return;
            CGModule mod = Modules[id];

            //if (LMB && Sel.SelectedModules.Count<=1)
            if (EV.type == EventType.MouseUp && !Sel.SelectedModules.Contains(mod))
                Sel.Select(Modules[id]);

            Rect winRect = mod.Properties.Dimensions;

            // Draw Title Buttons
            // Enabled
            EditorGUI.BeginChangeCheck();
            mod.Active = GUI.Toggle(new Rect(2, 2, 16, 16), mod.Active, "");
            if (EditorGUI.EndChangeCheck())
                EditorUtility.SetDirty(Generator);

            //Edit Title & Color
            if (editTitleModule == mod)
            {
                GUI.SetNextControlName("editTitle" + id.ToString());
                mod.ModuleName = GUI.TextField(new Rect(30, 5, winRect.width - 120, 16), mod.ModuleName);
                mod.Properties.BackgroundColor = EditorGUI.ColorField(new Rect(winRect.width - 70, 5, 32, 16), mod.Properties.BackgroundColor);
            }


            if (GUI.Button(new Rect(winRect.width - 32, 6, 16, 16), new GUIContent(CurvyStyles.EditTexture, "Rename"), CurvyStyles.BorderlessButton))
            {
                editTitleModule = mod;
                Sel.Select(mod);
                EditorGUI.FocusTextInControl("editTitle" + id.ToString());
            }

            // Help
            if (GUI.Button(new Rect(winRect.width - 16, 6, 16, 16), new GUIContent(CurvyStyles.HelpTexture, "Help"), CurvyStyles.BorderlessButton))
            {
                string url = DTUtility.GetHelpUrl(mod);
                if (!string.IsNullOrEmpty(url))
                    Application.OpenURL(url);
            }


            // Check errors
            if (mod.CircularReferenceError)
                EditorGUILayout.HelpBox("Circular Reference", MessageType.Error);
            // Draw Slots
            DTGUI.PushColor(mod.Properties.BackgroundColor.SkinAwareColor(true));
            GUILayout.Space(1);
            EditorGUILayout.BeginVertical(CurvyStyles.ModuleWindowSlotBackground);
            DTGUI.PopColor();
            UpdateLinks(mod);
            OnModuleWindowSlotGUI(mod);
            EditorGUILayout.EndVertical();

            CGModuleEditorBase ed = GetModuleEditor(mod);

            if (ed && ed.target != null)
            {

                if (EditorGUILayout.BeginFadeGroup(mShowDebug.faded))
                    ed.OnInspectorDebugGUIINTERNAL(Repaint);
                EditorGUILayout.EndFadeGroup();

                // Draw Module Options

                //I don't see the need for this, but I am not familiar enough with CG editor's code to feel confident to remove it
                mod.Properties.Expanded.valueChanged.RemoveListener(Repaint);
                mod.Properties.Expanded.valueChanged.AddListener(Repaint);

                if (!CurvyProject.Instance.CGAutoModuleDetails)
                    mod.Properties.Expanded.target = GUILayout.Toggle(mod.Properties.Expanded.target, new GUIContent(mod.Properties.Expanded.target ? CurvyStyles.CollapseTexture : CurvyStyles.ExpandTexture, "Show Details"), CurvyStyles.ShowDetailsButton);

                // === Module Details ===
                // Handle Auto-Folding
                if (DTGUI.IsLayout && CurvyProject.Instance.CGAutoModuleDetails)
                    mod.Properties.Expanded.target = (mod == Sel.SelectedModule);

                if (EditorGUILayout.BeginFadeGroup(mod.Properties.Expanded.faded))
                {
                    EditorGUIUtility.labelWidth = (mod.Properties.LabelWidth);
                    // Draw Inspectors using Modules Background color
                    DTGUI.PushColor(ed.Target.Properties.BackgroundColor.SkinAwareColor(true));
                    EditorGUILayout.BeginVertical(CurvyStyles.ModuleWindowBackground);
                    DTGUI.PopColor();

                    ed.RenderGUI(true);
                    if (ed.NeedRepaint)
                        mDoRepaint = true;
                    GUILayout.Space(2);
                    EditorGUILayout.EndVertical();
                }
                EditorGUILayout.EndFadeGroup();


            }

            // Make it dragable
            GUI.DragWindow(new Rect(0, 0, winRect.width, CurvyStyles.ModuleWindowTitleHeight));

        }

        void OnModuleWindowSlotGUI(CGModule module)
        {

            int i = 0;

            while (module.Input.Count > i || module.Output.Count > i)
            {
                GUILayout.BeginHorizontal();

                if (module.Input.Count > i)
                {
                    CGModuleInputSlot slot = module.Input[i];
                    Color linkDataTypeColor = getTypeColor(slot.Info.DataTypes);
                    if (Canvas.IsLinkDrag && !slot.CanLinkTo(Canvas.LinkDragFrom))
                        linkDataTypeColor = new Color(0.2f, 0.2f, 0.2f).SkinAwareColor(true);
                    DTGUI.PushColor(linkDataTypeColor);
                    GUILayout.Box("<", CurvyStyles.Slot);
                    DTGUI.PopColor();
                    string postfix = "";
                    if (slot.Info.Array && slot.Info.ArrayType == SlotInfo.SlotArrayType.Normal)
                        postfix = (slot.LastDataCountINTERNAL > 0) ? "[" + slot.LastDataCountINTERNAL.ToString() + "]" : "[]";
                    GUILayout.Label(new GUIContent(ObjectNames.NicifyVariableName(slot.Info.DisplayName) + postfix, slot.Info.Tooltip), CurvyStyles.GetSlotLabelStyle(slot));

                    // LinkDrag?
                    if (Canvas.IsLinkDrag)
                    {
                        // If ending drag over dropzone, create static link
                        if (EV.type == EventType.MouseUp && slot.DropZone.Contains(EV.mousePosition) && slot.CanLinkTo(Canvas.LinkDragFrom))
                            finishLink(slot);
                    }
                    // Clicking on Dropzone to pick existing link
                    else if (LMB && slot.Count == 1 && slot.DropZone.Contains(EV.mousePosition))
                    {
                        CGModuleOutputSlot linkedOutSlot = slot.SourceSlot();
                        linkedOutSlot.UnlinkFrom(slot);
                        EditorUtility.SetDirty(slot.Module);
                        startLinkDrag(linkedOutSlot);
                        GUIUtility.ExitGUI();
                    }
                }

                if (module.Output.Count > i)
                {
                    CGModuleOutputSlot slot = module.Output[i];
                    string postfix = "";
                    if (slot.Info.Array && slot.Info.ArrayType == SlotInfo.SlotArrayType.Normal)
                        postfix = (slot.Data != null && slot.Data.Length > 1) ? "[" + slot.Data.Length.ToString() + "]" : "";

                    GUILayout.Label(new GUIContent(ObjectNames.NicifyVariableName(slot.Info.DisplayName) + postfix, slot.Info.Tooltip), CurvyStyles.GetSlotLabelStyle(slot));
                    DTGUI.PushColor(getTypeColor(slot.Info.DataTypes));
                    GUILayout.Box(">", CurvyStyles.Slot);
                    DTGUI.PopColor();
                    // Debug
                    /*
                    if (Generator.ShowDebug)
                    {
                        GUI.enabled = slot.Data != null && slot.Data.Length>0;
                        if (GUILayout.Button(new GUIContent(CurvyStyles.DebugTexture, "Show Dump"), CurvyStyles.SmallButton, GUILayout.Width(16), GUILayout.Height(16)))
                            DTDebugWindow.Open(slot.Data[0].GetType().Name + ":", slot.Data[0].ToDumpString());
                        GUI.enabled = true;
                    }
                    */
                    // Start Linking?
                    if (LMB && !Canvas.IsSelectionRectDrag && slot.DropZone.Contains(EV.mousePosition))
                    {
                        startLinkDrag(slot);
                    }

                }
                GUILayout.EndHorizontal();
                i++;
            }


        }

        void UpdateLinks(CGModule module)
        {

            int i = 0;
            float h = 18;

            while (module.Input.Count > i || module.Output.Count > i)
            {
                float y = CurvyStyles.ModuleWindowTitleHeight + h * i;

                if (module.Input.Count > i)
                {
                    CGModuleInputSlot slot = module.Input[i];
                    slot.DropZone = new Rect(0, y, module.Properties.Dimensions.width / 2, h);
                    slot.Origin = new Vector2(module.Properties.Dimensions.xMin, module.Properties.Dimensions.yMin + y + h / 2);
                }

                if (module.Output.Count > i)
                {
                    CGModuleOutputSlot slot = module.Output[i];
                    slot.DropZone = new Rect(module.Properties.Dimensions.width / 2, y, module.Properties.Dimensions.width / 2, h);
                    slot.Origin = new Vector2(module.Properties.Dimensions.xMax, module.Properties.Dimensions.yMin + y + h / 2);
                }
                i++;
            }
        }

        void DrawToolbar()
        {
            GUILayout.BeginHorizontal(CurvyStyles.Toolbar);
            // Clear
            if (GUILayout.Button(new GUIContent(CurvyStyles.DeleteTexture, "Clear"), EditorStyles.miniButton) && EditorUtility.DisplayDialog("Clear", "Clear graph?", "Yes", "No"))
            {
                Sel.Select(null);
                Generator.Clear();
                Repaint();
                GUIUtility.ExitGUI();
            }

            GUILayout.Space(10);

            // Refresh
            if (GUILayout.Button(new GUIContent(CurvyStyles.RefreshTexture, "Refresh"), EditorStyles.miniButton, GUILayout.ExpandWidth(false)) && !DTGUI.IsLayout)
            {
                Modules = null;
                Generator.Refresh(true);
                Repaint();
                GUIUtility.ExitGUI();
            }

            // reorder
            mDoReorder = GUILayout.Button(new GUIContent(CurvyStyles.ReorderTexture, "Reorder modules"), EditorStyles.miniButton, GUILayout.ExpandWidth(false)) && !DTGUI.IsLayout;

            // Debug
            EditorGUI.BeginChangeCheck();
            mShowDebug.target = GUILayout.Toggle(mShowDebug.target, new GUIContent(CurvyStyles.DebugTexture, "Debug"), EditorStyles.miniButton);
            if (EditorGUI.EndChangeCheck())
            {
                Generator.ShowDebug = mShowDebug.target;
                SceneView.RepaintAll();
            }

            GUILayout.Space(10);


            // Expanded/Collapsed actions
            CurvyProject.Instance.CGAutoModuleDetails = GUILayout.Toggle(CurvyProject.Instance.CGAutoModuleDetails, new GUIContent(CurvyStyles.CGAutoFoldTexture, "Auto-Expand selected module"), EditorStyles.miniButton);
            if (GUILayout.Button(new GUIContent(CurvyStyles.ExpandTexture, "Expand all"), EditorStyles.miniButton))
                CGEditorUtility.SetModulesExpandedState(true, Generator.Modules.ToArray());
            if (GUILayout.Button(new GUIContent(CurvyStyles.CollapseTexture, "Collapse all"), EditorStyles.miniButton))
                CGEditorUtility.SetModulesExpandedState(false, Generator.Modules.ToArray());
            // Sync Selection
            CurvyProject.Instance.CGSynchronizeSelection = GUILayout.Toggle(CurvyProject.Instance.CGSynchronizeSelection, new GUIContent(CurvyStyles.SynchronizeTexture, "Synchronize Selection"), EditorStyles.miniButton);

            // Save Template
            GUILayout.Space(10);
            GUI.enabled = Sel.SelectedModule != null;
            if (GUILayout.Button(new GUIContent(CurvyStyles.AddTemplateTexture, "Save Selection as Template"), EditorStyles.miniButton))
                TemplateWizard.Open(Sel.SelectedModules, UI);

            GUI.enabled = true;
            GUILayout.FlexibleSpace();
            GUILayout.Label(new GUIContent(CurvyStyles.TexGridSnap, "Snap Grid Size\n(Hold Alt while dragging to snap)"));
            CurvyProject.Instance.CGGraphSnapping = (int)GUILayout.HorizontalSlider(CurvyProject.Instance.CGGraphSnapping, 1, 20, GUILayout.Width(60));
            GUILayout.Label(CurvyProject.Instance.CGGraphSnapping.ToString(), GUILayout.Width(20));
            CurvyProject.Instance.CGShowHelp = GUILayout.Toggle(CurvyProject.Instance.CGShowHelp, new GUIContent(CurvyStyles.HelpTexture, "Show Help"), EditorStyles.miniButton, GUILayout.Height(20));
            GUILayout.EndHorizontal();
        }

        void DrawStatusbar()
        {
            Rect r = new Rect(-1, position.height - mStatusbarHeight, 201, mStatusbarHeight - 1);
            // Performance
            EditorGUI.HelpBox(r, string.Format("Exec. Time (Avg): {0:0.###} ms", Generator.DEBUG_ExecutionTime.AverageMS), MessageType.None);
            // Message
            if (StatusBar.Render(new Rect(200, position.height - mStatusbarHeight, position.width, mStatusbarHeight - 1)))
                mDoRepaint = true;
        }

        void DrawLinks()
        {
            Vector3 a;
            Vector3 b;
            Vector3 at;
            Vector3 bt;
            float d;
            Vector3 off;
            float yDiff;
            Rect r = new Rect();

            foreach (CGModule mod in Modules)
            {
                //Debug.Log(mod.name + ":" + mod.Properties.Dimensions.yMin+" to "+mod.Properties.Dimensions.yMax);
                if (mod.OutputByName != null)
                {
                    foreach (CGModuleOutputSlot slotOut in mod.OutputByName.Values)
                    {
                        a = slotOut.Origin;
                        foreach (CGModuleSlot slotIn in slotOut.LinkedSlots)
                        {
                            b = slotIn.Origin;

                            r.Set(a.x, a.y, b.x - a.x, b.y - a.y);
                            // draw only visible lines
                            if (Canvas.ViewPort.Overlaps(r, true))
                            {
                                d = Mathf.Abs(b.x - a.x);
                                off = new Vector3(Mathf.Min(d / 2, Mathf.Max(d, 80)), 0, 0);
                                yDiff = Mathf.Max(slotIn.Module.Properties.Dimensions.yMin - mod.Properties.Dimensions.yMax, mod.Properties.Dimensions.yMin - b.y);
                                if (yDiff > 0)
                                    off.x = Mathf.Min(yDiff, 80);

                                at = a + off;
                                bt = b - off;
                                if (EV.type == EventType.Repaint)
                                {
                                    float w = (Sel.SelectedLink != null && Sel.SelectedLink.IsBetween(slotOut, slotIn)) ? 7 : 2;
                                    Color slotColor = getTypeColor(slotOut.Info.DataTypes);
                                    Handles.DrawBezier(a, b, at, bt, slotColor, CurvyStyles.LineTexture, w);
                                    if (((CGModuleInputSlot)slotIn).InputInfo.RequestDataOnly || slotIn.OnRequestModule != null)
                                    {
                                        Vector3 yOff = new Vector3(0, 4, 0);
                                        Handles.DrawBezier(a + yOff, b + yOff, at + yOff, bt + yOff, slotColor, CurvyStyles.LineTexture, w);
                                    }

                                }
                                if (LMB && HandleUtility.DistancePointBezier(EV.mousePosition, a, b, at, bt) < 4)
                                {
                                    Sel.Select(slotOut.Module.GetOutputLink((CGModuleOutputSlot)slotOut, (CGModuleInputSlot)slotIn));
                                }
                            }
                        }

                    }
                }
            }

        }

        void loadTypeColors()
        {
            TypeColors.Clear();

            Type tt = typeof(CGData);
            foreach (Type t in TypeExt.GetLoadedTypes())
            {
                if (t.IsSubclassOf(tt))
                {
                    object[] ai = t.GetCustomAttributes(typeof(CGDataInfoAttribute), true);
                    if (ai.Length > 0)
                    {
                        TypeColors.Add(t, ((CGDataInfoAttribute)ai[0]).Color);
                    }
                }
            }
        }

        public void destroyEditors()
        {
            List<CGModuleEditorBase> ed = new List<CGModuleEditorBase>(ModuleEditors.Values);
            for (int i = ed.Count - 1; i >= 0; i--)
                DestroyImmediate(ed[i]);
            ModuleEditors.Clear();
            InPlaceEditTarget = null;
            InPlaceEditInitiatedBy = null;
        }

        internal CGModuleEditorBase GetModuleEditor(CGModule module)
        {
            CGModuleEditorBase ed;
            if (!ModuleEditors.TryGetValue(module, out ed))
            {
                ed = Editor.CreateEditor(module) as CGModuleEditorBase;
                if (ed)
                {
                    ed.Graph = this;
                    ModuleEditors.Add(module, ed);
                }
                else
                    DTLog.LogError("[Curvy] Curvy Generator: Missing editor script for module '" + module.GetType().Name + "' !");
            }

            return ed;
        }

        Color getTypeColor(System.Type[] type)
        {
            Color c = Color.white; ;
            if (type.Length == 1)
                TypeColors.TryGetValue(type[0], out c);

            return c;//.SkinAwareColor();
        }

        #region ### Actions ###

        void startLinkDrag(CGModuleSlot slot)
        {
            Sel.Clear();
            Canvas.LinkDragFrom = (CGModuleOutputSlot)slot;
            StatusBar.SetMessage("Hold <b><Ctrl></b> to quickly create & connect a module!");
        }

        void finishLink(CGModuleInputSlot target)
        {
            StatusBar.Clear();
            Canvas.LinkDragFrom.LinkTo(target);
            EditorUtility.SetDirty(target.Module);
            if (!DTGUI.IsLayout)
                GUIUtility.ExitGUI();
        }
        #endregion



    }


}
