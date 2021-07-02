// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy.Generator;
using System.Collections.Generic;
using FluffyUnderware.Curvy.Generator.Modules;
using FluffyUnderware.DevToolsEditor;

namespace FluffyUnderware.CurvyEditor.Generator.Modules
{
    [CustomEditor(typeof(CreateGameObject))]
    public class CreateGameObjectEditor : CGModuleEditor<CreateGameObject>
    {
        public override void OnModuleDebugGUI()
        {
            base.OnModuleDebugGUI();
            if (Target)
            {
                
            }
        }

        

    }
}
