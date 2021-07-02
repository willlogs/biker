// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;

public class ToggleBehaviourByTrigger : MonoBehaviour
{
    public Behaviour UIElement;

    void OnTriggerEnter()
    {
        if (UIElement)
            UIElement.enabled = !UIElement.enabled;
    }
}
