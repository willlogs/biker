using DG.Tweening;
using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    public class BikeController : MonoBehaviour
    {
        #region publics
        public SplinePositioner splineFollower;

        public void Steer(Vector3 diff, bool hasInput)
        {
            if (_canControl && _grounded)
            {
                //transform.Rotate(0, diff.x * Time.deltaTime * _mdRotationSpeed, 0);
                Quaternion before = transform.rotation;
                before = new Quaternion(0, diff.x, 0, 1) * before;
                transform.rotation = Quaternion.Lerp(transform.rotation, before, Time.deltaTime * _mdRotationSpeed);

                if (hasInput)
                {
                    Accelerate();
                    AlignVelocity();
                }
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

        public void DeactivateControl()
        {
            _canControl = false;
        }

        public void ActivateControl()
        {
            _canControl = true;
        }

        public void FollowSpline()
        {
            _followingSpline = true;
            _rb.isKinematic = true;
            _splineFollowerT.parent = null;
            _position = 0;

            _positionTween = DOTween.To(() => _position, (x) =>{ _position = x;  }, 1f, _splineDuration).SetEase(Ease.Linear);
        }

        public void DontFollowSpline()
        {
            _followingSpline = false;
            _rb.isKinematic = false;

            try
            {
                Destroy(splineFollower.spline.gameObject);
                Destroy(splineFollower);
            }
            catch { }

            splineFollower = _splineFollowerT.gameObject.AddComponent<SplinePositioner>();

            _splineFollowerT.position = transform.position;
            _splineFollowerT.parent = transform;

            _rb.velocity = transform.forward * 10f;
            _position = 0;
            _positionTween.Kill();
        }

        #endregion


        #region privates

        [SerializeField] private Transform _steeringWheelT, _bikeBaseT, _centerOfMass, _splineFollowerT, _groundRayStartT;
        [SerializeField] private float _maxSpeed, _maxAngleDiff = 35, _rotationSpeed, _mdRotationSpeed = 20, _acc, _splineDuration = 5, _angularVelocityAgeFactor = 0.1f;
        [SerializeField] private Rigidbody _rb, _followerRB;
        [SerializeField] private Animator _animator;
        [SerializeField] private LayerMask _groundDetectorMask;
        [SerializeField] private bool _grounded = false;

        private float _curSpeed, _position = 0;
        private bool _canControl = true, _followingSpline;
        private Tweener _positionTween;

        private float _zQAngularVelocity;

        private void Start()
        {
            _rb.centerOfMass = _centerOfMass.localPosition;
        }

        private void OnEnable()
        {

        }

        private void Update()
        {
            Ray r = new Ray(_groundRayStartT.position, Vector3.down);
            RaycastHit info;
            Physics.Raycast(r, out info, 0.5f, _groundDetectorMask, QueryTriggerInteraction.Ignore);
            _grounded = info.collider != null;

            _zQAngularVelocity = (1 - _angularVelocityAgeFactor) * _zQAngularVelocity + _angularVelocityAgeFactor * transform.rotation.z;

            RotateWheel();

            if (_followingSpline)
            {
                splineFollower.SetPercent(_position);
                transform.position = Vector3.Lerp(transform.position, _splineFollowerT.position, Time.deltaTime * 10);
                transform.rotation = Quaternion.Lerp(transform.rotation, _splineFollowerT.rotation, Time.deltaTime * 10);
                _rb.velocity = _followerRB.velocity;
            }
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

            print(_zQAngularVelocity);
            _rb.angularVelocity = new Vector3(_rb.angularVelocity.x, _rb.angularVelocity.y, _rb.angularVelocity.z - _zQAngularVelocity);
            /*_bikeBaseT.localRotation = Quaternion.Lerp(
                _bikeBaseT.localRotation,
                Quaternion.Euler(
                    _bikeBaseT.localRotation.x,
                    _bikeBaseT.localRotation.y,
                    -_maxAngleDiff * qy * 3
                ),
                Time.deltaTime * _rotationSpeed
            );*/
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