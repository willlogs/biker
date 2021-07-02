// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools.Extensions;
using System;
using System.Linq;
using System.Text;
using System.Reflection;
using FluffyUnderware.DevTools;
using UnityEngine.Assertions;

#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#endif




namespace FluffyUnderware.Curvy.Generator
{
    /// <summary>
    /// Additional properties for CGData based classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CGDataInfoAttribute : Attribute
    {
        public readonly Color Color;

        public CGDataInfoAttribute(Color color)
        {
            Color = color;
        }

        public CGDataInfoAttribute(float r, float g, float b, float a = 1)
        {
            Color = new Color(r, g, b, a);
        }

        public CGDataInfoAttribute(string htmlColor)
        {
            Color = htmlColor.ColorFromHtml();
        }
    }

    /// <summary>
    /// Data Base class
    /// </summary>
    public class CGData
    {
        public string Name;

        public virtual int Count
        {
            get { return 0; }
        }

        public static implicit operator bool(CGData a)
        {
            return !ReferenceEquals(a, null);
        }

        public virtual T Clone<T>() where T : CGData
        {
            return new CGData() as T;
        }

        /// <summary>
        /// Searches FMapArray and returns the index that covers the fValue as well as the percentage between index and index+1
        /// </summary>
        /// <param name="FMapArray">array of sorted values ranging from 0..1</param>
        /// <param name="fValue">a value 0..1</param>
        /// <param name="frag">fragment between the resulting and the next index (0..1)</param>
        /// <returns>the index where fValue lies in</returns>
        protected int getGenericFIndex(ref float[] FMapArray, float fValue, out float frag)
        {

            if (fValue == 1)
            {
                frag = 1;
                return FMapArray.Length - 2;
            }
            fValue = Mathf.Repeat(fValue, 1);
            for (int i = 1; i < FMapArray.Length; i++)
                if (FMapArray[i] > fValue)
                {
                    frag = (fValue - FMapArray[i - 1]) / (FMapArray[i] - FMapArray[i - 1]);
#if CURVY_SANITY_CHECKS
                    Assert.IsTrue(frag >= 0);
                    Assert.IsTrue(frag <= 1);
#endif
                    return i - 1;
                }
            frag = 0;
            return 0;
        }
    }

    /// <summary>
    /// Rasterized Shape Data (Polyline)
    /// </summary>
    [CGDataInfo(0.73f, 0.87f, 0.98f)]
    public class CGShape : CGData
    {
        //TODO Debug time checks that F arrays contain values between 0 and 1
        //TODO enhance documentation of this class
        //TODO why is it named F and not RelativePosition, or RelativeDistance. Is the usage of F correct? If not, rename both SourceF and F arrays
        /// <summary>
        /// Relative position (0..1, NOT TF!) within the length of the source spline (if any)
        /// </summary>
        public float[] SourceF = new float[0];

        /// <summary>
        /// Relative position (0..1, NOT TF!) within the length of the shape
        /// </summary>
        //OPTIM can the storage of this array be avoided by storing only SourceF and the start and end Distance, and infere F values only when needed?
        //OPTIM can we just assign SourceF to F when start and end distances are equal to respectively 0 and 1? (which is the case most of the time)
        public float[] F = new float[0];

        //TODO Update this comment and all similar comments and variable/method names to make it clear whether the position (and similar) are computed in local or global space
        /// <summary>
        /// Shape/Path Position
        /// </summary>
        public Vector3[] Position = new Vector3[0];
        /// <summary>
        /// Normal
        /// </summary>
        public Vector3[] Normal = new Vector3[0];
        /// <summary>
        /// Arbitrary mapped value, usually U
        /// </summary>
        /*TODO Map is defined in CGShape but:
         1- filling it inside an instance of CGPath (which inherits from CGShape) is useless, since Map is used only by CGVolume when it takes it from a CGShape, and not a CGPath. So an optimization would be to not fill Map for instances not consumed by CGVolume
         2- I hope that storing it might be not needed, and calculating it only when needed might be possible
        */
        public float[] Map = new float[0];
        /// <summary>
        /// Groups/Patches
        /// </summary>
        public List<SamplePointsMaterialGroup> MaterialGroups = new List<SamplePointsMaterialGroup>();
        /// <summary>
        /// Whether the source is managed or not
        /// </summary>
        /// <remarks>This could be used to determine if values needs to be transformed into generator space or not</remarks>
        public bool SourceIsManaged;
        /// <summary>
        /// Whether the base spline is closed or not
        /// </summary>
        public bool Closed;
        /// <summary>
        /// Whether the Shape/Path is seamless, i.e. Closed==true and the whole length is covered
        /// </summary>
        public bool Seamless;
        /// <summary>
        /// Length in world units
        /// </summary>
        public float Length;

        /// <summary>
        /// Gets the number of sample points
        /// </summary>
        public override int Count
        {
            get { return F.Length; }
        }

        #region ### Private fields ###
        // Caching
        /*
        float mCacheLastSourceF = float.MaxValue;
        int mCacheLastSourceIndex;
         
        float mCacheLastSourceFrag;
        */
        float mCacheLastF = float.MaxValue;
        int mCacheLastIndex;
        float mCacheLastFrag;

        #endregion

        public CGShape() : base() { }

        public CGShape(CGShape source) : base()
        {
            Position = (Vector3[])source.Position.Clone();
            Normal = (Vector3[])source.Normal.Clone();
            Map = (float[])source.Map.Clone();
            F = (float[])source.F.Clone();
            SourceF = (float[])source.SourceF.Clone();
            MaterialGroups = source.MaterialGroups.Select(g => g.Clone()).ToList();
            Closed = source.Closed;
            Seamless = source.Seamless;
            Length = source.Length;
            SourceIsManaged = source.SourceIsManaged;
        }

        public override T Clone<T>()
        {
            return new CGShape(this) as T;
        }

        public static void Copy(CGShape dest, CGShape source)
        {
            Array.Resize(ref dest.Position, source.Position.Length);
            source.Position.CopyTo(dest.Position, 0);
            Array.Resize(ref dest.Normal, source.Normal.Length);
            source.Normal.CopyTo(dest.Normal, 0);
            Array.Resize(ref dest.Map, source.Map.Length);
            source.Map.CopyTo(dest.Map, 0);
            Array.Resize(ref dest.F, source.F.Length);
            source.F.CopyTo(dest.F, 0);
            Array.Resize(ref dest.SourceF, source.SourceF.Length);
            source.SourceF.CopyTo(dest.SourceF, 0);
            dest.MaterialGroups = source.MaterialGroups.Select(g => g.Clone()).ToList();
            dest.Closed = source.Closed;
            dest.Seamless = source.Seamless;
            dest.Length = source.Length;
        }

        //TODO documentation and whatnot
        public void Copy(CGShape source) { Copy(this, source); }

        /// <summary>
        /// Converts absolute (World Units) to relative (F) distance
        /// </summary>
        /// <param name="distance">distance in world units</param>
        /// <returns>Relative distance (0..1)</returns>
        public float DistanceToF(float distance)
        {
            return Mathf.Clamp(distance, 0, Length) / Length;
        }

        /// <summary>
        /// Converts relative (F) to absolute distance (World Units)
        /// </summary>
        /// <param name="f">relative distance (0..1)</param>
        /// <returns>Distance in World Units</returns>
        public float FToDistance(float f)
        {
            return Mathf.Clamp01(f) * Length;
        }

