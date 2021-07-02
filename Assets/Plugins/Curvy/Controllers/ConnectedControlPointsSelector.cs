// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;

namespace FluffyUnderware.Curvy.Controllers
{
    /// <summary>
    /// A class used by <see cref="SplineController"/> to define custom selection logic to select between the possible connected splines when the controller reaches a <see cref="CurvyConnection"/>
    /// </summary>
    abstract public class ConnectedControlPointsSelector : MonoBehaviour
    {
        /// <summary>
        /// Select, from the current connection, a Control Point to continue moving through.
        /// </summary>
        /// <param name="caller">The spline controller that is calling this selector</param>
        /// <param name="connection">The connection the caller reached and for which it needs to select a Control Point to follow</param>
        /// <param name="currentControlPoint">the Control Point, part of the connection, the controller is at.</param>
        abstract public CurvySplineSegment SelectConnectedControlPoint(SplineController caller, CurvyConnection connection, CurvySplineSegment currentControlPoint);
    }
}
