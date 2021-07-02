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
using FluffyUnderware.DevToolsEditor;

namespace FluffyUnderware.CurvyEditor.Generator
{ 
    public class CanvasSelection
    {
        public List<CGModule> SelectedModules = new List<CGModule>();

        public CGModuleLink SelectedLink { get; private set; }
        public CGModule SelectedModule 
        {
            get { return (SelectedModules.Count > 0) ? SelectedModules[0] : null; }
        }

        public CGGraph Parent;


        public object[] SelectedObjects
        {
            get
            {
                if (SelectedLink!=null)
                    return new object[1]{SelectedLink};
                return SelectedModules.ToArray();
            }
        }

        public CanvasSelection(CGGraph parent)
        {
            Parent = parent;
        }

        public void Clear()
        {
            SelectedLink = null;
            SelectedModules.Clear();
            if (CurvyProject.Instance.CGSynchronizeSelection)
                DTSelection.Clear();
        }

        /// <summary>
        /// Selects nothing (null), a link or one or more modules
        /// </summary>
        /// <param name="mod"></param>
        public void Select(params object[] objects)
        {
            Clear();
            if (objects==null || objects.Length==0)
                return;
            if (objects[0] is List<CGModule>)
                objects = ((List<CGModule>)objects[0]).ToArray();
            if (objects[0] is CGModuleLink)
                SelectedLink = (CGModuleLink)objects[0];
            else
            {
                List<Component>cmp=new List<Component>();
                foreach (object o in objects)
                    if (o is CGModule) {
                        SelectedModules.Add((CGModule)o);
                        cmp.Add((CGModule)o);
                    }

                if (CurvyProject.Instance.CGSynchronizeSelection)
                    DTSelection.SetGameObjects(cmp.ToArray());
            }
            
        }

       

        /// <summary>
        /// Adds or removes a module from the selection
        /// </summary>
        public void MultiSelectModule(CGModule mod)
        {
            if (mod == null)
                return;
            if (SelectedModules.Contains(mod))
                SelectedModules.Remove(mod);
            else
                SelectedModules.Add(mod);
            
        }
    }
}
