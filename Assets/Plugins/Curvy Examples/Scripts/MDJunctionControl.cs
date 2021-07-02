// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy;
using UnityEngine.UI;

namespace FluffyUnderware.Curvy.Examples
{
    public class MDJunctionControl : CurvyMetadataBase
    {
        public bool UseJunction;

        public void Toggle()
        {
            UseJunction = !UseJunction;
        }
        
    }
}