        /// <summary>
        /// Gets the index of a certain F
        /// </summary>
        /// <param name="f">F (0..1)</param>
        /// <param name="frag">fragment between the resulting and the next index (0..1)</param>
        /// <returns>the resulting index</returns>
        public int GetFIndex(float f, out float frag)
        {
            if (mCacheLastF != f)
            {
                mCacheLastF = f;
                mCacheLastIndex = getGenericFIndex(ref F, f, out mCacheLastFrag);
            }
            frag = mCacheLastFrag;
            return mCacheLastIndex;
        }

        /*
        /// <summary>
        /// Gets the index of a certain SourceF
        /// </summary>
        /// <param name="sourceF">F (0..1)</param>
        /// <param name="frag">fragment between the resulting and the next index (0..1)</param>
        /// <returns>the resulting index</returns>
        public int GetSourceFIndex(float sourceF, out float frag)
        {
            if (mCacheLastSourceF != sourceF)
            {
                mCacheLastSourceF = sourceF;

                mCacheLastSourceIndex = getGenericFIndex(ref F, sourceF, out mCacheLastSourceFrag);
            }
            frag = mCacheLastSourceFrag;
            return mCacheLastSourceIndex;
        }
        */
        /// <summary>
        /// Interpolates Position by F
        /// </summary>
        /// <param name="f">0..1</param>
        /// <returns>the interpolated position</returns>
        public Vector3 InterpolatePosition(float f)
        {
            float frag;
            int idx = GetFIndex(f, out frag);
            return OptimizedOperators.LerpUnclamped(Position[idx], Position[idx + 1], frag);
        }

        /// <summary>
        /// Interpolates Normal by F
        /// </summary>
        /// <param name="f">0..1</param>
        /// <returns>the interpolated normal</returns>
        public Vector3 InterpolateUp(float f)
        {
            float frag;
            int idx = GetFIndex(f, out frag);
            return Vector3.SlerpUnclamped(Normal[idx], Normal[idx + 1], frag);
        }

        /// <summary>
        /// Interpolates Position and Normal by F
        /// </summary>
        /// <param name="f">0..1</param>
        /// <param name="position"></param>
        /// <param name="up">a.k.a normal</param>
        public void Interpolate(float f, out Vector3 position, out Vector3 up)
        {
            float frag;
            int idx = GetFIndex(f, out frag);
            position = OptimizedOperators.LerpUnclamped(Position[idx], Position[idx + 1], frag);
            up = Vector3.SlerpUnclamped(Normal[idx], Normal[idx + 1], frag);
        }

        public void Move(ref float f, ref int direction, float speed, CurvyClamping clamping)
        {
            f = CurvyUtility.ClampTF(f + speed * direction, ref direction, clamping);
        }

        public void MoveBy(ref float f, ref int direction, float speedDist, CurvyClamping clamping)
        {
            float dist = CurvyUtility.ClampDistance(FToDistance(f) + speedDist * direction, ref direction, clamping, Length);
            f = DistanceToF(dist);
        }

        /// <summary>
        /// Recalculate Length and F[] (by measuring a polyline built from all Position points)
        /// </summary>
        /// <remarks>Call this after TRS'ing a shape</remarks>
        public virtual void Recalculate()
        {
            Length = 0;
            float[] dist = new float[Count];

            for (int i = 1; i < Count; i++)
            {
                dist[i] = dist[i - 1] + (Position[i] - Position[i - 1]).magnitude;

            }

            if (Count > 0)
            {
                Length = dist[Count - 1];
                if (Length > 0)
                {

                    F[0] = 0;
                    float oneOnLength = 1 / Length;
                    for (int i = 1; i < Count - 1; i++)
                        F[i] = dist[i] * oneOnLength;
                    F[Count - 1] = 1;
                }
                else
                    F = new float[Count];
            }

            //for (int i = 1; i < Count; i++)
            //    Direction[i] = (Position[i] - Position[i - 1]).normalized;
        }

        public void RecalculateNormals(List<int> softEdges)
        {
            //TODO this implementation works properly with 2D shapes, but creates invalid results with 3D paths. This is ok for now because the code calls it only on shapes, but it is a ticking bomb
            //TODO document the method after fixing it
            if (Normal.Length != Position.Length)
                Array.Resize(ref Normal, Position.Length);

            for (int mg = 0; mg < MaterialGroups.Count; mg++)
            {
                for (int p = 0; p < MaterialGroups[mg].Patches.Count; p++)
                {
                    SamplePointsPatch patch = MaterialGroups[mg].Patches[p];
                    Vector3 t;
                    int x;
                    for (int vt = 0; vt < patch.Count; vt++)
                    {
                        x = patch.Start + vt;
                        t = (Position[x + 1] - Position[x]).normalized;
                        Normal[x] = new Vector3(-t.y, t.x, 0);
#if CURVY_SANITY_CHECKS_PRIVATE
                        if (Normal[x].magnitude.Approximately(1f) == false)
                            Debug.LogError("Normal is not normalized");//happens if shape is not in the XY plane
#endif
                    }
                    t = (Position[patch.End] - Position[patch.End - 1]).normalized;
                    Normal[patch.End] = new Vector3(-t.y, t.x, 0);
#if CURVY_SANITY_CHECKS_PRIVATE
                    if (Normal[patch.End].magnitude.Approximately(1f) == false)
                        Debug.LogError("Normal is not normalized");//happens if shape is not in the XY plane
#endif
                }
            }

            // Handle soft edges
            for (int i = 0; i < softEdges.Count; i++)
            {
                int previous = softEdges[i] - 1;
                if (previous < 0)
                    previous = Position.Length - 1;

                int beforePrevious = previous - 1;
                if (beforePrevious < 0)
                    beforePrevious = Position.Length - 1;

                int next = softEdges[i] + 1;
                if (next == Position.Length)
                    next = 0;

                Normal[softEdges[i]] = Vector3.Slerp(Normal[beforePrevious], Normal[next], 0.5f);
                Normal[previous] = Normal[softEdges[i]];
            }

        }
    }

    /// <summary>
    /// Path Data (Shape + Direction (Spline Tangents) + Orientation/Up)
    /// </summary>
    [CGDataInfo(0.13f, 0.59f, 0.95f)]
    public class CGPath : CGShape
    {
        public Vector3[] Direction = new Vector3[0];


        public CGPath() : base() { }
        public CGPath(CGPath source) : base(source)
        {
            Direction = (Vector3[])source.Direction.Clone();
        }

        public override T Clone<T>()
        {
            return new CGPath(this) as T;
        }

        public static void Copy(CGPath dest, CGPath source)
        {
            CGShape.Copy(dest, source);
            Array.Resize(ref dest.Direction, source.Direction.Length);
            source.Direction.CopyTo(dest.Direction, 0);
        }

        /// <summary>
        /// Interpolates Position, Direction and Normal by F
        /// </summary>
        /// <param name="f">0..1</param>
        /// <param name="position"></param>
        /// <param name="direction">a.k.a tangent</param>
        /// <param name="up">a.k.a normal</param>
        public void Interpolate(float f, out Vector3 position, out Vector3 direction, out Vector3 up)
        {
            float frag;
            int idx = GetFIndex(f, out frag);
            position = OptimizedOperators.LerpUnclamped(Position[idx], Position[idx + 1], frag);
            direction = Vector3.SlerpUnclamped(Direction[idx], Direction[idx + 1], frag);
            up = Vector3.SlerpUnclamped(Normal[idx], Normal[idx + 1], frag);
        }

