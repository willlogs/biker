// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#endif
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools;
using FluffyUnderware.DevTools.Extensions;
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy
{
    /// <summary>
    /// Curvy Gizmo Helper Class
    /// </summary>
    public static class CurvyGizmoHelper
    {
        public static Matrix4x4 Matrix = Matrix4x4.identity;

        public static void SegmentCurveGizmo(CurvySplineSegment seg, Color col, float stepSize = 0.05f)
        {
#if CONTRACTS_FULL
            Contract.Requires(seg != null);
            Contract.Requires(seg.Spline != null);
#endif
            Matrix4x4 mat = Gizmos.matrix;
            Gizmos.matrix = Matrix;
            Gizmos.color = col;

            if (seg.Spline.Interpolation == CurvyInterpolation.Linear)
            {
                Gizmos.DrawLine(seg.Interpolate(0), seg.Interpolate(1));
                return;
            }
            Vector3 p = seg.Interpolate(0);
            for (float f = stepSize; f < 1; f += stepSize)
            {
                Vector3 p1 = seg.Interpolate(f);
                Gizmos.DrawLine(p, p1);
                p = p1;
            }
            Gizmos.DrawLine(p, seg.Interpolate(1));

            Gizmos.matrix = mat;
        }

        public static void SegmentApproximationGizmo(CurvySplineSegment seg, Color col)
        {
#if CONTRACTS_FULL
            Contract.Requires(seg != null);
            Contract.Requires(seg.Spline != null);
#endif
            Matrix4x4 mat = Gizmos.matrix;
            Gizmos.matrix = Matrix;
            Gizmos.color = col;
            Vector3 size = new Vector3(0.1f / seg.Spline.transform.localScale.x,
                                       0.1f / seg.Spline.transform.localScale.y,
                                       0.1f / seg.Spline.transform.localScale.z);

            Camera camera = Camera.current;
            Transform cameraTransform = camera.transform;
            Vector3 cameraPosition = cameraTransform.position;
            Vector3 cameraZDirection = cameraTransform.TransformDirection(new Vector3(0f, 0f, 1f));
            Matrix4x4 gizmoMatrix = Gizmos.matrix;

            for (int i = 0; i < seg.Approximation.Length; i++)
            {
                Vector3 approximationPosition = seg.Approximation[i];
                float handleSize = DTUtility.GetHandleSize(gizmoMatrix.MultiplyPoint(approximationPosition), camera, cameraPosition, cameraZDirection, cameraTransform);
                Gizmos.DrawCube(approximationPosition, handleSize.Multiply(size));
            }
            Gizmos.matrix = mat;
        }

        public static void SegmentOrientationAnchorGizmo(CurvySplineSegment seg, Color col)
        {
#if CONTRACTS_FULL
            Contract.Requires(seg != null);
            Contract.Requires(seg.Spline != null);
#endif
            if (seg.ApproximationUp.Length == 0)
                return;
            Matrix4x4 mat = Gizmos.matrix;
            Gizmos.matrix = Matrix;
            Gizmos.color = col;
            Vector3 scl = new Vector3(1 / seg.Spline.transform.localScale.x,
                                      1 / seg.Spline.transform.localScale.y,
                                      1 / seg.Spline.transform.localScale.z);

            Vector3 u = seg.ApproximationUp[0];
            u.Set(u.x * scl.x, u.y * scl.y, u.z * scl.z);
            Gizmos.DrawRay(seg.Approximation[0], u * CurvyGlobalManager.GizmoOrientationLength * 1.75f);
            Gizmos.matrix = mat;
        }

        public static void SegmentOrientationGizmo(CurvySplineSegment seg, Color col)
        {
#if CONTRACTS_FULL
            Contract.Requires(seg != null);
            Contract.Requires(seg.Spline != null);
#endif
            Matrix4x4 mat = Gizmos.matrix;
            Gizmos.matrix = Matrix;
            Gizmos.color = col;
            Vector3 scale = new Vector3(1 / seg.Spline.transform.localScale.x,
                                      1 / seg.Spline.transform.localScale.y,
                                      1 / seg.Spline.transform.localScale.z);

            for (int i = 0; i < seg.ApproximationUp.Length; i++)
            {
                Vector3 position = seg.Approximation[i];
                Vector3 u = seg.ApproximationUp[i];
                u.Set(u.x * scale.x, u.y * scale.y, u.z * scale.z);
                
                Gizmos.DrawLine(position,
                    position.Addition(u.Multiply(CurvyGlobalManager.GizmoOrientationLength)
                    )
                );
            }
            Gizmos.matrix = mat;
        }

        public static void SegmentTangentGizmo(CurvySplineSegment seg, Color col)
        {
            Matrix4x4 mat = Gizmos.matrix;
            Gizmos.matrix = Matrix;
            int segCacheSize = seg.CacheSize;

            for (int i = 0; i < seg.ApproximationT.Length; i++)
            {
                //updating gizmo color
                if (i == 0)
                    Gizmos.color = Color.blue;
                else if (i == 1)
                    Gizmos.color = col;
                else if (i == segCacheSize)
                    Gizmos.color = Color.black;

                Vector3 position = seg.Approximation[i];
                Gizmos.DrawLine(position,
                        position.Addition(seg.ApproximationT[i].Multiply(CurvyGlobalManager.GizmoOrientationLength)
                        )
                    );
            }
            Gizmos.matrix = mat;
        }

        public static void ControlPointGizmo(CurvySplineSegment cp, bool selected, Color col)
        {
            Matrix4x4 mat = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.identity;

            Gizmos.color = col;
            Vector3 p = Matrix.MultiplyPoint(cp.transform.localPosition);
            float size = (selected) ? 1 : 0.7f;

            if (cp.Spline && cp.Spline.RestrictTo2D)
                Gizmos.DrawCube(p, Vector3.one * DTUtility.GetHandleSize(p) * size * CurvyGlobalManager.GizmoControlPointSize);
            else
                Gizmos.DrawSphere(p, DTUtility.GetHandleSize(p) * size * CurvyGlobalManager.GizmoControlPointSize);

            Gizmos.matrix = mat;
        }

#if UNITY_EDITOR
        public static void ConnectionGizmo(CurvySplineSegment connectedControlPoint)
        {
#if CURVY_SANITY_CHECKS
            Assert.IsTrue(connectedControlPoint.Connection != null);
#endif 

            Matrix4x4 initialMatrix = Gizmos.matrix;
            Gizmos.matrix = Matrix;

            Gizmos.color = connectedControlPoint.Connection.GetGizmoColor();
            Vector3 localPosition = connectedControlPoint.transform.localPosition;
            Gizmos.DrawWireSphere(localPosition, DTUtility.GetHandleSize(localPosition) * CurvyGlobalManager.GizmoControlPointSize * 1.3f);
            Gizmos.matrix = initialMatrix;
        }
#endif 

        public static void BoundsGizmo(CurvySplineSegment cp, Color col)
        {
            Matrix4x4 mat = Gizmos.matrix;
            Gizmos.matrix = Matrix;

            Gizmos.color = col;
            Gizmos.DrawWireCube(cp.Bounds.center, cp.Bounds.size);
            Gizmos.matrix = mat;
        }

    }
}
