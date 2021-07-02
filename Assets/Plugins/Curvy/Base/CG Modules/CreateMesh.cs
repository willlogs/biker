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
using System.Globalization;
using System.Linq;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools;
using UnityEngine.Rendering;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Create/Mesh", ModuleName = "Create Mesh")]
    [HelpURL(CurvySpline.DOCLINK + "cgcreatemesh")]
    public class CreateMesh : CGModule
    {
        /// <summary>
        /// The default value of Tag of created objects
        /// </summary>
        private const string DefaultTag = "Untagged";


        [HideInInspector]
        [InputSlotInfo(typeof(CGVMesh), Array = true, Name = "VMesh")]
        public CGModuleInputSlot InVMeshArray = new CGModuleInputSlot();

        [HideInInspector]
        [InputSlotInfo(typeof(CGSpots), Array = true, Name = "Spots", Optional = true)]
        public CGModuleInputSlot InSpots = new CGModuleInputSlot();

        [SerializeField, CGResourceCollectionManager("Mesh", ShowCount = true)]
        CGMeshResourceCollection m_MeshResources = new CGMeshResourceCollection();

        #region ### Serialized Fields ###

        [Tab("General")]

        [Tooltip("Merge meshes")]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        [SerializeField]
        bool m_Combine;

        [Tooltip("Merge meshes sharing the same Index")]
#if UNITY_EDITOR
        [FieldCondition("canUpdate", true, false, ConditionalAttribute.OperatorEnum.AND, "canGroupMeshes", true, false, Action = ConditionalAttribute.ActionEnum.Enable)]
#endif
        [SerializeField]
        bool m_GroupMeshes = true;

        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        CGYesNoAuto m_AddNormals = CGYesNoAuto.Auto;

        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        CGYesNoAuto m_AddTangents = CGYesNoAuto.No;

        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        bool m_AddUV2 = true;

        [SerializeField]
        [Tooltip("If enabled, meshes will have the Static flag set, and will not be updated in Play Mode")]
        [FieldCondition("canModifyStaticFlag", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        bool m_MakeStatic;

        [SerializeField]
        [Tooltip("The Layer of the created game object")]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        [Layer]
        int m_Layer;

        [SerializeField]
        [Tooltip("The Tag of the created game object")]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        [Tag]
        string m_Tag = DefaultTag;

        [Tab("Renderer")]
        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        bool m_RendererEnabled = true;

        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        ShadowCastingMode m_CastShadows = ShadowCastingMode.On;

        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        bool m_ReceiveShadows = true;

        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        LightProbeUsage m_LightProbeUsage = LightProbeUsage.BlendProbes;

        [HideInInspector]
        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        bool m_UseLightProbes = true;


        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        ReflectionProbeUsage m_ReflectionProbes = ReflectionProbeUsage.BlendProbes;
        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        Transform m_AnchorOverride;

        [Tab("Collider")]
        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        CGColliderEnum m_Collider = CGColliderEnum.Mesh;

        [FieldCondition("m_Collider", CGColliderEnum.Mesh)]
        [SerializeField]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        bool m_Convex;

        [SerializeField]
        [FieldCondition("EnableIsTrigger", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        bool m_IsTrigger;

#if UNITY_2017_3_OR_NEWER
        [Tooltip("Options used to enable or disable certain features in Collider mesh cooking. See Unity's MeshCollider.cookingOptions for more details")]
        [FieldCondition("m_Collider", CGColliderEnum.Mesh)]
        [SerializeField]
        [EnumFlag]
        [FieldCondition("canUpdate", true, Action = ConditionalAttribute.ActionEnum.Enable)]
        MeshColliderCookingOptions m_CookingOptions = MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning;
#endif

#if UNITY_EDITOR
        [FieldCondition("canUpdate", true, false, ConditionalAttribute.OperatorEnum.AND, "m_Collider", CGColliderEnum.None, true, Action = ConditionalAttribute.ActionEnum.Enable)]
#endif
        [Label("Auto Update")]
        [SerializeField]
        bool m_AutoUpdateColliders = true;

#if UNITY_EDITOR
        [FieldCondition("canUpdate", true, false, ConditionalAttribute.OperatorEnum.AND, "m_Collider", CGColliderEnum.None, true, Action = ConditionalAttribute.ActionEnum.Enable)]
#endif
        [SerializeField]
        PhysicMaterial m_Material;

        #endregion

        #region ### Public Properties ###

        #region --- General ---
        public bool Combine
        {
            get { return m_Combine; }
            set
            {
                if (m_Combine != value)
                    m_Combine = value;
                Dirty = true;
            }
        }

        public bool GroupMeshes
        {
            get { return m_GroupMeshes; }
            set
            {
                if (m_GroupMeshes != value)
                    m_GroupMeshes = value;
                Dirty = true;
            }
        }

        public CGYesNoAuto AddNormals
        {
            get { return m_AddNormals; }
            set
            {
                if (m_AddNormals != value)
                    m_AddNormals = value;
                Dirty = true;
            }
        }

        public CGYesNoAuto AddTangents
        {
            get { return m_AddTangents; }
            set
            {
                if (m_AddTangents != value)
                    m_AddTangents = value;
                Dirty = true;
            }
        }

        public bool AddUV2
        {
            get { return m_AddUV2; }
            set
            {
                if (m_AddUV2 != value)
                    m_AddUV2 = value;
                Dirty = true;
            }
        }


        public int Layer
        {
            get { return m_Layer; }
            set
            {
                int v = Mathf.Clamp(value, 0, 32);
                if (m_Layer != v)
                    m_Layer = v;
                Dirty = true;
            }
        }

        public string Tag
        {
            get { return m_Tag; }
            set
            {
                if (m_Tag != value)//TODO get rid of value comparison in all properties, or at least add the Dirty = true line inside the if
                    m_Tag = value;
                Dirty = true;
            }
        }

        public bool MakeStatic
        {
            get { return m_MakeStatic; }
            set
            {
                if (m_MakeStatic != value)
                    m_MakeStatic = value;
                Dirty = true;
            }
        }
        #endregion

        #region --- Renderer ---
        public bool RendererEnabled
        {
            get { return m_RendererEnabled; }
            set
            {
                if (m_RendererEnabled != value)
                    m_RendererEnabled = value;
                Dirty = true;
            }
        }

        public ShadowCastingMode CastShadows
        {
            get { return m_CastShadows; }
            set
            {
                if (m_CastShadows != value)
                    m_CastShadows = value;
                Dirty = true;
            }
        }

        public bool ReceiveShadows
        {
            get { return m_ReceiveShadows; }
            set
            {
                if (m_ReceiveShadows != value)
                    m_ReceiveShadows = value;
                Dirty = true;
            }
        }

        public bool UseLightProbes
        {
            get { return m_UseLightProbes; }
            set
            {
                if (m_UseLightProbes != value)
                    m_UseLightProbes = value;
                Dirty = true;
            }
        }

        public LightProbeUsage LightProbeUsage
        {
            get { return m_LightProbeUsage; }
            set
            {
                if (m_LightProbeUsage != value)
                    m_LightProbeUsage = value;
                Dirty = true;
            }
        }


        public ReflectionProbeUsage ReflectionProbes
        {
            get { return m_ReflectionProbes; }
            set
            {
                if (m_ReflectionProbes != value)
                    m_ReflectionProbes = value;
                Dirty = true;
            }
        }

        public Transform AnchorOverride
        {
            get { return m_AnchorOverride; }
            set
            {
                if (m_AnchorOverride != value)
                    m_AnchorOverride = value;
                Dirty = true;
            }
        }

        #endregion

        #region --- Collider ---

        public CGColliderEnum Collider
        {
            get { return m_Collider; }
            set
            {
                if (m_Collider != value)
                    m_Collider = value;
                Dirty = true;
            }
        }

        public bool AutoUpdateColliders
        {
            get { return m_AutoUpdateColliders; }
            set
            {
                if (m_AutoUpdateColliders != value)
                    m_AutoUpdateColliders = value;
                Dirty = true;
            }
        }

        public bool Convex
        {
            get { return m_Convex; }
            set
            {
                if (m_Convex != value)
                    m_Convex = value;
                Dirty = true;
            }
        }

        /// <summary>
        /// Is the created collider a trigger
        /// </summary>
        public bool IsTrigger
        {
            get { return m_IsTrigger; }
            set
            {
                if (m_IsTrigger != value)
                    m_IsTrigger = value;
                Dirty = true;
            }
        }

#if UNITY_2017_3_OR_NEWER
        /// <summary>
        /// Options used to enable or disable certain features in Collider mesh cooking. See Unity's MeshCollider.cookingOptions for more details
        /// </summary>
        public MeshColliderCookingOptions CookingOptions
        {
            get { return m_CookingOptions; }
            set
            {
                if (m_CookingOptions != value)
                    m_CookingOptions = value;
                Dirty = true;
            }
        }
#endif

        public PhysicMaterial Material
        {
            get { return m_Material; }
            set
            {
                if (m_Material != value)
                    m_Material = value;
                Dirty = true;
            }
        }

        #endregion

        public CGMeshResourceCollection Meshes
        {
            get { return m_MeshResources; }
        }

        public int MeshCount
        {
            get { return Meshes.Count; }
        }

        public int VertexCount { get; private set; }

        #endregion

        #region ### Private Fields & Properties ###

        int mCurrentMeshCount;

        bool canGroupMeshes
        {
            get
            {
                return (InSpots.IsLinked && m_Combine);
            }
        }

        private bool canModifyStaticFlag
        {
            get
            {
#if UNITY_EDITOR
                return Application.isPlaying == false;
#else
                return false;
#endif
            }
        }

        private bool canUpdate
        {
            get
            {
                return !Application.isPlaying || !MakeStatic;
            }
        }

        //Do not remove, used in FieldCondition in this file
        private bool EnableIsTrigger
        {
            get
            {
                return canUpdate && (m_Collider != CGColliderEnum.Mesh || m_Convex);
            }
        }


        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            AddNormals = m_AddNormals;
            AddTangents = m_AddTangents;
            Collider = m_Collider;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            Combine = false;
            GroupMeshes = true;
            AddNormals = CGYesNoAuto.Auto;
            AddTangents = CGYesNoAuto.No;
            MakeStatic = false;
            Material = null;
            Layer = 0;
            Tag = DefaultTag;
            CastShadows = ShadowCastingMode.On;
            RendererEnabled = true;
            ReceiveShadows = true;
            UseLightProbes = true;
            LightProbeUsage = LightProbeUsage.BlendProbes;
            ReflectionProbes = ReflectionProbeUsage.BlendProbes;
            AnchorOverride = null;
            Collider = CGColliderEnum.Mesh;
            AutoUpdateColliders = true;
            Convex = false;
            IsTrigger = false;
            AddUV2 = true;
#if UNITY_2017_3_OR_NEWER
            CookingOptions = MeshColliderCookingOptions.WeldColocatedVertices | MeshColliderCookingOptions.CookForFasterSimulation | MeshColliderCookingOptions.EnableMeshCleaning;
#endif
            Clear();

        }

        /*! \endcond */
        #endregion

        #region ### Public Methods ###

        public override void OnTemplateCreated()
        {
            Clear();
        }

        public void Clear()
        {
            mCurrentMeshCount = 0;
            removeUnusedResource();
            Resources.UnloadUnusedAssets();
        }

        public override void OnStateChange()
        {
            base.OnStateChange();
            if (!IsConfigured)
                Clear();

        }

        public override void Refresh()
        {

            base.Refresh();
            if (canUpdate)
            {
                List<CGVMesh> VMeshes = InVMeshArray.GetAllData<CGVMesh>();
                List<CGSpots> Spots = InSpots.GetAllData<CGSpots>();

                CGSpot[] flattenedSpotsArray;
                switch (Spots.Count)
                {
                    case 1:
                        flattenedSpotsArray = Spots[0] != null ? Spots[0].Points : null;
                        break;
                    case 0:
                        flattenedSpotsArray = null;
                        break;
                    default:
                        flattenedSpotsArray = Spots.Where(s => s != null).SelectMany(s => s.Points).ToArray();
                        break;
                }

                mCurrentMeshCount = 0;
                VertexCount = 0;

                if (VMeshes.Count > 0 && (!InSpots.IsLinked || (flattenedSpotsArray != null && flattenedSpotsArray.Length > 0)))
                {
                    if (flattenedSpotsArray != null && flattenedSpotsArray.Length > 0)
                        createSpotMeshes(ref VMeshes, flattenedSpotsArray, Combine);
                    else
                        createMeshes(ref VMeshes, Combine);
                }
                // Cleanup
                removeUnusedResource();

                // Update Colliders?
                if (AutoUpdateColliders)
                    UpdateColliders();
            }
            else
                UIMessages.Add("In Play Mode, and when Make Static is enabled, mesh generation is stopped to avoid overriding the optimizations Unity do to static game objects'meshs.");
        }

        /// <summary>
        /// Save the created mesh(es)'s gameobject(s) to the scene
        /// </summary>
        /// <param name="parent">the parent transform to which the saved GameObject will be attached. If null, saved GameObject will be at the hierarchy's root</param>
        /// <returns>The created GameObject</returns>
        public GameObject SaveToScene(Transform parent = null)
        {
            List<Component> managedResources;
            List<string> names;
            GetManagedResources(out managedResources, out names);
            if (managedResources.Count == 0)
                return null;

            GameObject result;
            if (managedResources.Count > 1)
            {
                result = new GameObject(ModuleName);
                result.transform.parent = parent;
                for (int i = 0; i < managedResources.Count; i++)
                    SaveMeshResourceToScene(managedResources[i], result.transform);
            }
            else
                result = SaveMeshResourceToScene(managedResources[0], parent);

            result.transform.position = this.transform.position;
            result.transform.rotation = this.transform.rotation;
            result.transform.localScale = this.transform.localScale;
            return result;
        }

        public void UpdateColliders()
        {
            bool success = true;
            for (int r = 0; r < m_MeshResources.Count; r++)
            {
                if (m_MeshResources.Items[r] == null)
                    continue;
#if UNITY_2017_3_OR_NEWER
                if (!m_MeshResources.Items[r].UpdateCollider(Collider, Convex, IsTrigger, Material, CookingOptions))
#else
                if (!m_MeshResources.Items[r].UpdateCollider(Collider, Convex, IsTrigger, Material))
#endif
                    success = false;
            }
            if (!success)
                UIMessages.Add("Error setting collider!");
        }


        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */

        private static GameObject SaveMeshResourceToScene(Component managedResource, Transform newParent)
        {
            MeshFilter meshFilter = managedResource.GetComponent<MeshFilter>();
            GameObject duplicateGameObject = managedResource.gameObject.DuplicateGameObject(newParent);
            duplicateGameObject.name = managedResource.name;
            duplicateGameObject.GetComponent<CGMeshResource>().Destroy();
            duplicateGameObject.GetComponent<MeshFilter>().sharedMesh = Component.Instantiate(meshFilter.sharedMesh);

            return duplicateGameObject;
        }

        void createMeshes(ref List<CGVMesh> vMeshes, bool combine)
        {
            const int VertexCountLimit = 65534;

            if (combine && vMeshes.Count > 1)
            {
                int currentIndex = 0;
                while (currentIndex < vMeshes.Count)
                {
                    int firstIndexInCombinedMeshes = currentIndex;
                    int totalVertexCount = 0;
                    while (currentIndex < vMeshes.Count && totalVertexCount + vMeshes[currentIndex].Count <= VertexCountLimit)
                    {
                        totalVertexCount += vMeshes[currentIndex].Count;
                        currentIndex++;
                    }

                    if (totalVertexCount == 0)
                    {
                        UIMessages.Add(string.Format(CultureInfo.InvariantCulture, "Mesh of index {0}, and subsequent ones, skipped because vertex count {2} > {1}", currentIndex, VertexCountLimit, vMeshes[currentIndex].Count));
                        break;
                    }

                    CGVMesh curVMesh = new CGVMesh();
                    curVMesh.MergeVMeshes(vMeshes, firstIndexInCombinedMeshes, currentIndex - 1);
                    writeVMeshToMesh(ref curVMesh);
                }
            }
            else
            {
                for (int index = 0; index < vMeshes.Count; index++)
                {
                    CGVMesh vMesh = vMeshes[index];
                    if (vMesh.Count < VertexCountLimit)
                        writeVMeshToMesh(ref vMesh);
                    else
                        UIMessages.Add(string.Format(CultureInfo.InvariantCulture, "Mesh of index {0} skipped because vertex count {2} > {1}", index, VertexCountLimit, vMesh.Count));
                }
            }
        }

        void createSpotMeshes(ref List<CGVMesh> vMeshes, CGSpot[] spots, bool combine)
        {
            int exceededVertexCount = 0;
            int vmCount = vMeshes.Count;
            CGSpot spot;

            if (combine)
            {
                List<CGSpot> sortedSpots = new List<CGSpot>(spots);
                if (GroupMeshes)
                    sortedSpots.Sort((CGSpot a, CGSpot b) => a.Index.CompareTo(b.Index));

                spot = sortedSpots[0];
                CGVMesh curVMesh = new CGVMesh(vMeshes[spot.Index]);
                if (spot.Position != Vector3.zero || spot.Rotation != Quaternion.identity || spot.Scale != Vector3.one)
                    curVMesh.TRS(spot.Matrix);
                for (int s = 1; s < sortedSpots.Count; s++)
                {
                    spot = sortedSpots[s];
                    // Filter spot.index not in vMeshes[]
                    if (spot.Index > -1 && spot.Index < vmCount)
                    {
                        if (curVMesh.Count + vMeshes[spot.Index].Count > 65534 || (GroupMeshes && spot.Index != sortedSpots[s - 1].Index))
                        { // write curVMesh 
                            writeVMeshToMesh(ref curVMesh);
                            curVMesh = new CGVMesh(vMeshes[spot.Index]);
                            if (!spot.Matrix.isIdentity)
                                curVMesh.TRS(spot.Matrix);
                        }
                        else
                        { // Add new vMesh to curVMesh
                            if (!spot.Matrix.isIdentity)
                                curVMesh.MergeVMesh(vMeshes[spot.Index], spot.Matrix);
                            else
                                curVMesh.MergeVMesh(vMeshes[spot.Index]);
                        }
                    }
                }
                writeVMeshToMesh(ref curVMesh);
            }
            else
            {
                for (int s = 0; s < spots.Length; s++)
                {
                    spot = spots[s];
                    // Filter spot.index not in vMeshes[]
                    if (spot.Index > -1 && spot.Index < vmCount)
                    {
                        // Don't touch vertices, TRS Resource instead
                        if (vMeshes[spot.Index].Count < 65535)
                        {
                            CGVMesh vmesh = vMeshes[spot.Index];
                            CGMeshResource res = writeVMeshToMesh(ref vmesh);
                            if (spot.Position != Vector3.zero || spot.Rotation != Quaternion.identity || spot.Scale != Vector3.one)
                                spot.ToTransform(res.Filter.transform);
                        }
                        else
                            exceededVertexCount++;
                    }
                }
            }

            if (exceededVertexCount > 0)
                UIMessages.Add(string.Format(CultureInfo.InvariantCulture, "{0} meshes skipped (VertexCount>65534)", exceededVertexCount));
        }

        /// <summary>
        /// create a mesh resource and copy vmesh data to the mesh!
        /// </summary>
        /// <param name="vmesh"></param>
        CGMeshResource writeVMeshToMesh(ref CGVMesh vmesh)
        {
            CGMeshResource res;
            Mesh mesh;

            bool wantNormals = (AddNormals != CGYesNoAuto.No);
            bool wantTangents = (AddTangents != CGYesNoAuto.No);
            res = getNewMesh();
            if (canModifyStaticFlag)
                res.Filter.gameObject.isStatic = false;
            mesh = res.Prepare();
            res.gameObject.layer = Layer;
            res.gameObject.tag = Tag;
            vmesh.ToMesh(ref mesh);
            VertexCount += vmesh.Count;
            if (AddUV2 && !vmesh.HasUV2)
                mesh.uv2 = CGUtility.CalculateUV2(vmesh.UV);
            if (wantNormals && !vmesh.HasNormals)
                mesh.RecalculateNormals();
            if (wantTangents && !vmesh.HasTangents)
                res.Filter.CalculateTangents();


            // Reset Transform
            res.Filter.transform.localPosition = Vector3.zero;
            res.Filter.transform.localRotation = Quaternion.identity;
            res.Filter.transform.localScale = Vector3.one;
            if (canModifyStaticFlag)
                res.Filter.gameObject.isStatic = MakeStatic;
            res.Renderer.sharedMaterials = vmesh.GetMaterials();


            return res;
        }

        /// <summary>
        /// remove all mesh resources not currently used (>=mCurrentMeshCount)
        /// </summary>
        void removeUnusedResource()
        {
            for (int r = mCurrentMeshCount; r < Meshes.Count; r++)
                DeleteManagedResource("Mesh", Meshes.Items[r]);
            Meshes.Items.RemoveRange(mCurrentMeshCount, Meshes.Count - mCurrentMeshCount);

        }

        /// <summary>
        /// gets a new mesh resource and increase mCurrentMeshCount
        /// </summary>
        CGMeshResource getNewMesh()
        {
            CGMeshResource r;
            // Reuse existing resources
            if (mCurrentMeshCount < Meshes.Count)
            {
                r = Meshes.Items[mCurrentMeshCount];
                if (r == null)
                {
                    r = ((CGMeshResource)AddManagedResource("Mesh", "", mCurrentMeshCount));
                    Meshes.Items[mCurrentMeshCount] = r;
                }
            }
            else
            {
                r = ((CGMeshResource)AddManagedResource("Mesh", "", mCurrentMeshCount));
                Meshes.Items.Add(r);
            }

            // Renderer settings
            r.Renderer.shadowCastingMode = CastShadows;
            r.Renderer.enabled = RendererEnabled;
            r.Renderer.receiveShadows = ReceiveShadows;
            r.Renderer.lightProbeUsage = LightProbeUsage;
            r.Renderer.reflectionProbeUsage = ReflectionProbes;

            r.Renderer.probeAnchor = AnchorOverride;

            if (!r.ColliderMatches(Collider))
                r.RemoveCollider();

            //RenameResource("Mesh", r, mCurrentMeshCount);
            mCurrentMeshCount++;
            return r;
        }


        /*! \endcond */
        #endregion


    }

}