        public void Interpolate(float f, float angleF, out Vector3 pos, out Vector3 dir, out Vector3 up)
        {
            Interpolate(f, out pos, out dir, out up);
            if (angleF != 0)
            {
                Quaternion R = Quaternion.AngleAxis(angleF * -360, dir);
                up = R * up;
            }
        }

        public Vector3 InterpolateDirection(float f)
        {
            float frag;
            int idx = GetFIndex(f, out frag);
            return Vector3.SlerpUnclamped(Direction[idx], Direction[idx + 1], frag);
        }
    }

    /// <summary>
    /// Volume Data (Path + Vertex, VertexNormal, Cross)
    /// </summary>
    [CGDataInfo(0.08f, 0.4f, 0.75f)]
    public class CGVolume : CGPath
    {
        /// <summary>
        /// Vertices
        /// </summary>
        public Vector3[] Vertex = new Vector3[0];
        /// <summary>
        /// Normals
        /// </summary>
        public Vector3[] VertexNormal = new Vector3[0];
        public float[] CrossF = new float[0];
        public float[] CrossMap = new float[0];
        /// <summary>
        /// Length of a given cross segment. Will be calculated on demand only!
        /// </summary>
        public float[] SegmentLength;
        /// <summary>
        /// Gets the number of cross shape's sample points
        /// </summary>
        public int CrossSize { get { return CrossF.Length; } }
        /// <summary>
        /// Whether the Cross base spline is closed or not
        /// </summary>
        public bool CrossClosed;//TODO make obsolete then remove this, it is not needed by Curvy
        /// <summary>
        /// Whether the Cross shape covers the whole length of the base spline
        /// </summary>
        public bool CrossSeamless;
        /// <summary>
        /// A shift of the Cross's F value that is applied when using the interpolation methods on the volume, like <see cref="InterpolateVolume"/>
        /// </summary>
        public float CrossFShift;

        public SamplePointsMaterialGroupCollection CrossMaterialGroups;

        public int VertexCount { get { return Vertex.Length; } }

        #region ### Constructors ###

        public CGVolume() : base() { }

        public CGVolume(int samplePoints, CGShape crossShape) : base()
        {
            CrossF = (float[])crossShape.F.Clone();
            CrossMap = (float[])crossShape.Map.Clone();
            CrossClosed = crossShape.Closed;
            CrossSeamless = crossShape.Seamless;
            CrossMaterialGroups = new SamplePointsMaterialGroupCollection(crossShape.MaterialGroups);
            SegmentLength = new float[Count];
            Vertex = new Vector3[CrossSize * samplePoints];
            VertexNormal = new Vector3[Vertex.Length];
        }

        public CGVolume(CGPath path, CGShape crossShape)
            : base(path)
        {
            CrossF = (float[])crossShape.F.Clone();
            CrossMap = (float[])crossShape.Map.Clone();
            SegmentLength = new float[Count];
            CrossClosed = crossShape.Closed;
            CrossSeamless = crossShape.Seamless;
            CrossMaterialGroups = new SamplePointsMaterialGroupCollection(crossShape.MaterialGroups);
            Vertex = new Vector3[CrossSize * Count];
            VertexNormal = new Vector3[Vertex.Length];
        }

        public CGVolume(CGVolume source)
            : base(source)
        {
            Vertex = (Vector3[])source.Vertex.Clone();
            VertexNormal = (Vector3[])source.VertexNormal.Clone();
            CrossF = (float[])source.CrossF.Clone();
            CrossMap = (float[])source.CrossMap.Clone();
            SegmentLength = new float[Count];
            CrossClosed = source.Closed;
            CrossSeamless = source.CrossSeamless;
            CrossFShift = source.CrossFShift;
            CrossMaterialGroups = new SamplePointsMaterialGroupCollection(source.CrossMaterialGroups);
        }

        #endregion

        public static CGVolume Get(CGVolume data, CGPath path, CGShape crossShape)
        {
            if (data == null)
                return new CGVolume(path, crossShape);

            Copy(data, path);
            Array.Resize(ref data.SegmentLength, data.CrossF.Length);
            data.SegmentLength = new float[data.Count];
            // Volume
            Array.Resize(ref data.CrossF, crossShape.F.Length);
            crossShape.F.CopyTo(data.CrossF, 0);

            Array.Resize(ref data.CrossMap, crossShape.Map.Length);
            crossShape.Map.CopyTo(data.CrossMap, 0);

            data.CrossClosed = crossShape.Closed;
            data.CrossSeamless = crossShape.Seamless;
            data.CrossMaterialGroups = new SamplePointsMaterialGroupCollection(crossShape.MaterialGroups);
            Array.Resize(ref data.Vertex, data.CrossSize * data.Position.Length);
            Array.Resize(ref data.VertexNormal, data.Vertex.Length);
            return data;
        }


        public override T Clone<T>()
        {
            return new CGVolume(this) as T;
        }



        public void InterpolateVolume(float f, float crossF, out Vector3 pos, out Vector3 dir, out Vector3 up)
        {
            float frag;
            float cfrag;
            int v0Idx = GetVertexIndex(f, crossF, out frag, out cfrag);

            // (2)-(3)
            //  | \ |
            // (0)-(1)
            Vector3 xd, zd;
            Vector3 v0 = Vertex[v0Idx];
            Vector3 v1 = Vertex[v0Idx + 1];
            Vector3 v2 = Vertex[v0Idx + CrossSize];

            if (frag + cfrag > 1)
            {
                Vector3 v3 = Vertex[v0Idx + CrossSize + 1];
                xd = v3 - v2;
                zd = v3 - v1;
                pos = v2 - zd * (1 - frag) + xd * (cfrag);
            }
            else
            {
                xd = v1 - v0;
                zd = v2 - v0;
                pos = v0 + zd * frag + xd * cfrag;
            }

            dir = zd.normalized;
            up = Vector3.Cross(zd, xd);
        }

        public Vector3 InterpolateVolumePosition(float f, float crossF)
        {
            float frag;
            float cfrag;
            int v0Idx = GetVertexIndex(f, crossF, out frag, out cfrag);
            // (2)-(3)
            //  | \ |
            // (0)-(1)
            Vector3 xd, zd;
            Vector3 v0 = Vertex[v0Idx];
            Vector3 v1 = Vertex[v0Idx + 1];
            Vector3 v2 = Vertex[v0Idx + CrossSize];

            if (frag + cfrag > 1)
            {
                Vector3 v3 = Vertex[v0Idx + CrossSize + 1];
                xd = v3 - v2;
                zd = v3 - v1;
                return v2 - zd * (1 - frag) + xd * (cfrag);
            }
            else
            {
                xd = v1 - v0;
                zd = v2 - v0;
                return v0 + zd * frag + xd * cfrag;
            }
        }

