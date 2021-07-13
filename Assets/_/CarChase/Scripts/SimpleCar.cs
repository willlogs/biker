using DG.Tweening;
using FluffyUnderware.Curvy.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.CarChase
{
    public class SimpleCar : MonoBehaviour
    {
        public void StopMoving(float duration)
        {
            DOTween.To(() => _splineController.Speed, (x) => { _splineController.Speed = x; }, 0, duration);
        }

        public void Crash()
        {
            _animator.SetTrigger("Crash");
        }

        [SerializeField] private Animator _animator;
        [SerializeField] private SplineController _splineController;
        [SerializeField] private Transform _splineFollowerT;
        [SerializeField] private float _followSpeed = 5;

        Transform _offsetKeeper;
        bool hasOffset = false;

        private void Start()
        {
            _splineFollowerT.parent = null;

            _splineController.Position = _splineController.Spline.GetNearestPointTF(transform.position, Space.World);
        }

        private void Update()
        {
            if (!hasOffset)
            {
                hasOffset = true;
                _offsetKeeper = new GameObject().transform;

                _offsetKeeper.rotation = transform.rotation;
                _offsetKeeper.position = transform.position;

                _offsetKeeper.parent = _splineFollowerT;
            }
        }

        private void FixedUpdate()
        {
            if (hasOffset)
            {
                transform.forward = _offsetKeeper.forward;
                float magDiff = (_splineFollowerT.position - transform.position).magnitude;

                if (magDiff < 50f)
                {
                    transform.position = Vector3.Lerp(transform.position, _offsetKeeper.position, Time.fixedDeltaTime * _followSpeed);
                }
                else
                {
                    transform.position = _offsetKeeper.position;
                }
            }
        }
    }
}