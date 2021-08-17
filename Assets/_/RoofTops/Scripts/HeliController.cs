using DG.Tweening;
using PT.Utils;
using FluffyUnderware.Curvy.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Rooftop
{
    public class HeliController : MonoBehaviour
    {
        public void StartFollowing(Transform target)
        {
            if (!_isFollowing)
            {
                _isFollowing = true;
                _thirdStage = false;
                _splinePositioner.Position = 0;
                _targetT = target;
                BeforeDelay();
            }
        }

        public void StopFollowing()
        {
            if (_isFollowing)
            {
                _isFollowing = false;
                _splinePositioner.Position = 0;
            }
        }

        [SerializeField] private SplineController _splinePositioner;
        [SerializeField] private float _reachTime, _delayTime, _tillStopTime, _followSpeed = 10f, _zOffset = 5;
        [SerializeField] private Transform _targetT;
        [SerializeField] private GameObject _smokePrefab, _explosionPrefab;
        [SerializeField] private Material _destroyedMat;
        [SerializeField] private Renderer _renderer;
        [SerializeField] private bool _isLast = false, _canBeDestroyed = false;
        [SerializeField] private int _shotsBeforeDestruction = 3;

        private bool _isFollowing = false, _thirdStage = false, _isDead = false;

        private Tweener _tweener;

        private void Update()
        {
            if (_isFollowing)
            {
                Vector3 targetP = _splinePositioner.transform.position;

                if(!_thirdStage)
                    targetP.z = _targetT.position.z + _zOffset;
                else
                {
                    targetP.z = transform.position.z;
                }

                transform.position = Vector3.Lerp(transform.position, targetP, Time.deltaTime * _followSpeed);
            }
        }

        private void BeforeDelay()
        {
            _tweener = DOTween.To(() => _splinePositioner.Position, (x) => { _splinePositioner.Position = x; }, 0.4f, _reachTime).OnComplete(() => {
                Delay();
            });
        }

        private void Delay()
        {
            _tweener = DOTween.To(() => _splinePositioner.Position, (x) => { _splinePositioner.Position = x; }, 0.6f, _delayTime).OnComplete(() => {
                AfterDelay();
            });
        }

        private void AfterDelay()
        {
            _thirdStage = true;
            _tweener = DOTween.To(() => _splinePositioner.Position, (x) => { _splinePositioner.Position = x; }, 1f, _tillStopTime).OnComplete(() =>
            {
                Destroy(gameObject);
            });
        }

        private int _shotsReceived = 0;
        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == 9 && !_isDead)
            {
                _shotsReceived++;
                GameObject go = Instantiate(_smokePrefab);
                go.transform.position = collision.GetContact(0).point;
                go.transform.parent = transform;

                if(_shotsReceived > _shotsBeforeDestruction)
                {
                    BlowUp();
                }
            }
        }

        private void BlowUp()
        {
            _thirdStage = true;
            TimeManager.Instance.GoNormal(0.5f);

            if (_canBeDestroyed)
            {
                _isDead = true;
                _isFollowing = false;
                GameObject go = Instantiate(_explosionPrefab);
                go.transform.position = transform.position;
                _renderer.material = _destroyedMat;
                gameObject.AddComponent<Rigidbody>();
            }
        }
    }
}