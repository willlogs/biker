// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using System.Reflection;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.DevTools;
using UnityEngine.Assertions;

#if CONTRACTS_FULL
using System.Diagnostics.Contracts;
#endif
namespace FluffyUnderware.Curvy
{

    public class CurvyEventArgs : EventArgs
    {
        /// <summary>
        /// The component raising the event
        /// </summary>
        public readonly MonoBehaviour Sender;
        /// <summary>
        /// Custom data
        /// </summary>
        public readonly object Data;

        public CurvyEventArgs(MonoBehaviour sender, object data)
        {
            Sender = sender;
            Data = data;

#if CURVY_SANITY_CHECKS
            Assert.IsTrue(Sender != null);
#endif
        }
    }

    #region ### Spline Events ###

    /// <summary>
    /// Class used by spline related events
    /// </summary>
    [System.Serializable]
    public class CurvySplineEvent : UnityEventEx<CurvySplineEventArgs> { }

    /// <summary>
    /// Class used by spline related events
    /// </summary>
    [System.Serializable]
    public class CurvyControlPointEvent : UnityEventEx<CurvyControlPointEventArgs> { }

    /// <summary>
    /// EventArgs used by CurvyControlPointEvent events
    /// </summary>
    public class CurvyControlPointEventArgs : CurvySplineEventArgs
    {
        /// <summary>
        /// Event Mode
        /// </summary>
        public enum ModeEnum
        {
            /// <summary>
            /// Send for events that are not related to control points adding or removal
            /// </summary>
            None,
            /// <summary>
            /// Send when a Control point is added before an existing one
            /// </summary>
            AddBefore,
            /// <summary>
            /// Send when a Control point is added after an existing one
            /// </summary>
            AddAfter,
            /// <summary>
            /// Send when a Control point is deleted
            /// </summary>
            Delete
        }

        /// <summary>
        /// Determines the action this event was raised for
        /// </summary>
        public readonly ModeEnum Mode;
        /// <summary>
        /// Related Control Point
        /// </summary>
        public readonly CurvySplineSegment ControlPoint;

        public CurvyControlPointEventArgs(MonoBehaviour sender, CurvySpline spline, CurvySplineSegment cp, ModeEnum mode = ModeEnum.None, object data = null) : base(sender, spline, data)
        {
            ControlPoint = cp;
            Mode = mode;
        }
    }



    /// <summary>
    /// EventArgs used by CurvySplineEvent events
    /// </summary>
    public class CurvySplineEventArgs : CurvyEventArgs
    {
        /// <summary>
        /// The related spline
        /// </summary>
        public readonly CurvySpline Spline;

        public CurvySplineEventArgs(MonoBehaviour sender, CurvySpline spline, object data = null) : base(sender, data)
        {
            Spline = spline;


#if CURVY_SANITY_CHECKS
            Assert.IsTrue(Spline != null);
#endif
        }
    }

    #endregion

    #region ### CG Events ###

    /// <summary>
    /// Curvy Generator related event
    /// </summary>
    [System.Serializable]
    public class CurvyCGEvent : UnityEventEx<CurvyCGEventArgs> { }

    /// <summary>
    /// EventArgs for CurvyCGEvent events
    /// </summary>
    public class CurvyCGEventArgs : System.EventArgs
    {
        /// <summary>
        /// the component raising the event
        /// </summary>
        public readonly MonoBehaviour Sender;
        /// <summary>
        /// The related CurvyGenerator
        /// </summary>
        public readonly CurvyGenerator Generator;
        /// <summary>
        /// The related CGModule
        /// </summary>
        public readonly CGModule Module;

        public CurvyCGEventArgs(CGModule module)
        {
            Sender = module;
            Generator = module.Generator;
            Module = module;

#if CURVY_SANITY_CHECKS
            Assert.IsTrue(Sender != null);
#endif
        }

        public CurvyCGEventArgs(CurvyGenerator generator, CGModule module)
        {
            Sender = generator;
            Generator = generator;
            Module = module;

#if CURVY_SANITY_CHECKS
            Assert.IsTrue(Sender != null);
#endif
        }

    }

    #endregion

}
