using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.GunPlay
{
    public class GunController : MonoBehaviour
    {
        #region publics

        public void ApplyMovement(Vector3 diff, bool hasInput)
        {
            if(diff.magnitude > 0)
            {
                _gunAimingTarget.Translate(diff * Time.unscaledDeltaTime * _targetMovementSpeed);
            }

            if (hasInput)
            {
                Shoot();
            }
        }

        #endregion

        #region privates

        [SerializeField] private bool _isInGunPlayMode = false;

        [SerializeField] private Transform _gunAimingTarget;
        [SerializeField] private float _targetMovementSpeed = 5;

        [SerializeField] private Gun _gun;

        private void Shoot()
        {
            _gun.Shoot();
        }

        #endregion
    }
}