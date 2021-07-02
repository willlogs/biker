// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.ComponentModel;
using FluffyUnderware.Curvy.ThirdParty.LibTessDotNet;
#if UNITY_EDITOR
using UnityEditor;
#endif
using FluffyUnderware.DevTools;


namespace FluffyUnderware.Curvy.Utils
{

    /// <summary>
    /// Taken from my asset Frame Rate Booster
    /// https://assetstore.unity.com/packages/tools/utilities/frame-rate-booster-120660
    /// </summary>
    public static class OptimizedOperators
    {
        public static Vector3 Addition(this Vector3 a, Vector3 b)
        {
            a.x += b.x;
            a.y += b.y;
            a.z += b.z;
            return a;
        }

        public static Vector3 UnaryNegation(this Vector3 a)
        {
            Vector3 result;
            result.x = -a.x;
            result.y = -a.y;
            result.z = -a.z;
            return result;
        }

        public static Vector3 Subtraction(this Vector3 a, Vector3 b)
        {
            a.x -= b.x;
            a.y -= b.y;
            a.z -= b.z;
            return a;

        }

        public static Vector3 Multiply(this Vector3 a, float d)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;
            return a;
        }

        public static Vector3 Multiply(this float d, Vector3 a)
        {
            a.x *= d;
            a.y *= d;
            a.z *= d;
            return a;
        }

        public static Vector3 Division(this Vector3 a, float d)
        {
            float inversed = 1 / d;
            a.x *= inversed;
            a.y *= inversed;
            a.z *= inversed;
            return a;
        }

        public static Vector3 Normalize(this Vector3 value)
        {
            Vector3 result;
            float num = (float)Math.Sqrt(value.x * (double)value.x + value.y * (double)value.y + value.z * (double)value.z);
            if (num > 9.99999974737875E-06)
            {
                float inversed = 1 / num;
                result.x = value.x * inversed;
                result.y = value.y * inversed;
                result.z = value.z * inversed;
            }
            else
            {
                result.x = 0;
                result.y = 0;
                result.z = 0;
            }
            return result;
        }

