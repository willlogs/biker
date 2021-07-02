// =====================================================================
// Copyright 2013-2018 ToolBuddy
// All rights reserved
// 
// http://www.toolbuddy.net
// =====================================================================

using UnityEngine;
using System.Collections;
using FluffyUnderware.Curvy.Controllers;
using FluffyUnderware.Curvy.Components;

namespace FluffyUnderware.Curvy.Examples
{
    [ExecuteInEditMode]
    public class TrainCarManager : MonoBehaviour
    {
        public SplineController Waggon;
        public SplineController FrontAxis;
        public SplineController BackAxis;

        public float Position
        {
            get
            {
                return Waggon.AbsolutePosition;
            }
            set
            {
                if (Waggon.AbsolutePosition != value)
                {
                    Waggon.AbsolutePosition = value;
                    FrontAxis.AbsolutePosition = value + mTrain.AxisDistance / 2;
                    BackAxis.AbsolutePosition = value - mTrain.AxisDistance / 2;
                }
            }
        }

        TrainManager mTrain;

        void LateUpdate()
        {
            if (!mTrain)
                return;

            if (BackAxis.Spline == FrontAxis.Spline &&
                FrontAxis.RelativePosition > BackAxis.RelativePosition)
            {
                float carPosition = Waggon.AbsolutePosition;
                float frontAxisPosition = FrontAxis.AbsolutePosition;
                float backAxisPosition = BackAxis.AbsolutePosition;

                if (Mathf.Abs(Mathf.Abs(frontAxisPosition - backAxisPosition) - mTrain.AxisDistance) >= mTrain.Limit)
                {
                    FrontAxis.AbsolutePosition = carPosition + mTrain.AxisDistance / 2;
                    BackAxis.AbsolutePosition = carPosition - mTrain.AxisDistance / 2;
                }
            }
        }



        public void setup()
        {
            mTrain = GetComponentInParent<TrainManager>();
            if (mTrain.Spline)
            {
                setController(Waggon, mTrain.Spline, mTrain.Speed);
                setController(FrontAxis, mTrain.Spline, mTrain.Speed);
                setController(BackAxis, mTrain.Spline, mTrain.Speed);
            }
        }

        void setController(SplineController c, CurvySpline spline, float speed)
        {
            c.Spline = spline;
            c.Speed = speed;
            c.OnControlPointReached.AddListenerOnce(OnCPReached);
        }

        public void OnCPReached(CurvySplineMoveEventArgs e)
        {
            MDJunctionControl jc = e.ControlPoint.GetMetadata<MDJunctionControl>();
            SplineController splineController = e.Sender;
            splineController.ConnectionBehavior = (jc && jc.UseJunction == false)
                    ? SplineControllerConnectionBehavior.CurrentSpline
                    : SplineControllerConnectionBehavior.RandomSpline;
        }

    }
}
