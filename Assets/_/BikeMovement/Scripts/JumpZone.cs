using DG.Tweening;
using Dreamteck.Splines;
using PT.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PT.Bike
{
    public class JumpZone : MonoBehaviour
    {
        public SplineComputer spline;

        public UnityEvent<Transform> OnPlayerEntered;

        private void OnTriggerEnter(Collider other)
        {
            BikeController bc = other.GetComponent<BikeController>();
            if(bc != null)
            {
                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                other.transform.DORotateQuaternion(Quaternion.LookRotation((spline.GetPoint(0).tangent2 - spline.GetPoint(0).position).normalized, spline.GetPoint(0).normal), 0.5f).OnComplete(() =>
                {
                    bc.FollowSpline();
                    bc.splineFollower.spline = spline;
                    bc.splineFollower.transform.position = spline.GetPoint(0).position;
                    bc.splineFollower.motion.offset = transform.position - spline.GetPoint(0).position;
                    bc.splineFollower.enabled = true;
                    bc.DeactivateControl();
                    
                    if (pbc != null)
                    {
                        pbc.ActivateShootingMode();
                        OnPlayerEntered?.Invoke(other.transform);

                        TimeManager.Instance.SlowDown(0.5f);
                    }
                });                
            }
        }
    }
}