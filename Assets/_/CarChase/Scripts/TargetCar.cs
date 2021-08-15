using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Utils;
using PT.AI;
using PT.Bike;
using UnityEngine.SceneManagement;

namespace PT.CarChase
{
    public class TargetCar : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private Transform[] _trace;
        [SerializeField] private float _slowMoFactor;
        [SerializeField] private GameObject _zoneEffect;

        private bool _isIn = false;
        private PlayerBikeController _player;

        private static int _count = 0;

        public void WinSituation()
        {
            TimeManager.Instance.DoWithDelay(3f, () =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            });
        }

        public void OnCrash()
        {
            _count--;
            if(_count <= 0)
            {
                WinSituation();
            }
            else{
                _player.DeactivateShootingMode();
                _player.ContinueMoving();
                TimeManager.Instance.GoNormal();
            }
        }

        private void Start()
        {
            _count++;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!_isIn && other.gameObject.layer == 8)
            {
                _zoneEffect.SetActive(false);
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