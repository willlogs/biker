// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if UNITY_2018_3_OR_NEWER && (NET_STANDARD_2_0 || NET_4_6)
#define CSHARP_7_2_OR_NEWER
#endif

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.Curvy;
using FluffyUnderware.Curvy.Utils;
using System.Collections.Generic;
using FluffyUnderware.DevTools;
using UnityEngine.Serialization;
using System.Reflection;
using System;
using System.Collections.ObjectModel;
using System.Linq;
#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#endif
using JetBrains.Annotations;
using UnityEngine.Assertions;


namespace FluffyUnderware.Curvy
{

    /// <summary>
    /// Here you can find all the default values for CurvySplineSegment's serialized fields. If you don't find a field here, this means that it's type's default value is the same than the field's default value
    /// </summary>
    //TODO Use in the Reset method? That method needs to be reworked I think
    public static class CurvySplineSegmentDefaultValues
    {
        public const CurvyOrientationSwirl Swirl = CurvyOrientationSwirl.None;
        public const bool AutoHandles = true;
        public const float AutoHandleDistance = 0.39f;
        public static readonly Vector3 HandleIn = new Vector3(-1, 0, 0);
        public static readonly Vector3 HandleOut = new Vector3(1, 0, 0);
    }

    /// <summary>
    /// Class covering a Curvy Spline Segment / ControlPoint
    /// </summary>
    public partial class CurvySplineSegment : MonoBehaviour, IPoolable
    {

        #region ### Serialized Fields ###

        #region --- General ---

        [Group("General")]
        [FieldAction("CBBakeOrientation", Position = ActionAttribute.ActionPositionEnum.Below)]
        [Label("Bake Orientation", "Automatically apply orientation to CP transforms?")]
        [SerializeField]
        bool m_AutoBakeOrientation;

        [Group("General")]
        [Tooltip("Check to use this transform's rotation")]
        [FieldCondition("IsOrientationAnchorEditable", true)]
        [SerializeField]
        bool m_OrientationAnchor;

        [Label("Swirl", "Add Swirl to orientation?")]
        [Group("General")]
        [FieldCondition("canHaveSwirl", true)]
        [SerializeField]
        CurvyOrientationSwirl m_Swirl = CurvySplineSegmentDefaultValues.Swirl;

        [Label("Turns", "Number of swirl turns")]
        [Group("General")]
        [FieldCondition("canHaveSwirl", true, false, ConditionalAttribute.OperatorEnum.AND, "m_Swirl", CurvyOrientationSwirl.None, true)]
        [SerializeField]
        float m_SwirlTurns;

        #endregion

        #region --- Bezier ---

        [Section("Bezier Options", Sort = 1, HelpURL = CurvySpline.DOCLINK + "curvysplinesegment_bezier")]
        [GroupCondition("interpolation", CurvyInterpolation.Bezier)]
        [SerializeField]
        bool m_AutoHandles = CurvySplineSegmentDefaultValues.AutoHandles;

