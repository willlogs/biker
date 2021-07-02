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
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy.Generator
{
    /// <summary>
    /// Base class for spline input modules
    /// </summary>
    public abstract class SplineInputModuleBase : CGModule
    {
        #region ### Serialized Fields ###
        /// <summary>
        /// Makes this module use the cached approximations of the spline's positions and tangents
        /// </summary>
        [Tab("General")]
        [SerializeField]
        [Tooltip("Makes this module use the cached approximations of the spline's positions and tangents")]
        bool m_UseCache;
        [Tooltip("Whether to use local or global coordinates of the input's control points.\r\nUsing the global space when the input's transform is updating every frame will lead to the generator refreshing too frequently")]
        [SerializeField]
        private bool m_UseGlobalSpace;

        [Tab("Range")]
        [SerializeField]
        protected CurvySplineSegment m_StartCP;
        [FieldCondition("m_StartCP", null, true, Action = ActionAttribute.ActionEnum.Enable)]
        [SerializeField]
        protected CurvySplineSegment m_EndCP;





        #endregion

        #region ### Public Properties ###

        /// <summary>
        /// Makes this module use the cached approximations of the spline's positions and tangents
        /// </summary>
        public bool UseCache
        {
            get { return m_UseCache; }
            set
            {
                if (m_UseCache != value)
                    m_UseCache = value;
                Dirty = true;
            }
        }

        public CurvySplineSegment StartCP
        {
            get { return m_StartCP; }
            set
            {
                if (m_StartCP != value)
                {
                    m_StartCP = value;
                    ValidateStartAndEndCps();
                }
                Dirty = true;
            }
        }

        public CurvySplineSegment EndCP
        {
            get { return m_EndCP; }
            set
            {
                if (m_EndCP != value)
                {
                    m_EndCP = value;
                    ValidateStartAndEndCps();
                }
                Dirty = true;
            }
        }

        /// <summary>
        /// Whether to use local or global coordinates of the input's control points.
        /// Using the global space will dirty the module whenever the spline's transform is updated
        /// </summary>
        public bool UseGlobalSpace
        {
            get { return m_UseGlobalSpace; }
            set
            {
                m_UseGlobalSpace = value;
                Dirty = true;
            }
        }

        public override bool IsConfigured
        {
            get
            {
                return base.IsConfigured && InputSpline != null;
            }
        }

        public override bool IsInitialized
        {
            get
            {
                return base.IsInitialized && (InputSpline == null || InputSpline.IsInitialized);
            }
        }

        public bool PathIsClosed
        {
            get { return IsConfigured && getPathClosed(InputSpline); }
        }

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        protected override void OnEnable()
        {
            base.OnEnable();
            Properties.MinWidth = 250;
            OnSplineAssigned();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            if (InputSpline)
            {
                InputSpline.OnRefresh.RemoveListener(OnSplineRefreshed);
                InputSpline.OnGlobalCoordinatesChanged -= OnInputSplineCoordinatesChanged;
            }
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            if (isActiveAndEnabled)
                ValidateStartAndEndCps();
            OnSplineAssigned();
            Dirty = true;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            InputSpline = null;
            UseCache = false;
            StartCP = null;
            EndCP = null;
            UseGlobalSpace = false;
        }

        /*! \endcond */
        #endregion

        /// <summary>
        /// Checks that StartCP and EndCp values are correct, and fix them if they are not.
        /// </summary>
        private void OnSplineRefreshed(CurvySplineEventArgs e)
        {
            if (!enabled || !gameObject.activeInHierarchy)
                return;
            if (InputSpline == e.Spline)
                ForceRefresh();
            else
                e.Spline.OnRefresh.RemoveListener(OnSplineRefreshed);
        }

        private void OnInputSplineCoordinatesChanged(CurvySpline sender)
        {
            if (!enabled || !gameObject.activeInHierarchy)
                return;
            if (InputSpline == sender)
            {
                if (UseGlobalSpace)
                    ForceRefresh();
            }
            else
                InputSpline.OnGlobalCoordinatesChanged -= OnInputSplineCoordinatesChanged;
        }

        private void ForceRefresh()
        {
            Dirty = true;
#if UNITY_EDITOR
            // The Update order makes the spline and CP update happen before the generator, so from first look, no need to force the generator update, it should happen naturally. But unfortunately things are not that simple :(
            // So the explanation is that the whole chain of actions that lead to this method being called starts with the change of the transform of a CP. For some reason (I am sure about this for 90%) the transform change's is acknowledged in the spline's (or CP's) Update while in Play mode, but only in editorUpdate when in Edit mode. And editorUpdate gets called after all the Updates (including the generator's update), so to make sure the generator is up to date after the spline's update we have to force it's update when not in Play mode
            if (!Application.isPlaying)
                Generator.TryAutoRefresh();
#endif
        }

        private bool getPathClosed(CurvySpline spline)
        {
            if (!spline || !spline.Closed)
                return false;
            return EndCP == null;
        }

        #region GetSplineData

        protected CGData GetSplineData(CurvySpline spline, bool fullPath /*is spline a path*/, CGDataRequestRasterization raster, CGDataRequestMetaCGOptions options)
        {
            if (spline == null || spline.Count == 0)
                return null;

            // calc start & end point (distance)
            float pathLength = StartCP && EndCP ? EndCP.Distance - StartCP.Distance : spline.Length;
            float startDist;
            float endDist;
            {
#if CURVY_SANITY_CHECKS
                Assert.IsTrue(raster.Start >= 0);
                Assert.IsTrue(raster.Start <= 1);
#endif
                startDist = pathLength * raster.Start;
                if (StartCP)
                    startDist = (startDist + StartCP.Distance) % spline.Length;
#if CURVY_SANITY_CHECKS
                Assert.IsTrue(startDist >= 0);
                Assert.IsTrue(startDist <= spline.Length);

                Assert.IsTrue(raster.RasterizedRelativeLength >= 0);
                Assert.IsTrue(raster.RasterizedRelativeLength <= 1);
#endif
                endDist = startDist + pathLength * raster.RasterizedRelativeLength;

            }

            float stepDist;
            {
                float samplingPointsPerUnit = CurvySpline.CalculateSamplingPointsPerUnit(
                    raster.Resolution,
                    spline.MaxPointsPerUnit);

                float sampledDistance = (endDist - startDist);
                stepDist = Mathf.Min(
                    sampledDistance / (pathLength * raster.RasterizedRelativeLength * samplingPointsPerUnit),
                    //To ensure that rasterized shapes have at least 3 vertices to generate valid meshes
                    sampledDistance / 3f);
            }

            CGShape data;
            {
                data = (fullPath) ? new CGPath() : new CGShape();
                data.Length = endDist - startDist;
                data.SourceIsManaged = IsManagedResource(spline);
                data.Closed = spline.Closed;
                data.Seamless = spline.Closed && raster.RasterizedRelativeLength == 1;
            }


            if (data.Length == 0)
                return data;

            // Scan input spline and fetch a list of control points that provide special options (Hard Edge, MaterialID etc...)
            int materialID;
            float maxStep;
            List<ControlPointOption> optionsSegs;
            {
                if (options)
                {
                    optionsSegs = CGUtility.GetControlPointsWithOptions(options,
                        spline,
                        startDist,
                        endDist,
                        raster.Mode == CGDataRequestRasterization.ModeEnum.Optimized,
                        out materialID,
                        out maxStep);
                }
                else
                {
                    optionsSegs = new List<ControlPointOption>();
                    materialID = 0;
                    maxStep = float.MaxValue;
                }
            }

            // initialize with start TF
            float tf = spline.DistanceToTF(startDist);
            float startTF = tf;
            float endTF = (endDist > spline.Length && spline.Closed) ? spline.DistanceToTF(endDist - spline.Length) + 1 : spline.DistanceToTF(endDist);
            float currentDistance = startDist;

            // Setup vars
            List<Vector3> positions = new List<Vector3>();
            List<float> relativeFs = new List<float>();
            List<float> sourceFs = new List<float>();
            List<Vector3> tangents = new List<Vector3>();//OPTIM give these lists the correct length, or at least a close value, to avoid resizing the list too many times
            List<Vector3> normals = new List<Vector3>();
            List<int> softEdges = new List<int>();
            List<SamplePointUData> extendedUVData = new List<SamplePointUData>();
            bool duplicatePoint = false;//TODO BUG? why is duplicatePoint not used before assigning it in the ModeEnum.Optimized case?
                                        // we have at least one Material Group
            SamplePointsMaterialGroup materialGroup = new SamplePointsMaterialGroup(materialID);
            // and at least one patch within that group
            SamplePointsPatch patch = new SamplePointsPatch(0);
            CurvyClamping clampMode = (data.Closed) ? CurvyClamping.Loop : CurvyClamping.Clamp;
            int dead = 100000;

            //BUG? there is a lot of code that is quite the same, but not completly, between the two following cases. I sens potential bugs here
            //OPTIM in the following, a lot of spline methods have a call to TFToSegment inside them. Instead of letting each one of these methods call TFToSegment, call it once and give it to all the methods
            switch (raster.Mode)
            {
                case CGDataRequestRasterization.ModeEnum.Even:
                    #region --- Even ---
                    {
                        // we advance the spline using a fixed distance
                        while (currentDistance <= endDist && --dead > 0)
                        {
                            tf = spline.DistanceToTF(spline.ClampDistance(currentDistance, clampMode));

                            float currentF;
                            {
                                currentF = (currentDistance - startDist) / data.Length;//curDist / endDist;
                                if (Mathf.Approximately(1, currentF))
                                    currentF = 1;
                            }

                            //Position, tangent and up
                            Vector3 currentPosition;
                            Vector3 currentTangent;
                            Vector3 currentUp;
                            {
                                float localF;
                                CurvySplineSegment segment = spline.TFToSegment(tf, out localF, CurvyClamping.Clamp);
                                if (fullPath) // add path values
                                {
                                    if (UseCache)
                                        segment.InterpolateAndGetTangentFast(localF, out currentPosition, out currentTangent);
                                    else
                                        segment.InterpolateAndGetTangent(localF, out currentPosition, out currentTangent);

                                    //OPTIM get orientation at the same time you get position and tangent
                                    currentUp = segment.GetOrientationUpFast(localF);
                                }
                                else
                                {
                                    currentPosition = (UseCache) ? segment.InterpolateFast(localF) : segment.Interpolate(localF);
                                    currentTangent = Vector3.zero;
                                    currentUp = Vector3.zero;
                                }
                            }

                            AddPoint((currentDistance / spline.Length) % 1, currentF, fullPath, currentPosition, currentTangent, currentUp, sourceFs, relativeFs, positions, tangents, normals);

                            if (duplicatePoint) // HardEdge, IncludeCP, MaterialID changes etc. need an extra vertex
                            {
                                AddPoint((currentDistance / spline.Length) % 1, currentF, fullPath, currentPosition, currentTangent, currentUp, sourceFs, relativeFs, positions, tangents, normals);
                                duplicatePoint = false;
                            }
                            // Advance
                            currentDistance += stepDist;

                            // Check next Sample Point's options. If the next point would be past a CP with options
                            if (optionsSegs.Count > 0 && currentDistance >= optionsSegs[0].Distance)
                            {
                                if (optionsSegs[0].UVEdge || optionsSegs[0].UVShift)
                                    extendedUVData.Add(new SamplePointUData(positions.Count, optionsSegs[0].UVEdge, optionsSegs[0].FirstU, optionsSegs[0].SecondU));
                                // clamp point at CP and maybe duplicate the next sample point
                                currentDistance = optionsSegs[0].Distance;
                                duplicatePoint = optionsSegs[0].HardEdge || optionsSegs[0].MaterialID != materialGroup.MaterialID || (options.CheckExtendedUV && optionsSegs[0].UVEdge);
                                // end the current patch...
                                if (duplicatePoint)
                                {
                                    patch.End = positions.Count;
                                    materialGroup.Patches.Add(patch);
                                    // if MaterialID changes, we start a new MaterialGroup
                                    if (materialGroup.MaterialID != optionsSegs[0].MaterialID)
                                    {
                                        data.MaterialGroups.Add(materialGroup);
                                        materialGroup = new SamplePointsMaterialGroup(optionsSegs[0].MaterialID);
                                    }
                                    // in any case we start a new patch
                                    patch = new SamplePointsPatch(positions.Count + 1);
                                    if (!optionsSegs[0].HardEdge)
                                        softEdges.Add(positions.Count + 1);
                                    // Extended UV
                                    if (optionsSegs[0].UVEdge || optionsSegs[0].UVShift)
                                        extendedUVData.Add(new SamplePointUData(positions.Count + 1, optionsSegs[0].UVEdge, optionsSegs[0].FirstU, optionsSegs[0].SecondU));
                                }
                                // and remove the CP from the options
                                optionsSegs.RemoveAt(0);
                            }

                            // Ensure last sample point position is at the desired end distance
                            if (currentDistance > endDist && currentF < 1) // next loop curF will be 1
                                currentDistance = endDist;
                        }
                        if (dead <= 0)
                            Debug.LogError("[Curvy] He's dead, Jim! Deadloop in SplineInputModuleBase.GetSplineData (Even)! Please send a bug report.");
                        // store the last open patch
                        patch.End = positions.Count - 1;
                        materialGroup.Patches.Add(patch);
                        // ExplicitU on last Vertex?
                        //if (optionsSegs.Count > 0 && optionsSegs[0].UVShift)
                        //    extendedUVData.Add(new SamplePointUData(pos.Count - 1, optionsSegs[0].UVEdge, optionsSegs[0].FirstU, optionsSegs[0].SecondU));
                        // if path is closed and no hard edges involved, we need to smooth first normal
                        if (data.Closed && !spline[0].GetMetadata<MetaCGOptions>(true).HardEdge)
                            softEdges.Add(0);

                        FillData(data, materialGroup, sourceFs, relativeFs, fullPath, positions, tangents, normals, UseGlobalSpace, spline.transform, Generator.transform);

                    }
                    #endregion
                    break;
                case CGDataRequestRasterization.ModeEnum.Optimized:
                    #region --- Optimized ---
                    {
                        float stepSizeTF = stepDist / spline.Length;
                        float maxAngle = raster.AngleThreshold;

                        Vector3 currentPosition;
                        Vector3 currentTangent;
                        {
                            if (UseCache)
                                spline.InterpolateAndGetTangentFast(tf, out currentPosition, out currentTangent);
                            else
                                spline.InterpolateAndGetTangent(tf, out currentPosition, out currentTangent);
                        }

                        while (tf < endTF && dead-- > 0)
                        {
                            AddPoint((currentDistance / spline.Length) % 1, (currentDistance - startDist) / data.Length, fullPath, currentPosition, currentTangent, spline.GetOrientationUpFast(tf % 1), sourceFs, relativeFs, positions, tangents, normals);
                            // Advance
                            float stopAt = (optionsSegs.Count > 0) ? optionsSegs[0].TF : endTF;

                            bool atStopPoint = MoveByAngleExt(spline, UseCache, ref tf,
                                maxStep, maxAngle, out currentPosition, out currentTangent, stopAt, data.Closed, stepSizeTF);

                            currentDistance = spline.TFToDistance(tf, clampMode);
                            if (currentDistance < startDist)
                                currentDistance += spline.Length;

                            if (Mathf.Approximately(tf, endTF) || tf > endTF)
                            {
                                currentDistance = endDist;
                                endTF = (data.Closed) ? DTMath.Repeat(endTF, 1) : Mathf.Clamp01(endTF);
                                if (fullPath)
                                {
                                    if (UseCache)
                                        spline.InterpolateAndGetTangentFast(endTF, out currentPosition, out currentTangent);
                                    else
                                        spline.InterpolateAndGetTangent(endTF, out currentPosition, out currentTangent);
                                }
                                else
                                    currentPosition = (UseCache) ? spline.InterpolateFast(endTF) : spline.Interpolate(endTF);

                                AddPoint((currentDistance / spline.Length) % 1, (currentDistance - startDist) / data.Length, fullPath, currentPosition, currentTangent, spline.GetOrientationUpFast(endTF), sourceFs, relativeFs, positions, tangents, normals);
                                break;
                            }
                            if (atStopPoint)
                            {
                                if (optionsSegs.Count > 0)
                                {
                                    if (optionsSegs[0].UVEdge || optionsSegs[0].UVShift)
                                        extendedUVData.Add(new SamplePointUData(positions.Count, optionsSegs[0].UVEdge, optionsSegs[0].FirstU, optionsSegs[0].SecondU));
                                    // clamp point at CP and maybe duplicate the next sample point
                                    currentDistance = optionsSegs[0].Distance;
                                    maxStep = (optionsSegs[0].MaxStepDistance);
                                    duplicatePoint = optionsSegs[0].HardEdge || optionsSegs[0].MaterialID != materialGroup.MaterialID || (options.CheckExtendedUV && optionsSegs[0].UVEdge);
                                    if (duplicatePoint)
                                    {
                                        // end the current patch...
                                        patch.End = positions.Count;
                                        materialGroup.Patches.Add(patch);
                                        // if MaterialID changes, we start a new MaterialGroup
                                        if (materialGroup.MaterialID != optionsSegs[0].MaterialID)
                                        {
                                            data.MaterialGroups.Add(materialGroup);
                                            materialGroup = new SamplePointsMaterialGroup(optionsSegs[0].MaterialID);
                                        }


                                        // in any case we start a new patch
                                        patch = new SamplePointsPatch(positions.Count + 1);
                                        if (!optionsSegs[0].HardEdge)
                                            softEdges.Add(positions.Count + 1);
                                        // Extended UV
                                        if (optionsSegs[0].UVEdge || optionsSegs[0].UVShift)
                                            extendedUVData.Add(new SamplePointUData(positions.Count + 1, optionsSegs[0].UVEdge, optionsSegs[0].FirstU, optionsSegs[0].SecondU));
                                        AddPoint((currentDistance / spline.Length) % 1, (currentDistance - startDist) / data.Length, fullPath, currentPosition, currentTangent, spline.GetOrientationUpFast(tf), sourceFs, relativeFs, positions, tangents, normals);
                                    }
                                    // and remove the CP from the options
                                    optionsSegs.RemoveAt(0);

                                }
                                else
                                {
                                    AddPoint((currentDistance / spline.Length) % 1, (currentDistance - startDist) / data.Length, fullPath, currentPosition, currentTangent, spline.GetOrientationUpFast(tf), sourceFs, relativeFs, positions, tangents, normals);
                                    break;
                                }
                            }

                        }
                        if (dead <= 0)
                            Debug.LogError("[Curvy] He's dead, Jim! Deadloop in SplineInputModuleBase.GetSplineData (Optimized)! Please send a bug report.");
                        // store the last open patch
                        patch.End = positions.Count - 1;
                        materialGroup.Patches.Add(patch);
                        // ExplicitU on last Vertex?
                        if (optionsSegs.Count > 0 && optionsSegs[0].UVShift)
                            extendedUVData.Add(new SamplePointUData(positions.Count - 1, optionsSegs[0].UVEdge, optionsSegs[0].FirstU, optionsSegs[0].SecondU));
                        // if path is closed and no hard edges involved, we need to smooth first normal
                        if (data.Closed && !spline[0].GetMetadata<MetaCGOptions>(true).HardEdge)
                            softEdges.Add(0);
                        FillData(data, materialGroup, sourceFs, relativeFs, fullPath, positions, tangents, normals, UseGlobalSpace, spline.transform, Generator.transform);
                    }
                    #endregion
                    break;
            }
            data.Map = (float[])data.F.Clone();
            if (!fullPath)
            {
                data.RecalculateNormals(softEdges);
                //TODO BUG? the condition that leads to filling extendedUVData is different than this condition. Are they equivalent?
                if (options && options.CheckExtendedUV)
                    CalculateExtendedUV(spline, startTF, endTF, extendedUVData, data);
            }
            return data;
        }

        private static void FillData(CGShape dataToFill, SamplePointsMaterialGroup materialGroup, List<float> sourceFs, List<float> relativeFs, bool isFullPath, List<Vector3> positions, List<Vector3> tangents, List<Vector3> normals, bool considerSplineTransform, Transform splineTransform, Transform generatorTransform)
        {
            if (considerSplineTransform)
            {
                //OPTIM do not do the transform if the spline and generator transforms are the same
                for (int i = 0; i < positions.Count; i++)
                    positions[i] = generatorTransform.InverseTransformPoint(splineTransform.TransformPoint(positions[i]));
                for (int i = 0; i < tangents.Count; i++)
                    tangents[i] = generatorTransform.InverseTransformDirection(splineTransform.TransformDirection(tangents[i]));
                for (int i = 0; i < normals.Count; i++)
                    normals[i] = generatorTransform.InverseTransformDirection(splineTransform.TransformDirection(normals[i]));
            }

            //OPTIM find a way to have the inputs already as arrays, instead of calling ToArray on them
            dataToFill.MaterialGroups.Add(materialGroup);
            dataToFill.SourceF = sourceFs.ToArray();
            dataToFill.F = relativeFs.ToArray();
            dataToFill.Position = positions.ToArray();

            if (isFullPath)
            {
                ((CGPath)dataToFill).Direction = tangents.ToArray();
                dataToFill.Normal = normals.ToArray();
            }
        }

        static private void AddPoint(float sourceF, float relativeF, bool isFullPath, Vector3 position, Vector3 tangent, Vector3 up, List<float> sourceFList, List<float> relativeFList, List<Vector3> positionList, List<Vector3> tangentList, List<Vector3> upList)
        {
#if CURVY_SANITY_CHECKS
            if (relativeF < 0 || relativeF > 1)
                DTLog.LogError("[Curvy] Invalid point's relativeF value " + relativeF);

            if (sourceF < 0 || sourceF > 1)
                DTLog.LogError("[Curvy] Invalid point's sourceF value " + sourceF);
#endif
            sourceFList.Add(sourceF);
            positionList.Add(position);
            relativeFList.Add(relativeF);
            if (isFullPath)
            {
                tangentList.Add(tangent);
                upList.Add(up);
            }
        }


        private static bool MoveByAngleExt(CurvySpline spline, bool useCache, ref float tf, float maxDistance, float maxAngle, out Vector3 pos, out Vector3 tan, float stopTF, bool loop, float stepDist)
        {
#if CURVY_SANITY_CHECKS
            Assert.IsTrue(maxAngle >= 0);
            Assert.IsTrue(spline.Count != 0);
#endif

            if (!loop)
                tf = Mathf.Clamp01(tf);
            float tn = (loop) ? tf % 1 : tf;
            float localF;
            CurvySplineSegment segment;

            segment = spline.TFToSegment(tn, out localF, CurvyClamping.Clamp);
            if (useCache)
                segment.InterpolateAndGetTangentFast(localF, out pos, out tan);
            else
                segment.InterpolateAndGetTangent(localF, out pos, out tan);
            Vector3 lastPos = pos;
            Vector3 lastTan = tan;

            float movedDistance = 0;
            float angleAccumulator = 0;

            if (stopTF < tf && loop)
                stopTF++;

            bool earlyExitConditionMet = false;
            while (tf < stopTF && earlyExitConditionMet == false)
            {
                tf = Mathf.Min(stopTF, tf + stepDist);
                tn = (loop) ? tf % 1 : tf;

                segment = spline.TFToSegment(tn, out localF, CurvyClamping.Clamp);
                if (useCache)
                    segment.InterpolateAndGetTangentFast(localF, out pos, out tan);
                else
                    segment.InterpolateAndGetTangent(localF, out pos, out tan);

                Vector3 movement;
                {
                    //Optimized way of substracting lastPos from pos. Optimization works with Mono platforms
                    movement.x = pos.x - lastPos.x;
                    movement.y = pos.y - lastPos.y;
                    movement.z = pos.z - lastPos.z;
                }
                movedDistance += movement.magnitude;

                float tangentsAngle = Vector3.Angle(lastTan, tan);
                angleAccumulator += tangentsAngle;

                // Check if conditions are met
                if (movedDistance >= maxDistance // max distance reached
                    || angleAccumulator >= maxAngle // max angle reached
                    || (tangentsAngle == 0 && angleAccumulator > 0))// current step is linear while the whole movement is not.
                    earlyExitConditionMet = true;
                else
                {
                    lastPos = pos;
                    lastTan = tan;
                }
            }

            return Mathf.Approximately(tf, stopTF);
        }

        #region CalculateExtendedUV



        static void CalculateExtendedUV(CurvySpline spline, float startTF, float endTF, List<SamplePointUData> ext, CGShape data)
        {
            // we have a list of data, either UV Edge (double then) or Explicit
            // unlike easy mode, U is bound to Shape's SourceF, not F!

            // for the first vertex, find the reference CP and calculate starting U (first vertex never has matching Udata, even if it's over a reference CP!!!)

            CurvySplineSegment refCCW, refCW;
            MetaCGOptions optCCW = findPreviousReferenceCPOptions(spline, startTF, out refCCW);
            MetaCGOptions optCW = findNextReferenceCPOptions(spline, startTF, out refCW);
            // we now know the U range the first vertex is in, so let's calculate it's actual U value
            // get the distance delta within that range
            float frag;
            // Special case: CW is first CP (implies closed spline)
            if (spline.FirstVisibleControlPoint == refCW)
                frag = ((data.SourceF[0] * spline.Length) - refCCW.Distance) / (spline.Length - refCCW.Distance);
            else
                frag = ((data.SourceF[0] * spline.Length) - refCCW.Distance) / (refCW.Distance - refCCW.Distance);
            ext.Insert(0, new SamplePointUData(0, (startTF == 0 && optCCW.UVEdge), frag * (optCW.FirstU - optCCW.GetDefinedFirstU(0)) + optCCW.GetDefinedFirstU(0), (startTF == 0 && optCCW.UVEdge) ? optCCW.SecondU : 0));

            // Do the same for the last vertex, find the reference CP and calculate starting U (first vertex never has matching Udata, even if it's over a reference CP!!!)
            if (ext[ext.Count - 1].Vertex < data.Count - 1)
            {
                optCCW = findPreviousReferenceCPOptions(spline, endTF, out refCCW);
                optCW = findNextReferenceCPOptions(spline, endTF, out refCW);
                float cwU = optCW.FirstU;
                float ccwU = optCCW.GetDefinedSecondU(0);
                // Special case: CW is first CP (implies closed spline)
                if (spline.FirstVisibleControlPoint == refCW)
                {
                    frag = ((data.SourceF[data.Count - 1] * spline.Length) - refCCW.Distance) / (spline.Length - refCCW.Distance);
                    // either take the ending U from 2nd U of first CP or raise last U to next int

                    if (optCW.UVEdge)
                        cwU = optCW.FirstU;
                    else if (ext.Count > 1)
                        cwU = Mathf.FloorToInt((ext[ext.Count - 1].UVEdge) ? ext[ext.Count - 1].SecondU : ext[ext.Count - 1].FirstU) + 1;
                    else
                        cwU = 1;
                }
                else
                {
                    frag = ((data.SourceF[data.Count - 1] * spline.Length) - refCCW.Distance) / (refCW.Distance - refCCW.Distance);
                }
                ext.Add(new SamplePointUData(data.Count - 1, false, frag * (cwU - ccwU) + ccwU, 0));
            }
            float startF = 0;
            float lo = (ext[0].UVEdge) ? ext[0].SecondU : ext[0].FirstU;
            float hi = ext[1].FirstU;
            float lenF = data.F[ext[1].Vertex] - data.F[ext[0].Vertex];
            int current = 1;
            //Debug.Log("lo=" + lo + ", hi=" + hi + ", length=" + lenF);
            for (int vt = 0; vt < data.Count - 1; vt++)
            {
                float curF = (data.F[vt] - startF) / lenF;
                //Debug.Log(vt + ":" + curF);
                data.Map[vt] = (hi - lo) * curF + lo;

                if (ext[current].Vertex == vt)
                {
                    // UVEdge?
                    if (ext[current].FirstU == ext[current + 1].FirstU)
                    {
                        lo = (ext[current].UVEdge) ? ext[current].SecondU : ext[current].FirstU;
                        current++;
                    }
                    else
                        lo = ext[current].FirstU;

                    hi = ext[current + 1].FirstU;
                    lenF = data.F[ext[current + 1].Vertex] - data.F[ext[current].Vertex];
                    startF = data.F[vt];
                    //Debug.Log("lo=" + lo + ", hi=" + hi + ", length=" + lenF);
                    current++;
                }
            }
            data.Map[data.Count - 1] = ext[ext.Count - 1].FirstU;
        }

        static MetaCGOptions findPreviousReferenceCPOptions(CurvySpline spline, float tf, out CurvySplineSegment cp)
        {
            MetaCGOptions options;
            cp = spline.TFToSegment(tf);
            do
            {
                options = cp.GetMetadata<MetaCGOptions>(true);
                if (spline.FirstVisibleControlPoint == cp)
                    return options;
                cp = spline.GetPreviousSegment(cp);
            }
            while (cp && !options.UVEdge && !options.ExplicitU && !options.HasDifferentMaterial);
            return options;
        }

        static MetaCGOptions findNextReferenceCPOptions(CurvySpline spline, float tf, out CurvySplineSegment cp)
        {
            MetaCGOptions options;
            float localF;
            cp = spline.TFToSegment(tf, out localF);

            do
            {
                cp = spline.GetNextControlPoint(cp);
                options = cp.GetMetadata<MetaCGOptions>(true);
                if (!spline.Closed && spline.LastVisibleControlPoint == cp)
                    return options;
            }
            while (!options.UVEdge && !options.ExplicitU && !options.HasDifferentMaterial && !(spline.FirstSegment == cp));
            return options;
        }
        #endregion

        #endregion

        #region Protected members

        protected abstract CurvySpline InputSpline
        {
            get;
            set;
        }

        protected virtual void OnSplineAssigned()
        {
            if (InputSpline)
            {
                InputSpline.OnRefresh.AddListenerOnce(OnSplineRefreshed);
                InputSpline.OnGlobalCoordinatesChanged += OnInputSplineCoordinatesChanged;
            }
        }

        protected void ValidateStartAndEndCps()
        {
            if (InputSpline == null)
                return;

            if (m_StartCP && m_StartCP.Spline != InputSpline)
            {
                DTLog.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Curvy] Input module {0}: StartCP is not part of the input spline ({1})", name, InputSpline.name));
                m_StartCP = null;
            }

            if (m_EndCP && m_EndCP.Spline != InputSpline)
            {
                DTLog.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Curvy] Input module {0}: EndCP is not part of the input spline ({1})", name, InputSpline.name));
                m_EndCP = null;
            }

            if (InputSpline.IsInitialized && m_EndCP != null && m_StartCP != null
                && InputSpline.GetControlPointIndex(m_EndCP) <= InputSpline.GetControlPointIndex(m_StartCP))
            {
                DTLog.LogError(string.Format(System.Globalization.CultureInfo.InvariantCulture, "[Curvy] Input module {0}: EndCP has an index ({1}) less or equal than StartCP ({2})", name, InputSpline.GetControlPointIndex(m_EndCP), InputSpline.GetControlPointIndex(m_StartCP)));
                m_EndCP = null;
            }
        }

        #endregion
    }
}
