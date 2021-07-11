using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Utils;
using PT.AI;
using PT.Bike;

namespace PT.CarChase
{
    public class TargetCar : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform[] _trace;
        [SerializeField] private float _slowMoFactor;

        private bool _isIn = false;
        private PlayerBikeController _player;

        public void WinSituation()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isIn && other.gameObject.layer == 8)
            {
                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                if (pbc != null)
                {
                    _isIn = true;

                    pbc.ActivateShootingMode(_target, true);
                    pbc.FollowTarget(transform);

                    _player = pbc;

                    TimeManager.Instance.SlowDown(0.5f, _slowMoFactor);
                }
            }
        }
    }
}