        public Vector3 InterpolateVolumeDirection(float f, float crossF)
        {
            float frag;
            float cfrag;
            int v0Idx = GetVertexIndex(f, crossF, out frag, out cfrag);

            // (2)-(3)
            //  | \ |
            // (0)-(1)
            if (frag + cfrag > 1)
            {
                Vector3 v1 = Vertex[v0Idx + 1];
                Vector3 v3 = Vertex[v0Idx + CrossSize + 1];
                return (v3 - v1).normalized;
            }
            else
            {
                Vector3 v0 = Vertex[v0Idx];
                Vector3 v2 = Vertex[v0Idx + CrossSize];
                return (v2 - v0).normalized;
            }

        }

        public Vector3 InterpolateVolumeUp(float f, float crossF)
        {
            float frag;
            float cfrag;
            int v0Idx = GetVertexIndex(f, crossF, out frag, out cfrag);

            // (2)-(3)
            //  | \ |
            // (0)-(1)
            Vector3 xd, zd;

            Vector3 v1 = Vertex[v0Idx + 1];
            Vector3 v2 = Vertex[v0Idx + CrossSize];

            if (frag + cfrag > 1)
            {
                Vector3 v3 = Vertex[v0Idx + CrossSize + 1];
                xd = v3 - v2;
                zd = v3 - v1;
            }
            else
            {
                Vector3 v0 = Vertex[v0Idx];
                xd = v1 - v0;
                zd = v2 - v0;
            }
            return Vector3.Cross(zd, xd);
        }

        public float GetCrossLength(float pathF)
        {
            int s0;
            int s1;
            float frag;
            GetSegmentIndices(pathF, out s0, out s1, out frag);

            if (SegmentLength[s0] == 0)
                SegmentLength[s0] = calcSegmentLength(s0);
            if (SegmentLength[s1] == 0)
                SegmentLength[s1] = calcSegmentLength(s1);

            return Mathf.LerpUnclamped(SegmentLength[s0], SegmentLength[s1], frag);
        }


        public float CrossFToDistance(float f, float crossF, CurvyClamping crossClamping = CurvyClamping.Clamp)
        {
            return GetCrossLength(f) * CurvyUtility.ClampTF(crossF, crossClamping);
        }

        public float CrossDistanceToF(float f, float distance, CurvyClamping crossClamping = CurvyClamping.Clamp)
        {
            float cl = GetCrossLength(f);
            return CurvyUtility.ClampDistance(distance, crossClamping, cl) / cl;
        }

        public void GetSegmentIndices(float pathF, out int s0Index, out int s1Index, out float frag)
        {
            s0Index = GetFIndex(Mathf.Repeat(pathF, 1), out frag);
            s1Index = s0Index + 1;
        }

        public int GetSegmentIndex(int segment)
        {
            return segment * CrossSize;
        }

        public int GetCrossFIndex(float crossF, out float frag)
        {
            float f = crossF + CrossFShift;
            if (f != 1)
                return getGenericFIndex(ref CrossF, Mathf.Repeat(f, 1), out frag);
            else
                return getGenericFIndex(ref CrossF, f, out frag);
        }

        /// <summary>
        /// Get the index of the first vertex belonging to the segment a certain F is part of
        /// </summary>
        /// <param name="pathF">position on the path (0..1)</param>
        /// <param name="pathFrag">remainder between the returned segment and the next segment</param>
        /// <returns>a vertex index</returns>
        public int GetVertexIndex(float pathF, out float pathFrag)
        {
            int pIdx = GetFIndex(pathF, out pathFrag);
            return pIdx * CrossSize;
        }

        /// <summary>
        /// Get the index of the first vertex of the edge a certain F and CrossF is part of
        /// </summary>
        /// <param name="pathF">position on the path (0..1)</param>
        /// <param name="crossF">position on the cross (0..1)</param>
        /// <param name="pathFrag">remainder between the segment and the next segment</param>
        /// <param name="crossFrag">remainder between the returned vertex and the next vertex</param>
        /// <returns>a vertex index</returns>
        public int GetVertexIndex(float pathF, float crossF, out float pathFrag, out float crossFrag)
        {
            int pIdx = GetVertexIndex(pathF, out pathFrag);
            int cIdx = GetCrossFIndex(crossF, out crossFrag);
            return pIdx + cIdx;
        }

        /// <summary>
        /// Gets all vertices belonging to one or more extruded shape segments
        /// </summary>
        /// <param name="segmentIndices">indices of segments in question</param>
        public Vector3[] GetSegmentVertices(params int[] segmentIndices)
        {
            Vector3[] verts = new Vector3[CrossSize * segmentIndices.Length];
            for (int i = 0; i < segmentIndices.Length; i++)
                Array.Copy(Vertex, segmentIndices[i] * CrossSize, verts, i * CrossSize, CrossSize);
            return verts;
        }


        float calcSegmentLength(int segmentIndex)
        {
            int vstart = segmentIndex * CrossSize;
            int vend = vstart + CrossSize - 1;
            float l = 0;
            for (int i = vstart; i < vend; i++)
                l += (Vertex[i + 1] - Vertex[i]).magnitude;

            return l;
        }

    }

    /// <summary>
    /// Bounds data class
    /// </summary>
    [CGDataInfo(1, 0.8f, 0.5f)]
    public class CGBounds : CGData
    {
        protected Bounds? mBounds;
        public Bounds Bounds
        {
            get
            {
                if (!mBounds.HasValue)
                    RecalculateBounds();
                return mBounds.Value;
            }
            set
            {
                if (mBounds != value)
                    mBounds = value;
            }
        }

        public float Depth
        {
            get
            {
                return Bounds.size.z;
            }
        }

        public CGBounds() : base() { }

        public CGBounds(Bounds bounds) : base()
        {
            Bounds = bounds;
        }

        public CGBounds(CGBounds source)
        {
            Name = source.Name;
            if (source.mBounds.HasValue) //Do not copy bounds if they are not computed yet
                Bounds = source.Bounds;
        }


        public virtual void RecalculateBounds()
        {
            Bounds = new Bounds();
        }

        public override T Clone<T>()
        {
            return new CGBounds(this) as T;
        }

        public static void Copy(CGBounds dest, CGBounds source)
        {
            if (source.mBounds.HasValue) //Do not copy bounds if they are not computed yet
                dest.Bounds = source.Bounds;
        }
    }

    /// <summary>
    /// SubMesh data (triangles, material)
    /// </summary>
    public class CGVSubMesh : CGData
    {
        public int[] Triangles;
        public Material Material;

        public override int Count
        {
            get
            {
                return Triangles.Length;
            }
        }

        public CGVSubMesh(Material material = null) : base()
        {
            Material = material;
            Triangles = new int[0];
        }

        public CGVSubMesh(int[] triangles, Material material = null) : base()
        {
            Material = material;
            Triangles = triangles;
        }

        public CGVSubMesh(int triangleCount, Material material = null) : base()
        {
            Material = material;
            Triangles = new int[triangleCount];
        }

        public CGVSubMesh(CGVSubMesh source) : base()
        {
            Material = source.Material;
            Triangles = (int[])source.Triangles.Clone();
        }

        public override T Clone<T>()
        {
            return new CGVSubMesh(this) as T;
        }

        public static CGVSubMesh Get(CGVSubMesh data, int triangleCount, Material material = null)
        {

            if (data == null)
                return new CGVSubMesh(triangleCount, material);

            Array.Resize(ref data.Triangles, triangleCount);
            data.Material = material;
            return data;
        }

