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

namespace FluffyUnderware.Curvy.Generator
{
    #region ### CGModule related ###

    /// <summary>
    /// Yes,No,Auto Enum
    /// </summary>
    public enum CGYesNoAuto
    {
        Yes,
        No,
        Auto
    }

    /// <summary>
    /// Source,Self Enum
    /// </summary>
    public enum CGReferenceMode
    {
        Source,
        Self
    }

    /// <summary>
    /// Aspect Mode correction modes enum
    /// </summary>
    public enum CGKeepAspectMode
    {
        Off,
        ScaleU,
        ScaleV
    }

    [Obsolete]
    public enum CGUVEnum
    {
        U,
        V
    }

    public enum CGColliderEnum
    {
        None,
        Mesh,
        Box,
        Sphere,
        Capsule
    }

    /// <summary>
    /// Spot (sort of transform) to be used by Curvy Generator
    /// </summary>
    [System.Serializable]
    public struct CGSpot : IEquatable<CGSpot>
    {
        [SerializeField]
        [Label("Idx")]
        int m_Index;
        [SerializeField]
        [VectorEx("Pos", Options = AttributeOptionsFlags.Compact, Precision = 4)]
        Vector3 m_Position;
        [SerializeField]
        [VectorEx("Rot", Options = AttributeOptionsFlags.Compact, Precision = 4)]
        Quaternion m_Rotation;
        [SerializeField]
        [VectorEx("Scl", Options = AttributeOptionsFlags.Compact, Precision = 4)]
        Vector3 m_Scale;

        /// <summary>
        /// Gets the ID
        /// </summary>
        public int Index
        {
            get { return m_Index; }
        }

        /// <summary>
        /// Gets or sets the position
        /// </summary>
        public Vector3 Position
        {
            get { return m_Position; }
            set
            {
                if (m_Position != value)
                    m_Position = value;
            }
        }

        /// <summary>
        /// Gets or sets the rotation
        /// </summary>
        public Quaternion Rotation
        {
            get { return m_Rotation; }
            set
            {
                if (m_Rotation != value)
                    m_Rotation = value;
            }
        }

        /// <summary>
        /// Gets or sets the scale
        /// </summary>
        public Vector3 Scale
        {
            get { return m_Scale; }
            set
            {
                if (m_Scale != value)
                    m_Scale = value;
            }
        }

        /// <summary>
        /// Gets a TRS matrix using Position, Rotation, Scale
        /// </summary>
        public Matrix4x4 Matrix
        {
            get { return Matrix4x4.TRS(m_Position, m_Rotation, m_Scale); }
        }

        public CGSpot(int index) : this(index, Vector3.zero, Quaternion.identity, Vector3.one) { }

        public CGSpot(int index, Vector3 position, Quaternion rotation, Vector3 scale)
        {
            m_Index = index;
            m_Position = position;
            m_Rotation = rotation;
            m_Scale = scale;
        }

        /// <summary>
        /// Sets a transform to match Position, Rotation, Scale in local space
        /// </summary>
        /// <param name="transform"></param>
        public void ToTransform(Transform transform)
        {
            transform.localPosition = Position;
            transform.localRotation = Rotation;
            transform.localScale = Scale;
        }

