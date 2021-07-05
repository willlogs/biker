using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Bike;
using PT.Utils;
using Dreamteck.Splines;
using DG.Tweening;

namespace PT.AI
{
    public class EnemyZone : MonoBehaviour
    {
        public SplineComputer spline;
        public float duration, slowmoFactor = 0.1f, endTimer = 4;

        [SerializeField] private AIPedesterian[] _enemies;
        [SerializeField] private Transform _targetTransform;

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
                    pbc._gunTargetT.DOMove(_targetTransform.position, 0.5f).SetUpdate(true);
                    TimeManager.Instance.SlowDown(0.5f, slowmoFactor);

                    /*BikeController bc = other.GetComponent<BikeController>();
                    if (bc != null)
                    {
                        FollowSpline(bc);
                    }*/
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            EndIt();
        }

        private void EndIt()
        {
            if (_isIn)
            {
                _isIn = false;
                _player.DeactivateShootingMode();
                TimeManager.Instance.GoNormal(0.5f);

                /*BikeController bc = other.GetComponent<BikeController>();
                if (bc != null)
                {
                    DontFollowSpline(bc);
                }*/
            }
        }

        private IEnumerator EndWithTimer()
        {
            yield return new WaitForSecondsRealtime(endTimer);
            EndIt();
        }

        private void FollowSpline(BikeController bc)
        {
            bc.FollowSpline(duration);
            bc.splineFollower.spline = spline;
            bc.splineFollower.transform.position = spline.GetPoint(0).position;
            bc.splineFollower.motion.offset = bc.transform.position - spline.GetPoint(0).position;
            bc.splineFollower.enabled = true;
            bc.DeactivateControl();
        }

        private static void DontFollowSpline(BikeController bc)
        {
            bc.DontFollowSpline();
            bc.splineFollower.spline = null;
            bc.splineFollower.enabled = false;
            bc.ActivateControl();
        }
    }
}