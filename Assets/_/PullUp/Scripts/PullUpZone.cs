using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Utils;
using PT.AI;
using PT.Bike;
using DG.Tweening;

namespace PT.PullUp
{
    public class PullUpZone : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform[] _trace;
        [SerializeField] private float _slowMoFactor;

        private bool _isIn = false;
        private PlayerBikeController _player;

        private void OnTriggerEnter(Collider other)
        {
            if (!_isIn && other.gameObject.layer == 8)
            {
                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                if (pbc != null)
                {
                    _isIn = true;

                    pbc.StopEverything();
                    pbc.transform.DORotateQuaternion(
                        Quaternion.FromToRotation(
                            pbc.transform.forward, 
                            _target.position - pbc.transform.position
                        ) * pbc.transform.rotation,
                        0.5f
                    ).OnComplete(() => {
                        pbc.ActivateShootingMode(_target, true);
                    });

                    _player = pbc;

                    TimeManager.Instance.SlowDown(0.5f, _slowMoFactor);
                }
            }
        }
    }
}