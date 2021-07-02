// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevTools;
using FluffyUnderware.Curvy.Utils;

using UnityEngine.Serialization;


namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Build/Volume Mesh", ModuleName = "Volume Mesh", Description = "Build a volume mesh")]
    [HelpURL(CurvySpline.DOCLINK + "cgbuildvolumemesh")]
    public class BuildVolumeMesh : CGModule
    {
        

        [HideInInspector]
        [InputSlotInfo(typeof(CGVolume))]
        public CGModuleInputSlot InVolume = new CGModuleInputSlot();

        [HideInInspector]
        [OutputSlotInfo(typeof(CGVMesh), Array = true)]
        public CGModuleOutputSlot OutVMesh = new CGModuleOutputSlot();

        #region ### Serialized Fields ###
        
        [Tab("General")]
        [SerializeField]
        bool m_GenerateUV = true;

        [SerializeField]
        bool m_Split;

        [Positive(MinValue = 1)]

        [FieldCondition("m_Split", true)]
        [SerializeField]
        float m_SplitLength = 100;

        [FieldAction("CBAddMaterial")]
        [SerializeField, FormerlySerializedAs("m_ReverseNormals")]
        bool m_ReverseTriOrder;

        // SubMesh-Settings

        [SerializeField, HideInInspector]
        List<CGMaterialSettingsEx> m_MaterialSettings = new List<CGMaterialSettingsEx>();

        [SerializeField, HideInInspector]
        Material[] m_Material = new Material[0];

        #endregion

        #region ### Public Properties ###

        public bool GenerateUV
        {
            get { return m_GenerateUV; }
            set
            {
                if (m_GenerateUV != value)
                    m_GenerateUV = value;
                Dirty = true;
            }
        }

        public bool ReverseTriOrder
        {
            get { return m_ReverseTriOrder; }
            set
            {
                if (m_ReverseTriOrder != value)
                    m_ReverseTriOrder = value;
                Dirty = true;
            }
        }

        public bool Split
        {
            get { return m_Split; }
            set
            {
                if (m_Split != value)
                    m_Split = value;
                Dirty = true;
            }
        }

        public float SplitLength
        {
            get { return m_SplitLength; }
            set
            {
                float v = Mathf.Max(1, value);
                if (m_SplitLength != v)
                    m_SplitLength = v;
                Dirty = true;
            }
        }

        public List<CGMaterialSettingsEx> MaterialSetttings
        {
            get { return m_MaterialSettings; }
        }

        public int MaterialCount
        {
            get { return m_MaterialSettings.Count; }
        }

        #endregion

        #region ### Private Fields & Properties ###

        List<SamplePointsMaterialGroupCollection> groupsByMatID;

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        protected override void Awake()
        {
            base.Awake();
            if (MaterialCount == 0)
                AddMaterial();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            GenerateUV = m_GenerateUV;
            ReverseTriOrder = m_ReverseTriOrder;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            GenerateUV = true;
            Split = false;
            SplitLength = 100;
            ReverseTriOrder = false;
            m_MaterialSettings = new List<CGMaterialSettingsEx>(new CGMaterialSettingsEx[1] { new CGMaterialSettingsEx() });
            m_Material = new Material[1] { CurvyUtility.GetDefaultMaterial() };
        }

        /*! \endcond */
        #endregion

        #region ### Public Methods ###

        public override void Refresh()
        {
            base.Refresh();
            CGVolume vol = InVolume.GetData<CGVolume>();


            if (vol && vol.Count > 0 && vol.CrossSize > 0 && vol.CrossMaterialGroups.Count > 0)
            {
                List<IntRegion> volSets = new List<IntRegion>();
                if (Split)
                {

                    float dist;
                    float lastdist = 0;
                    int lastIndex = 0;
                    for (int sample = 0; sample < vol.Count; sample++)
                    {
                            /*OPTIM F here, contrary to splines, is proportional to distance. So instead of working with distances, which means a call to vol.FToDistance at each iteration, just work with Fs
                             After some tests, this optimization is really not important for now. Maybe in the future, when other parts will be optimized, it would be worth doing
                             */
                        dist = vol.FToDistance(vol.F[sample]);
                        if (dist - lastdist >= SplitLength)
                        {
                            volSets.Add(new IntRegion(lastIndex, sample));
                            lastdist = dist;
                            lastIndex = sample;
                        }
                    }
                    if (lastIndex < vol.Count - 1)
                        volSets.Add(new IntRegion(lastIndex, vol.Count - 1));

                }
                else
                    volSets.Add(new IntRegion(0, vol.Count - 1));

                CGVMesh[] data = OutVMesh.GetAllData<CGVMesh>();
                System.Array.Resize(ref data, volSets.Count);

                prepare(vol);
                for (int sub = 0; sub < volSets.Count; sub++)
                {
                    data[sub] = CGVMesh.Get(data[sub], vol, volSets[sub], GenerateUV, ReverseTriOrder);
                    build(data[sub], vol, volSets[sub]);
                }

                OutVMesh.SetData(data);
            }
            else
                OutVMesh.SetData(null);

        }

        public int AddMaterial()
        {
            m_MaterialSettings.Add(new CGMaterialSettingsEx());
            m_Material = m_Material.Add(CurvyUtility.GetDefaultMaterial());
            Dirty = true;
            return MaterialCount;
        }

        public void RemoveMaterial(int index)
        {
            if (!validateMaterialIndex(index))
                return;
            m_MaterialSettings.RemoveAt(index);
            m_Material = m_Material.RemoveAt(index);
            Dirty = true;
        }

        public void SetMaterial(int index, Material mat)
        {
            if (!validateMaterialIndex(index) || mat == m_Material[index])
                return;
            if (m_Material[index] != mat)
            {
                m_Material[index] = mat;
                Dirty = true;
            }
        }

        public Material GetMaterial(int index)
        {
            if (!validateMaterialIndex(index))
                return null;
            return m_Material[index];
        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */

        void prepare(CGVolume vol)
        {
            // We have groups (different MaterialID) of patches (e.g. by Hard Edges).
            // Create Collection of groups sharing the same material ID
            groupsByMatID = getMaterialIDGroups(vol);
        }

        void build(CGVMesh vmesh, CGVolume vol, IntRegion subset)
        {

            // Because each Material ID forms a submesh
            // Do we need to calculate localU?
            if (GenerateUV)
            {
                System.Array.Resize(ref vmesh.UV, vmesh.Count);

            }
            // Prepare Submeshes
            prepareSubMeshes(vmesh, groupsByMatID, subset.Length, ref m_Material);
            //prepareSubMeshes(vmesh, groupsByMatID, vol.Count - 1, ref m_Material);

            SamplePointsMaterialGroupCollection col;
            SamplePointsMaterialGroup grp;

            int vtIdx = 0;
            int[] triIdx = new int[groupsByMatID.Count]; // triIdx for each submesh
            // for all sample segments (except the last) along the path, create Triangles to the next segment 
            for (int sample = subset.From; sample < subset.To; sample++)
            {
                // for each submesh (collection)
                for (int subMeshIdx = 0; subMeshIdx < groupsByMatID.Count; subMeshIdx++)
                {
                    col = groupsByMatID[subMeshIdx];
                    // create UV and triangles for all groups in submesh
                    for (int g = 0; g < col.Count; g++)
                    {
                        grp = col[g];
                        if (GenerateUV)
                            createMaterialGroupUV(vmesh, vol, grp, col.MaterialID, col.AspectCorrection, sample, vtIdx);
                        for (int p = 0; p < grp.Patches.Count; p++)
                            createPatchTriangles(ref vmesh.SubMeshes[subMeshIdx].Triangles, ref triIdx[subMeshIdx], vtIdx + grp.Patches[p].Start, grp.Patches[p].Count, vol.CrossSize, ReverseTriOrder);

                    }
                }
                vtIdx += vol.CrossSize;
            }

            // UV for last path segment
            if (GenerateUV)
            {
                // for each submesh (collection)
                for (int subMeshIdx = 0; subMeshIdx < groupsByMatID.Count; subMeshIdx++)
                {
                    col = groupsByMatID[subMeshIdx];
                    // create triangles
                    for (int g = 0; g < col.Count; g++)
                    {
                        grp = col[g];
                        createMaterialGroupUV(vmesh, vol, grp, col.MaterialID, col.AspectCorrection, subset.To, vtIdx);
                        
                    }
                }
            }
        }

        static void prepareSubMeshes(CGVMesh vmesh, List<SamplePointsMaterialGroupCollection> groupsBySubMeshes, int extrusions, ref Material[] materials)
        {
            vmesh.SetSubMeshCount(groupsBySubMeshes.Count);
            for (int g = 0; g < groupsBySubMeshes.Count; g++)
            {
                CGVSubMesh sm = vmesh.SubMeshes[g];
                vmesh.SubMeshes[g] = CGVSubMesh.Get(sm, groupsBySubMeshes[g].TriangleCount * extrusions * 3, materials[Mathf.Min(groupsBySubMeshes[g].MaterialID, materials.Length - 1)]);
            }
        }

        // OPTIMIZE: Store array of U values and just copy them
        void createMaterialGroupUV(CGVMesh vmesh, CGVolume vol, SamplePointsMaterialGroup grp, int matIndex, float grpAspectCorrection, int sample, int baseVertex)
        {
            CGMaterialSettingsEx mat = m_MaterialSettings[matIndex];

            float v = mat.UVOffset.y + vol.F[sample] * mat.UVScale.y * grpAspectCorrection;
            int hi = grp.EndVertex;

            bool swapUV = mat.SwapUV;
            Vector2[] uv = vmesh.UV;

            for (int c = grp.StartVertex; c <= hi; c++)
            {
                float u = mat.UVOffset.x + vol.CrossMap[c] * mat.UVScale.x;

                uv[baseVertex + c].x = swapUV ? v : u;
                uv[baseVertex + c].y = swapUV ? u : v;
            }
        }

        /// <summary>
        /// Creates triangles for a cross section
        /// </summary>
        /// <param name="triangles">the triangle array</param>
        /// <param name="triIdx">current tri index</param>
        /// <param name="curVTIndex">base vertex index of this cross section (i.e. the first vertex)</param>
        /// <param name="patchEndVT">size of the cross group (i.e. number of sample points to connect)</param>
        /// <param name="crossSize">number of vertices per cross section</param>
        /// <param name="reverse">whether triangles should flip (i.e. a reversed triangle order should be used)</param>
        /// <returns></returns>
        static int createPatchTriangles(ref int[] triangles, ref int triIdx, int curVTIndex, int patchSize, int crossSize, bool reverse)
        {
            int rv0 = (reverse) ? 1 : 0; // flipping +0 and +1 when reversing
            int rv1 = 1 - rv0;
            int nextCrossVT = curVTIndex + crossSize;
            for (int vt = 0; vt < patchSize; vt++)
            {
                triangles[triIdx + rv0] = curVTIndex + vt;
                triangles[triIdx + rv1] = nextCrossVT + vt;
                triangles[triIdx + 2] = curVTIndex + vt + 1;
                triangles[triIdx + rv0 + 3] = curVTIndex + vt + 1;
                triangles[triIdx + rv1 + 3] = nextCrossVT + vt;
                triangles[triIdx + 5] = nextCrossVT + vt + 1;
                triIdx += 6;
            }

            return curVTIndex + patchSize + 1;
        }

        /// <summary>
        /// Create collections of groups sharing same Material ID. Also ensures collection's MaterialID is valid!
        /// </summary>
        /// <param name="groups"></param>
        /// <returns></returns>
        List<SamplePointsMaterialGroupCollection> getMaterialIDGroups(CGVolume volume)
        {
            
            Dictionary<int, SamplePointsMaterialGroupCollection> matCollections = new Dictionary<int, SamplePointsMaterialGroupCollection>();

            SamplePointsMaterialGroupCollection col;

            for (int g = 0; g < volume.CrossMaterialGroups.Count; g++)
            {
                int matID = Mathf.Min(volume.CrossMaterialGroups[g].MaterialID, MaterialCount - 1);
                if (!matCollections.TryGetValue(matID, out col))
                {
                    col = new SamplePointsMaterialGroupCollection();
                    col.MaterialID = matID;
                    matCollections.Add(matID, col);
                }
                col.Add(volume.CrossMaterialGroups[g]);
            }

            List<SamplePointsMaterialGroupCollection> res = new List<SamplePointsMaterialGroupCollection>();
            
            foreach (SamplePointsMaterialGroupCollection item in matCollections.Values)
            {
                item.CalculateAspectCorrection(volume, MaterialSetttings[item.MaterialID]);
                res.Add(item);
            }
            return res;
            
        }

        bool validateMaterialIndex(int index)
        {
            if (index < 0 || index >= m_MaterialSettings.Count)
            {
                Debug.LogError("TriangulateTube: Invalid Material Index!");
                return false;
            }
            return true;
        }


        /*! \endcond */
        #endregion

  
        

     

    }
}
