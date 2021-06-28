using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PT.GunPlay
{
    public class Gun : MonoBehaviour
    {
        public UnityEvent OnShoot;

        public void Shoot()
        {
            if (!_isCoolingDown)
            {
                GameObject go = Instantiate(_bulletPrefab);
                go.transform.position = _shootingPointT.position;
                go.GetComponent<Bullet>().GetShot(_gunTargetT.position - _shootingPointT.position);
                OnShoot?.Invoke();

                CoolDown();
            }
        }

        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private Transform _shootingPointT, _gunTargetT;
        [SerializeField] private float _coolDownTime = 0.4f;

        [SerializeField] private int _chunkSize = 1;
        [SerializeField] private float _spread = 0f;

        private bool _isCoolingDown = false;
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
                _coolDownTimer += Time.deltaTime;

                if(_coolDownTimer > _coolDownTime)
                {
                    _isCoolingDown = false;
                }
            }
        }
    }
}
