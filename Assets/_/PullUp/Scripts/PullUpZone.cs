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

        [SerializeField] private AIPedesterian[] _enemies;

        private Quaternion _beforeRotation;

        private bool _isIn = false;
        private PlayerBikeController _player;

        private int _count = 0;

        private void Start()
        {
            _count = _enemies.Length;

            foreach(AIPedesterian e in _enemies)
            {
                e.OnDeath += Reduce;
            }
        }

        private void Reduce()
        {
            _count--;
            if(_count <= 0)
            {
                // get back to running away
                TimeManager.Instance.GoNormal(0.5f);

                _player.transform.DORotateQuaternion(
                    _beforeRotation,
                    0.5f
                ).OnComplete(() => {
                    _player.ContinueMoving();
                    _player.DeactivateShootingMode();
                });
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isIn && other.gameObject.layer == 8)
            {
                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                if (pbc != null)
                {
                    _isIn = true;

                    pbc.StopEverything();
                    _beforeRotation = pbc.transform.rotation;
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