        public bool Equals(CGSpot other)
        {
            return m_Index == other.m_Index && m_Position.Equals(other.m_Position) && m_Rotation.Equals(other.m_Rotation) && m_Scale.Equals(other.m_Scale);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is CGSpot && Equals((CGSpot)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = m_Index;
                hashCode = (hashCode * 397) ^ m_Position.GetHashCode();
                hashCode = (hashCode * 397) ^ m_Rotation.GetHashCode();
                hashCode = (hashCode * 397) ^ m_Scale.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(CGSpot left, CGSpot right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CGSpot left, CGSpot right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// Helper class used by various Curvy Generator modules
    /// </summary>
    [System.Serializable]
    public class CGMaterialSettings
    {
        public bool SwapUV = false;
        public CGKeepAspectMode KeepAspect = CGKeepAspectMode.Off;
        public float UVRotation = 0;
        public Vector2 UVOffset = Vector2.zero;
        public Vector2 UVScale = Vector2.one;
    }

    /// <summary>
    /// Helper class used by various Curvy Generator modules
    /// </summary>
    [System.Serializable]
    public class CGMaterialSettingsEx : CGMaterialSettings
    {
        public int MaterialID = 0;
    }

    /// <summary>
    /// Helper class used by InputMesh module
    /// </summary>
    [System.Serializable]
    public class CGMeshProperties
    {
        [SerializeField]
        Mesh m_Mesh;
        [SerializeField]
        Material[] m_Material = new Material[0];
        [SerializeField]
        [VectorEx]
        Vector3 m_Translation;
        [SerializeField]
        [VectorEx]
        Vector3 m_Rotation;
        [SerializeField]
        [VectorEx]
        Vector3 m_Scale = Vector3.one;


        public Mesh Mesh
        {
            get { return m_Mesh; }
            set
            {
                if (m_Mesh != value)
                    m_Mesh = value;
                if (m_Mesh && m_Mesh.subMeshCount != m_Material.Length)
                    System.Array.Resize(ref m_Material, m_Mesh.subMeshCount);
            }
        }
        public Material[] Material
        {
            get { return m_Material; }
            set
            {
                if (m_Material != value)
                    m_Material = value;
            }
        }

        public Vector3 Translation
        {
            get { return m_Translation; }
            set
            {
                if (m_Translation != value)
                    m_Translation = value;
            }
        }

        public Vector3 Rotation
        {
            get { return m_Rotation; }
            set
            {
                if (m_Rotation != value)
                    m_Rotation = value;
            }
        }

        public Vector3 Scale
        {
            get { return m_Scale; }
            set
            {
                if (m_Scale != value)
                    m_Scale = value;
            }
        }

        public Matrix4x4 Matrix
        {
            get { return Matrix4x4.TRS(Translation, Quaternion.Euler(Rotation), Scale); }
        }

        public CGMeshProperties() { }

        public CGMeshProperties(Mesh mesh)
        {
            Mesh = mesh;
            Material = (mesh != null) ? new Material[mesh.subMeshCount] : new Material[0];
        }
#if UNITY_EDITOR
        public void OnValidate()
        {
            Mesh = m_Mesh;
            Material = m_Material;
        }
#endif
    }

    /// <summary>
    /// Helper class used by InputGameObject module
    /// </summary>
    [System.Serializable]
    public class CGGameObjectProperties
    {
        [SerializeField]
        GameObject m_Object;
        [SerializeField]
        [VectorEx]
        Vector3 m_Translation;
        [SerializeField]
        [VectorEx]
        Vector3 m_Rotation;
        [SerializeField]
        [VectorEx]
        Vector3 m_Scale = Vector3.one;

        public GameObject Object
        {
            get { return m_Object; }
            set
            {
                if (m_Object != value)
                    m_Object = value;
            }
        }

        public Vector3 Translation
        {
            get { return m_Translation; }
            set
            {
                if (m_Translation != value)
                    m_Translation = value;
            }
        }

        public Vector3 Rotation
        {
            get { return m_Rotation; }
            set
            {
                if (m_Rotation != value)
                    m_Rotation = value;
            }
        }

        public Vector3 Scale
        {
            get { return m_Scale; }
            set
            {
                if (m_Scale != value)
                    m_Scale = value;
            }
        }

        public Matrix4x4 Matrix
        {
            get { return Matrix4x4.TRS(Translation, Quaternion.Euler(Rotation), Scale); }
        }

        public CGGameObjectProperties() { }

        public CGGameObjectProperties(GameObject gameObject)
        {
            Object = gameObject;
        }
    }

    /// <summary>
    /// An item that has a weight associated to it
    /// </summary>
    [Serializable]
    public class CGWeightedItem
    {
        [RangeEx(0, 1, Slider = true, Precision = 1)]
        [SerializeField]
        float m_Weight = 0.5f;

        public float Weight
        {
            get { return m_Weight; }
            set
            {
                float v = Mathf.Clamp01(value);
                if (m_Weight != v)
                    m_Weight = v;
            }
        }
    }

    /// <summary>
    /// Helper class used by VolumeSpots and others
    /// </summary>
    [System.Serializable]
    public class CGBoundsGroupItem : CGWeightedItem
    {
        public int Index;
    }

    #endregion

    #region ### Spline rasterization related ###

    /// <summary>
    /// Rasterization helper
    /// </summary>
    public struct ControlPointOption : IEquatable<ControlPointOption>
    {
        public float TF;
        public float Distance;
        public bool Include;
        public int MaterialID;
        public bool HardEdge;
        public float MaxStepDistance;
        public bool UVEdge;
        public bool UVShift;
        public float FirstU;
        public float SecondU;


        public ControlPointOption(float tf, float dist, bool includeAnyways, int materialID, bool hardEdge, float maxStepDistance, bool uvEdge, bool uvShift, float firstU, float secondU)
        {
            TF = tf;
            Distance = dist;
            Include = includeAnyways;
            MaterialID = materialID;
            HardEdge = hardEdge;
            if (maxStepDistance == 0)
                MaxStepDistance = float.MaxValue;
            else
                MaxStepDistance = maxStepDistance;
            UVEdge = uvEdge;
            UVShift = uvShift;
            FirstU = firstU;
            SecondU = secondU;
        }

        public bool Equals(ControlPointOption other)
        {
            return TF.Equals(other.TF) && Distance.Equals(other.Distance) && Include == other.Include && MaterialID == other.MaterialID && HardEdge == other.HardEdge && MaxStepDistance.Equals(other.MaxStepDistance) && UVEdge == other.UVEdge && UVShift == other.UVShift && FirstU.Equals(other.FirstU) && SecondU.Equals(other.SecondU);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is ControlPointOption && Equals((ControlPointOption)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = TF.GetHashCode();
                hashCode = (hashCode * 397) ^ Distance.GetHashCode();
                hashCode = (hashCode * 397) ^ Include.GetHashCode();
                hashCode = (hashCode * 397) ^ MaterialID;
                hashCode = (hashCode * 397) ^ HardEdge.GetHashCode();
                hashCode = (hashCode * 397) ^ MaxStepDistance.GetHashCode();
                hashCode = (hashCode * 397) ^ UVEdge.GetHashCode();
                hashCode = (hashCode * 397) ^ UVShift.GetHashCode();
                hashCode = (hashCode * 397) ^ FirstU.GetHashCode();
                hashCode = (hashCode * 397) ^ SecondU.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(ControlPointOption left, ControlPointOption right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ControlPointOption left, ControlPointOption right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// A patch of vertices to be connected by triangles (i.e. same Material and no hard edges within a patch)
    /// </summary>
    /// <remarks>The index values refer to rasterized points of CGShape</remarks>
    public struct SamplePointsPatch : IEquatable<SamplePointsPatch>
    {
        /// <summary>
        /// First Sample Point Index of the patch
        /// </summary>
        public int Start;
        /// <summary>
        /// Number of Sample Points of the patch
        /// </summary>
        public int Count;

        /// <summary>
        /// Last Sample Point Index of the patch
        /// </summary>
        public int End
        {
            get { return Start + Count; }
            set
            {
                Count = Mathf.Max(0, value - Start);
            }
        }

        public int TriangleCount
        {
            get
            {
                return Count * 2;
            }
        }


        public SamplePointsPatch(int start)
        {
            Start = start;
            Count = 0;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Size={0} ({1}-{2}, {3} Tris)", Count, Start, End, TriangleCount);
        }

        public bool Equals(SamplePointsPatch other)
        {
            return Start == other.Start && Count == other.Count;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is SamplePointsPatch && Equals((SamplePointsPatch)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start * 397) ^ Count;
            }
        }

        public static bool operator ==(SamplePointsPatch left, SamplePointsPatch right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SamplePointsPatch left, SamplePointsPatch right)
        {
            return !left.Equals(right);
        }
    }

    /// <summary>
    /// A section of one or more patches, all sharing the same MaterialID
    /// </summary>
    public class SamplePointsMaterialGroup
    {
        public int MaterialID;

        public List<SamplePointsPatch> Patches = new List<SamplePointsPatch>();

        public int TriangleCount
        {
            get
            {
                int cnt = 0;
                for (int p = 0; p < Patches.Count; p++)
                    cnt += Patches[p].TriangleCount;
                return cnt;
            }
        }

        public int StartVertex
        {
            get
            {
                return Patches[0].Start;
            }
        }

        public int EndVertex
        {
            get
            {
                return Patches[Patches.Count - 1].End;
            }
        }

        public int VertexCount
        {
            get
            {
                return EndVertex - StartVertex + 1;
            }
        }

        public SamplePointsMaterialGroup(int materialID)
        {
            MaterialID = materialID;
        }

        public void GetLengths(CGVolume volume, out float worldLength, out float uLength)
        {
            worldLength = 0;
            for (int v = StartVertex; v < EndVertex; v++)
                worldLength += (volume.Vertex[v + 1] - volume.Vertex[v]).magnitude;
            uLength = volume.CrossMap[EndVertex] - volume.CrossMap[StartVertex];
        }

        /// <summary>
        /// Returns a clone of the current instance.
        /// </summary>
        public SamplePointsMaterialGroup Clone()
        {
            return new SamplePointsMaterialGroup(MaterialID)
            {
                Patches = new List<SamplePointsPatch>(Patches)
            };
        }
    }

    public struct SamplePointUData : IEquatable<SamplePointUData>
    {
        public int Vertex;
        public bool UVEdge;
        public float FirstU;
        public float SecondU;

        public SamplePointUData(int vt, bool uvEdge, float uv0, float uv1)
        {
            Vertex = vt;
            UVEdge = uvEdge;
            FirstU = uv0;
            SecondU = uv1;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "SamplePointUData (Vertex={0},Edge={1},FirstU={2},SecondU={3}", Vertex, UVEdge, FirstU, SecondU);
        }

        public bool Equals(SamplePointUData other)
        {
            return Vertex == other.Vertex && UVEdge == other.UVEdge && FirstU.Equals(other.FirstU) && SecondU.Equals(other.SecondU);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            return obj is SamplePointUData && Equals((SamplePointUData)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Vertex;
                hashCode = (hashCode * 397) ^ UVEdge.GetHashCode();
                hashCode = (hashCode * 397) ^ FirstU.GetHashCode();
                hashCode = (hashCode * 397) ^ SecondU.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SamplePointUData left, SamplePointUData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SamplePointUData left, SamplePointUData right)
        {
            return !left.Equals(right);
        }
    }

    #endregion


    /// <summary>
    /// List of Material Groups
    /// </summary>
    public class SamplePointsMaterialGroupCollection : List<SamplePointsMaterialGroup>
    {

        public int TriangleCount
        {
            get
            {
                int cnt = 0;
                for (int g = 0; g < this.Count; g++)
                    cnt += this[g].TriangleCount;
                return cnt;
            }
        }

        public int MaterialID;
        public float AspectCorrection = 1;

        public SamplePointsMaterialGroupCollection() : base() { }
        public SamplePointsMaterialGroupCollection(int capacity) : base(capacity) { }
        public SamplePointsMaterialGroupCollection(IEnumerable<SamplePointsMaterialGroup> collection) : base(collection) { }

        public void CalculateAspectCorrection(CGVolume volume, CGMaterialSettingsEx matSettings)
        {
            if (matSettings.KeepAspect == CGKeepAspectMode.Off)
                AspectCorrection = 1;
            else
            {
                float totalLength = 0;
                float totalULength = 0;
                float l, u;
                for (int g = 0; g < Count; g++)
                {
                    this[g].GetLengths(volume, out l, out u);
                    totalLength += l;
                    totalULength += u;
                }
                AspectCorrection = volume.Length / (totalLength / totalULength);
            }
        }
    }

    /// <summary>
    /// Class referencing a particular module's output slot
    /// </summary>
    /// <remarks>When using, be sure to add the <see cref="CGDataReferenceSelectorAttribute"/> to the field</remarks>
    [System.Serializable]
    public class CGDataReference
    {
        [SerializeField]
        CGModule m_Module;
        [SerializeField]
        string m_SlotName;

        CGModuleOutputSlot mSlot;

        public CGData[] Data
        {
            get
            {
                return (Slot != null) ? Slot.Data : new CGData[0];
            }
        }

        public CGModuleOutputSlot Slot
        {
            get
            {
                if ((mSlot == null || mSlot.Module != m_Module || mSlot.Info == null || mSlot.Info.Name != m_SlotName) && m_Module != null && m_Module.Generator != null && m_Module.Generator.IsInitialized && !string.IsNullOrEmpty(m_SlotName))
                {
                    mSlot = m_Module.GetOutputSlot(m_SlotName);
                }
                return mSlot;
            }
        }

        public bool HasValue
        {
            get
            {
                CGModuleOutputSlot cgModuleOutputSlot = Slot;
                return (cgModuleOutputSlot != null) && cgModuleOutputSlot.Data.Length > 0;
            }
        }

        public bool IsEmpty
        {
            get { return string.IsNullOrEmpty(SlotName); }
        }

        public CGModule Module
        {
            get { return m_Module; }
        }
        public string SlotName
        {
            get { return m_SlotName; }
        }

        public CGDataReference()
        {
        }

        public CGDataReference(CGModule module, string slotName)
        {
            setINTERNAL(module, slotName);
        }

        public CGDataReference(CurvyGenerator generator, string moduleName, string slotName)
        {
            setINTERNAL(generator, moduleName, slotName);
        }

        public void Clear()
        {
            setINTERNAL(null, string.Empty);
        }

        public T GetData<T>() where T : CGData
        {
            return (Data.Length == 0) ? null : Data[0] as T;
        }

        public T[] GetAllData<T>() where T : CGData
        {
            return Data as T[];
        }

        #region ### Privates & Internals ###
        /*! \cond PRIVATE */

        public void setINTERNAL(CGModule module, string slotName)
        {
            m_Module = module;
            m_SlotName = slotName;
            mSlot = null;
        }

        public void setINTERNAL(CurvyGenerator generator, string moduleName, string slotName)
        {
            m_Module = generator.GetModule(moduleName);
            m_SlotName = slotName;
            mSlot = null;
        }

        /*! \endcond */
        #endregion

    }







}
