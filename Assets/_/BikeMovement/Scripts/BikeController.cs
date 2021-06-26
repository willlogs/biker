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
            transform.Rotate(0, diff.x * Time.deltaTime * _mdRotationSpeed, 0);

            if (hasInput)
            {
                Accelerate();
            }
            else
            {
                //DeAccelerate();
            }
        }

        public void Accelerate()
        {
            _rb.velocity += transform.forward * _acc;            

            if(_rb.velocity.magnitude > _maxSpeed)
            {
                _rb.velocity = _rb.velocity.normalized * _maxSpeed;
            }
        }

        public void DeAccelerate()
        {
            float diffuser = 0.95f;
            _rb.velocity = new Vector3(diffuser * _rb.velocity.x, _rb.velocity.y, diffuser * _rb.velocity.z);
        }

        #endregion


        #region privates

        [SerializeField] private Transform _steeringWheelT, _bikeBaseT;
        [SerializeField] private float _maxSpeed, _maxAngleDiff = 35, _rotationSpeed, _mdRotationSpeed = 20, _acc;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Animator _animator;

        private float _curSpeed;

        private void Start()
        {

        }

        private void OnEnable()
        {

        }

        private void Update()
        {
            AlignVelocity();
            RotateWheel();
        }

        private void AlignVelocity()
        {
            _rb.velocity = 0.95f * _rb.velocity + 0.05f * transform.forward * _rb.velocity.magnitude;
        }

        private void RotateWheel()
        {
            Vector3 noYVel = _rb.velocity;
            noYVel.y = 0;
            float qy = 0;

            if (noYVel.magnitude > 0.1f)
            {
                qy = Quaternion.FromToRotation(noYVel, transform.forward).y;
            }

            _steeringWheelT.localRotation = Quaternion.Lerp(
                    _steeringWheelT.localRotation,
                    Quaternion.Euler(
                        _steeringWheelT.localRotation.x,
                        _maxAngleDiff * qy * 3,
                        _steeringWheelT.localRotation.z
                    ),
                    Time.deltaTime * _rotationSpeed
            );

            _bikeBaseT.localRotation = Quaternion.Lerp(
                _bikeBaseT.localRotation,
                Quaternion.Euler(
                    _bikeBaseT.localRotation.x,
                    _bikeBaseT.localRotation.y,
                    -_maxAngleDiff * qy * 3
                ),
                Time.deltaTime * _rotationSpeed
            );
        }

        #endregion


        #region Gizmos

        private void OnDrawGizmos()
        {
            Vector3 start = transform.position + Vector3.up * 0.1f;

            Gizmos.DrawLine(start, start + transform.forward);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(start, start + _rb.velocity.normalized);
        }

        #endregion
    }
}