using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Rooftop
{
    public class HeliController : MonoBehaviour
    {
        public void StartFollowing()
        {
            _isFollowing = true;
            _thirdStage = false;
            _percentage = 0;
            BeforeDelay();
        }

        public void StopFollowing()
        {
            _isFollowing = false;
            _percentage = 0;
        }

        [SerializeField] private SplinePositioner _splinePositioner;
        [SerializeField] private float _reachTime, _delayTime, _tillStopTime, _followSpeed = 10f, _zOffset = 5;
        [SerializeField] private Transform _targetT;

        private bool _isFollowing = false, _thirdStage = false;
        private float _percentage = 0;

        private void Update()
        {
            if (_isFollowing)
            {
                _splinePositioner.SetPercent(_percentage);
                Vector3 targetP = _splinePositioner.transform.position;

                if(!_thirdStage)
                    targetP.z = _targetT.position.z + _zOffset;

                transform.position = Vector3.Lerp(transform.position, targetP, Time.deltaTime * _followSpeed);
            }
        }

        private void BeforeDelay()
        {
            DOTween.To(() => _percentage, (x) => { _percentage = x; }, 0.4f, _reachTime).OnComplete(() => {
                Delay();
            });
        }

        private void Delay()
        {
            DOTween.To(() => _percentage, (x) => { _percentage = x; }, 0.6f, _delayTime).OnComplete(() => {
                AfterDelay();
            });
        }

        private void AfterDelay()
        {
            _thirdStage = true;
            DOTween.To(() => _percentage, (x) => { _percentage = x; }, 1f, _tillStopTime);
        }
    }
}