// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using System.Collections;
using FluffyUnderware.DevTools;

namespace FluffyUnderware.Curvy
{

    /// <summary>
    /// Curvy Generator options Metadata class
    /// </summary>
    [HelpURL(CurvySpline.DOCLINK + "metacgoptions")]
    public class MetaCGOptions : CurvyMetadataBase
    {

        #region ### Serialized Fields ###

        [Positive]
        [SerializeField]
        int m_MaterialID;


        [SerializeField]
        bool m_HardEdge;
        [Positive(Tooltip = "Max step distance when using optimization")]
        [SerializeField]
        float m_MaxStepDistance;
        [Section("Extended UV", HelpURL = CurvySpline.DOCLINK + "metacgoptions_extendeduv")]
        [FieldCondition("showUVEdge", true)]
        [SerializeField]
        bool m_UVEdge;

        [Positive]
        [FieldCondition("showExplicitU", true)]
        [SerializeField]
        bool m_ExplicitU;
        [FieldCondition("showFirstU", true)]
        [FieldAction("CBSetFirstU")]
        [Positive]
        [SerializeField]
        float m_FirstU;
        [FieldCondition("showSecondU", true)]
        [Positive]
        [SerializeField]
        float m_SecondU;



        #endregion

        #region ### Public Properties ###

        /// <summary>
        /// Gets or sets Material ID
        /// </summary>
        public int MaterialID
        {
            get
            {
                return m_MaterialID;
            }
            set
            {
                int v = Mathf.Max(0, value);
                if (m_MaterialID != v)
                {
                    m_MaterialID = v;
                    NotifyModification();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to create a hard edge or not
        /// </summary>
        public bool HardEdge
        {
            get { return m_HardEdge; }
            set
            {
                if (m_HardEdge != value)
                {
                    m_HardEdge = value;
                    NotifyModification();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to create an UV edge or not
        /// </summary>
        public bool UVEdge
        {
            get { return m_UVEdge; }
            set
            {
                if (m_UVEdge != value)
                {
                    m_UVEdge = value;
                    NotifyModification();
                }
            }
        }

        /// <summary>
        /// Gets or sets whether to define explicit U values
        /// </summary>
        public bool ExplicitU
        {
            get { return m_ExplicitU; }
            set
            {
                if (m_ExplicitU != value)
                {
                    m_ExplicitU = value;
                    NotifyModification();
                }
            }
        }

        /// <summary>
        /// Gets or sets UV0
        /// </summary>
        public float FirstU
        {
            get { return m_FirstU; }
            set
            {
                if (m_FirstU != value)
                {
                    m_FirstU = value;
                    NotifyModification();
                }
            }
        }

        /// <summary>
        /// Gets or sets UV0
        /// </summary>
        public float SecondU
        {
            get { return m_SecondU; }
            set
            {
                if (m_SecondU != value)
                {
                    m_SecondU = value;
                    NotifyModification();
                }
            }
        }

        /// <summary>
        /// Gets or sets maximum vertex distance when using optimization (0=infinite)
        /// </summary>
        public float MaxStepDistance
        {
            get
            {
                return m_MaxStepDistance;
            }
            set
            {
                float v = Mathf.Max(0, value);
                if (m_MaxStepDistance != v)
                {
                    m_MaxStepDistance = v;
                    NotifyModification();
                }
            }
        }

        public bool HasDifferentMaterial
        {
            get
            {
                MetaCGOptions metaCgOptions = GetPreviousData<MetaCGOptions>(true);
                return metaCgOptions && metaCgOptions.MaterialID != MaterialID;
            }
        }

        #endregion

        #region ### Private Fields & Properties ###



        bool showUVEdge
        {
            get
            {
                return (ControlPoint && (Spline.Closed || (!(Spline.FirstVisibleControlPoint == ControlPoint) && !(Spline.LastVisibleControlPoint == ControlPoint))) && !HasDifferentMaterial);
            }
        }

        bool showExplicitU
        {
            get
            {
                return (ControlPoint && !UVEdge && !HasDifferentMaterial);
            }
        }

        bool showFirstU
        {
            get
            {
                bool res = false;
                if (ControlPoint)
                    res = UVEdge || ExplicitU || HasDifferentMaterial;

                return res;
            }
        }

        bool showSecondU
        {
            get
            {
                return UVEdge || HasDifferentMaterial;
            }
        }

        #endregion

        #region ### Unity Callbacks ###
        /*! \cond UNITY */



#if UNITY_EDITOR
        void OnValidate()
        {
            NotifyModification();
        }
#endif

        public void Reset()
        {
            MaterialID = 0;
            HardEdge = false;
            MaxStepDistance = 0;
            UVEdge = false;
            ExplicitU = false;
            FirstU = 0;
            SecondU = 0;
        }

        /*! \endcond */
        #endregion

        #region ### Public Methods ###

        public float GetDefinedFirstU(float defaultValue)
        {
            return (UVEdge || ExplicitU || HasDifferentMaterial) ? FirstU : defaultValue;
        }

        public float GetDefinedSecondU(float defaultValue)
        {
            return (UVEdge || HasDifferentMaterial) ? SecondU : GetDefinedFirstU(defaultValue);
        }

        #endregion

        #region ### Privates ###
        /*! \cond PRIVATES */



        /*! \endcond */
        #endregion
    }
}
