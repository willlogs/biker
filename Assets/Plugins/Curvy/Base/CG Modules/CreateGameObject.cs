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
using FluffyUnderware.Curvy.Utils;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.Curvy.Generator.Modules
{
    [ModuleInfo("Create/GameObject", ModuleName = "Create GameObject")]
    [HelpURL(CurvySpline.DOCLINK + "cgcreategameobject")]
    public class CreateGameObject : CGModule
    {
        [HideInInspector]
        [InputSlotInfo(typeof(CGGameObject), Array = true, Name = "GameObject")]
        public CGModuleInputSlot InGameObjectArray = new CGModuleInputSlot();

        [HideInInspector]
        [InputSlotInfo(typeof(CGSpots), Name = "Spots")]
        public CGModuleInputSlot InSpots = new CGModuleInputSlot();

        [SerializeField, CGResourceCollectionManager("GameObject", ShowCount = true)]
        CGGameObjectResourceCollection m_Resources = new CGGameObjectResourceCollection();

        #region ### Serialized Fields ###

        [Tab("General")]
        [SerializeField]
        bool m_MakeStatic;
        [SerializeField]
        [Layer]
        int m_Layer;

        #endregion

        #region ### Public Properties ###

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

        public CGGameObjectResourceCollection GameObjects
        {
            get { return m_Resources; }
        }

        public int GameObjectCount
        {
            get { return GameObjects.Count; }
        }

        #endregion

        #region ### Private Fields & Properties ###
        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            Dirty = true;
        }
#endif

        public override void Reset()
        {
            base.Reset();
            MakeStatic = false;
            Layer = 0;
            Clear();
        }

        protected override void OnDestroy()
        {
            if (!Generator.Destroying)
                DeleteAllPrefabPools();
            base.OnDestroy();
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
            for (int i = 0; i < GameObjects.Count; i++)
                DeleteManagedResource("GameObject", GameObjects.Items[i], GameObjects.PoolNames[i]);
            GameObjects.Items.Clear();
            GameObjects.PoolNames.Clear();
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

            List<CGGameObject> VGO = InGameObjectArray.GetAllData<CGGameObject>();
            CGSpots Spots = InSpots.GetData<CGSpots>();

            Clear();
            List<IPool> existingPools = GetAllPrefabPools();
            HashSet<string> usedPools = new HashSet<string>();

            if (VGO.Count > 0 && Spots.Count > 0)
            {
                CGSpot spot;
                for (int s = 0; s < Spots.Count; s++)
                {
                    spot = Spots.Points[s];
                    int id = spot.Index;
                    if (id >= 0 && id < VGO.Count && VGO[id].Object != null)
                    {
                        CGGameObject inputCGGameObject = VGO[id];

                        string poolIdent = GetPrefabPool(inputCGGameObject.Object).Identifier;
                        usedPools.Add(poolIdent);
                        Transform res = (Transform)AddManagedResource("GameObject", poolIdent, s);
                        res.gameObject.isStatic = MakeStatic;
                        res.gameObject.layer = Layer;
                        res.localPosition = spot.Position;
                        res.localRotation = spot.Rotation;
                        res.localScale = new Vector3(inputCGGameObject.Object.transform.localScale.x * spot.Scale.x * inputCGGameObject.Scale.x, inputCGGameObject.Object.transform.localScale.y * spot.Scale.y * inputCGGameObject.Scale.y, inputCGGameObject.Object.transform.localScale.z * spot.Scale.z * inputCGGameObject.Scale.z);

                        if (inputCGGameObject.Translate != Vector3.zero)
                            res.Translate(inputCGGameObject.Translate);
                        if (inputCGGameObject.Rotate != Vector3.zero)
                            res.Rotate(inputCGGameObject.Rotate);

                        GameObjects.Items.Add(res);
                        GameObjects.PoolNames.Add(poolIdent);

                    }
                }
            }

            // Remove unused pools
            foreach (IPool pool in existingPools)
            {
                if (!usedPools.Contains(pool.Identifier))
                    Generator.PoolManager.DeletePool(pool);
            }

        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATE */


        /*! \endcond */
        #endregion

    }

}