        public void ShiftIndices(int offset, int startIndex = 0)
        {
            for (int i = startIndex; i < Triangles.Length; i++)
                Triangles[i] += offset;
        }

        public void Add(CGVSubMesh other, int shiftIndexOffset = 0)
        {
            int trianglesLength = Triangles.Length;
            int otherTriangleLength = other.Triangles.Length;

            if (otherTriangleLength == 0)
                return;

            int[] oldTriangles = Triangles;
            Triangles = new int[trianglesLength + otherTriangleLength];
            Array.Copy(oldTriangles, Triangles, trianglesLength);
            Array.Copy(other.Triangles, 0, Triangles, trianglesLength, otherTriangleLength);

            if (shiftIndexOffset != 0)
                ShiftIndices(shiftIndexOffset, trianglesLength);
        }
    }

    /// <summary>
    /// Mesh Data (Bounds + Vertex,UV,UV2,Normal,Tangents,SubMehes)
    /// </summary>
    [CGDataInfo(0.98f, 0.5f, 0)]
    public class CGVMesh : CGBounds
    {

#if CONTRACTS_FULL
        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Vertex != null);
            Contract.Invariant(UV != null);
            Contract.Invariant(UV2 != null);
            Contract.Invariant(Normal != null);
            Contract.Invariant(Tangents != null);
            Contract.Invariant(SubMeshes != null);

            Contract.Invariant(UV.Length == 0 || UV.Length == Vertex.Length);
            Contract.Invariant(UV2.Length == 0 || UV2.Length == Vertex.Length);
            Contract.Invariant(Normal.Length == 0 || Normal.Length == Vertex.Length);
            Contract.Invariant(Tangents.Length == 0 || Tangents.Length == Vertex.Length);
        }
