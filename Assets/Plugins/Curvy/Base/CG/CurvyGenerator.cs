// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

#if UNITY_EDITOR && UNITY_2017_2_OR_NEWER
#define QUEUEABLE_EDITOR_UPDATE
#endif

using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using FluffyUnderware.DevTools;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace FluffyUnderware.Curvy.Generator
{
    /// <summary>
    /// Curvy Generator component
    /// </summary>
    [ExecuteInEditMode]
    [HelpURL(CurvySpline.DOCLINK + "generator")]
    [AddComponentMenu("Curvy/Generator", 3)]
    [RequireComponent(typeof(PoolManager))]
    public class CurvyGenerator : DTVersionedMonoBehaviour
    {

        #region ### Serialized Fields ###

        [Tooltip("Show Debug Output?")]
        [SerializeField]
        bool m_ShowDebug;

        [Tooltip("Whether to automatically refresh the generator's output when necessary")]
        [SerializeField]
        bool m_AutoRefresh = true;

        [FieldCondition("m_AutoRefresh", true)]
        [Positive(Tooltip = "The minimum delay between two automatic generator's refreshing while in Play mode, in milliseconds")]
        [SerializeField]
        int m_RefreshDelay = 0;

        [FieldCondition("m_AutoRefresh", true)]
        [Positive(Tooltip = "The minimum delay between two automatic generator's refreshing while in Edit mode, in milliseconds")]
        [SerializeField]
        int m_RefreshDelayEditor = 10;

        [Section("Events", false, false, 1000, HelpURL = CurvySpline.DOCLINK + "generator_events")]
        [SerializeField]
        protected CurvyCGEvent m_OnRefresh = new CurvyCGEvent();

#if QUEUEABLE_EDITOR_UPDATE
        [Section("Advanced Settings", Sort = 2000, HelpURL = CurvySpline.DOCLINK + "generator_events", Expanded = false)]
        [Label(Tooltip = "Force this script to update in Edit mode as often as in Play mode. Most users don't need that.")]
        [SerializeField]
        bool m_ForceFrequentUpdates;
#endif

        /// <summary>
        /// List of modules this Generator contains
        /// </summary>
        [HideInInspector]
        public List<CGModule> Modules = new List<CGModule>();

        [SerializeField, HideInInspector]
        internal int m_LastModuleID;

        #endregion

        #region ### Public Properties ###

        /// <summary>
        /// Gets or sets whether to show debug outputs
        /// </summary>
        public bool ShowDebug
        {
            get { return m_ShowDebug; }
            set
            {
                if (m_ShowDebug != value)
                    m_ShowDebug = value;
            }
        }
        /// <summary>
        /// Gets or sets whether to automatically call <see cref="Refresh"/> if necessary
        /// </summary>
        public bool AutoRefresh
        {
            get { return m_AutoRefresh; }
            set
            {
                if (m_AutoRefresh != value)
                    m_AutoRefresh = value;
            }
        }
        /// <summary>
        /// Gets or sets the minimum delay between two consecutive calls to <see cref="Refresh"></see> while playing
        /// </summary>
        public int RefreshDelay
        {
            get { return m_RefreshDelay; }
            set
            {
                int v = Mathf.Max(0, value);
                if (m_RefreshDelay != v)
                    m_RefreshDelay = v;
            }
        }
        /// <summary>
        /// Gets or sets the minimum delay between two consecutive calls to <see cref="Refresh"></see> in the editor
        /// </summary>
        public int RefreshDelayEditor
        {
            get { return m_RefreshDelayEditor; }
            set
            {
                int v = Mathf.Max(0, value);
                if (m_RefreshDelayEditor != v)
                    m_RefreshDelayEditor = v;
            }
        }

#if QUEUEABLE_EDITOR_UPDATE
        /// <summary>
        /// By default Unity calls scripts' update less frequently in Edit mode. ForceFrequentUpdates forces this script to update in Edit mode as often as in Play mode. Most users don't need that.
        /// </summary>
        public bool ForceFrequentUpdates
        {
            get { return m_ForceFrequentUpdates; }
            set { m_ForceFrequentUpdates = value; }
        }
#endif

        /// <summary>
        /// Gets the PoolManager
        /// </summary>
        public PoolManager PoolManager
        {
            get
            {
                if (mPoolManager == null)
                    mPoolManager = GetComponent<PoolManager>();
                return mPoolManager;
            }
        }

        /// <summary>
        /// Event raised after refreshing the Generator
        /// </summary>
        public CurvyCGEvent OnRefresh
        {
            get { return m_OnRefresh; }
            set
            {
                if (m_OnRefresh != value)
                    m_OnRefresh = value;

            }
        }

        /// <summary>
        /// Gets whether the generator and all its dependencies are fully initialized
        /// </summary>
        public bool IsInitialized { get { return mInitialized; } }
        /// <summary>
        /// Gets whether the Generator is about to get destroyed
        /// </summary>
        public bool Destroying { get; private set; }

        /// <summary>
        /// Dictionary to get a module by it's ID
        /// </summary>
        public Dictionary<int, CGModule> ModulesByID = new Dictionary<int, CGModule>();

        #endregion

        #region ### Private Fields ###

        bool mInitialized;
        bool mInitializedPhaseOne;
        bool mNeedSort = true;
        double mLastUpdateTime;
        PoolManager mPoolManager;

#if UNITY_EDITOR || CURVY_DEBUG
        // Debugging:
        public TimeMeasure DEBUG_ExecutionTime = new TimeMeasure(5);
#endif
#if UNITY_EDITOR
        // Refresh-Handling
        double mLastEditorUpdateTime;

#endif

        /// <summary>
        /// Used in the modules reordering logic. Value's unit is pixels.
        /// </summary>
        private const int ModulesReorderingDeltaX = 50;
        /// <summary>
        /// Used in the modules reordering logic. Value's unit is pixels.
        /// </summary>
        private const int ModulesReorderingDeltaY = 20;

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */

        void OnEnable()
        {
            PoolManager.AutoCreatePools = true;
#if UNITY_EDITOR
            EditorApplication.update += editorUpdate;
            if (!Application.isPlaying)
            {
                UnityEditorInternal.ComponentUtility.MoveComponentUp(this);
            }
#endif
        }

        void OnDisable()
        {
            mInitialized = false;
            mInitializedPhaseOne = false;
            mNeedSort = true;
#if UNITY_EDITOR
            EditorApplication.update -= editorUpdate;
#endif
        }


        void OnDestroy()
        {
            Destroying = true;
        }

#if UNITY_EDITOR
        private void editorUpdate()
        {
            if (AutoRefresh && Application.isPlaying == false)
            {
#if QUEUEABLE_EDITOR_UPDATE
                if (ForceFrequentUpdates)
                    EditorApplication.QueuePlayerLoopUpdate();
                else
#endif
                    Update();
            }
        }
#endif

        void Update()
        {
            if (!IsInitialized)
                Initialize();
            else
                TryAutoRefresh();
        }


        /*! \endcond */
        #endregion

        #region ### Public Static Methods ###

        /// <summary>
        /// Creates a new GameObject with a CurvyGenerator attached
        /// </summary>
        /// <returns>the Generator component</returns>
        public static CurvyGenerator Create()
        {
            GameObject go = new GameObject("Curvy Generator", typeof(CurvyGenerator));
            return go.GetComponent<CurvyGenerator>();
        }

        #endregion

        #region ### Public Methods ###

        /// <summary>
        /// Adds a Module
        /// </summary>
        /// <typeparam name="T">type of the Module</typeparam>
        /// <returns>the new Module</returns>
        public T AddModule<T>() where T : CGModule
        {
            return (T)AddModule(typeof(T));
        }
        /// <summary>
        /// Adds a Module
        /// </summary>
        /// <param name="type">type of the Module</param>
        /// <returns>the new Module</returns>
        public CGModule AddModule(System.Type type)
        {
            GameObject go = new GameObject("");
            go.transform.SetParent(transform, false);
            CGModule mod = (CGModule)go.AddComponent(type);
            mod.SetUniqueIdINTERNAL();
            Modules.Add(mod);
            ModulesByID.Add(mod.UniqueID, mod);
            return mod;
        }

        /// <summary>
        /// Auto-Arrange modules' graph canvas position
        /// In other words, this alligns the graph with the top left corner of the canvas. This does not modify the modules position relatively to each other
        /// </summary>
        public void ArrangeModules()
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            foreach (CGModule mod in Modules)
            {
                min.x = Mathf.Min(mod.Properties.Dimensions.x, min.x);
                min.y = Mathf.Min(mod.Properties.Dimensions.y, min.y);
            }
            min -= new Vector2(10, 10);
            foreach (CGModule mod in Modules)
            {
                mod.Properties.Dimensions.x -= min.x;
                mod.Properties.Dimensions.y -= min.y;
            }
        }

        /// <summary>
        /// Changes the modules' positions to make the graph easier to read.
        /// </summary>
        public void ReorderModules()
        {
            Dictionary<CGModule, Rect> initialModulesPositions;
            {
                initialModulesPositions = new Dictionary<CGModule, Rect>(Modules.Count);
                foreach (CGModule cgModule in Modules)
                    initialModulesPositions[cgModule] = cgModule.Properties.Dimensions;
            }


            List<CGModule> endpointModules = Modules.Where(m => m.OutputLinks.Any() == false).ToList();


            //A dictionary that gives for each module the set of all the modules that are connected to its inputs, whether directly or indirectly
            Dictionary<CGModule, HashSet<CGModule>> modulesRecursiveInputs = new Dictionary<CGModule, HashSet<CGModule>>(Modules.Count);
            foreach (CGModule module in endpointModules)
                UpdateModulesRecursiveInputs(modulesRecursiveInputs, module);

            HashSet<int> reordredModuleIds = new HashSet<int>();
            for (int index = 0; index < endpointModules.Count; index++)
            {
                float endPointY = index == 0
                    ? 0
                    //Draw under the previous endpoint recursive inputs
                    : modulesRecursiveInputs[endpointModules[index - 1]].Max(m => m.Properties.Dimensions.yMax) + ModulesReorderingDeltaY;

                CGModule endpointModule = endpointModules[index];
                //Set the endpoint's position
                endpointModule.Properties.Dimensions.position = new Vector2(0, endPointY);
                reordredModuleIds.Add(endpointModule.UniqueID);
                //And then its children's positions, recursively
                ReorderEndpointRecursiveInputs(endpointModule, reordredModuleIds, modulesRecursiveInputs);
            }

            ArrangeModules();
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                //Dirty scene if something changed
                if (Modules.Exists(m => m.Properties.Dimensions != initialModulesPositions[m]))
                    EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
#endif
        }



        /// <summary>
        /// Clear the whole generator
        /// </summary>
        public void Clear()
        {
            clearModules();
        }

        /// <summary>
        /// Deletes a module (same as PCGModule.Delete())
        /// </summary>
        /// <param name="module">a module</param>
        public void DeleteModule(CGModule module)
        {
            if (module)
                module.Delete();
        }

        /// <summary>
        /// Find modules of a given type
        /// </summary>
        /// <typeparam name="T">the module type</typeparam>
        /// <param name="includeOnRequestProcessing">whether to include IOnRequestProcessing modules</param>
        /// <returns>a list of zero or more modules</returns>
        public List<T> FindModules<T>(bool includeOnRequestProcessing = false) where T : CGModule //TODO: don't make includeOnRequestProcessing optional, it is confusing since you expect the method to look into all modules
        {
            List<T> res = new List<T>();
            for (int i = 0; i < Modules.Count; i++)
                if (Modules[i] is T && (includeOnRequestProcessing || !(Modules[i] is IOnRequestProcessing)))
                    res.Add((T)Modules[i]);
            return res;
        }

        /// <summary>
        /// Gets a list of modules, either including or excluding IOnRequestProcessing modules
        /// </summary>
        /// <param name="includeOnRequestProcessing">whether to include IOnRequestProcessing modules</param>
        public List<CGModule> GetModules(bool includeOnRequestProcessing = false)//TODO: don't make includeOnRequestProcessing optional, it is confusing since you expect the method to look into all modules
        {

            if (includeOnRequestProcessing)
                return new List<CGModule>(Modules);
            else
            {
                List<CGModule> res = new List<CGModule>();
                for (int i = 0; i < Modules.Count; i++)
                    if (!(Modules[i] is IOnRequestProcessing))
                        res.Add(Modules[i]);
                return res;
            }
        }

        /// <summary>
        /// Gets a module by ID, either including or excluding IOnRequestProcessing modules
        /// </summary>
        /// <param name="moduleID">the ID of the module in question</param>
        /// <param name="includeOnRequestProcessing">whether to include IOnRequestProcessing modules</param>
        public CGModule GetModule(int moduleID, bool includeOnRequestProcessing = false)
        {
            CGModule res;
            if (ModulesByID.TryGetValue(moduleID, out res) && (includeOnRequestProcessing || !(res is IOnRequestProcessing)))
                return res;
            else
                return null;
        }

        /// <summary>
        /// Gets a module by ID, either including or excluding IOnRequestProcessing modules (Generic version)
        /// </summary>
        /// <typeparam name="T">type of the module</typeparam>
        /// <param name="moduleID">the ID of the module in question</param>
        /// <param name="includeOnRequestProcessing">whether to include IOnRequestProcessing modules</param>
        public T GetModule<T>(int moduleID, bool includeOnRequestProcessing = false) where T : CGModule
        {
            return GetModule(moduleID, includeOnRequestProcessing) as T;
        }

        /// <summary>
        /// Gets a module by name, either including or excluding IOnRequestProcessing modules 
        /// </summary>
        /// <param name="moduleName"></param>
        /// <param name="includeOnRequestProcessing"></param>
        public CGModule GetModule(string moduleName, bool includeOnRequestProcessing = false)
        {
            for (int i = 0; i < Modules.Count; i++)
                if (Modules[i].ModuleName.Equals(moduleName, System.StringComparison.CurrentCultureIgnoreCase) && (includeOnRequestProcessing || !(Modules[i] is IOnRequestProcessing)))
                    return Modules[i];

            return null;
        }

        /// <summary>
        /// Gets a module by name, either including or excluding IOnRequestProcessing modules (Generic version)
        /// </summary>
        /// <typeparam name="T">type of the module</typeparam>
        /// <param name="moduleName">the ID of the module in question</param>
        /// <param name="includeOnRequestProcessing">whether to include IOnRequestProcessing modules</param>
        public T GetModule<T>(string moduleName, bool includeOnRequestProcessing = false) where T : CGModule
        {
            return GetModule(moduleName, includeOnRequestProcessing) as T;
        }

        /// <summary>
        /// Gets a module's output slot by module ID and slotName
        /// </summary>
        /// <param name="moduleId">Id of the module</param>
        /// <param name="slotName">Name of the slot</param>
        public CGModuleOutputSlot GetModuleOutputSlot(int moduleId, string slotName)
        {
            CGModule mod = GetModule(moduleId);
            if (mod)
                return mod.GetOutputSlot(slotName);
            else
                return null;
        }

        /// <summary>
        /// Gets a module's output slot by module name and slotName
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <param name="slotName">Name of the slot</param>
        public CGModuleOutputSlot GetModuleOutputSlot(string moduleName, string slotName)
        {
            CGModule mod = GetModule(moduleName);
            if (mod)
                return mod.GetOutputSlot(slotName);
            else
                return null;
        }

        //TODO initialize earlier
        /// <summary>
        /// Initializes the Generator
        /// </summary>
        /// <param name="force">true to force reinitialization</param>
        public void Initialize(bool force = false)
        {
            if (!mInitializedPhaseOne || force)
            {
                // Read modules
                ModulesByID.Clear();

                Modules.Clear();
                GetComponentsInChildren(Modules);
                //Not all modules are part of this generator. This happens for example if a generator creates GameObjects that are generators themselves
                Modules.RemoveAll(m => m.transform.parent != this.transform);

                for (int i = 0; i < Modules.Count; i++)
                {
                    if (!Modules[i].IsInitialized || force)
                        Modules[i].Initialize();

                    if (ModulesByID.ContainsKey(Modules[i].UniqueID))
                    {
                        Debug.LogError("ID of '" + Modules[i].ModuleName + "' isn't unique!");
                        return;
                    }
                    ModulesByID.Add(Modules[i].UniqueID, Modules[i]);
                }

                if (Modules.Count > 0)
                {
                    // Sort them
                    sortModulesINTERNAL();
                }
                mInitializedPhaseOne = true;
            }
            for (int m = 0; m < Modules.Count; m++)
                if (Modules[m] is IExternalInput && !Modules[m].IsInitialized)
                    return;

            mInitialized = true;
            mInitializedPhaseOne = false;
            mNeedSort = mNeedSort || force;
            Refresh(true);
        }

        /// <summary>
        /// Refreshes the Generator
        /// </summary>
        /// <param name="forceUpdate">true to force a refresh of all modules</param>
        public void Refresh(bool forceUpdate = false)
        {
            if (!IsInitialized)
                return;
            if (mNeedSort)
                //BUG this does not sort modules correctly
                doSortModules();//This is supposed to sort a module in a way that for each module, all its input modules are set in the modules list (which defines the updating order) before the said module

            CGModule firstChanged = null;

            for (int i = 0; i < Modules.Count; i++)
            {
                if (forceUpdate && Modules[i] is IOnRequestProcessing)
                    Modules[i].Dirty = true; // Dirty state will be resetted to false, but last data will be deleted - forcing a recalculation
                if (!(Modules[i] is INoProcessing) // ignore INoProcessing modules
                    && (Modules[i].Dirty // update dirty modules
                        || (forceUpdate && !(Modules[i] is IOnRequestProcessing)))) //update non dirty modules when forceUpdate is true, except IOnRequestProcessing modules, which by the way are never dirty, because the Dirty setter handles them differently, which I think is bad design, but this is not a major issue
                {
                    Modules[i].checkOnStateChangedINTERNAL();//BUG? this can set dirty to true, so shouldn't it be called before checking the value of Dirty earlier in this method?
                    if (Modules[i].IsInitialized && Modules[i].IsConfigured)
                    {

                        if (firstChanged == null)
                        {
#if UNITY_EDITOR || CURVY_DEBUG
                            DEBUG_ExecutionTime.Start();
#endif
                            firstChanged = Modules[i];
                        }

                        //OPTIM? remove this check, or make its compilation conditional
                        foreach (CGModuleInputSlot inputSlot in Modules[i].Input)
                        {
                            foreach (CGModuleSlot linkedSlot in inputSlot.LinkedSlots)
                                if (linkedSlot.Module.IsConfigured && linkedSlot.Module.Dirty)
                                    DTLog.LogError("[Curvy] Getting data from a dirty module. This shouldn't happen at all. Please raise a bug report. Source module is " + linkedSlot.Module);
                        }

                        Modules[i].doRefresh();
                    }
                }
            }
            if (firstChanged != null)
            {
#if UNITY_EDITOR
                DEBUG_ExecutionTime.Stop();
                if (!Application.isPlaying)
                    EditorUtility.UnloadUnusedAssetsImmediate();//OPTIM is this necessary? It was added in changeset SHA-1: 4b04d2ea947cdb84fdc45f07f3e5cbf7b9531c94
#endif
                OnRefreshEvent(new CurvyCGEventArgs(this, firstChanged));
            }
        }

        /// <summary>
        /// Will try to auto refresh the generator. Basically this calls <see cref="Refresh"/> if <see cref="AutoRefresh"/> is set and the refresh delays are respected
        /// </summary>
        public void TryAutoRefresh()
        {
            if (AutoRefresh)
            {
                if (Application.isPlaying)
                {
                    if (DTTime.TimeSinceStartup - mLastUpdateTime > RefreshDelay * 0.001f)
                    {
                        mLastUpdateTime = DTTime.TimeSinceStartup;
                        Refresh();
                    }
                }
#if UNITY_EDITOR
                else
                {
                    if (DTTime.TimeSinceStartup - mLastEditorUpdateTime > RefreshDelayEditor * 0.001f)
                    {
                        mLastEditorUpdateTime = DTTime.TimeSinceStartup;
                        Refresh();
                    }
                }
#endif
            }
        }

        #endregion

        #region ### Protected Members ###

        protected CurvyCGEventArgs OnRefreshEvent(CurvyCGEventArgs e)
        {
            if (OnRefresh != null)
                OnRefresh.Invoke(e);
            return e;
        }

        #endregion

        #region ### Privates and Internals ###
        /*! \cond PRIVATE */

        void clearModules()
        {
            //BUG when a module is a child of another one, destroying the first destroys the second, which lead to unwanted behavior in this loop
            for (int i = Modules.Count - 1; i >= 0; i--)
            {
                if (Application.isPlaying)
                    Destroy(Modules[i].gameObject);
#if UNITY_EDITOR
                else
                    Undo.DestroyObjectImmediate(Modules[i].gameObject);
#endif
            }

            Modules.Clear();
            ModulesByID.Clear();
            m_LastModuleID = 0;
        }

        /// <summary>
        /// Ensures a module name is unique
        /// </summary>
        /// <param name="name">desired name</param>
        /// <returns>unique name</returns>
        public string getUniqueModuleNameINTERNAL(string name)
        {
            string newName = name;
            bool isUnique;
            int c = 1;
            do
            {
                isUnique = true;
                foreach (CGModule mod in Modules)
                {
                    if (mod.ModuleName.Equals(newName, System.StringComparison.CurrentCultureIgnoreCase))
                    {
                        newName = name + (c++).ToString(System.Globalization.CultureInfo.InvariantCulture);
                        isUnique = false;
                        break;
                    }
                }

            } while (!isUnique);
            return newName;
        }



        /// <summary>
        /// INTERNAL! Don't call this by yourself! 
        /// </summary>
        internal void sortModulesINTERNAL()
        {
            mNeedSort = true;
        }

        bool doSortModules()
        {
            //DESIGN OPTIM: CGModule has members that are needed only in this method, and are confusing outside of this contexte, so inline everyting here and get rid of these members
            List<CGModule> unsorted = new List<CGModule>(Modules);

            List<CGModule> noAncestor = new List<CGModule>();
            List<CGModule> needNoSort = new List<CGModule>();


            // initialize
            for (int m = unsorted.Count - 1; m >= 0; m--)
            {
                unsorted[m].initializeSort();
                if (unsorted[m] is INoProcessing)
                {
                    needNoSort.Add(unsorted[m]);
                    unsorted.RemoveAt(m);
                }
                else if (unsorted[m].SortAncestors == 0)
                {
                    noAncestor.Add(unsorted[m]);
                    unsorted.RemoveAt(m);
                }
            }

            Modules.Clear();

            // Sort
            int index = 0;
            while (noAncestor.Count > 0)
            {
                // get a module without ancestors
                CGModule mod = noAncestor[0];
                noAncestor.RemoveAt(0);
                // decrement child ancestors and fetch childs without ancestors
                List<CGModule> newModsWithoutAncestors = mod.decrementChilds();
                // Add them to noAncestor list
                noAncestor.AddRange(newModsWithoutAncestors);
                // and remove from unsorted
                for (int i = 0; i < newModsWithoutAncestors.Count; i++)
                    unsorted.Remove(newModsWithoutAncestors[i]);
                // add current module to sorted
                Modules.Add(mod);
                mod.transform.SetSiblingIndex(index++);
            }

            // These modules got errors!
            for (int circ = 0; circ < unsorted.Count; circ++)
                unsorted[circ].CircularReferenceError = true;

            //Debug.Log("====: NeedNoSort=" + needNoSort.Count + ", Unsorted=" + unsorted.Count);
            //foreach (var m in Modules)
            //    Debug.Log("Sort: " + m.ModuleName);

            Modules.AddRange(unsorted);
            Modules.AddRange(needNoSort);



            mNeedSort = false;
            return (unsorted.Count > 0);
        }

        /// <summary>
        /// Sets the position of an endpoint module's recursive inputs in a way that makes the graph easy to read
        /// </summary>
        /// <param name="endPoint">The module which recursive inputs are to be reordred</param>
        /// <param name="reordredModuleIds">Set of modules already reordred</param>
        /// <param name="modulesRecursiveInputs"> A dictionary that gives for each module the set of all the modules that are connected to its inputs, whether directly or indirectly</param>
        static private void ReorderEndpointRecursiveInputs(CGModule endPoint, HashSet<int> reordredModuleIds, Dictionary<CGModule, HashSet<CGModule>> modulesRecursiveInputs)
        {
            float nextInputEndingX = endPoint.Properties.Dimensions.xMin - ModulesReorderingDeltaX;
            float nextInputStartingY = endPoint.Properties.Dimensions.yMin;

            List<CGModule> inputModules = endPoint.Input.SelectMany(i => i.GetLinkedModules()).ToList();
            foreach (CGModule inputModule in inputModules)
            {
                float inputModuleXPosition = nextInputEndingX - inputModule.Properties.Dimensions.width;
                //If module is processed for the first time, process it normally ...
                if (reordredModuleIds.Contains(inputModule.UniqueID) == false)
                {
                    inputModule.Properties.Dimensions.position = new Vector2(inputModuleXPosition, nextInputStartingY);
                    reordredModuleIds.Add(inputModule.UniqueID);
                    ReorderEndpointRecursiveInputs(inputModule, reordredModuleIds, modulesRecursiveInputs);
                }
                //... otherwise allow it to be repositioned only when pushed to the left
                else if (inputModuleXPosition < inputModule.Properties.Dimensions.xMin)
                {
                    inputModule.Properties.Dimensions.position = new Vector2(inputModuleXPosition, inputModule.Properties.Dimensions.yMin);
                    ReorderEndpointRecursiveInputs(inputModule, reordredModuleIds, modulesRecursiveInputs);
                }
                nextInputStartingY = Math.Max(nextInputStartingY, modulesRecursiveInputs[inputModule].Max(m => m.Properties.Dimensions.yMax) + ModulesReorderingDeltaY);
            }
        }

        /// <summary>
        /// Adds to the modules recursive inputs dictionary the entries corresponding to the given module 
        /// </summary>
        /// <returns>The recursive inputs of the given module</returns>
        static private HashSet<CGModule> UpdateModulesRecursiveInputs(Dictionary<CGModule, HashSet<CGModule>> modulesRecursiveInputs, CGModule moduleToAdd)
        {
            if (modulesRecursiveInputs.ContainsKey(moduleToAdd))
                return modulesRecursiveInputs[moduleToAdd];

            List<CGModule> inputModules = moduleToAdd.Input.SelectMany(i => i.GetLinkedModules()).ToList();
            HashSet<CGModule> result = new HashSet<CGModule>();
            result.Add(moduleToAdd);
            result.UnionWith(inputModules.SelectMany(i => UpdateModulesRecursiveInputs(modulesRecursiveInputs, i)));
            modulesRecursiveInputs[moduleToAdd] = result;
            return result;
        }


        /*! \endcond */
        #endregion


    }
}
