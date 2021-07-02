// =====================================================================
// Copyright 2013-2019 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using FluffyUnderware.Curvy.Generator.Modules;
using UnityEngine;

/// <summary>
/// Modifies the Mix Curve of the Variable Mix Shapes in example scene 27
/// </summary>
public class MixingAnimator : MonoBehaviour
{
    public ModifierVariableMixShapes VariableMixShapes;

    void Update()
    {
        Keyframe[] mixCurveKeys = VariableMixShapes.MixCurve.keys;
        mixCurveKeys[1].value = Mathf.Sin(Time.time);
        VariableMixShapes.MixCurve.keys = mixCurveKeys;
        VariableMixShapes.Dirty = true;
    }
}