#endif



        public Vector3[] Vertex;
        public Vector2[] UV;
        public Vector2[] UV2;
        public Vector3[] Normal;
        public Vector4[] Tangents;
        public CGVSubMesh[] SubMeshes;
        /// <summary>
        /// Gets the number of vertices
        /// </summary>
        public override int Count
        {
            get
            {
                return Vertex.Length;
            }
        }

        public bool HasUV { get { return UV.Length > 0; } }
        public bool HasUV2 { get { return UV2.Length > 0; } }
        public bool HasNormals { get { return Normal.Length > 0; } }
        public bool HasTangents { get { return Tangents.Length > 0; } }

        public int TriangleCount
        {
            get
            {
                int cnt = 0;
                for (int i = 0; i < SubMeshes.Length; i++)
                    cnt += SubMeshes[i].Triangles.Length;
                return cnt / 3;
            }
        }

        public CGVMesh() : this(0) { }
        public CGVMesh(int vertexCount, bool addUV = false, bool addUV2 = false, bool addNormals = false, bool addTangents = false) : base()
        {
            Vertex = new Vector3[vertexCount];
            UV = addUV
                ? new Vector2[vertexCount]
                : new Vector2[0];
            UV2 = addUV2
                ? new Vector2[vertexCount]
                : new Vector2[0];
            Normal = addNormals
                ? new Vector3[vertexCount]
                : new Vector3[0];
            Tangents = addTangents
                ? new Vector4[vertexCount]
                : new Vector4[0];

            SubMeshes = new CGVSubMesh[0];
        }
        public CGVMesh(CGVolume volume) : this(volume.Vertex.Length)
        {
            Array.Copy(volume.Vertex, Vertex, volume.Vertex.Length);
        }

        public CGVMesh(CGVolume volume, IntRegion subset)
            : this((subset.LengthPositive + 1) * volume.CrossSize, false, false, true)
        {
            int start = subset.Low * volume.CrossSize;
            Array.Copy(volume.Vertex, start, Vertex, 0, Vertex.Length);
            Array.Copy(volume.VertexNormal, start, Normal, 0, Normal.Length);
        }

        public CGVMesh(CGVMesh source) : base(source)
        {
            Vertex = (Vector3[])source.Vertex.Clone();
            UV = (Vector2[])source.UV.Clone();
            UV2 = (Vector2[])source.UV2.Clone();
            Normal = (Vector3[])source.Normal.Clone();
            Tangents = (Vector4[])source.Tangents.Clone();
            SubMeshes = new CGVSubMesh[source.SubMeshes.Length];
            for (int i = 0; i < source.SubMeshes.Length; i++)
                SubMeshes[i] = new CGVSubMesh(source.SubMeshes[i]);
        }

        public CGVMesh(CGMeshProperties meshProperties) : this(meshProperties.Mesh, meshProperties.Material, meshProperties.Matrix) { }

        public CGVMesh(Mesh source, Material[] materials, Matrix4x4 trsMatrix) : base()
        {
            Name = source.name;
            Vertex = (Vector3[])source.vertices.Clone();
            Normal = (Vector3[])source.normals.Clone();
            Tangents = (Vector4[])source.tangents.Clone();
            UV = (Vector2[])source.uv.Clone();
            UV2 = (Vector2[])source.uv2.Clone();
            SubMeshes = new CGVSubMesh[source.subMeshCount];
            for (int s = 0; s < source.subMeshCount; s++)
                SubMeshes[s] = new CGVSubMesh(source.GetTriangles(s), (materials.Length > s) ? materials[s] : null);

            Bounds = source.bounds;

            if (!trsMatrix.isIdentity)
                TRS(trsMatrix);

        }

        public override T Clone<T>()
        {
            return new CGVMesh(this) as T;
        }

        public static CGVMesh Get(CGVMesh data, CGVolume source, bool addUV, bool reverseNormals)
        {
            return Get(data, source, new IntRegion(0, source.Count - 1), addUV, reverseNormals);
        }

        public static CGVMesh Get(CGVMesh data, CGVolume source, IntRegion subset, bool addUV, bool reverseNormals)
        {
            int start = subset.Low * source.CrossSize;
            int size = (subset.LengthPositive + 1) * source.CrossSize;

            if (data == null)
            {
                data = new CGVMesh(size, addUV, false, true);
            }
            else
            {
                if (data.Vertex.Length != size)
                    data.Vertex = new Vector3[size];
                if (data.Normal.Length != size)
                    data.Normal = new Vector3[size];

                int uvSize = (addUV) ? source.Vertex.Length : 0;
                if (data.UV.Length != uvSize)
                    data.UV = new Vector2[uvSize];

                //data.SubMeshes = new CGVSubMesh[0];//BUG? why is this commented?

                if (data.UV2.Length != 0)
                    data.UV2 = new Vector2[0];
                if (data.Tangents.Length != 0)
                    data.Tangents = new Vector4[0];
            }

            Array.Copy(source.Vertex, start, data.Vertex, 0, size);
            Array.Copy(source.VertexNormal, start, data.Normal, 0, size);

            if (reverseNormals)
                //OPTIM merge loop with normals copy
                for (int n = 0; n < data.Normal.Length; n++)
                {
                    data.Normal[n].x = -data.Normal[n].x;
                    data.Normal[n].y = -data.Normal[n].y;
                    data.Normal[n].z = -data.Normal[n].z;
                }

            return data;
        }


        public void SetSubMeshCount(int count)
        {
            Array.Resize(ref SubMeshes, count);
        }

        public void AddSubMesh(CGVSubMesh submesh = null)
        {
            SubMeshes = SubMeshes.Add(submesh);
        }

        /// <summary>
        /// Combine/Merge another VMesh into this
        /// </summary>
        /// <param name="source"></param>
        public void MergeVMesh(CGVMesh source)
        {
            int preMergeVertexCount = Count;
            // Add base data
            copyData(ref source.Vertex, ref Vertex, preMergeVertexCount, source.Count);

            MergeUVsNormalsAndTangents(source, preMergeVertexCount);

            // Add Submeshes
            for (int sm = 0; sm < source.SubMeshes.Length; sm++)
                GetMaterialSubMesh(source.SubMeshes[sm].Material).Add(source.SubMeshes[sm], preMergeVertexCount);

            mBounds = null;
        }
        /// <summary>
        /// Combine/Merge another VMesh into this, applying a matrix
        /// </summary>
        /// <param name="source"></param>
        /// <param name="matrix"></param>
        public void MergeVMesh(CGVMesh source, Matrix4x4 matrix)
        {
            int preMergeVertexCount = Count;
            // Add base data

            Array.Resize(ref Vertex, Count + source.Count);
            int c = Count;
            for (int v = preMergeVertexCount; v < c; v++)
                Vertex[v] = matrix.MultiplyPoint3x4(source.Vertex[v - preMergeVertexCount]);

            MergeUVsNormalsAndTangents(source, preMergeVertexCount);

            // Add Submeshes
            for (int sm = 0; sm < source.SubMeshes.Length; sm++)
                GetMaterialSubMesh(source.SubMeshes[sm].Material).Add(source.SubMeshes[sm], preMergeVertexCount);

            mBounds = null;
        }

        /// <summary>
        /// Combine/Merge multiple CGVMeshes into this
        /// </summary>
        /// <param name="vMeshes">list of CGVMeshes</param>
        /// <param name="startIndex">Index of the first element of the list to merge</param>
        /// <param name="endIndex">Index of the last element of the list to merge</param>
        public void MergeVMeshes(List<CGVMesh> vMeshes, int startIndex, int endIndex)
        {
            Assert.IsTrue(endIndex < vMeshes.Count);

            int totalVertexCount = 0;
            bool hasNormals = false;
            bool hasTangents = false;
            bool hasUV = false;
            bool hasUV2 = false;
            Dictionary<Material, List<int[]>> submeshesByMaterial = new Dictionary<Material, List<int[]>>();
            Dictionary<Material, int> trianglesIndexPerMaterial = new Dictionary<Material, int>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                CGVMesh cgvMesh = vMeshes[i];
                totalVertexCount += cgvMesh.Count;
                hasNormals |= cgvMesh.HasNormals;
                hasTangents |= cgvMesh.HasTangents;
                hasUV |= cgvMesh.HasUV;
                hasUV2 |= cgvMesh.HasUV2;

                for (int sm = 0; sm < cgvMesh.SubMeshes.Length; sm++)
                {
                    CGVSubMesh subMesh = cgvMesh.SubMeshes[sm];
                    if (submeshesByMaterial.ContainsKey(subMesh.Material) == false)
                    {
                        submeshesByMaterial[subMesh.Material] = new List<int[]>(1);
                        trianglesIndexPerMaterial[subMesh.Material] = 0;
                    }

                    List<int[]> trianglesOfMaterial = submeshesByMaterial[subMesh.Material];
                    trianglesOfMaterial.Add(subMesh.Triangles);
                }
            }

            Vertex = new Vector3[totalVertexCount];
            if (hasNormals)
                Normal = new Vector3[totalVertexCount];
            if (hasTangents)
                Tangents = new Vector4[totalVertexCount];
            if (hasUV)
                UV = new Vector2[totalVertexCount];
            if (hasUV2)
                UV2 = new Vector2[totalVertexCount];

            foreach (KeyValuePair<Material, List<int[]>> pair in submeshesByMaterial)
            {
                List<int[]> materialTriangleArrays = pair.Value;

                CGVSubMesh subMesh = new CGVSubMesh(pair.Key);
                int totalTrianglesCount = 0;
                for (int arraysIndex = 0; arraysIndex < pair.Value.Count; arraysIndex++)
                    totalTrianglesCount += materialTriangleArrays[arraysIndex].Length;
                subMesh.Triangles = new int[totalTrianglesCount];
                AddSubMesh(subMesh);
            }


            int currentVertexCount = 0;
            for (int i = startIndex; i <= endIndex; i++)
            {
                CGVMesh source = vMeshes[i];

                Array.Copy(source.Vertex, 0, Vertex, currentVertexCount, source.Vertex.Length);
                if (hasNormals && source.HasNormals)
                    Array.Copy(source.Normal, 0, Normal, currentVertexCount, source.Normal.Length);
                if (hasTangents && source.HasTangents)
                    Array.Copy(source.Tangents, 0, Tangents, currentVertexCount, source.Tangents.Length);
                if (hasUV && source.HasUV)
                    Array.Copy(source.UV, 0, UV, currentVertexCount, source.UV.Length);
                if (hasUV2 && source.HasUV2)
                    Array.Copy(source.UV2, 0, UV2, currentVertexCount, source.UV2.Length);

                // Add Submeshes
                for (int subMeshIndex = 0; subMeshIndex < source.SubMeshes.Length; subMeshIndex++)
                {
                    CGVSubMesh sourceSubMesh = source.SubMeshes[subMeshIndex];
                    Material sourceMaterial = sourceSubMesh.Material;
                    int[] sourceTriangles = sourceSubMesh.Triangles;
                    int sourceTrianglesLength = sourceTriangles.Length;

                    int[] destinationTriangles = GetMaterialSubMesh(sourceMaterial).Triangles;

                    int trianglesIndex = trianglesIndexPerMaterial[sourceMaterial];

                    if (sourceTrianglesLength != 0)
                    {
                        if (currentVertexCount == 0)
                            Array.Copy(sourceTriangles, 0, destinationTriangles, trianglesIndex, sourceTrianglesLength);
                        else
                            for (int j = 0; j < sourceTrianglesLength; j++)
                                destinationTriangles[trianglesIndex + j] = sourceTriangles[j] + currentVertexCount;

                        trianglesIndexPerMaterial[sourceMaterial] = trianglesIndex + sourceTrianglesLength;

                    }
                }
                currentVertexCount += source.Vertex.Length;
            }

        }

        private void MergeUVsNormalsAndTangents(CGVMesh source, int preMergeVertexCount)
        {
            int sourceLength = source.Count;
            if (sourceLength == 0)
                return;

            int postMergeVetexCount = preMergeVertexCount + sourceLength;
            if (HasUV || source.HasUV)
            {
                Vector2[] preMergeUV = UV;
                UV = new Vector2[postMergeVetexCount];
                if (HasUV)
                    Array.Copy(preMergeUV, UV, preMergeVertexCount);
                if (source.HasUV)
                    Array.Copy(source.UV, 0, UV, preMergeVertexCount, sourceLength);

            }

            if (HasUV2 || source.HasUV2)
            {
                Vector2[] preMergeUV2 = UV2;
                UV2 = new Vector2[postMergeVetexCount];
                if (HasUV2)
                    Array.Copy(preMergeUV2, UV2, preMergeVertexCount);
                if (source.HasUV2)
                    Array.Copy(source.UV2, 0, UV2, preMergeVertexCount, sourceLength);

            }

            if (HasNormals || source.HasNormals)
            {
                Vector3[] preMergeNormal = Normal;
                Normal = new Vector3[postMergeVetexCount];
                if (HasNormals)
                    Array.Copy(preMergeNormal, Normal, preMergeVertexCount);
                if (source.HasNormals)
                    Array.Copy(source.Normal, 0, Normal, preMergeVertexCount, sourceLength);

            }

            if (HasTangents || source.HasTangents)
            {
                Vector4[] preMergeTangents = Tangents;
                Tangents = new Vector4[postMergeVetexCount];
                if (HasTangents)
                    Array.Copy(preMergeTangents, Tangents, preMergeVertexCount);
                if (source.HasTangents)
                    Array.Copy(source.Tangents, 0, Tangents, preMergeVertexCount, sourceLength);

            }
        }

        /// <summary>
        /// Gets the submesh using a certain material
        /// </summary>
        /// <param name="mat">the material the submesh should use</param>
        /// <param name="createIfMissing">whether to create the submesh if no existing one matches</param>
        /// <returns>a submesh using the given material</returns>
        public CGVSubMesh GetMaterialSubMesh(Material mat, bool createIfMissing = true)
        {
            // already having submesh with matching material?
            for (int sm = 0; sm < SubMeshes.Length; sm++)
                if (SubMeshes[sm].Material == mat)
                    return SubMeshes[sm];

            // else create new
            if (createIfMissing)
            {
                CGVSubMesh sm = new CGVSubMesh(mat);
                AddSubMesh(sm);
                return sm;
            }
            else
                return null;
        }

        /// <summary>
        /// Creates a Mesh from the data
        /// </summary>
        public Mesh AsMesh()
        {
            Mesh msh = new Mesh();
            ToMesh(ref msh);
            return msh;
        }

        /// <summary>
        /// Copies the data into an existing Mesh
        /// </summary>
        public void ToMesh(ref Mesh msh)
        {

            msh.vertices = Vertex;

            if (HasUV)
                msh.uv = UV;
            if (HasUV2)
                msh.uv2 = UV2;

            if (HasNormals)
                msh.normals = Normal;

            if (HasTangents)
                msh.tangents = Tangents;

            msh.subMeshCount = SubMeshes.Length;
            for (int s = 0; s < SubMeshes.Length; s++)
                msh.SetTriangles(SubMeshes[s].Triangles, s);


        }

        /// <summary>
        /// Gets a list of all Materials used
        /// </summary>
        public Material[] GetMaterials()
        {
            List<Material> mats = new List<Material>();
            for (int s = 0; s < SubMeshes.Length; s++)
                mats.Add(SubMeshes[s].Material);
            return mats.ToArray();
        }

        public override void RecalculateBounds()
        {
            if (Count == 0)
            {
                mBounds = new Bounds(Vector3.zero, Vector3.zero);
            }
            else
            {
                Bounds b = new Bounds(Vertex[0], Vector3.zero);
                int u = Vertex.Length;
                for (int i = 1; i < u; i++)
                    b.Encapsulate(Vertex[i]);
                mBounds = b;
            }
        }

        public void RecalculateUV2()
        {
            UV2 = CGUtility.CalculateUV2(UV);
        }

        /// <summary>
        /// Applies the translation, rotation and scale defined by the given matrix
        /// </summary>
        public void TRS(Matrix4x4 matrix)
        {
            int count = Count;
            for (int vertexIndex = 0; vertexIndex < count; vertexIndex++)
                Vertex[vertexIndex] = matrix.MultiplyPoint3x4(Vertex[vertexIndex]);

            count = Normal.Length;
            for (int vertexIndex = 0; vertexIndex < count; vertexIndex++)
                Normal[vertexIndex] = matrix.MultiplyVector(Normal[vertexIndex]);

            count = Tangents.Length;
            for (int vertexIndex = 0; vertexIndex < count; vertexIndex++)
            {
                //Keep in mind that Tangents is a Vector4 array
                Vector3 tangent = matrix.MultiplyVector(Tangents[vertexIndex]);
                Tangents[vertexIndex].x = tangent.x;
                Tangents[vertexIndex].y = tangent.y;
                Tangents[vertexIndex].z = tangent.z;
            }

            mBounds = null;
        }

        void copyData<T>(ref T[] src, ref T[] dst, int currentSize, int extraSize)
        {
            if (extraSize == 0)
                return;

            T[] oldDestination = dst;
            dst = new T[currentSize + extraSize];
            Array.Copy(oldDestination, dst, currentSize);
            Array.Copy(src, 0, dst, currentSize, extraSize);
        }

    }

    /// <summary>
    /// GameObject data (Bounds + Object)
    /// </summary>
    [CGDataInfo("#FFF59D")]
    public class CGGameObject : CGBounds
    {
        public GameObject Object;
        public Vector3 Translate;
        public Vector3 Rotate;
        public Vector3 Scale = Vector3.one;

        public Matrix4x4 Matrix
        {
            get { return Matrix4x4.TRS(Translate, Quaternion.Euler(Rotate), Scale); }
        }

        public CGGameObject() : base() { }

        public CGGameObject(CGGameObjectProperties properties) : this(properties.Object, properties.Translation, properties.Rotation, properties.Scale) { }

        public CGGameObject(GameObject obj) : this(obj, Vector3.zero, Vector3.zero, Vector3.one) { }

        public CGGameObject(GameObject obj, Vector3 translate, Vector3 rotate, Vector3 scale)
            : base()
        {
            Object = obj;
            Translate = translate;
            Rotate = rotate;
            Scale = scale;
            if (Object)
                Name = Object.name;
        }

        public CGGameObject(CGGameObject source) : base(source)
        {
            Object = source.Object;
            Translate = source.Translate;
            Rotate = source.Rotate;
            Scale = source.Scale;
        }

        public override T Clone<T>()
        {
            return new CGGameObject(this) as T;
        }

        [Obsolete("Member not used by Curvy, will get remove. Copy it if you still need it")]
        public static CGGameObject Get(CGGameObject data, GameObject obj, Vector3 translate, Vector3 rotate, Vector3 scale)
        {
            if (data == null)
                return new CGGameObject(obj);

            data.Object = obj;
            data.Name = (obj != null) ? obj.name : null;
            data.Translate = translate;
            data.Rotate = rotate;
            data.Scale = scale;
            return data;
        }

        public override void RecalculateBounds()
        {
            if (Object == null)
            {
                mBounds = new Bounds();
            }
            else
            {
                Renderer[] renderer = Object.GetComponentsInChildren<Renderer>(true);
                Collider[] collider = Object.GetComponentsInChildren<Collider>(true);
                Bounds bounds;
                if (renderer.Length > 0)
                {
                    bounds = renderer[0].bounds;
                    for (int i = 1; i < renderer.Length; i++)
                        bounds.Encapsulate(renderer[i].bounds);
                    for (int i = 0; i < collider.Length; i++)
                        bounds.Encapsulate(collider[i].bounds);
                }
                else if (collider.Length > 0)
                {
                    bounds = collider[0].bounds;
                    for (int i = 1; i < collider.Length; i++)
                        bounds.Encapsulate(collider[i].bounds);
                }
                else
                    bounds = new Bounds();

                Vector3 rotationlessBoundsSize = (Quaternion.Inverse(Object.transform.localRotation) * bounds.size);
                bounds.size = new Vector3(
                    rotationlessBoundsSize.x * Scale.x,
                    rotationlessBoundsSize.y * Scale.y,
                    rotationlessBoundsSize.z * Scale.z);

                mBounds = bounds;
            }
        }
    }

    /// <summary>
    /// Spots Collection Data
    /// </summary>
    [CGDataInfo(0.96f, 0.96f, 0.96f)]
    public class CGSpots : CGData
    {
        public CGSpot[] Points;

        public override int Count
        {
            get
            {
                return Points.Length;
            }
        }

        public CGSpots() : base()
        {
            Points = new CGSpot[0];
        }

        public CGSpots(params CGSpot[] points) : base()
        {
            Points = points;
        }

        public CGSpots(params List<CGSpot>[] lists) : base()
        {
            int c = 0;
            for (int i = 0; i < lists.Length; i++)
                c += lists[i].Count;
            Points = new CGSpot[c];
            c = 0;
            for (int i = 0; i < lists.Length; i++)
            {
                lists[i].CopyTo(Points, c);
                c += lists[i].Count;
            }
        }

        public CGSpots(CGSpots source) : base()
        {
            Points = source.Points;
        }

        public override T Clone<T>()
        {
            return new CGSpots(this) as T;
        }
    }



    #region ### IOnRequestProcessing ###

    /// <summary>
    /// Request Parameter base class
    /// </summary>
    public class CGDataRequestParameter
    {
        public static implicit operator bool(CGDataRequestParameter a)
        {
            return !ReferenceEquals(a, null);
        }
    }

    /// <summary>
    /// Additional Spline Request parameters
    /// </summary>
    public class CGDataRequestMetaCGOptions : CGDataRequestParameter
    {
        /// <summary>
        /// Whether Hard Edges should produce extra samples
        /// </summary>
        /// <remarks>This may result in extra samples at affected Control Points</remarks>
        public bool CheckHardEdges;
        /// <summary>
        /// Whether MaterialID's should be stored
        /// </summary>
        /// <remarks>This may result in extra samples at affected Control Points</remarks>
        public bool CheckMaterialID;
        /// <summary>
        /// Whether all Control Points should be included
        /// </summary>
        public bool IncludeControlPoints;
        /// <summary>
        /// Whether UVEdge, ExplicitU and custom U settings should be included
        /// </summary>
        public bool CheckExtendedUV;


        public CGDataRequestMetaCGOptions(bool checkEdges, bool checkMaterials, bool includeCP, bool extendedUV)
        {
            CheckHardEdges = checkEdges;
            CheckMaterialID = checkMaterials;
            IncludeControlPoints = includeCP;
            CheckExtendedUV = extendedUV;
        }

        public override bool Equals(object obj)
        {
            CGDataRequestMetaCGOptions O = obj as CGDataRequestMetaCGOptions;
            if (O == null)
                return false;
            return (CheckHardEdges == O.CheckHardEdges && CheckMaterialID == O.CheckMaterialID && IncludeControlPoints == O.IncludeControlPoints && CheckExtendedUV == O.CheckExtendedUV);
        }

        public override int GetHashCode()
        {
            return new { A = CheckHardEdges, B = CheckMaterialID, C = IncludeControlPoints, D = CheckExtendedUV }.GetHashCode(); //OPTIM avoid array creation
        }

    }

    /// <summary>
    /// Shape Rasterization Request parameters
    /// </summary>
    public class CGDataRequestShapeRasterization : CGDataRequestRasterization
    {
        /// <summary>
        /// The <see cref="CGPath.F"/> array of the <see cref="CGPath"/> instance used for the shape extrusion that requests the current Shape rasterization
        /// </summary>
        public float[] PathF;

        public CGDataRequestShapeRasterization(float[] pathF, float start, float rasterizedRelativeLength, int resolution, float angle, ModeEnum mode = ModeEnum.Even): base(start, rasterizedRelativeLength, resolution, angle, mode)
        {
            PathF = pathF;
        }

        public override bool Equals(object obj)
        {
            CGDataRequestShapeRasterization other = obj as CGDataRequestShapeRasterization;
            if (other == null)
                return false;

            return base.Equals(obj) && other.PathF.Length == PathF.Length && other.PathF.SequenceEqual(PathF);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (base.GetHashCode() * 397) ^ (PathF != null
                           ? PathF.GetHashCode()
                           : 0);
            }
        }
    }

    /// <summary>
    /// Rasterization Request parameters
    /// </summary>
    public class CGDataRequestRasterization : CGDataRequestParameter
    {
#if CONTRACTS_FULL
        [ContractInvariantMethod]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Required for code contracts.")]
        private void ObjectInvariant()
        {
            Contract.Invariant(Start.IsRatio());
            Contract.Invariant(RasterizedRelativeLength.IsRatio());
            Contract.Invariant(Resolution > 0);
            Contract.Invariant(Resolution <= 100);
            Contract.Invariant(SplineAbsoluteLength.IsPositiveNumber());
            Contract.Invariant(AngleThreshold.IsIn0To180Range());
        }
#endif


        public enum ModeEnum
        {
            /// <summary>
            /// Distribute sample points evenly spread
            /// </summary>
            Even,
            /// <summary>
            /// Use Source' curvation to optimize the result
            /// </summary>
            Optimized
        }

        /// <summary>
        /// Relative Start Position (0..1)
        /// </summary>
        public float Start;

        /// <summary>
        /// Relative Length. A value of 1 means the full spline length
        /// </summary>
        public float RasterizedRelativeLength;

        /// <summary>
        /// Maximum number of samplepoints
        /// </summary>
        public int Resolution;

        /// <summary>
        /// Angle resolution (0..100) for optimized mode
        /// </summary>
        public float AngleThreshold;

        /// <summary>
        /// Rasterization mode
        /// </summary>
        public ModeEnum Mode;

        public CGDataRequestRasterization(float start, float rasterizedRelativeLength, int resolution, float angle, ModeEnum mode = ModeEnum.Even)
        {
#if CONTRACTS_FULL
            Contract.Requires(rasterizedRelativeLength.IsRatio());
#endif


            Start = Mathf.Repeat(start, 1);
            RasterizedRelativeLength = Mathf.Clamp01(rasterizedRelativeLength);
            Resolution = resolution;
            AngleThreshold = angle;
            Mode = mode;
        }

        public CGDataRequestRasterization(CGDataRequestRasterization source) : this(source.Start, source.RasterizedRelativeLength, source.Resolution, source.AngleThreshold, source.Mode)
        {
        }

        public override bool Equals(object obj)
        {
            CGDataRequestRasterization O = obj as CGDataRequestRasterization;
            if (O == null)
                return false;
            return (Start == O.Start && RasterizedRelativeLength == O.RasterizedRelativeLength && Resolution == O.Resolution && AngleThreshold == O.AngleThreshold && Mode == O.Mode);
        }

        public override int GetHashCode()
        {
            return new { A = Start, B = RasterizedRelativeLength, C = Resolution, D = AngleThreshold, E = Mode}.GetHashCode(); //OPTIM avoid array creation
        }
    }



    #endregion

}
