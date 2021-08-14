using DG.Tweening;
using FluffyUnderware.Curvy.Controllers;
using PT.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    public class BikeController : MonoBehaviour
    {
        #region publics
        public SplineController splineFollower;
        public bool _autoAccelerate;

        public void Steer(Vector3 diff, bool hasInput)
        {
            if (_canControl && _grounded)
            {
                float rotationMultiplier = _mdRotationCurve.Evaluate(Mathf.Abs(diff.x));
                _steeringWheelT.localRotation = Quaternion.Lerp(
                        _steeringWheelT.localRotation,
                        Quaternion.Euler(
                            _steeringWheelT.localRotation.x,
                            _maxAngleDiff * rotationMultiplier * Mathf.Sign(diff.x),
                            _steeringWheelT.localRotation.z
                        ),
                        Time.fixedDeltaTime * _rotationSpeed
                );

                float newVel = Mathf.Clamp(diff.x * _mdRotationSpeed, -_angularVelocityCap, _angularVelocityCap);
                _angularVelocity = _angularVelocity * (1 - _angularInputFactor) + newVel * _angularInputFactor;
                _rb.angularVelocity = new Vector3(_rb.angularVelocity.x, _angularVelocity * _mdRotationSpeed, _rb.angularVelocity.z);

                if (hasInput)
                {
                    if (!_isSpeeding)
                    {
                        _isSpeeding = true;
                        try
                        {
                            _bikeMaxSpeedTweener.Kill();
                        }
                        catch { }

                        _bikeMaxSpeedTweener = DOTween.To(() => _maxSpeed, (x) => { _maxSpeed = x; }, 30, 0.5f);
                    }

                    Accelerate();
                    AlignVelocity();
                }
                else
                {
                    if (_isSpeeding)
                    {
                        _isSpeeding = false;
                        try
                        {
                            _bikeMaxSpeedTweener.Kill();
                        }
                        catch { }

                        _bikeMaxSpeedTweener = DOTween.To(() => _maxSpeed, (x) => { _maxSpeed = x; }, 20, 0.5f);
                    }

                    Accelerate();
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
            splineFollower.Position = 0;

            _positionTween = DOTween.To(() => splineFollower.Position, (x) =>{ splineFollower.Position = x;  }, 1f, _splineDuration).SetEase(Ease.Linear);
        }

        public void DontFollowSpline()
        {
            _followingSpline = false;
            _rb.isKinematic = false;

            _splineFollowerT.position = transform.position;
            _splineFollowerT.parent = transform;

            _rb.velocity = transform.forward * _maxSpeed;
            splineFollower.Position = 0;
            _positionTween.Kill();
        }

        public void Crash(Vector3 point)
        {
            Vector3 impactDir = point - transform.position;
            impactDir = _cameraT.forward;

            foreach(MonoBehaviour obj in _ikSolvers)
            {
                obj.enabled = false;
            }

            foreach(Collider c in _helperColliders)
            {
                c.enabled = false;
            }

            _playerPolyT.parent = null;

            _corpseRenderer.transform.parent = null;
            _corpseRenderer.SetActive(true);
            _characterRagdoll.transform.parent = null;
            _characterRagdoll.gameObject.SetActive(true);
            _characterRagdoll.Activate();
            _dummyMan.SetActive(false);

            _rb.angularDrag = 0;
            _rb.angularVelocity = new Vector3(
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f),
                Random.Range(-1f, 1f)
            ).normalized * 10;
            _rb.velocity = impactDir * 10 + Vector3.up * 10;
            _ragdollBaseRB.velocity = _rb.velocity * 3;

            _cameraT.parent = null;
            _cameraT.DOMove(point + transform.up * 3 + transform.forward * -3, 1f);

            Destroy(gameObject.GetComponent<PlayerBikeController>());
            Destroy(this);
        }

        #endregion


        #region privates

        [SerializeField] private Transform _steeringWheelT, _bikeBaseT, _centerOfMass, _splineFollowerT, _groundRayStartT;
        [SerializeField] private float _maxSpeed, _maxAngleDiff = 35, _rotationSpeed, _mdRotationSpeed = 20, _acc, _splineDuration = 5, _angularVelocityAgeFactor = 0.1f, _angularVelocityCap = 10f, _angularInputFactor = 0.1f;
        [SerializeField] private Rigidbody _rb, _followerRB, _ragdollBaseRB;
        [SerializeField] private Animator _animator;
        [SerializeField] private LayerMask _groundDetectorMask;
        [SerializeField] private bool _grounded = false;
        [SerializeField] private AnimationCurve _mdRotationCurve;

        [SerializeField] private Transform _playerPolyT, _cameraT;

        [SerializeField] private RagdollManager _characterRagdoll;
        [SerializeField] private GameObject _dummyMan, _corpseRenderer;


        [SerializeField] private MonoBehaviour[] _ikSolvers;
        [SerializeField] private Collider[] _helperColliders;

        private bool _canControl = true, _followingSpline;
        private Tweener _positionTween;

        private float _zQAngularVelocity;
        private float _angularVelocity;

        private Tweener _bikeMaxSpeedTweener;
        private bool _isSpeeding = true;

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

            _rb.angularVelocity = new Vector3(_rb.angularVelocity.x, _rb.angularVelocity.y, _rb.angularVelocity.z - _zQAngularVelocity);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == 12)
            {
                Crash(collision.GetContact(0).point);
            }
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