using FluffyUnderware.Curvy.Controllers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.CarChase
{
    public class SimpleCar : MonoBehaviour
    {
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
            else
            {
                transform.forward = _offsetKeeper.forward;

                Vector3 newPos = _offsetKeeper.position;
                float magDiff = (_splineFollowerT.position - transform.position).magnitude;

                if (magDiff < 50f)
                {
                    transform.position = Vector3.Lerp(transform.position, _offsetKeeper.position, Time.deltaTime * _followSpeed);
                }
                else
                {
                    transform.position = _offsetKeeper.position;
                }
            }
        }
    }
}