using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    public class JumpZone : MonoBehaviour
    {
        public SplineComputer spline;

        private void OnTriggerEnter(Collider other)
        {
            BikeController bc = other.GetComponent<BikeController>();
            if(bc != null)
            {
                other.transform.DORotateQuaternion(Quaternion.LookRotation((spline.GetPoint(0).tangent2 - spline.GetPoint(0).position).normalized, spline.GetPoint(0).normal), 0.5f).OnComplete(() =>
                {
                    bc.splineFollower.spline = spline;
                    bc.splineFollower.motion.offset = transform.position - spline.GetPoint(0).position;
                    bc.splineFollower.enabled = true;
                    bc.DeactivateControl();
                    
                    PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                    if (pbc != null)
                    {
                        pbc.ActivateShootingMode();

                        Time.timeScale = 0.3f;
                        Time.fixedDeltaTime = 0.02f * 0.3f;
                    }
                });                
            }
        }
    }
}