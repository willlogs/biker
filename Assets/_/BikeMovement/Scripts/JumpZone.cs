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
        public float duration = 4;

        public UnityEvent<Transform> OnPlayerEntered;

        private void OnTriggerEnter(Collider other)
        {
            BikeController bc = other.GetComponent<BikeController>();
            if(bc != null)
            {
                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();

                // set rotation
                other.transform.DORotateQuaternion(Quaternion.LookRotation((spline.GetPoint(0).tangent2 - spline.GetPoint(0).position).normalized, spline.GetPoint(0).normal), 0.5f).OnComplete(() =>
                {
                    bc.FollowSpline(duration);
                    bc.splineFollower.spline = spline;
                    bc.splineFollower.motion.offset = bc.transform.position - spline.GetPoint(0).position;
                    bc.splineFollower.enabled = true;
                    bc.DeactivateControl();
                    
                    if (pbc != null)
                    {
                        pbc._gunTargetT.position = pbc.transform.position + pbc.transform.forward * 5 + pbc.transform.up * 2;
                        pbc.ActivateShootingMode();
                        OnPlayerEntered?.Invoke(other.transform);

                        TimeManager.Instance.SlowDown(0.5f);
                    }
                });                
            }
        }
    }
}