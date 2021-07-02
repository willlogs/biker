// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that makes some Animation methods available to Unity Events
/// </summary>
public class AnimationHelper : MonoBehaviour {

    public void Play(Animation animation)
    {
        animation.Play();
    }

    public void RewindThenPlay(Animation animation)
    {
        animation.Rewind();
        animation.Play();
    }
}
