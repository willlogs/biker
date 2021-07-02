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
    /// <summary>
    /// For modules that don't process anything
    /// </summary>
    public interface INoProcessing
    {
    }

    /// <summary>
    /// For modules that rely on external input (Splines, Meshes etc..)
    /// </summary>
    public interface IExternalInput
    {
        /// <summary>
        /// Whether the module currently supports an IPE session
        /// </summary>
        bool SupportsIPE { get; }
    }

    /// <summary>
    /// For modules that process data on demand
    /// </summary>
    public interface IOnRequestProcessing
    {
        CGData[] OnSlotDataRequest(CGModuleInputSlot requestedBy, CGModuleOutputSlot requestedSlot, params CGDataRequestParameter[] requests);
    }

    /// <summary>
    /// For modules that output instances of <see cref="CGPath"/>
    /// </summary>
    public interface IPathProvider
    {
        bool PathIsClosed { get; }
    }

    /// <summary>
    /// For modules that output instances of <see cref="CGPath"/> on demand
    /// </summary>
    [Obsolete("IOnRequestPath is an interface with no added value, and will get removed in a future update. Use IOnRequestProcessing or IPathProvider instead")]
    public interface IOnRequestPath : IOnRequestProcessing, IPathProvider
    {

    }

    /// <summary>
    /// Resource Loader Interface
    /// </summary>
    public interface ICGResourceLoader
    {
        Component Create(CGModule cgModule, string context);
        void Destroy(CGModule cgModule, Component obj, string context, bool kill);
    }

    /// <summary>
    /// Resource Collection interface
    /// </summary>
    public interface ICGResourceCollection
    {
        int Count { get; }
        Component[] ItemsArray { get; }
    }
}
