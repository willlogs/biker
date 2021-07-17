using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Utils;
using PT.AI;
using PT.Bike;
using DG.Tweening;

namespace PT.CarChase
{
    public class ChaseSection : MonoBehaviour
    {
        [SerializeField] private ChaserCar[] _chasers;
        [SerializeField] private Transform _chaserPlace;

        [SerializeField] private float _slowMoFactor;

        private bool _isIn = false;
        private PlayerBikeController _player;

        private int _curIndex = 0;

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

                    pbc.ActivateShootingMode();
                    pbc.transform.DOMoveX(transform.position.x, 0.5f).OnComplete(() =>
                    {
                        pbc.FollowTarget(transform);
                    });
                    pbc.transform.DORotateQuaternion(Quaternion.identity, 0.3f);

                    _player = pbc;

                    TimeManager.Instance.SlowDown(0.5f, _slowMoFactor);
                    ActivateSequence();
                }
            }
        }

        private void ActivateSequence()
        {
            if (_curIndex < _chasers.Length)
            {
                _chasers[_curIndex].gameObject.SetActive(true);
                _chasers[_curIndex].OnDestroy += ActivateSequence;
                _chasers[_curIndex].transform.DOMove(new Vector3(_chasers[_curIndex].transform.position.x, _chasers[_curIndex].transform.position.y, _chaserPlace.position.z), 2f).SetUpdate(true);

                _curIndex++;
            }
            else
            {
                // end section
                TimeManager.Instance.GoNormal(0.5f);
                _player.DeactivateShootingMode();
                _player.ContinueMoving();
            }
        }
    }
}