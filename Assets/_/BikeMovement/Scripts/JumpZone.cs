using DG.Tweening;
using FluffyUnderware.Curvy;
using PT.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PT.Bike
{
    public class JumpZone : MonoBehaviour
    {
        public CurvySpline spline;

        public UnityEvent<Transform> OnPlayerEntered;

        [SerializeField] private float slowMoFactor = 0.5f;

        private void OnTriggerEnter(Collider other)
        {
            BikeController bc = other.GetComponent<BikeController>();
            if(bc != null)
            {
                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                other.transform.DORotateQuaternion(Quaternion.LookRotation((spline[1].transform.position - spline[0].transform.position).normalized, spline[0].transform.up), 0.5f).OnComplete(() =>
                {
                    bc.FollowSpline();
                    bc.splineFollower.Spline = spline;
                    /*bc.splineFollower.transform.position = spline.GetPoint(0).position;
                    bc.splineFollower.motion.offset = transform.position - spline.GetPoint(0).position;*/
                    bc.splineFollower.enabled = true;
                    bc.DeactivateControl();
                    
                    if (pbc != null)
                    {
                        pbc.ActivateShootingMode();
                        OnPlayerEntered?.Invoke(other.transform);

                        TimeManager.Instance.SlowDown(slowMoFactor);
                    }
                });                
            }
        }
    }
}