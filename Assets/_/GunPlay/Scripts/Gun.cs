using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PT.GunPlay
{
    public class Gun : MonoBehaviour
    {
        public UnityEvent OnShoot;

        public void Shoot(bool withTarget = true)
        {
            if (!_isCoolingDown)
            {
                GameObject go = Instantiate(_bulletPrefab);
                go.transform.position = _shootingPointT.position;

                if (withTarget)
                {
                    RaycastHit hitInfo;
                    Physics.Raycast(_shootingPointT.position, _gunTargetT.position - _shootingPointT.position, out hitInfo, 50, layerMask, QueryTriggerInteraction.Ignore);
                    if (hitInfo.collider != null)
                    {
                        GameObject goalGO = new GameObject();
                        goalGO.transform.position = hitInfo.point;
                        goalGO.transform.parent = hitInfo.collider.transform;

                        go.GetComponent<Bullet>().GetShot(
                            _gunTargetT.position - _shootingPointT.position,
                            goalGO.transform
                        );
                    }
                    else
                    {
                        go.GetComponent<Bullet>().GetShot(
                            _gunTargetT.position - _shootingPointT.position
                        );
                    }
                }
                else
                {
                    go.GetComponent<Bullet>().GetShot(
                        _gunTargetT.position - _shootingPointT.position
                    );
                }
                OnShoot?.Invoke();

                CoolDown();
            }
        }

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _shootingPointT, _gunTargetT;
        [SerializeField] private float _coolDownTime = 0.4f;

        [SerializeField] private int _chunkSize = 1;
        [SerializeField] private float _spread = 0f;
        [SerializeField] private Rigidbody _parentRB;
        [SerializeField] private bool _unscaled = false;

        [SerializeField] private LayerMask layerMask;

        private bool _isCoolingDown = true;
        private float _coolDownTimer;

        private void CoolDown()
        {
            _coolDownTimer = 0f;
            _isCoolingDown = true;
        }

        private void Update()
        {
            if (_isCoolingDown)
            {
                if (_unscaled)
                {
                    _coolDownTimer += Time.unscaledDeltaTime;
                }
                else
                {
                    _coolDownTimer += Time.deltaTime;
                }

                if(_coolDownTimer > _coolDownTime)
                {
                    _isCoolingDown = false;
                }
            }
        }
    }
}