        [RangeEx(0, 1, "Distance %", "Handle length by distance to neighbours")]
        [FieldCondition("m_AutoHandles", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        [SerializeField]
        float m_AutoHandleDistance = CurvySplineSegmentDefaultValues.AutoHandleDistance;

        [VectorEx(Precision = 3, Options = AttributeOptionsFlags.Clipboard | AttributeOptionsFlags.Negate, Color = "#FFFF00")]
        [SerializeField, FormerlySerializedAs("HandleIn")]
        Vector3 m_HandleIn = CurvySplineSegmentDefaultValues.HandleIn;

        [VectorEx(Precision = 3, Options = AttributeOptionsFlags.Clipboard | AttributeOptionsFlags.Negate, Color = "#00FF00")]
        [SerializeField, FormerlySerializedAs("HandleOut")]
        Vector3 m_HandleOut = CurvySplineSegmentDefaultValues.HandleOut;

        #endregion

        #region --- TCB ---

        [Section("TCB Options", Sort = 1, HelpURL = CurvySpline.DOCLINK + "curvysplinesegment_tcb")]
        [GroupCondition("interpolation", CurvyInterpolation.TCB)]
        [GroupAction("TCBOptionsGUI", Position = ActionAttribute.ActionPositionEnum.Below)]

        [Label("Local Tension", "Override Spline Tension?")]
        [SerializeField, FormerlySerializedAs("OverrideGlobalTension")]
        bool m_OverrideGlobalTension;

        [Label("Local Continuity", "Override Spline Continuity?")]
        [SerializeField, FormerlySerializedAs("OverrideGlobalContinuity")]
        bool m_OverrideGlobalContinuity;

        [Label("Local Bias", "Override Spline Bias?")]
        [SerializeField, FormerlySerializedAs("OverrideGlobalBias")]
        bool m_OverrideGlobalBias;
        [Tooltip("Synchronize Start and End Values")]
        [SerializeField, FormerlySerializedAs("SynchronizeTCB")]
        bool m_SynchronizeTCB = true;
        [Label("Tension"), FieldCondition("m_OverrideGlobalTension", true)]
        [SerializeField, FormerlySerializedAs("StartTension")]
        float m_StartTension;

        [Label("Tension (End)"), FieldCondition("m_OverrideGlobalTension", true, false, ConditionalAttribute.OperatorEnum.AND, "m_SynchronizeTCB", false, false)]
        [SerializeField, FormerlySerializedAs("EndTension")]
        float m_EndTension;

        [Label("Continuity"), FieldCondition("m_OverrideGlobalContinuity", true)]
        [SerializeField, FormerlySerializedAs("StartContinuity")]
        float m_StartContinuity;

        [Label("Continuity (End)"), FieldCondition("m_OverrideGlobalContinuity", true, false, ConditionalAttribute.OperatorEnum.AND, "m_SynchronizeTCB", false, false)]
        [SerializeField, FormerlySerializedAs("EndContinuity")]
        float m_EndContinuity;

        [Label("Bias"), FieldCondition("m_OverrideGlobalBias", true)]
        [SerializeField, FormerlySerializedAs("StartBias")]
        float m_StartBias;

        [Label("Bias (End)"), FieldCondition("m_OverrideGlobalBias", true, false, ConditionalAttribute.OperatorEnum.AND, "m_SynchronizeTCB", false, false)]
        [SerializeField, FormerlySerializedAs("EndBias")]
        float m_EndBias;

        #endregion
        /*
#region --- CG Options ---
        
        /// <summary>
        /// Material ID (used by CG)
        /// </summary>
        [Section("Generator Options", true, Sort = 5, HelpURL = CurvySpline.DOCLINK + "curvysplinesegment_cg")]
        [Positive(Label="Material ID")]
        [SerializeField]
        int m_CGMaterialID;

        /// <summary>
        /// Whether to create a hard edge or not (used by PCG)
        /// </summary>
        [Label("Hard Edge")]
        [SerializeField]
        bool m_CGHardEdge;
        /// <summary>
        /// Maximum vertex distance when using optimization (0=infinite)
        /// </summary>
        [Positive(Label="Max Step Size",Tooltip="Max step distance when using optimization")]
        [SerializeField]
        float m_CGMaxStepDistance;
#endregion
        */
        #region --- Connections ---

        [SerializeField, HideInInspector]
        CurvySplineSegment m_FollowUp;
        [SerializeField, HideInInspector]
        ConnectionHeadingEnum m_FollowUpHeading = ConnectionHeadingEnum.Auto;
        //DESIGN: shouldn't these two be part of Connection? By spreading them on the ControlPoints, we risk a desynchronisation between m_ConnectionSyncPosition's value of a CP and the one of the connected CP
        [SerializeField, HideInInspector]
        bool m_ConnectionSyncPosition;
        [SerializeField, HideInInspector]
        bool m_ConnectionSyncRotation;

        [SerializeField, HideInInspector]
        CurvyConnection m_Connection;

        #endregion

        #endregion

        #region ### Private Fields ###

        private int cacheSize = -1;

        //Because Unity pre 2019 doesn't act like it is supposed to, I have to make two different codes for cachedTransform. Here is the issue:
        //cachedTransform is used as an optim. The idea is to get transform once at script's start, and then use it later. Execution order says that CurvySplineSegment runs before CurvySpline. So all CSS's OnEnable methods should run before CS's ones. But this is not the case in pre 2019. So you end up with CS's OnEnable accessing (through public members) to CSS's cachedTransform, which is still set to null because its OnEnable was not called yet.
#if (UNITY_2019_1_OR_NEWER)
        private Transform cachedTransform;
#else
        private Transform _cachedTransform;
        private Transform cachedTransform
        {
            get
            {
                if (ReferenceEquals(_cachedTransform, null))
                    _cachedTransform = transform;
                return _cachedTransform;
            }
            set
            {
                _cachedTransform = value;
            }
        }
#endif


        /// <summary>
        /// This exists because Transform can not be accessed in non main threads. So before refreshing the spline, we store the local position here so it can be accessed in multithread spline refreshing code
        /// </summary>
        /// <remarks>Warning: Make sure it is set with valid value before using it</remarks>
        private Vector3 threadSafeLocalPosition;
        /// <summary>
        /// Same as <see cref="threadSafeLocalPosition"/>, but for the next CP. Is equal to <see cref="threadSafeLocalPosition"/> if no next cp. Takes into consideration Follow-Ups if spline uses them to define its shape
        /// </summary>
        private Vector3 threadSafeNextCpLocalPosition;
        /// <summary>
        /// Same as <see cref="threadSafeLocalPosition"/>, but for the next CP. Is equal to <see cref="threadSafeLocalPosition"/> if no previous cp. Takes into consideration Follow-Ups if spline uses them to define its shape
        /// </summary>
        private Vector3 threadSafePreviousCpLocalPosition;
        /// <summary>
        /// This exists because Transform can not be accesed in non main threads. So before refreshing the spline, we store the local rotation here so it can be accessed in multithread spline refreshing code
        /// </summary>
        /// <remarks>Warning: Make sure it is set with valid value before using it</remarks>
        private Quaternion threadSafeLocalRotation;
        /// <summary>
        /// The cached result of Spline.GetNextControlPoint(this)
        /// OPTIM: use this more often?
        /// </summary>
        CurvySplineSegment cachedNextControlPoint;
        CurvySpline mSpline;
        float mStepSize;
        Bounds? mBounds;

        /// <summary>
        /// The Metadata components added to this GameObject
        /// </summary>
        private readonly HashSet<CurvyMetadataBase> mMetadata = new HashSet<CurvyMetadataBase>();
        /// <summary>
        /// The local position used in the segment approximations cache latest computation
        /// </summary>
        private Vector3 lastProcessedLocalPosition;
        /// <summary>
        /// The local rotation used in the segment approximations cache latest computation
        /// </summary>
        private Quaternion lastProcessedLocalRotation;

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */


        private void Awake()
        {
            //Happens when duplicating a spline that has a connection. This can be avoided
            if (Connection && Connection.ControlPointsList.Contains(this) == false)
                SetConnection(null);

            cachedTransform = transform;
            ReloadMetaData();
        }

        void OnEnable()
        {
            Awake();
        }

#if UNITY_EDITOR

        void OnDrawGizmos()
        {
            if (Spline && Spline.ShowGizmos)
            {
                bool willOnDrawGizmosSelectedGetCalled = false;
                Transform testedTransform = gameObject.transform;
                do
                {
                    willOnDrawGizmosSelectedGetCalled = Selection.Contains(testedTransform.gameObject.GetInstanceID());
                    testedTransform = testedTransform.parent;
                }
                while (!willOnDrawGizmosSelectedGetCalled && ReferenceEquals(testedTransform, null) == false);

                if (willOnDrawGizmosSelectedGetCalled == false)
                    doGizmos(false);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (Spline)
                doGizmos(true);
        }
#endif

        void OnDestroy()
        {
            //BUG? Why do we have that realDestroy boolean? Why not always do the same thing? This might hide something bad
            //When asked about this jake said:
            //That was quite a dirty hack as far as I remember, to counter issues with Unity's serialization
            //TBH I'm not sure if those issues still are present, so you might want to see if it's working without it now.
            bool realDestroy = true;
#if UNITY_EDITOR
            if (EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying)
                realDestroy = false;
#endif
            //Debug.Log("realDestroy " + realDestroy);
#if UNITY_EDITOR
            //mSpline is non null when the user delete only this CP. mSpline is null when the user deletes the spline, which then leads to this method to be called
            if (mSpline != null)
            {
                if (!Application.isPlaying &&
                    (CurvySpline._newSelectionInstanceIDINTERNAL == 0 || CurvySpline._newSelectionInstanceIDINTERNAL == GetInstanceID())
                    )
                {
                    if (Spline.GetPreviousControlPoint(this))
                        CurvySpline._newSelectionInstanceIDINTERNAL = Spline.GetPreviousControlPoint(this).GetInstanceID();
                    else if (Spline.GetNextControlPoint(this))
                        CurvySpline._newSelectionInstanceIDINTERNAL = Spline.GetNextControlPoint(this).GetInstanceID();
                    else
                        CurvySpline._newSelectionInstanceIDINTERNAL = mSpline.GetInstanceID();
                }
            }
#endif
            if (realDestroy)
            {
                Disconnect();
            }
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            //Debug.Log("    OnValidate " + name);
            SetAutoHandles(m_AutoHandles);
            SetConnection(m_Connection);
            if (mSpline != null)
            {
                Spline.SetDirtyAll(SplineDirtyingType.Everything, true);
                Spline.InvalidateControlPointsRelationshipCacheINTERNAL();
            }
        }

#endif

        /// <summary>
        /// Resets the properties of this control point, but will not remove its Connection if it has any.
        /// </summary>
        public void Reset()
        {
            m_OrientationAnchor = false;
            m_Swirl = CurvyOrientationSwirl.None;
            m_SwirlTurns = 0;
            // Bezier
            m_AutoHandles = true;
            m_AutoHandleDistance = 0.39f;
            m_HandleIn = new Vector3(-1, 0, 0);
            m_HandleOut = new Vector3(1, 0, 0);
            // TCB
            m_SynchronizeTCB = true;
            m_OverrideGlobalTension = false;
            m_OverrideGlobalContinuity = false;
            m_OverrideGlobalBias = false;
            m_StartTension = 0;
            m_EndTension = 0;
            m_StartContinuity = 0;
            m_EndContinuity = 0;
            m_StartBias = 0;
            m_EndBias = 0;
            if (mSpline)
            {
                Spline.SetDirty(this, SplineDirtyingType.Everything);
                Spline.InvalidateControlPointsRelationshipCacheINTERNAL();
            }
        }
        /*! \endcond */
        #endregion

        #region ### Privates & Internals ###
        /*! \cond PRIVATE */

        #region Properties used in inspector's field condition and group condition

        // Used as a group condition
        private CurvyInterpolation interpolation
        {
            get { return Spline ? Spline.Interpolation : CurvyInterpolation.Linear; }
        }

        // Used as a field condition
        private bool isDynamicOrientation
        {
            get { return Spline && Spline.Orientation == CurvyOrientation.Dynamic; }
        }

        // Used as a field condition
        private bool IsOrientationAnchorEditable
        {
            get
            {
                CurvySpline curvySpline = Spline;
                return isDynamicOrientation && curvySpline.IsControlPointVisible(this) && curvySpline.FirstVisibleControlPoint != this && curvySpline.LastVisibleControlPoint != this;
            }
        }

        // Used as a field condition
        private bool canHaveSwirl
        {
            get
            {
                CurvySpline curvySpline = Spline;
                return isDynamicOrientation && curvySpline && curvySpline.IsControlPointAnOrientationAnchor(this) && (curvySpline.Closed || curvySpline.LastVisibleControlPoint != this);
            }
        }

        #endregion


        private ControlPointExtrinsicProperties extrinsicPropertiesINTERNAL;

        /// <summary>
        /// Properties describing the relationship between this CurvySplineSegment and its containing CurvySpline.
        /// </summary>
        internal void SetExtrinsicPropertiesINTERNAL(ControlPointExtrinsicProperties value)
        {
            extrinsicPropertiesINTERNAL = value;
        }

        internal
#if CSHARP_7_2_OR_NEWER
            ref readonly
#endif
            ControlPointExtrinsicProperties GetExtrinsicPropertiesINTERNAL()
        {
            return
#if CSHARP_7_2_OR_NEWER
                ref
#endif
                    extrinsicPropertiesINTERNAL;
        }

        private void CheckAgainstMetaDataDuplication()
        {
            if (Metadata.Count > 1)
            {
                HashSet<Type> metaDataTypes = new HashSet<Type>();
                foreach (CurvyMetadataBase metaData in Metadata)
                {
                    Type componentType = metaData.GetType();
                    if (metaDataTypes.Contains(componentType))
                        DTLog.LogWarning(String.Format("[Curvy] Game object '{0}' has multiple Components of type '{1}'. Control Points should have no more than one Component instance for each MetaData type.", this.ToString(), componentType));
                    else
                        metaDataTypes.Add(componentType);
                }
            }
        }

        /// <summary>
        /// Sets the connection handler this Control Point is using
        /// </summary>
        /// <param name="newConnection"></param>
        /// <returns>Whether a modification was done or not</returns>
        /// <remarks>If set to null, FollowUp wil be set to null to</remarks>
        private bool SetConnection(CurvyConnection newConnection)
        {
            bool modificationDone = false;
            if (m_Connection != newConnection)
            {
                modificationDone = true;
                m_Connection = newConnection;
            }
            if (m_Connection == null && m_FollowUp != null)
            {
                modificationDone = true;
                m_FollowUp = null;
            }
            return modificationDone;
        }

        /// <summary>
        /// Returns a different ConnectionHeadingEnum value when connectionHeading has a value that is no more valid in the context of this spline. For example, heading to start (Minus) when there is no previous CP
        /// </summary>
        private static ConnectionHeadingEnum GetValidateConnectionHeading(ConnectionHeadingEnum connectionHeading, [CanBeNull]CurvySplineSegment followUp)
        {
            if (followUp == null)
                return connectionHeading;

            if ((connectionHeading == ConnectionHeadingEnum.Minus && CanFollowUpHeadToStart(followUp) == false)
                || (connectionHeading == ConnectionHeadingEnum.Plus && CanFollowUpHeadToEnd(followUp) == false))
                return ConnectionHeadingEnum.Auto;

            return connectionHeading;
        }

        /// <summary>
        /// Sets Auto Handles. When setting it the value of connected control points is also updated
        /// </summary>
        /// <param name="newValue"></param>
        /// <returns>Whether a modifcation was done or not</returns>
        private bool SetAutoHandles(bool newValue)
        {
            bool modificationDone = false;
            if (Connection)
            {
                ReadOnlyCollection<CurvySplineSegment> controlPoints = Connection.ControlPointsList;
                for (int index = 0; index < controlPoints.Count; index++)
                {
                    CurvySplineSegment controlPoint = controlPoints[index];
                    modificationDone = modificationDone || controlPoint.m_AutoHandles != newValue;
                    controlPoint.m_AutoHandles = newValue;
                }
            }
            else
            {
                modificationDone = m_AutoHandles != newValue;
                m_AutoHandles = newValue;
            }
            return modificationDone;
        }

        /// <summary>
        /// Internal, Gets localF by an index of mApproximation
        /// </summary>
        float getApproximationLocalF(int idx)
        {
            return idx * mStepSize;
        }

        #region approximations cache computation

        internal void refreshCurveINTERNAL()
        {
            CurvySpline spline = Spline;
            bool isControlPointASegment = spline.IsControlPointASegment(this);
            int newCacheSize;
            if (isControlPointASegment)
            {
#if CURVY_SANITY_CHECKS
                Assert.IsNotNull(cachedNextControlPoint);
#endif
                newCacheSize = CurvySpline.CalculateCacheSize(
                    spline.CacheDensity,
                    (cachedNextControlPoint.threadSafeLocalPosition - threadSafeLocalPosition).magnitude,
                    spline.MaxPointsPerUnit);
            }
            else
                newCacheSize = 0;

            CacheSize = newCacheSize;
            Array.Resize(ref Approximation, newCacheSize + 1);
            Array.Resize(ref ApproximationT, newCacheSize + 1);
            Array.Resize(ref ApproximationDistances, newCacheSize + 1);
            Array.Resize(ref ApproximationUp, newCacheSize + 1);

            Approximation[0] = threadSafeLocalPosition;
            ApproximationDistances[0] = 0;
            //  ApproximationT[0] and ApproximationUp[0] are handled later
            mBounds = null;
            Length = 0;
            mStepSize = 1f / newCacheSize;


            bool hasNextControlPoint = ReferenceEquals(cachedNextControlPoint, null) == false;
            if (newCacheSize != 0)
                Approximation[newCacheSize] = hasNextControlPoint
                    ? cachedNextControlPoint.threadSafeLocalPosition
                    : threadSafeLocalPosition;

            if (isControlPointASegment)
            {
                float segmentLength = 0;
                switch (spline.Interpolation)
                {
                    case CurvyInterpolation.Bezier:
                        segmentLength = InterpolateBezierSegment(cachedNextControlPoint, newCacheSize);
                        break;
                    case CurvyInterpolation.CatmullRom:
                        segmentLength = InterpolateCatmullSegment(cachedNextControlPoint, newCacheSize);
                        break;
                    case CurvyInterpolation.TCB:
                        segmentLength = InterpolateTCBSegment(cachedNextControlPoint, newCacheSize, spline.Tension, spline.Continuity, spline.Bias);
                        break;
                    case CurvyInterpolation.Linear:
                        segmentLength = InterpolateLinearSegment(cachedNextControlPoint, newCacheSize);
                        break;
                    default:
                        DTLog.LogError("[Curvy] Invalid interpolation value " + spline.Interpolation);
                        break;
                }
                Length = segmentLength;

                Vector3 tangent = Approximation[newCacheSize] - Approximation[newCacheSize - 1];
                Length += tangent.magnitude;
                ApproximationDistances[newCacheSize] = Length;
                ApproximationT[newCacheSize - 1] = tangent.normalized;
                // ApproximationT[cacheSize] is set in Spline's Refresh method
                ApproximationT[newCacheSize] = ApproximationT[newCacheSize - 1];
            }
            else
            {
                if (hasNextControlPoint)
                    ApproximationT[0] = (cachedNextControlPoint.threadSafeLocalPosition - Approximation[0]).normalized;
                else
                {
                    short previousControlPointIndex = spline.GetPreviousControlPointIndex(this);
                    if (previousControlPointIndex != -1)
                        ApproximationT[0] = (Approximation[0] - spline.ControlPointsList[previousControlPointIndex].threadSafeLocalPosition).normalized;
                    else
                        ApproximationT[0] = threadSafeLocalRotation * Vector3.forward;
                }

                //Last visible control point gets the last tangent from the previous segment. This is done in Spline's Refresh method 

            }

            lastProcessedLocalPosition = threadSafeLocalPosition;
        }


        #region Inlined code for optimization


        private float InterpolateBezierSegment(CurvySplineSegment nextControlPoint, int newCacheSize)
        {
            //The following code is the inlined version of this code:
            //        for (int i = 1; i < CacheSize; i++)
            //        {
            //            Approximation[i] = interpolateBezier(i * mStepSize);
            //            t = (Approximation[i] - Approximation[i - 1]);
            //            Length += t.magnitude;
            //            ApproximationDistances[i] = Length;
            //            ApproximationT[i - 1] = t.normalized;
            //        }

            float lengthAccumulator = 0;
            CurvySplineSegment ncp = nextControlPoint;
            Vector3 p0 = threadSafeLocalPosition;
            Vector3 t0 = p0 + HandleOut;
            Vector3 p1 = ncp.threadSafeLocalPosition;
            Vector3 t1 = p1 + ncp.HandleIn;

            const double Ft2 = 3;
            const double Ft3 = -3;
            const double Fu1 = 3;
            const double Fu2 = -6;
            const double Fu3 = 3;
            const double Fv1 = -3;
            const double Fv2 = 3;

            double FAX = -p0.x + Ft2 * t0.x + Ft3 * t1.x + p1.x;
            double FBX = Fu1 * p0.x + Fu2 * t0.x + Fu3 * t1.x;
            double FCX = Fv1 * p0.x + Fv2 * t0.x;
            double FDX = p0.x;

            double FAY = -p0.y + Ft2 * t0.y + Ft3 * t1.y + p1.y;
            double FBY = Fu1 * p0.y + Fu2 * t0.y + Fu3 * t1.y;
            double FCY = Fv1 * p0.y + Fv2 * t0.y;
            double FDY = p0.y;

            double FAZ = -p0.z + Ft2 * t0.z + Ft3 * t1.z + p1.z;
            double FBZ = Fu1 * p0.z + Fu2 * t0.z + Fu3 * t1.z;
            double FCZ = Fv1 * p0.z + Fv2 * t0.z;
            double FDZ = p0.z;
            Vector3 tangent;
            for (int i = 1; i < newCacheSize; i++)
            {
                float localF = i * mStepSize;

                Approximation[i].x = (float)(((FAX * localF + FBX) * localF + FCX) * localF + FDX);
                Approximation[i].y = (float)(((FAY * localF + FBY) * localF + FCY) * localF + FDY);
                Approximation[i].z = (float)(((FAZ * localF + FBZ) * localF + FCZ) * localF + FDZ);

                tangent.x = (Approximation[i].x - Approximation[i - 1].x);
                tangent.y = (Approximation[i].y - Approximation[i - 1].y);
                tangent.z = (Approximation[i].z - Approximation[i - 1].z);

                float tMagnitude = Mathf.Sqrt((float)(tangent.x * tangent.x + tangent.y * tangent.y + tangent.z * tangent.z));
                lengthAccumulator += tMagnitude;
                ApproximationDistances[i] = lengthAccumulator;
                if ((double)tMagnitude > 9.99999974737875E-06)
                {
                    float oneOnMagnitude = 1 / tMagnitude;
                    ApproximationT[i - 1].x = tangent.x * oneOnMagnitude;
                    ApproximationT[i - 1].y = tangent.y * oneOnMagnitude;
                    ApproximationT[i - 1].z = tangent.z * oneOnMagnitude;
                }
                else
                {
                    ApproximationT[i - 1].x = 0;
                    ApproximationT[i - 1].y = 0;
                    ApproximationT[i - 1].z = 0;
                }
            }
            return lengthAccumulator;
        }


        private float InterpolateTCBSegment(CurvySplineSegment nextControlPoint, int newCacheSize, float splineTension, float splineContinuity, float splineBias)
        {
            //The following code is the inlined version of this code:
            //        for (int i = 1; i < CacheSize; i++)
            //        {
            //            Approximation[i] = interpolateCatmull(i * mStepSize);
            //            t = (Approximation[i] - Approximation[i - 1]);
            //            Length += t.magnitude;
            //            ApproximationDistances[i] = Length;
            //            ApproximationT[i - 1] = t.normalized;
            //        }

            float lengthAccumulator = 0;

            float ft0 = StartTension;
            float ft1 = EndTension;
            float fc0 = StartContinuity;
            float fc1 = EndContinuity;
            float fb0 = StartBias;
            float fb1 = EndBias;

            if (!OverrideGlobalTension)
                ft0 = ft1 = splineTension;
            if (!OverrideGlobalContinuity)
                fc0 = fc1 = splineContinuity;
            if (!OverrideGlobalBias)
                fb0 = fb1 = splineBias;

            Vector3 p0 = threadSafeLocalPosition;
            Vector3 p1 = threadSafeNextCpLocalPosition;
            Vector3 t0 = threadSafePreviousCpLocalPosition;
            Vector3 t1 = nextControlPoint.threadSafeNextCpLocalPosition;

            double FFA = (1 - ft0) * (1 + fc0) * (1 + fb0);
            double FFB = (1 - ft0) * (1 - fc0) * (1 - fb0);
            double FFC = (1 - ft1) * (1 - fc1) * (1 + fb1);
            double FFD = (1 - ft1) * (1 + fc1) * (1 - fb1);

            double DD = 2;
            double Ft1 = -FFA / DD;
            double Ft2 = (+4 + FFA - FFB - FFC) / DD;
            double Ft3 = (-4 + FFB + FFC - FFD) / DD;
            double Ft4 = FFD / DD;
            double Fu1 = +2 * FFA / DD;
            double Fu2 = (-6 - 2 * FFA + 2 * FFB + FFC) / DD;
            double Fu3 = (+6 - 2 * FFB - FFC + FFD) / DD;
            double Fu4 = -FFD / DD;
            double Fv1 = -FFA / DD;
            double Fv2 = (FFA - FFB) / DD;
            double Fv3 = FFB / DD;
            double Fw2 = +2 / DD;

            double FAX = Ft1 * t0.x + Ft2 * p0.x + Ft3 * p1.x + Ft4 * t1.x;
            double FBX = Fu1 * t0.x + Fu2 * p0.x + Fu3 * p1.x + Fu4 * t1.x;
            double FCX = Fv1 * t0.x + Fv2 * p0.x + Fv3 * p1.x;
            double FDX = Fw2 * p0.x;

            double FAY = Ft1 * t0.y + Ft2 * p0.y + Ft3 * p1.y + Ft4 * t1.y;
            double FBY = Fu1 * t0.y + Fu2 * p0.y + Fu3 * p1.y + Fu4 * t1.y;
            double FCY = Fv1 * t0.y + Fv2 * p0.y + Fv3 * p1.y;
            double FDY = Fw2 * p0.y;

            double FAZ = Ft1 * t0.z + Ft2 * p0.z + Ft3 * p1.z + Ft4 * t1.z;
            double FBZ = Fu1 * t0.z + Fu2 * p0.z + Fu3 * p1.z + Fu4 * t1.z;
            double FCZ = Fv1 * t0.z + Fv2 * p0.z + Fv3 * p1.z;
            double FDZ = Fw2 * p0.z;
            Vector3 tangent;
            for (int i = 1; i < newCacheSize; i++)
            {
                float localF = i * mStepSize;

                Approximation[i].x = (float)(((FAX * localF + FBX) * localF + FCX) * localF + FDX);
                Approximation[i].y = (float)(((FAY * localF + FBY) * localF + FCY) * localF + FDY);
                Approximation[i].z = (float)(((FAZ * localF + FBZ) * localF + FCZ) * localF + FDZ);

                tangent.x = (Approximation[i].x - Approximation[i - 1].x);
                tangent.y = (Approximation[i].y - Approximation[i - 1].y);
                tangent.z = (Approximation[i].z - Approximation[i - 1].z);

                float tMagnitude = Mathf.Sqrt((float)(tangent.x * tangent.x + tangent.y * tangent.y + tangent.z * tangent.z));
                lengthAccumulator += tMagnitude;
                ApproximationDistances[i] = lengthAccumulator;
                if ((double)tMagnitude > 9.99999974737875E-06)
                {
                    float oneOnMagnitude = 1 / tMagnitude;
                    ApproximationT[i - 1].x = tangent.x * oneOnMagnitude;
                    ApproximationT[i - 1].y = tangent.y * oneOnMagnitude;
                    ApproximationT[i - 1].z = tangent.z * oneOnMagnitude;
                }
                else
                {
                    ApproximationT[i - 1].x = 0;
                    ApproximationT[i - 1].y = 0;
                    ApproximationT[i - 1].z = 0;
                }
            }
            return lengthAccumulator;
        }

        private float InterpolateCatmullSegment(CurvySplineSegment nextControlPoint, int newCacheSize)
        {
            //The following code is the inlined version of this code:
            //        for (int i = 1; i < CacheSize; i++)
            //        {
            //            Approximation[i] = interpolateTCB(i * mStepSize);
            //            t = (Approximation[i] - Approximation[i - 1]);
            //            Length += t.magnitude;
            //            ApproximationDistances[i] = Length;
            //            ApproximationT[i - 1] = t.normalized;
            //        }

            float lengthAccumulator = 0;

            Vector3 p0 = threadSafeLocalPosition;
            Vector3 p1 = threadSafeNextCpLocalPosition;
            Vector3 t0 = threadSafePreviousCpLocalPosition;
            Vector3 t1 = nextControlPoint.threadSafeNextCpLocalPosition;

            const double Ft1 = -0.5;
            const double Ft2 = 1.5;
            const double Ft3 = -1.5;
            const double Ft4 = 0.5;
            const double Fu2 = -2.5;
            const double Fu3 = 2;
            const double Fu4 = -0.5;
            const double Fv1 = -0.5;
            const double Fv3 = 0.5;

            double FAX = Ft1 * t0.x + Ft2 * p0.x + Ft3 * p1.x + Ft4 * t1.x;
            double FBX = t0.x + Fu2 * p0.x + Fu3 * p1.x + Fu4 * t1.x;
            double FCX = Fv1 * t0.x + Fv3 * p1.x;
            double FDX = p0.x;

            double FAY = Ft1 * t0.y + Ft2 * p0.y + Ft3 * p1.y + Ft4 * t1.y;
            double FBY = t0.y + Fu2 * p0.y + Fu3 * p1.y + Fu4 * t1.y;
            double FCY = Fv1 * t0.y + Fv3 * p1.y;
            double FDY = p0.y;

            double FAZ = Ft1 * t0.z + Ft2 * p0.z + Ft3 * p1.z + Ft4 * t1.z;
            double FBZ = t0.z + Fu2 * p0.z + Fu3 * p1.z + Fu4 * t1.z;
            double FCZ = Fv1 * t0.z + Fv3 * p1.z;
            double FDZ = p0.z;
            Vector3 tangent;
            for (int i = 1; i < newCacheSize; i++)
            {
                float localF = i * mStepSize;

                Approximation[i].x = (float)(((FAX * localF + FBX) * localF + FCX) * localF + FDX);
                Approximation[i].y = (float)(((FAY * localF + FBY) * localF + FCY) * localF + FDY);
                Approximation[i].z = (float)(((FAZ * localF + FBZ) * localF + FCZ) * localF + FDZ);

                tangent.x = (Approximation[i].x - Approximation[i - 1].x);
                tangent.y = (Approximation[i].y - Approximation[i - 1].y);
                tangent.z = (Approximation[i].z - Approximation[i - 1].z);

                float tMagnitude = Mathf.Sqrt((float)(tangent.x * tangent.x + tangent.y * tangent.y + tangent.z * tangent.z));
                lengthAccumulator += tMagnitude;
                ApproximationDistances[i] = lengthAccumulator;
                if ((double)tMagnitude > 9.99999974737875E-06)
                {
                    float oneOnMagnitude = 1 / tMagnitude;
                    ApproximationT[i - 1].x = tangent.x * oneOnMagnitude;
                    ApproximationT[i - 1].y = tangent.y * oneOnMagnitude;
                    ApproximationT[i - 1].z = tangent.z * oneOnMagnitude;
                }
                else
                {
                    ApproximationT[i - 1].x = 0;
                    ApproximationT[i - 1].y = 0;
                    ApproximationT[i - 1].z = 0;
                }
            }
            return lengthAccumulator;
        }

        private float InterpolateLinearSegment(CurvySplineSegment nextControlPoint, int newCacheSize)
        {
            //The following code is the inlined version of this code:
            //        for (int i = 1; i < CacheSize; i++)
            //        {
            //            Approximation[i] = interpolateLinear(i * mStepSize);
            //            t = (Approximation[i] - Approximation[i - 1]);
            //            Length += t.magnitude;
            //            ApproximationDistances[i] = Length;
            //            ApproximationT[i - 1] = t.normalized;
            //        }

            float lengthAccumulator = 0;
            Vector3 pStart = threadSafeLocalPosition;
            Vector3 pEnd = nextControlPoint.threadSafeLocalPosition;
            Vector3 tangent;
            for (int i = 1; i < newCacheSize; i++)
            {
                float localF = i * mStepSize;
                Approximation[i] = OptimizedOperators.LerpUnclamped(pStart, pEnd, localF);

                tangent.x = (Approximation[i].x - Approximation[i - 1].x);
                tangent.y = (Approximation[i].y - Approximation[i - 1].y);
                tangent.z = (Approximation[i].z - Approximation[i - 1].z);

                float tMagnitude = Mathf.Sqrt((float)(tangent.x * tangent.x + tangent.y * tangent.y + tangent.z * tangent.z));
                lengthAccumulator += tMagnitude;
                ApproximationDistances[i] = lengthAccumulator;
                if ((double)tMagnitude > 9.99999974737875E-06)
                {
                    float oneOnMagnitude = 1 / tMagnitude;
                    ApproximationT[i - 1].x = tangent.x * oneOnMagnitude;
                    ApproximationT[i - 1].y = tangent.y * oneOnMagnitude;
                    ApproximationT[i - 1].z = tangent.z * oneOnMagnitude;
                }
                else
                {
                    ApproximationT[i - 1].x = 0;
                    ApproximationT[i - 1].y = 0;
                    ApproximationT[i - 1].z = 0;
                }
            }
            return lengthAccumulator;
        }

        #endregion

        internal void refreshOrientationNoneINTERNAL()
        {
            Array.Clear(ApproximationUp, 0, ApproximationUp.Length);
            lastProcessedLocalRotation = threadSafeLocalRotation;
        }

        internal void refreshOrientationStaticINTERNAL()
        {
            Vector3 firstUp = ApproximationUp[0] = getOrthoUp0INTERNAL();
            if (Approximation.Length > 1)
            {
                int cachedCachesize = CacheSize;
                Vector3 lastUp = ApproximationUp[cachedCachesize] = getOrthoUp1INTERNAL();
                float oneOnCachSize = 1f / cachedCachesize;
                for (int i = 1; i < cachedCachesize; i++)
                    ApproximationUp[i] = Vector3.SlerpUnclamped(firstUp, lastUp, i * oneOnCachSize);
            }

            lastProcessedLocalRotation = threadSafeLocalRotation;
        }

        /// <summary>
        /// Set each point's up as the initialUp rotated by the same rotation than the one that rotates initial tangent to the point's tangent
        /// </summary>
        /// <remarks>Does not handle swirl</remarks>
        /// <param name="initialUp"></param>
        internal void refreshOrientationDynamicINTERNAL(Vector3 initialUp)
        {
            int upsLength = ApproximationUp.Length;
            ApproximationUp[0] = initialUp;
            for (int i = 1; i < upsLength; i++)
            {
                //Inlined version of ups[i] = DTMath.ParallelTransportFrame(ups[i-1], tangents[i - 1], tangents[i]) and with less checks for performance reasons
                Vector3 tan0 = ApproximationT[i - 1];
                Vector3 tan1 = ApproximationT[i];
                //Inlined version of Vector3 A = Vector3.Cross(tan0, tan1);
                Vector3 A;
                {
                    A.x = tan0.y * tan1.z - tan0.z * tan1.y;
                    A.y = tan0.z * tan1.x - tan0.x * tan1.z;
                    A.z = tan0.x * tan1.y - tan0.y * tan1.x;
                }
                //Inlined version of float a = (float)Math.Atan2(A.magnitude, Vector3.Dot(tan0, tan1));
                float a = (float)Math.Atan2(
                    Math.Sqrt(A.x * A.x + A.y * A.y + A.z * A.z),
                    tan0.x * tan1.x + tan0.y * tan1.y + tan0.z * tan1.z);
                ApproximationUp[i] = Quaternion.AngleAxis(Mathf.Rad2Deg * a, A) * ApproximationUp[i - 1];
            }

            lastProcessedLocalRotation = threadSafeLocalRotation;
        }

        #endregion

        internal void ClearBoundsINTERNAL()
        {
            mBounds = null;
        }

        /// <summary>
        /// Gets Transform.up orthogonal to ApproximationT[0]
        /// </summary>
        internal Vector3 getOrthoUp0INTERNAL()
        {
            Vector3 u = threadSafeLocalRotation * Vector3.up;
            Vector3.OrthoNormalize(ref ApproximationT[0], ref u);
            return u;
        }

        private Vector3 getOrthoUp1INTERNAL()
        {
            CurvySplineSegment nextControlPoint = Spline.GetNextControlPoint(this);
            Quaternion nextRotation = nextControlPoint
                ? nextControlPoint.threadSafeLocalRotation
                : threadSafeLocalRotation;
            Vector3 u = nextRotation * Vector3.up;
            Vector3.OrthoNormalize(ref ApproximationT[CacheSize], ref u);
            return u;
        }

        internal void UnsetFollowUpWithoutDirtyingINTERNAL()
        {
            m_FollowUp = null;
            m_FollowUpHeading = ConnectionHeadingEnum.Auto;
        }

#if UNITY_EDITOR
        void doGizmos(bool selected)
        {
            if (CurvyGlobalManager.Gizmos == CurvySplineGizmos.None)
                return;

            // Skip if the segment isn't in view
            Camera cam = Camera.current;
            bool camAware = (cam != null);
            float camDistance = 0;
            if (camAware)
            {
                if (!cam.BoundsInView(Bounds))
                    return;

                camDistance = (cam.transform.position - Bounds.ClosestPoint(cam.transform.position)).magnitude;
            }

            bool viewCurve = (CurvyGlobalManager.Gizmos & CurvySplineGizmos.Curve) == CurvySplineGizmos.Curve;


            CurvyGizmoHelper.Matrix = Spline.transform.localToWorldMatrix;

            // Connections
            if (Connection)
                CurvyGizmoHelper.ConnectionGizmo(this);//OPTIM the gizmo is drawn for each cp of the connection, leading to overdrawing

            // Control Point
            if (viewCurve)
            {
                CurvyGizmoHelper.ControlPointGizmo(this, selected, (selected) ? Spline.GizmoSelectionColor : Spline.GizmoColor);
            }

            if (!Spline.IsControlPointASegment(this))
                return;

            if (Spline.Dirty)
                Spline.Refresh();

            if (viewCurve)
            {
                float steps = 20;
                if (camAware)
                {
                    float df = Mathf.Clamp(camDistance, 1, 3000) / 3000;
                    df = (df < 0.01f) ? DTTween.SineOut(df, 0, 1) : DTTween.QuintOut(df, 0, 1);
                    steps = Mathf.Clamp((Length * CurvyGlobalManager.SceneViewResolution * 0.1f) / df, 1, 10000);
                }
                CurvyGizmoHelper.SegmentCurveGizmo(this, (selected) ? Spline.GizmoSelectionColor : Spline.GizmoColor, 1 / steps);
            }

            if (Approximation.Length > 0 && (CurvyGlobalManager.Gizmos & CurvySplineGizmos.Approximation) == CurvySplineGizmos.Approximation)
                CurvyGizmoHelper.SegmentApproximationGizmo(this, Spline.GizmoColor * 0.8f);

            if (Spline.Orientation != CurvyOrientation.None && ApproximationUp.Length > 0 && (CurvyGlobalManager.Gizmos & CurvySplineGizmos.Orientation) == CurvySplineGizmos.Orientation)
            {
                CurvyGizmoHelper.SegmentOrientationGizmo(this, CurvyGlobalManager.GizmoOrientationColor);
                if (Spline.IsControlPointAnOrientationAnchor(this) && Spline.Orientation == CurvyOrientation.Dynamic)
                    CurvyGizmoHelper.SegmentOrientationAnchorGizmo(this, CurvyGlobalManager.GizmoOrientationColor);
            }

            if (ApproximationT.Length > 0 && (CurvyGlobalManager.Gizmos & CurvySplineGizmos.Tangents) == CurvySplineGizmos.Tangents)
                CurvyGizmoHelper.SegmentTangentGizmo(this, GizmoTangentColor);
        }
#endif

        /// <summary>
        /// Set the correct values to the thread safe local positions and rotation
        /// When multithreading, you can't access Transform in the not main threads. Here we cache that data so it is available for threads
        /// </summary>
        internal void PrepareThreadCompatibleDataINTERNAL(bool useFollowUp)
        {
            CurvySpline spline = Spline;
            CurvySplineSegment previousCP = spline.GetPreviousControlPoint(this);
            CurvySplineSegment nextCP = spline.GetNextControlPoint(this);

            //TODO: get rid of this the day you will be able to access transforms in threads
            threadSafeLocalPosition = cachedTransform.localPosition;
            threadSafeLocalRotation = cachedTransform.localRotation;

            //This isn't cached for thread compatibility, but for performance
            cachedNextControlPoint = nextCP;

            if (useFollowUp)
            {
                CurvySplineSegment followUpPreviousCP;
                bool hasFollowUp = FollowUp != null;
                if (hasFollowUp && ReferenceEquals(spline.FirstVisibleControlPoint, this))
                    followUpPreviousCP = CurvySpline.GetFollowUpHeadingControlPoint(FollowUp, this.FollowUpHeading);
                else
                    followUpPreviousCP = previousCP;
                CurvySplineSegment followUpNextCP;
                if (hasFollowUp && ReferenceEquals(spline.LastVisibleControlPoint, this))
                    followUpNextCP = CurvySpline.GetFollowUpHeadingControlPoint(FollowUp, this.FollowUpHeading);
                else
                    followUpNextCP = nextCP;

                if (ReferenceEquals(followUpPreviousCP, null) == false)
                {
                    threadSafePreviousCpLocalPosition = ReferenceEquals(followUpPreviousCP.Spline, spline) ?
                        followUpPreviousCP.cachedTransform.localPosition :
                        spline.transform.InverseTransformPoint(followUpPreviousCP.cachedTransform.position);
                }
                else
                    threadSafePreviousCpLocalPosition = threadSafeLocalPosition;

                if (ReferenceEquals(followUpNextCP, null) == false)
                {
                    threadSafeNextCpLocalPosition = ReferenceEquals(followUpNextCP.Spline, spline) ?
                        followUpNextCP.cachedTransform.localPosition :
                        spline.transform.InverseTransformPoint(followUpNextCP.cachedTransform.position);
                }
                else
                    threadSafeNextCpLocalPosition = threadSafeLocalPosition;
            }
            else
            {
                threadSafePreviousCpLocalPosition = ReferenceEquals(previousCP, null) == false ? previousCP.cachedTransform.localPosition :
                    threadSafeLocalPosition;

                threadSafeNextCpLocalPosition = ReferenceEquals(nextCP, null) == false ? nextCP.cachedTransform.localPosition :
                    threadSafeLocalPosition;
            }
        }

        /*! \endcond */
        #endregion

        /// <summary>
        /// Contains data about a control point related to it's parent spline. For example, is a control point a valid segment in the spline or not.
        /// </summary>
#if CSHARP_7_2_OR_NEWER
        readonly
#endif
        internal struct ControlPointExtrinsicProperties : IEquatable<ControlPointExtrinsicProperties>
        {
            private readonly bool isVisible;
            /// <summary>
            /// Is the control point part of a segment (whether starting it or ending it)
            /// </summary>
            internal bool IsVisible
            {
                get { return isVisible; }
            }

            private readonly short segmentIndex;
            /// <summary>
            /// Index of the segment that this control point starts. -1 if control point does not start a segment.
            /// </summary>
            internal short SegmentIndex
            {
                get { return segmentIndex; }
            }

            private readonly short controlPointIndex;
            /// <summary>
            /// Index of the control point
            /// </summary>
            internal short ControlPointIndex
            {
                get { return controlPointIndex; }
            }

            private readonly short nextControlPointIndex;
            /// <summary>
            /// The index of the next control point on the spline. Is -1 if none. Follow-Up not considered
            /// </summary>
            internal short NextControlPointIndex
            {
                get { return nextControlPointIndex; }
            }

            private readonly short previousControlPointIndex;
            /// <summary>
            /// The index of the previous control point on the spline. Is -1 if none. Follow-Up not considered. 
            /// </summary>
            internal short PreviousControlPointIndex
            {
                get { return previousControlPointIndex; }
            }

            private readonly bool previousControlPointIsSegment;
            /// <summary>
            /// Is previous Control Point a segment start?
            /// </summary>
            internal bool PreviousControlPointIsSegment
            {
                get { return previousControlPointIsSegment; }
            }

            private readonly bool nextControlPointIsSegment;
            /// <summary>
            /// Is next Control Point a segment start?
            /// </summary>
            internal bool NextControlPointIsSegment
            {
                get { return nextControlPointIsSegment; }
            }

            private readonly bool canHaveFollowUp;
            /// <summary>
            /// Can this control point have a Follow-Up? This is true if the control point is visible and does not have a previous or next control point on its spline
            /// </summary>
            internal bool CanHaveFollowUp
            {
                get { return canHaveFollowUp; }
            }

            /// <summary>
            /// Is the control point the start of a segment?
            /// </summary>
            internal bool IsSegment { get { return SegmentIndex != -1; } }

            private readonly short orientationAnchorIndex;
            /// <summary>
            /// The index of the control point being the orientation anchor for the anchor group containing the current control point
            /// Is -1 for non visible control points
            /// </summary>
            internal short OrientationAnchorIndex
            {
                get { return orientationAnchorIndex; }
            }

            internal ControlPointExtrinsicProperties(bool isVisible, short segmentIndex, short controlPointIndex, short previousControlPointIndex, short nextControlPointIndex, bool previousControlPointIsSegment, bool nextControlPointIsSegment, bool canHaveFollowUp, short orientationAnchorIndex)
            {
                this.isVisible = isVisible;
                this.segmentIndex = segmentIndex;
                this.controlPointIndex = controlPointIndex;
                this.nextControlPointIndex = nextControlPointIndex;
                this.previousControlPointIndex = previousControlPointIndex;
                this.previousControlPointIsSegment = previousControlPointIsSegment;
                this.nextControlPointIsSegment = nextControlPointIsSegment;
                this.canHaveFollowUp = canHaveFollowUp;
                this.orientationAnchorIndex = orientationAnchorIndex;
            }

            public bool Equals(ControlPointExtrinsicProperties other)
            {
                return IsVisible == other.IsVisible && SegmentIndex == other.SegmentIndex && ControlPointIndex == other.ControlPointIndex && NextControlPointIndex == other.NextControlPointIndex && PreviousControlPointIndex == other.PreviousControlPointIndex && PreviousControlPointIsSegment == other.PreviousControlPointIsSegment && NextControlPointIsSegment == other.NextControlPointIsSegment && CanHaveFollowUp == other.CanHaveFollowUp && OrientationAnchorIndex == other.OrientationAnchorIndex;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                    return false;
                return obj is ControlPointExtrinsicProperties && Equals((ControlPointExtrinsicProperties)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = IsVisible.GetHashCode();
                    hashCode = (hashCode * 397) ^ SegmentIndex.GetHashCode();
                    hashCode = (hashCode * 397) ^ ControlPointIndex.GetHashCode();
                    hashCode = (hashCode * 397) ^ NextControlPointIndex.GetHashCode();
                    hashCode = (hashCode * 397) ^ PreviousControlPointIndex.GetHashCode();
                    hashCode = (hashCode * 397) ^ PreviousControlPointIsSegment.GetHashCode();
                    hashCode = (hashCode * 397) ^ NextControlPointIsSegment.GetHashCode();
                    hashCode = (hashCode * 397) ^ CanHaveFollowUp.GetHashCode();
                    hashCode = (hashCode * 397) ^ OrientationAnchorIndex.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(ControlPointExtrinsicProperties left, ControlPointExtrinsicProperties right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ControlPointExtrinsicProperties left, ControlPointExtrinsicProperties right)
            {
                return !left.Equals(right);
            }

        }
    }
}
