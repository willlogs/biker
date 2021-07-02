// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using FluffyUnderware.DevTools;
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy.Generator
{
    /// <summary>
    /// Curvy Generator Utility class
    /// </summary>
    public static class CGUtility
    {
        /// <summary>
        /// Calculates lightmap UV's
        /// </summary>
        /// <param name="uv">the UV to create UV2 for</param>
        /// <returns>UV2</returns>
        public static Vector2[] CalculateUV2(Vector2[] uv)
        {
            Vector2[] UV2 = new Vector2[uv.Length];
            float sx = 0;
            float sy = 0;
            for (int i = 0; i < uv.Length; i++)
            {
                sx = sx < uv[i].x ? uv[i].x : sx;
                sy = sy < uv[i].y ? uv[i].y : sy;
            }
            float oneOnSx = 1f / sx;
            float oneOnSy = 1f / sy;
            for (int i = 0; i < uv.Length; i++)
            {
                UV2[i].x = uv[i].x * oneOnSx;
                UV2[i].y = uv[i].y * oneOnSy;
            }

            return UV2;
        }
        //public static Vector2[] CalculateUV2(Vector2[] uv)
        //{
        //    Vector2[] UV2 = new Vector2[uv.Length];
        //    float sx = 0;
        //    float sy = 0;
        //    for (int i = 0; i < uv.Length; i++)
        //    {
        //        float x = Mathf.Abs(uv[i].x);
        //        sx = sx < x ? x : sx;
        //        float y = Mathf.Abs(uv[i].y);
        //        sy = sy < y ? y : sy;
        //    }
        //    float oneOnSx = 1f / sx;
        //    float oneOnSy = 1f / sy;
        //    for (int i = 0; i < uv.Length; i++)
        //    {
        //        UV2[i].x = Mathf.Abs(uv[i].x * oneOnSx);
        //        UV2[i].y = Mathf.Abs(uv[i].y * oneOnSy);
        //    }

        //    return UV2;
        //}



        #region ### Rasterization Helpers ###

        /// <summary>
        /// Rasterization Helper class
        /// </summary>
        public static List<ControlPointOption> GetControlPointsWithOptions(CGDataRequestMetaCGOptions options, CurvySpline shape, float startDist, float endDist, bool optimize, out int initialMaterialID, out float initialMaxStep)
        {
#if CURVY_SANITY_CHECKS
            Assert.IsTrue(shape.Count > 0);
#endif


            List<ControlPointOption> res = new List<ControlPointOption>();
            initialMaterialID = 0;
            initialMaxStep = float.MaxValue;
            CurvySplineSegment startSeg = shape.DistanceToSegment(startDist);

            float clampedEndDist = shape.ClampDistance(endDist, shape.Closed ? CurvyClamping.Loop : CurvyClamping.Clamp);
            if (clampedEndDist == 0)
                clampedEndDist = endDist;
            CurvySplineSegment finishSeg = (clampedEndDist == shape.Length) ? shape.LastVisibleControlPoint : shape.DistanceToSegment(clampedEndDist);
            if (endDist != shape.Length && endDist > finishSeg.Distance)
            {
                finishSeg = shape.GetNextControlPoint(finishSeg);
            }
            MetaCGOptions cgOptions;

            float loopOffset = 0;
            if (startSeg)
            {
                cgOptions = startSeg.GetMetadata<MetaCGOptions>(true);
                initialMaxStep = (cgOptions.MaxStepDistance == 0) ? float.MaxValue : cgOptions.MaxStepDistance;
                if (options.CheckMaterialID)
                    initialMaterialID = cgOptions.MaterialID;
                int currentMaterialID = initialMaterialID;

                float maxDist = cgOptions.MaxStepDistance;
                /*
                if ((options.CheckMaterialID && cgOptions.MaterialID != 0) ||
                       (optimize && cgOptions.MaxStepDistance != 0))
                    res.Add(new ControlPointOption(startSeg.LocalFToTF(0),
                                                   startSeg.Distance,
                                                   true,
                                                   cgOptions.MaterialID,
                                                   options.CheckHardEdges && cgOptions.HardEdge,
                                                   initialMaxStep,
                                                   (options.CheckExtendedUV && cgOptions.UVEdge),
                                                   options.CheckExtendedUV && cgOptions.ExplicitU,
                                                   cgOptions.FirstU,
                                                   cgOptions.SecondU));
                */


                CurvySplineSegment seg = shape.GetNextSegment(startSeg) ?? shape.GetNextControlPoint(startSeg);
                do
                {
                    cgOptions = seg.GetMetadata<MetaCGOptions>(true);
                    if (shape.GetControlPointIndex(seg) < shape.GetControlPointIndex(startSeg))
                        loopOffset = shape.Length;
                    if (options.IncludeControlPoints ||
                       (options.CheckHardEdges && cgOptions.HardEdge) ||
                       (options.CheckMaterialID && cgOptions.MaterialID != currentMaterialID) ||
                       (optimize && cgOptions.MaxStepDistance != maxDist) ||
                       (options.CheckExtendedUV && (cgOptions.UVEdge || cgOptions.ExplicitU))
                        )
                    {
                        bool matDiff = cgOptions.MaterialID != currentMaterialID;
                        maxDist = (cgOptions.MaxStepDistance == 0) ? float.MaxValue : cgOptions.MaxStepDistance;
                        currentMaterialID = options.CheckMaterialID ? cgOptions.MaterialID : initialMaterialID;
                        res.Add(new ControlPointOption(seg.TF + Mathf.FloorToInt(loopOffset / shape.Length),
                                                       seg.Distance + loopOffset,
                                                       options.IncludeControlPoints,
                                                       currentMaterialID,
                                                       options.CheckHardEdges && cgOptions.HardEdge,
                                                       cgOptions.MaxStepDistance,
                                                       (options.CheckExtendedUV && cgOptions.UVEdge) || matDiff,
                                                       options.CheckExtendedUV && cgOptions.ExplicitU,
                                                       cgOptions.FirstU,
                                                       cgOptions.SecondU));

                    }
                    seg = shape.GetNextSegment(seg);
                } while (seg && seg != finishSeg);
                // Check UV settings of last cp (not a segment if open spline!)
                if (options.CheckExtendedUV && !seg && shape.LastVisibleControlPoint == finishSeg)
                {
                    cgOptions = finishSeg.GetMetadata<MetaCGOptions>(true);
                    if (cgOptions.ExplicitU)
                        res.Add(new ControlPointOption(1,
                                                       finishSeg.Distance + loopOffset,
                                                       options.IncludeControlPoints,
                                                       currentMaterialID,
                                                       options.CheckHardEdges && cgOptions.HardEdge,
                                                       cgOptions.MaxStepDistance,
                                                       (options.CheckExtendedUV && cgOptions.UVEdge) || (options.CheckMaterialID && cgOptions.MaterialID != currentMaterialID),
                                                       options.CheckExtendedUV && cgOptions.ExplicitU,
                                                       cgOptions.FirstU,
                                                       cgOptions.SecondU));
                }
            }

            return res;
        }

        #endregion

    }
}
