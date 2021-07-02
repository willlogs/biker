// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.DevTools;
using JetBrains.Annotations;
using UnityEngine.Assertions;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Modifier/Mix Shapes", ModuleName = "Mix Shapes", Description = "Interpolates between two shapes")]
    [HelpURL(CurvySpline.DOCLINK + "cgmixshapes")]
#pragma warning disable 618
    public class ModifierMixShapes : CGModule, IOnRequestPath
#pragma warning restore 618
    {
        [HideInInspector]
        [InputSlotInfo(typeof(CGShape), Name = "Shape A")]
        public CGModuleInputSlot InShapeA = new CGModuleInputSlot();

        [HideInInspector]
        [InputSlotInfo(typeof(CGShape), Name = "Shape B")]
        public CGModuleInputSlot InShapeB = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGShape))]
        public CGModuleOutputSlot OutShape = new CGModuleOutputSlot();

        #region ### Serialized Fields ###

        [SerializeField, RangeEx(-1, 1, Tooltip = "Mix between the shapes. Values between -1 for Shape A and 1 for Shape B")]
        float m_Mix;

        #endregion

        #region ### Public Properties ###

        /// <summary>
        /// Defines how the result is interpolated. Values between -1 for Shape A and 1 for Shape B
        /// </summary>
        public float Mix
        {
            get { return m_Mix; }
            set
            {
                if (m_Mix != value)
                    m_Mix = value;
                Dirty = true;
            }
        }

        public bool PathIsClosed
        {
            get
            {
                return (IsConfigured) ? InShapeA.SourceSlot().PathProvider.PathIsClosed &&
                                        InShapeB.SourceSlot().PathProvider.PathIsClosed : false;
            }
        }

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        protected override void OnEnable()
        {
            base.OnEnable();
            Properties.MinWidth = 250;
            Properties.LabelWidth = 50;
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Mix = m_Mix;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            Mix = 0;
        }

        /*! \endcond */
        #endregion

        #region ### IOnRequestProcessing ###
        public CGData[] OnSlotDataRequest(CGModuleInputSlot requestedBy, CGModuleOutputSlot requestedSlot, params CGDataRequestParameter[] requests)
        {
            CGDataRequestRasterization raster = GetRequestParameter<CGDataRequestRasterization>(ref requests);
            if (!raster)
                return null;

            CGShape DataA = InShapeA.GetData<CGShape>(requests);
            CGShape DataB = InShapeB.GetData<CGShape>(requests);
            CGShape data = MixShapes(DataA, DataB, Mix, UIMessages);
            return new CGData[1] { data };
        }

        #endregion

        #region ### Public Methods ###

        /// <summary>
        /// Returns the mixed shape
        /// </summary>
        /// <param name="shapeA"></param>
        /// <param name="shapeB"></param>
        /// <param name="mix"> A value between -1 and 1. -1 will select the shape with the most points. 1 will select the other </param>
        /// <param name="warningsContainer">Is filled with warnings raised by the mixing logic</param>
        /// <param name="ignoreWarnings"> If true, warningsContainer will not be filled with warnings</param>
        /// <returns> The mixed shape</returns>
        public static CGShape MixShapes(CGShape shapeA, CGShape shapeB, float mix, [NotNull] List<string> warningsContainer, bool ignoreWarnings = false)
        {
            if (shapeA == null)
                return shapeB;

            if (shapeB == null)
                return shapeA;

            CGShape data = new CGShape();
            InterpolateShape(data, shapeA, shapeB, mix, warningsContainer, ignoreWarnings);
            return data;
        }

        /// <summary>
        /// Returns the mixed shape
        /// </summary>
        /// <param name="resultShape">A shape which will be filled with the data of the mixed shape</param>
        /// <param name="mix"> A value between -1 and 1. -1 will select shape A. 1 will select shape B </param>
        /// <param name="shapeA"> One of the two interpolated shapes</param>
        /// <param name="shapeB"> One of the two interpolated shapes</param>
        /// <param name="warningsContainer">Is filled with warnings raised by the mixing logic</param>
        /// <param name="ignoreWarnings"> If true, warningsContainer will not be filled with warnings</param>
        /// <returns> The mixed shape</returns>
        public static void InterpolateShape([NotNull]CGShape resultShape, CGShape shapeA, CGShape shapeB, float mix, [NotNull] List<string> warningsContainer, bool ignoreWarnings = false)
        {
            float interpolationTime = (mix + 1) * 0.5f;
#if CURVY_SANITY_CHECKS
            Assert.IsTrue(interpolationTime >= 0);
            Assert.IsTrue(interpolationTime <= 1);
#endif
            int shapeVertexCount = Mathf.Max(shapeA.Count, shapeB.Count);
            CGShape shapeWithMostVertices = shapeA.Count == shapeVertexCount
                ? shapeA
                : shapeB;

            Vector3[] positions = new Vector3[shapeVertexCount];
            Vector3[] normals = new Vector3[shapeVertexCount];
            if (shapeWithMostVertices == shapeA)
                for (int i = 0; i < shapeVertexCount; i++)
                {
                    float frag;
                    int idx = shapeB.GetFIndex(shapeA.F[i], out frag);

                    Vector3 bPosition;
                    {
                        bPosition.x = shapeB.Position[idx].x + (shapeB.Position[idx + 1].x - shapeB.Position[idx].x) * frag;
                        bPosition.y = shapeB.Position[idx].y + (shapeB.Position[idx + 1].y - shapeB.Position[idx].y) * frag;
                        bPosition.z = shapeB.Position[idx].z + (shapeB.Position[idx + 1].z - shapeB.Position[idx].z) * frag;
                    }
                    positions[i].x = shapeA.Position[i].x + (bPosition.x - shapeA.Position[i].x) * interpolationTime;
                    positions[i].y = shapeA.Position[i].y + (bPosition.y - shapeA.Position[i].y) * interpolationTime;
                    positions[i].z = shapeA.Position[i].z + (bPosition.z - shapeA.Position[i].z) * interpolationTime;

                    Vector3 bNormal = Vector3.SlerpUnclamped(shapeB.Normal[idx], shapeB.Normal[idx + 1], frag);
                    normals[i] = Vector3.SlerpUnclamped(shapeA.Normal[i], bNormal, interpolationTime);
                }
            else
                for (int i = 0; i < shapeVertexCount; i++)
                {
                    float frag;
                    int idx = shapeA.GetFIndex(shapeB.F[i], out frag);

                    Vector3 aPosition;
                    {
                        aPosition.x = shapeA.Position[idx].x + (shapeA.Position[idx + 1].x - shapeA.Position[idx].x) * frag;
                        aPosition.y = shapeA.Position[idx].y + (shapeA.Position[idx + 1].y - shapeA.Position[idx].y) * frag;
                        aPosition.z = shapeA.Position[idx].z + (shapeA.Position[idx + 1].z - shapeA.Position[idx].z) * frag;
                    }
                    positions[i].x = aPosition.x + (shapeB.Position[i].x - aPosition.x) * interpolationTime;
                    positions[i].y = aPosition.y + (shapeB.Position[i].y - aPosition.y) * interpolationTime;
                    positions[i].z = aPosition.z + (shapeB.Position[i].z - aPosition.z) * interpolationTime;

                    Vector3 aNormal = Vector3.SlerpUnclamped(shapeA.Normal[idx], shapeA.Normal[idx + 1], frag);
                    normals[i] = Vector3.SlerpUnclamped(aNormal, shapeB.Normal[i], interpolationTime);
                }

            resultShape.Position = positions;

            resultShape.F = new float[shapeVertexCount];
            // sets Length and F
            resultShape.Recalculate();

            /*TODO BUG the following 4 properties are tied to the shape geometry, and should be recomputed based on the mixed mesh's geometry instead of using an approximate result.
             This will be specially visible when shape A and shape B have very different values of those properties, such as one of them having different material groups while the other having only one.
             3 of the 4 properties use shapeWithMostVertices. The issue with this is that shapeWithMostVertices can switch between the shape A and shape B depending on the shape's rasterization properties. To test/reproduce, set a square and circle as shapes, and set their rasterization to Optimize = true and Angle Threshold = 120. In those conditions, the square has more vertices. Then set the threshold to 10. In those conditions the circle has more vertices
              */
            resultShape.Normal = normals;
            resultShape.Map = (float[])shapeWithMostVertices.Map.Clone();
            resultShape.SourceF = (float[])shapeWithMostVertices.SourceF.Clone();
            resultShape.MaterialGroups = shapeWithMostVertices.MaterialGroups.Select(g => g.Clone()).ToList();

            if (ignoreWarnings == false)
            {
                if (shapeA.Closed != shapeB.Closed)
                    warningsContainer.Add("Mixing inputs with different Closed values is not supported");
                if (shapeA.Seamless != shapeB.Seamless)
                    warningsContainer.Add("Mixing inputs with different Seamless values is not supported");
                if (shapeA.SourceIsManaged != shapeB.SourceIsManaged)
                    warningsContainer.Add("Mixing inputs with different SourceIsManaged values is not supported");
            }
            resultShape.Closed = shapeA.Closed;
            resultShape.Seamless = shapeA.Seamless;
            resultShape.SourceIsManaged = shapeA.SourceIsManaged;
        }

        #endregion
    }
}
