// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Controllers;
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.DevTools;
using UnityEngine.Assertions;

namespace FluffyUnderware.CurvyEditor.Controllers
{

    public class CurvyControllerEditor<T> : CurvyEditorBase<T> where T : CurvyController
    {
        protected override void OnEnable()
        {

#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged += OnStateChanged;
#endif

            base.OnEnable();

        }

        protected override void OnDisable()
        {

#if UNITY_2017_2_OR_NEWER
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
#else
            EditorApplication.playmodeStateChanged -= OnStateChanged;
#endif

            base.OnDisable();
            if (Application.isPlaying == false)
                if (Target)
                Target.Stop();
        }

#if UNITY_2017_2_OR_NEWER
        void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            OnStateChanged();
        }
#endif

        void OnStateChanged()
        {
            if (Application.isPlaying == false)
                Target.Stop();

        }

        protected override void OnReadNodes()
        {
            DTGroupNode node = Node.AddSection("Preview", ShowPreviewButtons);
            node.Expanded = false;
            node.SortOrder = 5000;
        }


        /// <summary>
        /// Show the preview buttons
        /// </summary>
        protected void ShowPreviewButtons(DTInspectorNode node)
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = !Application.isPlaying;

            bool isPlayingOrPaused = Target.PlayState == CurvyController.CurvyControllerState.Playing || Target.PlayState == CurvyController.CurvyControllerState.Paused;

            //TODO it would be nice to have two different icons, one for Play and one for Pause
            if (GUILayout.Toggle(isPlayingOrPaused, new GUIContent(CurvyStyles.TexPlay, "Play/Pause in Editor"), GUI.skin.button) != isPlayingOrPaused)
            {

                switch (Target.PlayState)
                {
                    case CurvyController.CurvyControllerState.Paused:
                    case CurvyController.CurvyControllerState.Stopped:
                        Target.Play();
                        break;
                    case CurvyController.CurvyControllerState.Playing:
                        Target.Pause();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            if (GUILayout.Button(new GUIContent(CurvyStyles.TexStop, "Stop/Reset")))
            {
                Target.Stop();
            }
            GUI.enabled = true;
            GUILayout.EndHorizontal();
        }
    }
}