        public static Vector3 LerpUnclamped(this Vector3 a, Vector3 b, float t)
        {
            a.x += (b.x - a.x) * t;
            a.y += (b.y - a.y) * t;
            a.z += (b.z - a.z) * t;
            return a;
        }
    }


    /// <summary>
    /// Curvy Utility class
    /// </summary>
    public static class CurvyUtility
    {
        #region ### Clamping Methods ###

        /// <summary>
        /// Clamps relative position
        /// </summary>
        public static float ClampTF(float tf, CurvyClamping clamping)
        {
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(tf, 1);
                case CurvyClamping.PingPong:
                    return Mathf.PingPong(tf, 1);
                case CurvyClamping.Clamp:
                    return Mathf.Clamp01(tf);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }


        /// <summary>
        /// Clamps relative position and sets new direction
        /// </summary>
        public static float ClampTF(float tf, ref int dir, CurvyClamping clamping)
        {
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(tf, 1);
                case CurvyClamping.PingPong:
                    if (Mathf.FloorToInt(tf) % 2 != 0)
                        dir *= -1;
                    return Mathf.PingPong(tf, 1);
                case CurvyClamping.Clamp:
                    return Mathf.Clamp01(tf);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Clamps a float to a range
        /// </summary>
        public static float ClampValue(float tf, CurvyClamping clamping, float minTF, float maxTF)
        {

            switch (clamping)
            {
                case CurvyClamping.Loop:
                    float v1 = DTMath.MapValue(0, 1, tf, minTF, maxTF);
                    return DTMath.MapValue(minTF, maxTF, Mathf.Repeat(v1, 1), 0, 1);
                case CurvyClamping.PingPong:
                    float v2 = DTMath.MapValue(0, 1, tf, minTF, maxTF);
                    return DTMath.MapValue(minTF, maxTF, Mathf.PingPong(v2, 1), 0, 1);
                case CurvyClamping.Clamp:
                    return Mathf.Clamp(tf, minTF, maxTF);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Clamps absolute position
        /// </summary>
        public static float ClampDistance(float distance, CurvyClamping clamping, float length)
        {
            if (length == 0)
                return 0;
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(distance, length);
                case CurvyClamping.PingPong:
                    return Mathf.PingPong(distance, length);
                case CurvyClamping.Clamp:
                    return Mathf.Clamp(distance, 0, length);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Clamps absolute position
        /// </summary>
        public static float ClampDistance(float distance, CurvyClamping clamping, float length, float min, float max)
        {
            if (length == 0)
                return 0;
            min = Mathf.Clamp(min, 0, length);
            max = Mathf.Clamp(max, min, length);
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return min + Mathf.Repeat(distance, max - min);
                case CurvyClamping.PingPong:
                    return min + Mathf.PingPong(distance, max - min);
                case CurvyClamping.Clamp:
                    return Mathf.Clamp(distance, min, max);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Clamps absolute position and sets new direction
        /// </summary>
        public static float ClampDistance(float distance, ref int dir, CurvyClamping clamping, float length)
        {
            if (length == 0)
                return 0;
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return Mathf.Repeat(distance, length);
                case CurvyClamping.PingPong:
                    if (Mathf.FloorToInt(distance / length) % 2 != 0)
                        dir *= -1;
                    return Mathf.PingPong(distance, length);
                case CurvyClamping.Clamp:
                    return Mathf.Clamp(distance, 0, length);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        /// <summary>
        /// Clamps absolute position and sets new direction
        /// </summary>
        public static float ClampDistance(float distance, ref int dir, CurvyClamping clamping, float length, float min, float max)
        {
            if (length == 0)
                return 0;
            min = Mathf.Clamp(min, 0, length);
            max = Mathf.Clamp(max, min, length);
            switch (clamping)
            {
                case CurvyClamping.Loop:
                    return min + Mathf.Repeat(distance, max - min);
                case CurvyClamping.PingPong:
                    if (Mathf.FloorToInt(distance / (max - min)) % 2 != 0)
                        dir *= -1;
                    return min + Mathf.PingPong(distance, max - min);
                case CurvyClamping.Clamp:
                    return Mathf.Clamp(distance, min, max);
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

        #endregion

        /// <summary>
        /// Gets the default material, i.e. Curvy/Resources/CurvyDefaultMaterial
        /// </summary>
        public static Material GetDefaultMaterial()
        {
            Material mat = Resources.Load("CurvyDefaultMaterial") as Material;
            if (mat == null)
            {
                mat = new Material(Shader.Find("Diffuse"));
            }
            return mat;
        }


        /// <summary>
        /// Does the same things as Mathf.Approximately, but with different handling of case where one of the two values is 0
        /// Considering inputs of 0 and 1E-7, Mathf.Approximately will return false, while this method will return true.
        /// </summary>
        public static bool Approximately(this float x, float y)
        {
            bool result;
            const float zeroComparaisonMargin = 0.000001f;

            float nearlyZero = Mathf.Epsilon * 8f;

            float absX = Math.Abs(x);
            float absY = Math.Abs(y);
            
            if (absY < nearlyZero)
                result = absX < zeroComparaisonMargin;
            else if (absX < nearlyZero)
                result = absY < zeroComparaisonMargin;
            else
                result = Mathf.Approximately(x, y);
            return result;
        }

        /// <summary>
        /// Finds the index of x in an array of sorted values (ascendant order). If x not found, the closest smaller value's index is returned if any, -1 otherwise
        /// </summary>
        /// <returns></returns>
        public static int InterpolationSearch(float[] array, float x)
        {
            int low = 0, high = (array.Length - 1);

            while (low <= high && array[low] <= x && x <= array[high])
            {
                if (low == high)
                {
                    if (array[low] == x)
                        return low;
                    break;
                }
                int index = low + (int)((((high - low) / (array[high] - array[low])) * (x - array[low])));
                if (array[index] == x)
                    return index;
                if (array[index] < x)
                    low = index + 1;
                else
                    high = index - 1;
            }

            if (low > high)
            {
                int temp = high;
                high = low;
                low = temp;
            }

            if (x <= array[low])
            {
                while (low >= 0)
                {
                    if (array[low] <= x)
                        return low;
                    low--;
                }

                return 0;
            }

            if (array[high] < x)
            {
                while (high < array.Length)
                {
                    if (x < array[high])
                        return high - 1;
                    high++;
                }

                return array.Length - 1;
            }

            return -1;
        }
    }

    #region ### Spline2Mesh ###

    /// <summary>
    /// Class to create a Mesh from a set of splines
    /// </summary>
    public class Spline2Mesh
    {
        #region ### Public Fields & Properties ###
        /// <summary>
        /// A list of splines (X/Y only) forming the resulting mesh
        /// </summary>
        public List<SplinePolyLine> Lines = new List<SplinePolyLine>();
        /// <summary>
        /// Winding rule used by triangulator
        /// </summary>
        public WindingRule Winding = WindingRule.EvenOdd;
        public Vector2 UVTiling = Vector2.one;
        public Vector2 UVOffset = Vector2.zero;
        public bool SuppressUVMapping;
        /// <summary>
        /// Whether UV2 should be set
        /// </summary>
        public bool UV2;
        /// <summary>
        /// Name of the returned mesh
        /// </summary>
        public string MeshName = string.Empty;
        /// <summary>
        /// Whether only vertices of the outline spline should be created
        /// </summary>
        public bool VertexLineOnly;

        public string Error { get; private set; }

        #endregion

        #region ### Private Fields ###

        Tess mTess;
        Mesh mMesh;

        #endregion

        #region ### Public Methods ###

        /// <summary>
        /// Create the Mesh using the current settings
        /// </summary>
        /// <param name="result">the resulting Mesh</param>
        /// <returns>true on success. If false, check the Error property!</returns>
        public bool Apply(out Mesh result)
        {
            mTess = null;
            mMesh = null;
            Error = string.Empty;
            bool triangulationSucceeded = triangulate();
            if (triangulationSucceeded)
            {
                mMesh = new Mesh();
                mMesh.name = MeshName;

                if (VertexLineOnly && Lines.Count > 0 && Lines[0] != null)
                    mMesh.vertices = Lines[0].GetVertices();
                else
                {
                    mMesh.vertices = UnityLibTessUtility.FromContourVertex(mTess.Vertices);
                    mMesh.triangles = mTess.Elements;
                }
                mMesh.RecalculateBounds();
                mMesh.RecalculateNormals();
                if (!SuppressUVMapping && !VertexLineOnly)
                {
                    Vector3 boundsSize = mMesh.bounds.size;
                    Vector3 boundsMin = mMesh.bounds.min;

                    float minSize = Mathf.Min(boundsSize.x, Mathf.Min(boundsSize.y, boundsSize.z));

                    bool minSizeIsX = minSize == boundsSize.x;
                    bool minSizeIsY = minSize == boundsSize.y;
                    bool minSizeIsZ = minSize == boundsSize.z;

                    Vector3[] vt = mMesh.vertices;
                    Vector2[] uv = new Vector2[vt.Length];

                    float maxU = 0;
                    float maxV = 0;

                    for (int i = 0; i < vt.Length; i++)
                    {
                        float u;
                        float v;
                        if (minSizeIsX)
                        {
                            u = UVOffset.x + (vt[i].y - boundsMin.y) / boundsSize.y;
                            v = UVOffset.y + (vt[i].z - boundsMin.z) / boundsSize.z;
                        }
                        else if (minSizeIsY)
                        {
                            u = UVOffset.x + (vt[i].z - boundsMin.z) / boundsSize.z;
                            v = UVOffset.y + (vt[i].x - boundsMin.x) / boundsSize.x;
                        }
                        else if (minSizeIsZ)
                        {
                            u = UVOffset.x + (vt[i].x - boundsMin.x) / boundsSize.x;
                            v = UVOffset.y + (vt[i].y - boundsMin.y) / boundsSize.y;
                        }
                        else
                            throw new InvalidOperationException("Couldn't find the minimal bound dimension");

                        u *= UVTiling.x;
                        v *= UVTiling.y;
                        maxU = u < maxU ? maxU : u;
                        maxV = v < maxV ? maxV : v;
                        uv[i].x = u;
                        uv[i].y = v;
                    }
                    mMesh.uv = uv;
                    Vector2[] uv2 = new Vector2[0];
                    if (UV2)
                    {
                        uv2 = new Vector2[uv.Length];
                        float oneOnMaxU = 1f / maxU;
                        float oneOnMaxV = 1f / maxV;
                        for (int i = 0; i < vt.Length; i++)
                        {
                            uv2[i].x = uv[i].x * oneOnMaxU;
                            uv2[i].y = uv[i].y * oneOnMaxV;
                        }
                    }
                    mMesh.uv2 = uv2;
                }
            }
            result = mMesh;
            return triangulationSucceeded;
        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */

        bool triangulate()
        {
            if (Lines.Count == 0)
            {
                Error = "Missing splines to triangulate";
                return false;
            }

            if (VertexLineOnly)
                return true;

            mTess = new Tess();

            for (int i = 0; i < Lines.Count; i++)
            {
                if (Lines[i].Spline == null)
                {
                    Error = "Missing Spline";
                    return false;
                }
                if (!polyLineIsValid(Lines[i]))
                {
                    Error = Lines[i].Spline.name + ": Angle must be >0";
                    return false;
                }
                Vector3[] verts = Lines[i].GetVertices();
                if (verts.Length < 3)
                {
                    Error = Lines[i].Spline.name + ": At least 3 Vertices needed!";
                    return false;
                }
                mTess.AddContour(UnityLibTessUtility.ToContourVertex(verts, false), Lines[i].Orientation);
            }
            try
            {
                mTess.Tessellate(Winding, ElementType.Polygons, 3);
                return true;
            }
            catch (System.Exception e)
            {
                Error = e.Message;
            }

            return false;
        }

        static bool polyLineIsValid(SplinePolyLine pl)
        {
            return (pl != null && pl.VertexMode == SplinePolyLine.VertexCalculation.ByApproximation ||
                    !Mathf.Approximately(0, pl.Angle));
        }

        /*! \endcond */
        #endregion
    }

    /// <summary>
    /// Spline Triangulation Helper Class
    /// </summary>
    [System.Serializable]
    public class SplinePolyLine
    {
        /// <summary>
        /// How to calculate vertices
        /// </summary>
        public enum VertexCalculation
        {
            /// <summary>
            /// Use Approximation points
            /// </summary>
            ByApproximation,
            /// <summary>
            /// By curvation angle
            /// </summary>
            ByAngle
        }

        /// <summary>
        /// Orientation order
        /// </summary>
        public ContourOrientation Orientation = ContourOrientation.Original;

        /// <summary>
        /// Base Spline
        /// </summary>
        public CurvySpline Spline;
        /// <summary>
        /// Vertex Calculation Mode
        /// </summary>
        public VertexCalculation VertexMode;
        /// <summary>
        /// Angle, used by VertexMode.ByAngle only
        /// </summary>
        public float Angle;
        /// <summary>
        /// Minimum distance, used by VertexMode.ByAngle only
        /// </summary>
        public float Distance;
        public Space Space;

        /// <summary>
        /// Creates a Spline2MeshCurve class using Spline2MeshCurve.VertexMode.ByApproximation
        /// </summary>
        public SplinePolyLine(CurvySpline spline) : this(spline, VertexCalculation.ByApproximation, 0, 0) { }
        /// <summary>
        /// Creates a Spline2MeshCurve class using Spline2MeshCurve.VertexMode.ByAngle
        /// </summary>
        public SplinePolyLine(CurvySpline spline, float angle, float distance) : this(spline, VertexCalculation.ByAngle, angle, distance) { }

        SplinePolyLine(CurvySpline spline, VertexCalculation vertexMode, float angle, float distance, Space space = Space.World)
        {
            Spline = spline;
            VertexMode = vertexMode;
            Angle = angle;
            Distance = distance;
            Space = space;
        }
        /// <summary>
        /// Gets whether the spline is closed
        /// </summary>
        public bool IsClosed
        {
            get
            {
                return (Spline && Spline.Closed);
            }
        }

        /// <summary>
        /// Get vertices calculated using the current VertexMode
        /// </summary>
        /// <returns>an array of vertices</returns>
        public Vector3[] GetVertices()
        {
            Vector3[] points = new Vector3[0];
            switch (VertexMode)
            {
                case VertexCalculation.ByAngle:
                    List<float> tf;
                    List<Vector3> tan;
                    points = GetPolygon(Spline, 0, 1, Angle, Distance, -1, out tf, out tan, false);
                    break;
                default:
                    points = Spline.GetApproximation();
                    break;
            }
            if (Space == Space.World)
            {
                for (int i = 0; i < points.Length; i++)
                    points[i] = Spline.transform.TransformPoint(points[i]);
            }
            return points;
        }

        /// <summary>
        /// Gets an array of sampled points that follow some restrictions on the distance between two consecutive points, and the angle of tangents between those points
        /// </summary>
        /// <param name="fromTF">start TF</param>
        /// <param name="toTF">end TF</param>
        /// <param name="maxAngle">maximum angle in degrees between tangents</param>
        /// <param name="minDistance">minimum distance between two points</param>
        /// <param name="maxDistance">maximum distance between two points</param>
        /// <param name="vertexTF">Stores the TF of the resulting points</param>
        /// <param name="vertexTangents">Stores the Tangents of the resulting points</param>
        /// <param name="includeEndPoint">Whether the end position should be included</param>
        /// <param name="stepSize">the stepsize to use</param>
        /// <returns>an array of interpolated positions</returns>
        static Vector3[] GetPolygon(CurvySpline spline, float fromTF, float toTF, float maxAngle, float minDistance, float maxDistance, out List<float> vertexTF, out List<Vector3> vertexTangents, bool includeEndPoint = true, float stepSize = 0.01f)
        {
            stepSize = Mathf.Clamp(stepSize, 0.002f, 1);
            maxDistance = (maxDistance == -1) ? spline.Length : Mathf.Clamp(maxDistance, 0, spline.Length);
            minDistance = Mathf.Clamp(minDistance, 0, maxDistance);
            if (!spline.Closed)
            {
                toTF = Mathf.Clamp01(toTF);
                fromTF = Mathf.Clamp(fromTF, 0, toTF);
            }
            List<Vector3> vPos = new List<Vector3>();
            List<Vector3> vTan = new List<Vector3>();
            List<float> vTF = new List<float>();

            int linearSteps = 0;
            float angleFromLast = 0;
            float distAccu = 0;
            Vector3 curPos = spline.Interpolate(fromTF);
            Vector3 curTangent = spline.GetTangent(fromTF);
            Vector3 lastPos = curPos;
            Vector3 lastTangent = curTangent;

            Action<float> addPoint = new System.Action<float>((f) =>
            {
                vPos.Add(curPos);
                vTan.Add(curTangent);
                vTF.Add(f);
                angleFromLast = 0;
                distAccu = 0;

                linearSteps = 0;
            });

            addPoint(fromTF);

            float tf = fromTF + stepSize;
            float t;
            while (tf < toTF)
            {
                t = tf % 1;
                // Get Point Pos & Tangent
                spline.InterpolateAndGetTangent(t, out curPos, out curTangent);
                if (curTangent == Vector3.zero)
                {
                    Debug.Log("zero Tangent! Oh no!");
                }
                distAccu += (curPos - lastPos).magnitude;
                if (curTangent == lastTangent)
                    linearSteps++;
                if (distAccu >= minDistance)
                {
                    // Exceeding distance?
                    if (distAccu >= maxDistance)
                        addPoint(t);
                    else // Check angle
                    {
                        angleFromLast += Vector3.Angle(lastTangent, curTangent);
                        // Max angle reached or entering/leaving a linear zone
                        if (angleFromLast >= maxAngle || (linearSteps > 0 && angleFromLast > 0))
                            addPoint(t);
                    }
                }
                tf += stepSize;
                lastPos = curPos;
                lastTangent = curTangent;
            }
            if (includeEndPoint)
            {
                vTF.Add(toTF % 1);
                Vector3 tangent;
                spline.InterpolateAndGetTangent(toTF % 1, out curPos, out tangent);
                vPos.Add(curPos);
                vTan.Add(tangent);
            }

            vertexTF = vTF;
            vertexTangents = vTan;
            return vPos.ToArray();
        }
    }
    #endregion
}
