using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.Utils;

namespace PT.GunPlay
{
    public class Bullet : PhysicalObject
    {
        #region publics
        public void GetShot(Vector3 direction)
        {
            transform.forward = direction.normalized;
            _rb.velocity = direction.normalized * _speed;
        }

        public void GetShot(Vector3 direction, Transform goal)
        {
            _isFollowing = true;
            _goal = goal;
            transform.forward = direction.normalized;
            _rb.velocity = direction.normalized * _speed;
        }
        #endregion

        #region protecteds
        [SerializeField] protected float _speed = 20;
        #endregion

        #region privates
        [SerializeField] private bool _isTesting;
        [SerializeField] private GameObject _impactPrefab;

        [SerializeField] private Transform _goal;
        private bool _isFollowing;

        private void Start()
        {
            if (_isTesting)
            {
                GetShot(transform.forward);
            }
        }

        private void Update()
        {
            if (_isFollowing)
            {
                Vector3 goalPos = _goal.position;
                Vector3 direction = (goalPos - transform.position).normalized;
                
                _rb.velocity = direction * _rb.velocity.magnitude;
                transform.forward = direction;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            GameObject go = Instantiate(_impactPrefab);
            go.transform.position = transform.position;
            Destroy(gameObject);
        }
        #endregion
    }
}
