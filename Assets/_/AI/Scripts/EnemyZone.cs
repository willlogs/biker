using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Bike;
using PT.Utils;

namespace PT.AI
{
    public class EnemyZone : MonoBehaviour
    {
        [SerializeField] private AIPedesterian[] _enemies;

        private int _count;
        private PlayerBikeController _player;
        private bool _isIn = false;

        private void OnEnable()
        {
            foreach(AIPedesterian enemy in _enemies)
            {
                enemy.OnDeath += RemoveEntity;
            }

            _count = _enemies.Length;
        }

        private void OnDestroy()
        {
            foreach (AIPedesterian enemy in _enemies)
            {
                enemy.OnDeath -= RemoveEntity;
            }
        }

        private void RemoveEntity()
        {
            _count--;
            if(_count <= 0 && _isIn)
            {
                _count = 0;
                try
                {
                    _player.DeactivateShootingMode();
                    TimeManager.Instance.GoNormal(0.5f);
                }
                catch { }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerBikeController pbc = other.GetComponent<PlayerBikeController>();
            if (pbc != null)
            {
                if (!_isIn)
                {
                    _isIn = true;
                    _player = pbc;
                    pbc.ActivateShootingMode();
                    TimeManager.Instance.SlowDown(0.5f, 0.1f);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (_isIn)
            {
                _isIn = false;
                _player.DeactivateShootingMode();
                TimeManager.Instance.GoNormal(0.5f);
            }
        }
    }
}