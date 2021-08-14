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
        [SerializeField] private Transform _chaserPlace, _targetT;

        [SerializeField] private float _slowMoFactor;

        private bool _isIn = false, _placing = false, _placed = false;
        private PlayerBikeController _player;

        private int _curIndex = 0;
        private float _placeTime = 0;

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
                    
                    pbc.transform.DOMoveX(transform.position.x, 0.2f).OnComplete(() =>
                    {
                        pbc.FollowTarget(transform);
                        pbc.ActivateShootingMode(_targetT);
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
                _chasers[_curIndex].transform.DOLocalMove(new Vector3(_chasers[_curIndex].transform.localPosition.x, _chasers[_curIndex].transform.localPosition.y, _chaserPlace.localPosition.z), 1.5f).SetUpdate(true).OnComplete(() => {
                    //_chasers[_curIndex].transform.DOMoveX(0, 0.5f);
                });

                /*_placing = true;
                _placed = false;
                _placeTime = 0;*/

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

        private void Update()
        {
            /*if(_placing && !_placed)
            {
                Vector3 targetPlace = new Vector3(_chaserPlace.transform.position.x, _chasers[_curIndex].transform.position.y, _chaserPlace.position.z);
                _chasers[_curIndex].transform.position = Vector3.Lerp(_chasers[_curIndex].transform.position, targetPlace, _placeTime);
                _placeTime += Time.unscaledDeltaTime / 2f;

                if((_chasers[_curIndex].transform.position - targetPlace).magnitude < 0.5f)
                {
                    _placed = true;
                }
            }*/
        }
    }
}