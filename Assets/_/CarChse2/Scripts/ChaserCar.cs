using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.CarChase
{
    public class ChaserCar : SimpleCar
    {
        public event Action OnDestroy;

        [SerializeField] private GameObject _explosionEffect;

        [SerializeField] private float _health = 200;

        private bool _isdead = false;

        public void Damage()
        {
            _health -= 30;
            if (_health <= 0)
            {
                _health = 0;
                Explode();
            }
        }

        public override void Crash()
        {
            Explode();
        }

        private void Explode()
        {
            if (!_isdead)
            {
                _isdead = true;
                GameObject go = Instantiate(_explosionEffect);
                go.transform.position = transform.position;

                _animator.SetTrigger("Crash");
                OnDestroy?.Invoke();
                transform.parent = null;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == 9)
            {
                Damage();
            }
        }
    }
}