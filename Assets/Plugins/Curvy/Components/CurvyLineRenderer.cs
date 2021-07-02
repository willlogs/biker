// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.Curvy.Components
{
    /// <summary>
    /// Class to drive a LineRenderer with a CurvySpline
    /// </summary>
    [AddComponentMenu("Curvy/Misc/Curvy Line Renderer")]
    [RequireComponent(typeof(LineRenderer))]
    [ExecuteInEditMode]
    [HelpURL(CurvySpline.DOCLINK + "curvylinerenderer")]
    public class CurvyLineRenderer : MonoBehaviour
    {
        public CurvySpline m_Spline;

        public CurvySpline Spline
        {
            get { return m_Spline; }
            set
            {
                if (m_Spline != value)
                {
                    UnbindEvents();
                    m_Spline = value;
                    BindEvents();
                    Refresh();
                }
            }
        }

        LineRenderer mRenderer;

        void Awake()
        {
            mRenderer = GetComponent<LineRenderer>();
            if (m_Spline == null)
            {
                DTLog.LogWarning(String.Format("[Curvy] CurvyLineRenderer in GameObject '{0}' had no assigned Spline.", this.name));
                m_Spline = GetComponent<CurvySpline>();
                if (ReferenceEquals(m_Spline, null) == false)
                    DTLog.LogWarning(String.Format("[Curvy] Spline '{0}' was assigned to the CurvyLineRenderer by default.", this.name));
            }
        }

        void OnEnable()
        {
            mRenderer = GetComponent<LineRenderer>();
            BindEvents();
        }

        void OnDisable()
        {
            UnbindEvents();
        }

        void Start()
        {
            Refresh();
        }

        private void Update()
        {
            EnforceWorldSpaceUsage();
        }

        private void EnforceWorldSpaceUsage()
        {
            if (mRenderer.useWorldSpace == false)
                mRenderer.useWorldSpace = true;
        }


#if UNITY_EDITOR
        void OnValidate()
        {
            UnbindEvents();
            BindEvents();

            if (Spline && Spline.IsInitialized && Spline.Dirty)
                return;

            Refresh();
        }
#endif

        public void Refresh()
        {
            if (Spline && Spline.IsInitialized)
            {
                EnforceWorldSpaceUsage();
                Vector3[] vts = Spline.GetApproximation(Space.World);
#if UNITY_5_6_OR_NEWER
                mRenderer.positionCount = vts.Length;
                mRenderer.SetPositions(vts);
#else
                mRenderer.numPositions = vts.Length;
                for (int v = 0; v < vts.Length; v++)
                    mRenderer.SetPosition(v, vts[v]);
#endif
            }
            else if (mRenderer != null)
            {
                EnforceWorldSpaceUsage();
#if UNITY_5_6_OR_NEWER
                mRenderer.positionCount = 0;
#else
                mRenderer.numPositions = 0;
#endif
            }
        }

        void OnSplineRefresh(CurvySplineEventArgs e)
        {
            Refresh();
        }

        private void OnSplineCoordinatesChanged(CurvySpline spline)
        {
            Refresh();
        }

        void BindEvents()
        {
            if (Spline)
            {
                Spline.OnRefresh.AddListenerOnce(OnSplineRefresh);
                Spline.OnGlobalCoordinatesChanged += OnSplineCoordinatesChanged;
            }
        }

        void UnbindEvents()
        {
            if (Spline)
            {
                Spline.OnRefresh.RemoveListener(OnSplineRefresh);
                Spline.OnGlobalCoordinatesChanged -= OnSplineCoordinatesChanged;
            }
        }

    }
}
