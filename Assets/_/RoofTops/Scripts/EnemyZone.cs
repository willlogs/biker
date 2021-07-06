using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Utils;
using PT.AI;
using PT.Bike;

namespace PT.Rooftop
{
    public class EnemyZone : MonoBehaviour
    {
        [SerializeField] private AIPedesterian[] _enemies;
        [SerializeField] private Transform[] _trace;
        [SerializeField] private float _slowMoFactor;

        private bool _isIn = false;
        private int _count;

        private PlayerBikeController _player;

        private void Start()
        {
            _count = _enemies.Length;
        }

        private void OnEnable()
        {
            foreach(AIPedesterian enemy in _enemies)
            {
                enemy.OnDeath += ReduceCount;
            }
        }

        private void OnDisable()
        {
            foreach (AIPedesterian enemy in _enemies)
            {
                enemy.OnDeath -= ReduceCount;
            }
        }

        private void OnDestroy()
        {
            foreach (AIPedesterian enemy in _enemies)
            {
                enemy.OnDeath -= ReduceCount;
            }
        }

        private void ReduceCount()
        {
            _count--;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isIn && other.gameObject.layer == 8)
            {
                PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
                if (pbc != null)
                {
                    _isIn = true;

                    pbc.ActivateShootingMode();
                    pbc.traceFollower.FollowTrace(_trace);
                    _player = pbc;

                    TimeManager.Instance.SlowDown(0.5f, _slowMoFactor);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isIn && other.gameObject.layer == 8)
            {
                _isIn = false;
                _player.DeactivateShootingMode();
                _player.traceFollower.DontFollowTrace();

                TimeManager.Instance.GoNormal(0.5f);
            }
        }
    }
}