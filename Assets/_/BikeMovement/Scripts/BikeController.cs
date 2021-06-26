using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    public class BikeController : MonoBehaviour
    {
        #region publics

        public void Steer(Vector3 diff, bool hasInput)
        {
            //float currDegree = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg;
            float currDegree = Vector3.SignedAngle(_moveDirection, transform.forward, Vector3.up);

            if ((diff.x > 0 && _maxAngleDiff > currDegree) || (diff.x < 0 && -_maxAngleDiff < currDegree))
            {
                _moveDirection = Quaternion.Euler(0, _mdRotationSpeed * diff.x, 0) * _moveDirection;
            }

            if (hasInput)
            {
                Accelerate();
            }
        }

        public void Accelerate()
        {
            _rb.velocity += transform.forward;

            float y = _rb.velocity.y;
            _rb.velocity = (0.9f * _rb.velocity) + (0.1f * transform.forward * _rb.velocity.magnitude) + Vector3.down * y;

            if(_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxSpeed;
            }
        }

        public void DeAccelerate()
        {
            _rb.velocity *= 0.5f;
        }

        #endregion


        #region privates

        [SerializeField] private Transform _steeringWheelT, _bikeBaseT;
        [SerializeField] private float _maxSpeed, _maxAngleDiff = 35, _rotationSpeed, _mdRotationSpeed = 20;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Animator _animator;
        [SerializeField] private AnimationCurve _diffEvaluator;

        private Vector3 _moveDirection;
        private float _curSpeed;

        private void Start()
        {
            
        }

        private void OnEnable()
        {
            _moveDirection = Vector3.forward;
        }

        private void Update()
        {
            RotateInDirection();
        }

        private void RotateInDirection()
        {
            float degree = Vector3.SignedAngle(_moveDirection, transform.forward, Vector3.up);
            float diffMagnitude = (transform.forward - _moveDirection).magnitude;

            _steeringWheelT.localRotation = Quaternion.Lerp(_steeringWheelT.localRotation, Quaternion.Euler(
                _steeringWheelT.localRotation.eulerAngles.x,
                -degree,
                _steeringWheelT.localRotation.eulerAngles.z
            ), Time.deltaTime * _rotationSpeed);


            if (diffMagnitude > 0.1f)
            {
                transform.forward = Vector3.Lerp(transform.forward, _moveDirection, Time.deltaTime * _rotationSpeed);
                /*transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(
                    transform.localRotation.x,
                    transform.localRotation.y - degree,
                    transform.localRotation.z
                ), Time.deltaTime * _rotationSpeed * 0.5f);*/
            }
        }

        #endregion


        #region Gizmos

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.black;
            Vector3 start = transform.position + Vector3.up * 0.1f;
            Gizmos.DrawLine(start, start + _moveDirection);

            Gizmos.color = Color.white;
            Gizmos.DrawLine(start, start + transform.forward);
        }

        #endregion
    }
}