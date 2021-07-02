// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using UnityEditor;
using FluffyUnderware.Curvy.Generator;
using FluffyUnderware.DevTools.Extensions;
using FluffyUnderware.DevToolsEditor;
using FluffyUnderware.DevToolsEditor.Data;

namespace FluffyUnderware.CurvyEditor.Generator
{
    internal class CanvasState
    {
        public bool IsWindowDrag;
        public bool IsCanvasDrag;
        public bool IsModuleDrag;
        public bool IsSelectionRectDrag;
        public CGModuleOutputSlot LinkDragFrom;
        public CGModuleOutputSlot AutoConnectFrom;

        CGGraph Parent;
        public CanvasSelection Sel { get { return Parent.Sel; } }
        public CanvasUI UI { get { return Parent.UI; } }
        Event EV { get { return Event.current; } }
        

        public CanvasState(CGGraph parent)
        {
            Parent = parent;
        }

        
        /// <summary>
        /// The module the mouse is hovering
        /// </summary>
        public CGModule MouseOverModule;
        

        /// <summary>
        /// Storing Event.current.mousePosition (in Canvasspace!)
        /// </summary>
        public Vector2 MousePosition;
        
        /// <summary>
        /// Canvas scrolling state
        /// </summary>
        public AnimVector2 Scroll = new AnimVector2();
        
        /// <summary>
        /// Left/Top offset of Canvas from window
        /// </summary>
        public Vector2 ClientRectOffset;
        /// <summary>
        /// Starting position of selection drag
        /// </summary>
        public Vector2 SelectionRectStart;

        /// <summary>
        /// Gets whether a link is currently dragged
        /// </summary>
        public bool IsLinkDrag
        {
            get { return LinkDragFrom != null; }
        }

        /// <summary>
        /// Whether the mouse is hovering over a module or not
        /// </summary>
        public bool IsMouseOverModule { get; private set; }

        /// <summary>
        /// Gets whether the user is currently dragging anything (Canvas, Module, Link, etc..)
        /// </summary>
        public bool IsDragging
        {
            get { return IsWindowDrag || IsCanvasDrag || IsModuleDrag || IsLinkDrag || IsSelectionRectDrag; }
        }

        public bool IsMouseOverCanvas
        {
            get
            {
                return ViewPort.Contains(MousePosition);
            }
        }
        /// <summary>
        /// Gets the canvas' scrollview size in window space
        /// </summary>
        public Rect ClientRect { get; private set; }
        /// <summary>
        /// Gets the total canvas size
        /// </summary>
        public Rect CanvasRect { get; private set; }

        /// <summary>
        /// Gets the visible rect of the canvas
        /// </summary>
        public Rect ViewPort
        {
            get
            {
                return new Rect(CanvasRect.x+Scroll.value.x, CanvasRect.y+Scroll.value.y,ClientRect.width,ClientRect.height);
            }
        }

        public Vector2 ViewPortMousePosition
        {
            get
            {
                return MousePosition + ClientRectOffset - ViewPort.min;
            }
        }

        public void SetClientRect(float xOffset, float yOffset,float xspace=0,float yspace=0)
        {
            ClientRectOffset = new Vector2(xOffset, yOffset);
            ClientRect = new Rect(ClientRectOffset.x, ClientRectOffset.y, (Parent.position.width - ClientRectOffset.x-xspace), (Parent.position.height - ClientRectOffset.y-yspace));
            
        }

        /// <summary>
        /// Grows the canvas to include the rect
        /// </summary>
        /// <param name="r">a rect in canvas space</param>
        public void EnlargeCanvasFor(Rect r)
        {
            //if (CurvyGUI.IsLayout)
            CanvasRect=CanvasRect.Include(r);
        }

        public void BeginGUI()
        {
            if (!DTGUI.IsLayout)
                CanvasRect = new Rect(0, 0, 0, 0);
            if (EV.isMouse)
                MousePosition = EV.mousePosition;
            //Debug.Log(EV.type);
            switch (EV.type)
            {
                
                case EventType.MouseDrag:
                    
                    if (!IsMouseOverCanvas && !IsSelectionRectDrag && !IsModuleDrag)
                        IsWindowDrag = true;
                    if (!IsDragging)
                    {
                        if (IsMouseOverModule)
                            IsModuleDrag = true;
                        else
                        {
                            IsSelectionRectDrag = true;
                            SelectionRectStart = ViewPortMousePosition;
                        }
                    }
                    break;
                case EventType.Used: // dirty, but works
                    IsWindowDrag = false;
                    break;
                case EventType.MouseUp:
                    IsModuleDrag = false;
                    if (EV.button == 1)
                        UI.ContextMenu();
                    break;
                
                case EventType.MouseDown:
                    if (EV.button == 1)
                    {
                        if (IsMouseOverModule && !Sel.SelectedModules.Contains(MouseOverModule))
                        {
                            Sel.Select(MouseOverModule);
                            FocusSelection();
                        }
                    }
                    else if (EV.button == 2)
                        IsCanvasDrag = true;
                    break;
                case EventType.KeyDown:
                    if (EV.keyCode == KeyCode.Space)
                        IsCanvasDrag = true;
                    break;
                case EventType.KeyUp:
                    IsCanvasDrag = false;
                    break;
                
                
            }

            if (EV.type != EventType.Layout)
                IsMouseOverModule = false;

            if (IsCanvasDrag)
                EditorGUIUtility.AddCursorRect(ViewPort, MouseCursor.Pan);
        }

