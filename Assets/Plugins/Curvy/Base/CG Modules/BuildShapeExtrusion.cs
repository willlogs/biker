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
using System.Linq;
using FluffyUnderware.DevTools;
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools.Extensions;
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Build/Shape Extrusion", ModuleName = "Shape Extrusion", Description = "Simple Shape Extrusion")]
    [HelpURL(CurvySpline.DOCLINK + "cgbuildshapeextrusion")]
    public class BuildShapeExtrusion : CGModule, IPathProvider
    {
        #region ### Enums ###

        public enum ScaleModeEnum
        {
            Simple,
            Advanced
        }

        public enum CrossShiftModeEnum
        {
            /// <summary>
            /// The start of the Shape is used
            /// </summary>
            None,
            /// <summary>
            /// The starting point is shifted to the collision point of the Path's orientation with the cross shape
            /// </summary>
            ByOrientation,//TODO rename to ByPathOrientation?
            /// <summary>
            /// The starting point is shifted by a user defined value
            /// </summary>
            Custom
        }

        #endregion

        [HideInInspector]
        [InputSlotInfo(typeof(CGPath), RequestDataOnly = true)]
        public CGModuleInputSlot InPath = new CGModuleInputSlot();

        [HideInInspector]
        [InputSlotInfo(typeof(CGShape), Array = true, ArrayType = SlotInfo.SlotArrayType.Hidden, RequestDataOnly = true)]
        public CGModuleInputSlot InCross = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGVolume))]
        public CGModuleOutputSlot OutVolume = new CGModuleOutputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGVolume))]
        public CGModuleOutputSlot OutVolumeHollow = new CGModuleOutputSlot();

        #region ### Serialized Fields ###

        #region TAB: Path
        [Tab("Path")]
        [FloatRegion(UseSlider = true, RegionOptionsPropertyName = "RangeOptions", Precision = 4)]
        [SerializeField]
        FloatRegion m_Range = FloatRegion.ZeroOne;
        [SerializeField, RangeEx(1, 100, "Resolution", "Defines how densely the path spline's sampling points are. When the value is 100, the number of sampling points per world distance unit is equal to the spline's Max Points Per Unit")]
        int m_Resolution = 50;
        [SerializeField]
        bool m_Optimize = true;
        [FieldCondition("m_Optimize", true)]
        [SerializeField, RangeEx(0.1f, 120, Tooltip = "Max angle")]
        float m_AngleThreshold = 10;

        #endregion

        #region TAB: Cross
        [Tab("Cross")]
        [FieldAction("CBEditCrossButton", Position = ActionAttribute.ActionPositionEnum.Above)]
        [FloatRegion(UseSlider = true, RegionOptionsPropertyName = "CrossRangeOptions", Precision = 4)]
        [SerializeField]
        FloatRegion m_CrossRange = FloatRegion.ZeroOne;
        [SerializeField, RangeEx(1, 100, "Resolution", Tooltip = "Defines how densely the cross spline's sampling points are. When the value is 100, the number of sampling points per world distance unit is equal to the spline's Max Points Per Unit")]
        int m_CrossResolution = 50;
        [SerializeField, Label("Optimize")]
        bool m_CrossOptimize = true;
        [FieldCondition("m_CrossOptimize", true)]
        [SerializeField, RangeEx(0.1f, 120, "Angle Threshold", Tooltip = "Max angle")]
        float m_CrossAngleThreshold = 10;

        //[Header("Options")]
        [SerializeField, Label("Include CP")]
        bool m_CrossIncludeControlpoints;
        [SerializeField, Label("Hard Edges")]
        bool m_CrossHardEdges;
        [SerializeField, Label("Materials")]
        bool m_CrossMaterials;
        [SerializeField, Label("Extended UV")]
        bool m_CrossExtendedUV;
        [SerializeField, Label("Shift", Tooltip = "Defines a shift to be applied on the output volume's cross.\r\nThis shift is used when interpolating values (position, normal, ...) along the volume's surface.")]
        CrossShiftModeEnum m_CrossShiftMode = CrossShiftModeEnum.ByOrientation;
        [SerializeField]
        [RangeEx(0, 1, "Value", "Shift By", Slider = true)]
        [FieldCondition("m_CrossShiftMode", CrossShiftModeEnum.Custom)]
        float m_CrossShiftValue;
        [Label("Reverse Normal", "Reverse Vertex Normals?")]
        [SerializeField]
        bool m_CrossReverseNormals;
        #endregion

        #region TAB: Scale

        [Tab("Scale")]
        [Label("Mode")]
        [SerializeField]
        ScaleModeEnum m_ScaleMode = ScaleModeEnum.Simple;

        [FieldCondition("m_ScaleMode", ScaleModeEnum.Advanced)]
        [Label("Reference")]
        [SerializeField]
        CGReferenceMode m_ScaleReference = CGReferenceMode.Self;

        [FieldCondition("m_ScaleMode", ScaleModeEnum.Advanced)]
        [Label("Offset")]
        [SerializeField]
        float m_ScaleOffset;

        [SerializeField, Label("Uniform Scaling", Tooltip = "The same scaling value is applied on all dimensions")]
        bool m_ScaleUniform = true;

        [SerializeField]
        float m_ScaleX = 1;

        [SerializeField]
        [FieldCondition("m_ScaleUniform", false)]
        float m_ScaleY = 1;

        [SerializeField]
        [FieldCondition("m_ScaleMode", ScaleModeEnum.Advanced)]
        [AnimationCurveEx("Multiplier X")]
        [Tooltip("Defines scale multiplier depending on the TF, the relative position of a point on the path")]
        AnimationCurve m_ScaleCurveX = AnimationCurve.Linear(0, 1, 1, 1);
        [SerializeField]
        [FieldCondition("m_ScaleUniform", false, false, ConditionalAttribute.OperatorEnum.AND, "m_ScaleMode", ScaleModeEnum.Advanced, false)]
        [AnimationCurveEx("Multiplier Y")]
        [Tooltip("Defines scale multiplier depending on the TF, the relative position of a point on the path")]
        AnimationCurve m_ScaleCurveY = AnimationCurve.Linear(0, 1, 1, 1);

        #endregion

        #region TAB: Hollow
        [Tab("Hollow")]
        [RangeEx(0, 1, Slider = true, Label = "Inset")]
        [SerializeField]
        float m_HollowInset;
        [Label("Reverse Normal", "Reverse Vertex Normals?")]
        [SerializeField]
        bool m_HollowReverseNormals;
        #endregion


        #endregion

        #region ### Public Properties ###

        #region TAB: Path


        public float From
        {
            get { return m_Range.From; }
            set
            {
                float v = Mathf.Repeat(value, 1);
                if (m_Range.From != v)
                    m_Range.From = v;

                Dirty = true;
            }
        }

        public float To
        {
            get { return m_Range.To; }
            set
            {
                float v = Mathf.Max(From, value);
                if (ClampPath)
                    v = DTMath.Repeat(value, 1);
                if (m_Range.To != v)
                    m_Range.To = v;

                Dirty = true;
            }
        }

        public float Length
        {
            get
            {
                return (ClampPath) ? m_Range.To - m_Range.From : m_Range.To;
            }
            set
            {
                float v = (ClampPath) ? value - m_Range.To : value;
                if (m_Range.To != v)
                    m_Range.To = v;
                Dirty = true;
            }
        }

        /// <summary>
        /// Defines how densely the path spline's sampling points are. When the value is 100, the number of sampling points per world distance unit is equal to the spline's MaxPointsPerUnit
        /// </summary>
        public int Resolution
        {
            get { return m_Resolution; }
            set
            {
                int v = Mathf.Clamp(value, 1, 100);
                if (m_Resolution != v)
                    m_Resolution = v;
                Dirty = true;
            }
        }

        public bool Optimize
        {
            get { return m_Optimize; }
            set
            {
                if (m_Optimize != value)
                    m_Optimize = value;
                Dirty = true;
            }
        }

        public float AngleThreshold
        {
            get { return m_AngleThreshold; }
            set
            {
                float v = Mathf.Clamp(value, 0.1f, 120);
                if (m_AngleThreshold != v)
                    m_AngleThreshold = v;
                Dirty = true;
            }
        }


        #endregion
        #region TAB: Cross
        public float CrossFrom
        {
            get { return m_CrossRange.From; }
            set
            {
                float v = Mathf.Repeat(value, 1);
                if (m_CrossRange.From != v)
                    m_CrossRange.From = v;

                Dirty = true;
            }
        }

        public float CrossTo
        {
            get { return m_CrossRange.To; }
            set
            {
                float v = Mathf.Max(CrossFrom, value);
                if (ClampCross)
                    v = DTMath.Repeat(value, 1);
                if (m_CrossRange.To != v)
                    m_CrossRange.To = v;

                Dirty = true;
            }
        }

        public float CrossLength
        {
            get
            {
                return (ClampCross) ? m_CrossRange.To - m_CrossRange.From : m_CrossRange.To;
            }
            set
            {
                float v = (ClampCross) ? value - m_CrossRange.To : value;
                if (m_CrossRange.To != v)
                    m_CrossRange.To = v;
                Dirty = true;
            }
        }

        /// <summary>
        /// Defines how densely the cross spline's sampling points are. When the value is 100, the number of sampling points per world distance unit is equal to the spline's MaxPointsPerUnit
        /// </summary>
        public int CrossResolution
        {
            get { return m_CrossResolution; }
            set
            {
                int v = Mathf.Clamp(value, 1, 100);
                if (m_CrossResolution != v)
                    m_CrossResolution = v;
                Dirty = true;
            }
        }

        public bool CrossOptimize
        {
            get { return m_CrossOptimize; }
            set
            {
                if (m_CrossOptimize != value)
                    m_CrossOptimize = value;
                Dirty = true;
            }
        }

        public float CrossAngleThreshold
        {
            get { return m_CrossAngleThreshold; }
            set
            {
                float v = Mathf.Clamp(value, 0.1f, 120);
                if (m_CrossAngleThreshold != v)
                    m_CrossAngleThreshold = v;
                Dirty = true;
            }
        }


        public bool CrossIncludeControlPoints
        {
            get { return m_CrossIncludeControlpoints; }
            set
            {
                if (m_CrossIncludeControlpoints != value)
                    m_CrossIncludeControlpoints = value;
                Dirty = true;
            }
        }

        public bool CrossHardEdges
        {
            get { return m_CrossHardEdges; }
            set
            {
                if (m_CrossHardEdges != value)
                    m_CrossHardEdges = value;
                Dirty = true;
            }
        }

        public bool CrossMaterials
        {
            get { return m_CrossMaterials; }
            set
            {
                if (m_CrossMaterials != value)
                    m_CrossMaterials = value;
                Dirty = true;
            }
        }

        public bool CrossExtendedUV
        {
            get { return m_CrossExtendedUV; }
            set
            {
                if (m_CrossExtendedUV != value)
                    m_CrossExtendedUV = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// Defines how the <see cref="CGVolume.CrossFShift"/> value is defined.
        /// </summary>
        public CrossShiftModeEnum CrossShiftMode
        {
            get { return m_CrossShiftMode; }
            set
            {
                if (m_CrossShiftMode != value)
                    m_CrossShiftMode = value;
                Dirty = true;
            }
        }

        public float CrossShiftValue
        {
            get { return m_CrossShiftValue; }
            set
            {
                float v = Mathf.Repeat(value, 1);
                if (m_CrossShiftValue != v)
                    m_CrossShiftValue = v;
                Dirty = true;
            }
        }

        public bool CrossReverseNormals
        {
            get { return m_CrossReverseNormals; }
            set
            {
                if (m_CrossReverseNormals != value)
                    m_CrossReverseNormals = value;
                Dirty = true;
            }
        }


        #endregion
        #region TAB: Scale

        public ScaleModeEnum ScaleMode
        {
            get { return m_ScaleMode; }
            set
            {
                if (m_ScaleMode != value)
                    m_ScaleMode = value;
                Dirty = true;
            }
        }

        public CGReferenceMode ScaleReference
        {
            get { return m_ScaleReference; }
            set
            {
                if (m_ScaleReference != value)
                    m_ScaleReference = value;
                Dirty = true;
            }
        }

        public bool ScaleUniform
        {
            get { return m_ScaleUniform; }
            set
            {
                if (m_ScaleUniform != value)
                    m_ScaleUniform = value;
                Dirty = true;
            }
        }

        public float ScaleOffset
        {
            get { return m_ScaleOffset; }
            set
            {
                if (m_ScaleOffset != value)
                    m_ScaleOffset = value;
                Dirty = true;
            }
        }

        public float ScaleX
        {
            get { return m_ScaleX; }
            set
            {
                if (m_ScaleX != value)
                    m_ScaleX = value;
                Dirty = true;
            }
        }

        public float ScaleY
        {
            get { return m_ScaleY; }
            set
            {
                if (m_ScaleY != value)
                    m_ScaleY = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// When using <see cref="ScaleMode"/> == ScaleModeEnum.Advanced, this curve defines a scale multiplier relatively to the TF value of a point on the path.
        /// <remarks>You will need to set this module's Dirty to true yourself if you modify the AnimationCurve without setting a new one</remarks>
        /// </summary>
        public AnimationCurve ScaleMultiplierX
        {
            get { return m_ScaleCurveX; }
            set
            {
                if (m_ScaleCurveX != value)
                    m_ScaleCurveX = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// When using <see cref="ScaleMode"/> == ScaleModeEnum.Advanced, this curve defines a scale multiplier relatively to the TF value of a point on the path.
        /// <remarks>You will need to set this module's Dirty to true yourself if you modify the AnimationCurve without setting a new one</remarks>
        /// </summary>
        public AnimationCurve ScaleMultiplierY
        {
            get { return m_ScaleCurveY; }
            set
            {
                if (m_ScaleCurveY != value)
                    m_ScaleCurveY = value;
                Dirty = true;
            }
        }

        #endregion
        #region TAB: Hollow

        public float HollowInset
        {
            get { return m_HollowInset; }
            set
            {
                float v = Mathf.Clamp01(value);
                if (m_HollowInset != v)
                    m_HollowInset = v;
                Dirty = true;
            }
        }

        public bool HollowReverseNormals
        {
            get { return m_HollowReverseNormals; }
            set
            {
                if (m_HollowReverseNormals != value)
                    m_HollowReverseNormals = value;
                Dirty = true;
            }
        }

        #endregion

        public int PathSamples
        {
            get;
            private set;
        }

        public int CrossSamples
        {
            get;
            private set;
        }

        public int CrossGroups { get; private set; }

        public IExternalInput Cross
        {
            get
            {
                return (IsConfigured) ? InCross.SourceSlot().ExternalInput : null;
            }
        }

        public Vector3 CrossPosition { get; protected set; }

        public Quaternion CrossRotation { get; protected set; }

        public bool PathIsClosed
        {
            get { return InPath.SourceSlot().PathProvider.PathIsClosed; }
        }

        #endregion

        #region ### Private Fields & Properties ###

        bool ClampPath { get { return (InPath.IsLinked) ? !InPath.SourceSlot().PathProvider.PathIsClosed : true; } }
        bool ClampCross { get { return (InCross.IsLinked) ? !InCross.SourceSlot().PathProvider.PathIsClosed : true; } }
        RegionOptions<float> RangeOptions
        {
            get
            {

                if (ClampPath)
                {
                    return RegionOptions<float>.MinMax(0, 1);
                }
                else
                {
                    return new RegionOptions<float>()
                    {
                        LabelFrom = "Start",
                        ClampFrom = DTValueClamping.Min,
                        FromMin = 0,
                        LabelTo = "Length",
                        ClampTo = DTValueClamping.Range,
                        ToMin = 0,
                        ToMax = 1
                    };
                }
            }
        }

        RegionOptions<float> CrossRangeOptions
        {
            get
            {

                if (ClampCross)
                {
                    return RegionOptions<float>.MinMax(0, 1);
                }
                else
                {
                    return new RegionOptions<float>()
                    {
                        LabelFrom = "Start",
                        ClampFrom = DTValueClamping.Min,
                        FromMin = 0,
                        LabelTo = "Length",
                        ClampTo = DTValueClamping.Range,
                        ToMin = 0,
                        ToMax = 1
                    };
                }
            }
        }

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        protected override void OnEnable()
        {
            base.OnEnable();
            Properties.MinWidth = 270;
            Properties.LabelWidth = 100;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            //TODO OPTIM each one of the following properties setting will set Dirty to true, and trigger a lot of work. You can avoid that by Dirty to true only once per OnValidate call
            Resolution = m_Resolution;
            Optimize = m_Optimize;

            CrossResolution = m_CrossResolution;
            CrossOptimize = m_CrossOptimize;

            CrossIncludeControlPoints = m_CrossIncludeControlpoints;
            CrossHardEdges = m_CrossHardEdges;
            ScaleMode = m_ScaleMode;
            ScaleReference = m_ScaleReference;
            ScaleUniform = m_ScaleUniform;
            ScaleOffset = m_ScaleOffset;
            ScaleX = m_ScaleX;
            ScaleY = m_ScaleY;
            ScaleMultiplierX = m_ScaleCurveX;
            ScaleMultiplierY = m_ScaleCurveY;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            From = 0;
            To = 1;
            Resolution = 50;
            AngleThreshold = 10;
            Optimize = true;
            CrossFrom = 0;
            CrossTo = 1;
            CrossResolution = 50;
            CrossAngleThreshold = 10;
            CrossOptimize = true;
            CrossIncludeControlPoints = false;
            CrossHardEdges = false;
            CrossMaterials = false;
            CrossShiftMode = CrossShiftModeEnum.ByOrientation;
            ScaleMode = ScaleModeEnum.Simple;
            ScaleUniform = true;
            ScaleX = 1;
            ScaleY = 1;
            ScaleMultiplierX = AnimationCurve.Linear(0, 1, 1, 1);
            ScaleMultiplierY = AnimationCurve.Linear(0, 1, 1, 1);
            HollowInset = 0;
            CrossExtendedUV = false;
            CrossReverseNormals = false;
            HollowReverseNormals = false;
            ScaleReference = CGReferenceMode.Self;
            ScaleOffset = 0;
        }


        /*! \endcond */
        #endregion

        #region ### Public Methods ###

        public override void Refresh()
        {
            base.Refresh();
            if (Length == 0)
            {
                OutVolume.SetData(null);
                OutVolumeHollow.SetData(null);
            }
            else
            {
                //OPTIM make it an array
                List<CGDataRequestParameter> req = new List<CGDataRequestParameter>();

                CGPath path;
                {
                    req.Add(new CGDataRequestRasterization(
                        this.From, this.Length,
                        Resolution,
                        AngleThreshold, (Optimize) ? CGDataRequestRasterization.ModeEnum.Optimized : CGDataRequestRasterization.ModeEnum.Even));
                    path = InPath.GetData<CGPath>(req.ToArray());
                    req.Clear();
                }

                List<CGShape> crosses;
                {
                    CGDataRequestRasterization rasterizationRequest;
                    {
                        bool useVariableShape = InCross.LinkedSlots.Count == 1 && InCross.LinkedSlots[0].Info is ShapeOutputSlotInfo && (InCross.LinkedSlots[0].Info as ShapeOutputSlotInfo).OutputsVariableShape;

                        if (useVariableShape && path)
                            rasterizationRequest = new CGDataRequestShapeRasterization(path.F,
                                this.CrossFrom, this.CrossLength,
                                CrossResolution,
                                CrossAngleThreshold, (CrossOptimize) ? CGDataRequestRasterization.ModeEnum.Optimized : CGDataRequestRasterization.ModeEnum.Even);
                        else
                            rasterizationRequest = new CGDataRequestRasterization(
                                this.CrossFrom, this.CrossLength,
                                CrossResolution,
                                CrossAngleThreshold, (CrossOptimize) ? CGDataRequestRasterization.ModeEnum.Optimized : CGDataRequestRasterization.ModeEnum.Even);
                    }

                    req.Add(rasterizationRequest);

                    if (CrossIncludeControlPoints || CrossHardEdges || CrossMaterials)
                        req.Add(new CGDataRequestMetaCGOptions(CrossHardEdges, CrossMaterials, CrossIncludeControlPoints, CrossExtendedUV));

                    crosses = InCross.GetAllData<CGShape>(req.ToArray());
                }


                bool isPathInvalid = !path || path.Count == 0;

                bool areCrossesInvalid;
                {
                    List<int> distinctCrossCounts = crosses.Select(c => c == null ? 0 : c.Count).Distinct().ToList();
                    if (distinctCrossCounts.Count() != 1 || distinctCrossCounts.First() == 0)
                    {
                        areCrossesInvalid = true;
                        UIMessages.Add("Shape Extrusion: All input Crosses are expected to have the same non zero number of sample points.");
                    }
                    else
                        areCrossesInvalid = false;
                }

                if (isPathInvalid || areCrossesInvalid)
                {
                    OutVolume.ClearData();
                    OutVolumeHollow.ClearData();
                    return;
                }

                CGShape initialCross = crosses[0];

#if UNITY_EDITOR
                //TODO move these warnings, and modify them, inside the raterizing modules themselves. Because the way it is done now, if there is a module between the InputSplineShape module and the current module, the warning will not happen

                //TODO add warnings to the "Rasterize Path" module similar to those below
                //Warning messages
                {
                    for (int index = 0; index < InPath.LinkedSlots.Count; index++)
                    {
                        CGModuleSlot linkedSlot = InPath.LinkedSlots[index];
                        if (linkedSlot.Module is InputSplinePath)
                        {
                            InputSplinePath inputSplineModule = linkedSlot.Module as InputSplinePath;
                            if (inputSplineModule
                                && inputSplineModule.UseCache
                                && path.Count > inputSplineModule.Spline.CacheSize * 1.30f
                                && path.Count > inputSplineModule.Spline.CacheSize + 30)
                                UIMessages.Add(String.Format(System.Globalization.CultureInfo.InvariantCulture, "The Cache Density of \"{0}\" might be too small for this module's Path Resolution. To get a more detailed extruded volume, you might need to increase Cache Density, or set the input module's Use Cache to false", inputSplineModule.Spline.gameObject.name));
                        }
                    }

                    for (int index = 0; index < InCross.LinkedSlots.Count; index++)
                    {
                        CGModuleSlot linkedSlot = InCross.LinkedSlots[index];
                        if (linkedSlot.Module is InputSplineShape)
                        {
                            InputSplineShape inputSplineModule = linkedSlot.Module as InputSplineShape;
                            if (inputSplineModule
                                && inputSplineModule.UseCache
                                && initialCross.Count > inputSplineModule.Shape.CacheSize * 1.30f
                                && initialCross.Count > inputSplineModule.Shape.CacheSize + 30)
                                UIMessages.Add(String.Format(System.Globalization.CultureInfo.InvariantCulture, "The Cache Density of \"{0}\" might be too small for this module's Cross Resolution. To get a more detailed extruded volume, you might need to increase Cache Density, or set the input module's Use Cache to false", inputSplineModule.Shape.gameObject.name));
                        }
                    }
                }
#endif
                CGVolume vol = CGVolume.Get(OutVolume.GetData<CGVolume>(), path, initialCross);
                CGVolume volHollow = (OutVolumeHollow.IsLinked) ? CGVolume.Get(OutVolumeHollow.GetData<CGVolume>(), path, initialCross) : null;
                bool hasHollowVolume = volHollow;
                PathSamples = path.Count;
                CrossSamples = initialCross.Count;
                CrossGroups = initialCross.MaterialGroups.Count;
                CrossPosition = vol.Position[0];
                CrossRotation = Quaternion.LookRotation(vol.Direction[0], vol.Normal[0]);

                Vector3 baseScale = (ScaleUniform) ? new Vector3(ScaleX, ScaleX, 1) : new Vector3(ScaleX, ScaleY, 1);
                Vector3 scl = baseScale;
                int vtIdx = 0;

                float[] scaleFArray = (ScaleReference == CGReferenceMode.Source) ? path.SourceF : path.F;

                float crossNormalMul = (CrossReverseNormals) ? -1 : 1;
                float hollowNormalMul = (HollowReverseNormals) ? -1 : 1;

                bool hasSingleCross = crosses.Count == 1;

                int samplesCount = path.Count;
                for (int sample = 0; sample < samplesCount; sample++)
                {
                    CGShape currentCross;
                    if (hasSingleCross)
                        currentCross = initialCross;
                    else
                    {
                        int crossIndex = Mathf.RoundToInt((crosses.Count - 1) * path.F[sample]);
#if CURVY_SANITY_CHECKS
                        Assert.IsTrue(path.F[sample] >= 0);
                        Assert.IsTrue(path.F[sample] <= 1);
                        Assert.IsTrue(crossIndex >= 0);
                        Assert.IsTrue(crossIndex < crosses.Count);
#endif
                        currentCross = crosses[crossIndex];
                    }

                    Vector3[] crossPositions = currentCross.Position;
                    Vector3[] crossNormals = currentCross.Normal;

                    Quaternion R = Quaternion.LookRotation(path.Direction[sample], path.Normal[sample]);

                    getScaleInternal(scaleFArray[sample], baseScale, ref scl);
                    Matrix4x4 mat = Matrix4x4.TRS(path.Position[sample], R, scl);
                    Matrix4x4 matHollow = hasHollowVolume
                        ? Matrix4x4.TRS(path.Position[sample], R, scl * (1 - HollowInset))
                        : default(Matrix4x4);

                    int currentCrossCount = currentCross.Count;
                    for (int c = 0; c < currentCrossCount; c++)
                    {
                        vol.Vertex[vtIdx] = mat.MultiplyPoint(crossPositions[c]);

                        Vector3 rotatedCrossNormal = R * crossNormals[c];
                        vol.VertexNormal[vtIdx].x = rotatedCrossNormal.x * crossNormalMul;
                        vol.VertexNormal[vtIdx].y = rotatedCrossNormal.y * crossNormalMul;
                        vol.VertexNormal[vtIdx].z = rotatedCrossNormal.z * crossNormalMul;


                        if (hasHollowVolume)
                        {
                            volHollow.Vertex[vtIdx] = matHollow.MultiplyPoint(crossPositions[c]);
                            volHollow.VertexNormal[vtIdx].x = rotatedCrossNormal.x * hollowNormalMul;
                            volHollow.VertexNormal[vtIdx].y = rotatedCrossNormal.y * hollowNormalMul;
                            volHollow.VertexNormal[vtIdx].z = rotatedCrossNormal.z * hollowNormalMul;
                        }

                        vtIdx++;
                    }
                }

                switch (CrossShiftMode)
                {
                    case CrossShiftModeEnum.ByOrientation:
                        vol.CrossFShift = 0;
                        // shift CrossF to match Path Orientation
                        Vector2 hit;
                        float frag;
                        for (int i = 0; i < initialCross.Count - 1; i++)
                            if (DTMath.RayLineSegmentIntersection(vol.Position[0], vol.Normal[0], vol.Vertex[i], vol.Vertex[i + 1], out hit, out frag))
                            {
                                vol.CrossFShift = DTMath.SnapPrecision(vol.CrossF[i] + (vol.CrossF[i + 1] - vol.CrossF[i]) * frag, 2);
                                break;
                            }

                        break;
                    case CrossShiftModeEnum.Custom:
                        vol.CrossFShift = CrossShiftValue;
                        break;
                    case CrossShiftModeEnum.None:
                        vol.CrossFShift = 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("CrossShiftMode");
                }

                if (volHollow != null)
                    volHollow.CrossFShift = vol.CrossFShift;

                OutVolume.SetData(vol);
                OutVolumeHollow.SetData(volHollow);
            }
        }

        public Vector3 GetScale(float f)
        {
            Vector3 baseScale = (ScaleUniform) ? new Vector3(ScaleX, ScaleX, 1) : new Vector3(ScaleX, ScaleY, 1);
            Vector3 res = Vector3.zero;
            getScaleInternal(f, baseScale, ref res);
            return res;
        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */

        void getScaleInternal(float f, Vector3 baseScale, ref Vector3 scale)
        {
            if (ScaleMode == ScaleModeEnum.Advanced)
            {
                float scf = DTMath.Repeat(f - ScaleOffset, 1);
                float scx = baseScale.x * ScaleMultiplierX.Evaluate(scf);
                scale.Set(scx,
                              (m_ScaleUniform) ? scx : baseScale.y * ScaleMultiplierY.Evaluate(scf),
                              1);
            }
            else
                scale = baseScale;

        }

        /*! \endcond */
        #endregion

    }
}
