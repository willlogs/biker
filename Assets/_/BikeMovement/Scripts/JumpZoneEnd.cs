using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    public class JumpZoneEnd : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            BikeController bc = other.GetComponent<BikeController>();
            if (bc != null)
            {
                bc.DontFollowSpline();
                bc.splineFollower.enabled = false;
                bc.ActivateControl();

                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                if (pbc != null)
                {
                    pbc.DeactivateShootingMode();

                    Time.timeScale = 1f;
                    Time.fixedDeltaTime = 0.01f;
                }
            }
        }
    }
}