        /// <summary>
        /// Processing of Events AFTER Module Window drawing (Beware! Window dragging eats up most events!)
        /// </summary>
        public void EndGUI()
        {
            switch (EV.type)
            {
                case EventType.MouseDrag:
                    // Drag canvas (i.e. change scroll offset)
                    if (IsCanvasDrag)
                    {
                        Scroll.value -= EV.delta;
                    }
                    break;
                case EventType.KeyDown:
                    if (EV.keyCode == KeyCode.Delete)
                    {
                        UI.Delete(Sel.SelectedObjects);
                        Sel.Clear();
                    }
                    break;
                case EventType.MouseUp:
                    if (EV.button == 0 &&
                        !IsDragging &&
                        !CurvyProject.Instance.CGAutoModuleDetails &&
                        (!Sel.SelectedLink && !MouseOverLink(Sel.SelectedLink)) &&
                        (!Sel.SelectedModule || !MouseOverModule)
                        )
                        Sel.Select(null);
                    if (IsLinkDrag && EV.control)
                    {
                        AutoConnectFrom = LinkDragFrom;
                        UI.AddModuleQuickmenu(LinkDragFrom);
                        
                    }
                    // Multi Selection
                    if (IsSelectionRectDrag)
                    {
                        HandleMultiSelection();
                        IsSelectionRectDrag = false;
                    }

                    LinkDragFrom = null;
                    IsCanvasDrag = false;
                    Parent.StatusBar.Clear();
                    
                    break;
                case EventType.DragUpdated:
                    UI.HandleDragDropProgress();
                    break;
                case EventType.DragPerform:
                    UI.HandleDragDropDone();
                    break;
            }
            
        }

        public void ViewPortRegisterWindow(CGModule module)
        {
            Rect winRect = module.Properties.Dimensions;
            EnlargeCanvasFor(winRect);
            
            if (!IsMouseOverModule && EV.type != EventType.Layout)
            {
                IsMouseOverModule = winRect.Contains(EV.mousePosition);
                MouseOverModule=(IsMouseOverModule) ? module:null;
            }
            
        }

        public void FocusSelection()
        {
            
            if (Sel.SelectedModule)
            {
                Rect dim = Sel.SelectedModule.Properties.Dimensions;
                Vector2 delta = Vector2.zero;
                if (dim.xMax > ViewPort.xMax)
                    delta.x = dim.xMax - ViewPort.xMax + 20;
                else if (dim.xMin < ViewPort.xMin)
                    delta.x = dim.xMin - ViewPort.xMin - 20;

                if (dim.yMax > ViewPort.yMax)
                    delta.y = dim.yMax - ViewPort.yMax + 20;
                else if (dim.yMin < ViewPort.yMin)
                    delta.y = dim.yMin - ViewPort.yMin - 20;
                
                Scroll.target = Scroll.value + delta;
            }
        }

        public bool MouseOverLink(CGModuleLink link)
        {
            if (link == null)
                return false;
            CGModuleOutputSlot outSlot = Parent.Generator.ModulesByID[link.ModuleID].GetOutputSlot(link.SlotName);
            CGModuleInputSlot inSlot = Parent.Generator.ModulesByID[link.TargetModuleID].GetInputSlot(link.TargetSlotName);
            Vector3 a = outSlot.Origin;
            Vector3 at = a + new Vector3(40, 0, 0);
            Vector3 b = inSlot.Origin;
            Vector3 bt = b + new Vector3(-40, 0, 0);
            return HandleUtility.DistancePointBezier(EV.mousePosition, a, b, at, bt) < 3;
        }
       
        

        public void HandleMultiSelection()
        {
            Rect selectionRect = new Rect().SetBetween(SelectionRectStart, ViewPortMousePosition);
            //if (selectionRect.size != Vector2.zero)
                Sel.Clear();
            //else
            //    Sel.SelectedModules.Clear();
            selectionRect.position -= ClientRectOffset-ViewPort.position;
            foreach (CGModule mod in Parent.Modules)
                if (selectionRect.Overlaps(mod.Properties.Dimensions,true))
                    Sel.MultiSelectModule(mod);

        }
    }
}
