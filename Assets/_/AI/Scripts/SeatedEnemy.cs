using PT.CarChase;
using PT.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.AI
{
    public class SeatedEnemy : MonoBehaviour
    {
        public void Damage()
        {
            _health -= 40;
            if (_health <= 0)
            {
                _health = 0;
                Die();
            }

            if(_slider != null)
                _slider.SetValue(_health/100);
        }

        [SerializeField] private float _health = 100;
        [SerializeField] private SimpleCar _car;
        [SerializeField] private CustomSlider _slider;

        private void Die()
        {
            _car.Crash();
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