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
        #endregion

        #region protecteds
        [SerializeField] protected float _speed = 20;
        #endregion

        #region privates
        [SerializeField] private bool _isTesting;

        private void Start()
        {
            if (_isTesting)
            {
                GetShot(transform.forward);
            }
        }
        #endregion
    }
}
