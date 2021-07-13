using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.GunPlay
{
    public class GunController : MonoBehaviour
    {
        #region publics
        public LayerMask aimLayerMask;

        Ray r;
        public void ApplyMovement(Vector3 diff, bool hasInput)
        {
            if (diff.magnitude > 0)
            {
                /*Vector3 trueDiff = new Vector3(-diff.y, diff.x);*/
                /*_gunAimingTarget.RotateAround(Camera.main.transform.position, trueDiff.normalized, diff.magnitude * Time.unscaledDeltaTime * _targetMovementSpeed);*/
                /*_gunAimingTarget.Translate((Camera.main.transform.up * diff.y + Camera.main.transform.right * diff.x) * Time.unscaledDeltaTime * _targetMovementSpeed);*/

                /*float _speedfactor = Time.unscaledDeltaTime * _targetMovementSpeed;
                Quaternion targetRotation = new Quaternion(trueDiff.x * _speedfactor, trueDiff.y * _speedfactor, 0, 1) * _aimPivotT.rotation;
                _aimPivotT.rotation = Quaternion.Lerp(_aimPivotT.rotation, targetRotation, Time.unscaledDeltaTime * 5f);*/

                Vector2 change = (Vector2)(diff * Time.fixedDeltaTime * _targetMovementSpeed);
                _aimUI.anchoredPosition += change;

                _aimUI.anchoredPosition = new Vector2(
                        Mathf.Clamp(_aimUI.anchoredPosition.x, -halfWidth, halfWidth),
                        Mathf.Clamp(_aimUI.anchoredPosition.y, -halfHeight, halfHeight)
                );

                Vector2 sp = _aimUI.anchoredPosition + Vector2.up * halfHeight + Vector2.right * halfWidth;
                r = Camera.main.ScreenPointToRay(sp);
                RaycastHit info;
                if (Physics.Raycast(r, out info, 50, aimLayerMask))
                {
                    _gunAimingTarget.position = info.point;
                }
                else
                {
                    _gunAimingTarget.position = r.origin + r.direction * 10;
                }
            }

            if (hasInput)
            {
                Shoot();
            }
        }

        #endregion

        #region privates
        [SerializeField] private RectTransform _aimUI;

        [SerializeField] private bool _isInGunPlayMode = false;

        [SerializeField] private Transform _gunAimingTarget, _aimPivotT;
        [SerializeField] private float _targetMovementSpeed = 5;

        [SerializeField] private Gun _gun;

        private float halfWidth, halfHeight;

        private void Start()
        {
            halfWidth = Screen.width * 0.5f;
            halfHeight = Screen.height * 0.5f;
        }

        private void Shoot()
        {
            _gun.Shoot();
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(r.origin, r.origin + r.direction * 3);
        }

        #endregion
    }
}