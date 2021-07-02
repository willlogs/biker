// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#endif
using UnityEngine;
using UnityEditor;
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.Curvy;
using UnityEngine.Assertions;

namespace FluffyUnderware.CurvyEditor
{
    [CustomEditor(typeof(MetaCGOptions))]
    [CanEditMultipleObjects]
    public class MetaCGOptionsEditor : DTEditor<MetaCGOptions>
    {

        [DrawGizmo(GizmoType.Active | GizmoType.NonSelected | GizmoType.InSelectionHierarchy)]
        static void MetaGizmoDrawer(MetaCGOptions data, GizmoType context)
        {
            if (data.Spline == null)
                return;

            if (CurvyGlobalManager.ShowMetadataGizmo && data.Spline.ShowGizmos)
            {
                if (data.HardEdge)
                {
                    Vector3 p = data.ControlPoint.transform.position;
                    p.y += HandleUtility.GetHandleSize(p) * 0.4f;
                    Handles.Label(p, "<b><color=\"#660000\">^</color></b>", DTStyles.BackdropHtmlLabel);
                }
                if (data.MaterialID != 0)
                    Handles.Label(data.Spline.ToWorldPosition(data.ControlPoint.Interpolate(0.5f)), "<b><color=\"#660000\">" + data.MaterialID.ToString() + "</color></b>", DTStyles.BackdropHtmlLabel);


            }
        }

        protected override void OnSceneGUI()
        {
            base.OnSceneGUI();

        }

        void CBSetFirstU()
        {
#if CONTRACTS_FULL
            Contract.Requires(Target.ControlPoint.Spline != null);
#endif
            //UVEdge || ExplicitU || HasDifferentMaterial
            if (!Target.UVEdge && !Target.HasDifferentMaterial && GUILayout.Button("Set U from neighbours"))
            {
                CurvySplineSegment targetControlPoint = Target.ControlPoint;
                CurvySpline targetSpline = targetControlPoint.Spline;

                float uValue;
                if (targetSpline.IsControlPointVisible(targetControlPoint))
                {
                    if (targetSpline.Count == 0)
                        uValue = 0;
                    else
                    {
                        CurvySplineSegment previousUWithDefinedCp;
                        CurvySpline curvySpline = Target.Spline;
                        {
                            CurvySplineSegment currentCp = curvySpline.GetPreviousControlPoint(targetControlPoint);
                            if (currentCp == null || targetControlPoint == curvySpline.FirstVisibleControlPoint)
                                previousUWithDefinedCp = targetControlPoint;
                            else
                            {
                                while (currentCp != curvySpline.FirstVisibleControlPoint)
                                {
                                    MetaCGOptions currentCpOptions = currentCp.GetMetadata<MetaCGOptions>(true);
                                    if (currentCpOptions.UVEdge || currentCpOptions.ExplicitU || currentCpOptions.HasDifferentMaterial)
                                        break;
                                    currentCp = curvySpline.GetPreviousControlPoint(currentCp);
                                }
                                previousUWithDefinedCp = currentCp;
                            }

                        }
                        MetaCGOptions previousDefinedOptions = previousUWithDefinedCp.GetMetadata<MetaCGOptions>(true);

                        CurvySplineSegment nextCpWithDefinedU;
                        {

                            CurvySplineSegment currentCp = curvySpline.GetNextControlPoint(targetControlPoint);
                            if (currentCp == null || targetControlPoint == curvySpline.LastVisibleControlPoint)
                                nextCpWithDefinedU = targetControlPoint;
                            else
                            {
                                while (currentCp != curvySpline.LastVisibleControlPoint)
                                {
                                    MetaCGOptions currentCpOptions = currentCp.GetMetadata<MetaCGOptions>(true);
                                    if (currentCpOptions.UVEdge || currentCpOptions.ExplicitU || currentCpOptions.HasDifferentMaterial)
                                        break;
                                    currentCp = curvySpline.GetNextControlPoint(currentCp);
                                }

                                nextCpWithDefinedU = currentCp;
                            }
                        }
                        if (curvySpline.Closed && nextCpWithDefinedU == curvySpline.LastVisibleControlPoint)
                            nextCpWithDefinedU = curvySpline.GetPreviousControlPoint(nextCpWithDefinedU);
                        MetaCGOptions nextDefinedOptions = nextCpWithDefinedU.GetMetadata<MetaCGOptions>(true);

                        float frag = (targetControlPoint.Distance - previousUWithDefinedCp.Distance) / (nextCpWithDefinedU.Distance - previousUWithDefinedCp.Distance);
#if CURVY_SANITY_CHECKS
                        Assert.IsFalse(float.IsNaN(frag));
#endif

                        float startingU = (previousUWithDefinedCp == targetControlPoint) ? 0 : previousDefinedOptions.GetDefinedSecondU(0);
                        float endingU = (nextCpWithDefinedU == targetControlPoint) ? 1 : nextDefinedOptions.GetDefinedFirstU(1);
                        uValue = Mathf.Lerp(startingU, endingU, frag);
                    }

                }
                else
                    uValue = 0;

                Target.FirstU = uValue;


                EditorUtility.SetDirty(target);
            }
        }
    }


}
