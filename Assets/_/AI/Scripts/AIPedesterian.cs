using DG.Tweening;
using PT.GunPlay;
using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.AI
{
    public class AIPedesterian : MonoBehaviour
    {
        public Transform[] waypointsT;

        [SerializeField] private float _waitBetweenWPs = 5, _movingSpeed = 4, _turnDuration = 1f, _health = 10f;
        [SerializeField] private Animator _animator;
        [SerializeField] private RagdollManager _ragdoll;
        [SerializeField] private Collider _mainCollider;
        [SerializeField] private Rigidbody _rb;
        [SerializeField] private Transform _targetT, _aimTarget;
        [SerializeField] private AimIK _aimIK;
        [SerializeField] private Gun _gun;

        private bool _isMoving, _aiming;
        private Tweener _currTweener, _rotationTweener;

        private int _wpIndex = 0;
        private float _baseHealth;

        private void Start()
        {
            _baseHealth = _health;
            StartMoving();

            if (!_aiming)
            {
                _aimIK.enabled = false;
            }
        }

        private void StartMoving()
        {
            if (waypointsT.Length > 0)
            {
                _wpIndex = GotoNext(_wpIndex);
            }
        }

        private IEnumerator IdleDelayAndGo()
        {
            _animator.SetBool("Walk", false);
            yield return new WaitForSeconds(_waitBetweenWPs);
            _wpIndex = GotoNext(_wpIndex);
        }

        private int GotoNext(int i)
        {
            _animator.SetBool("Walk", true);
            float duration = (waypointsT[i].position - transform.position).magnitude / _movingSpeed;
            _rotationTweener = transform.DORotateQuaternion(Quaternion.LookRotation((waypointsT[i].position - transform.position).normalized, transform.up), _turnDuration);
            _currTweener = transform.DOMove(waypointsT[i++].position, duration).SetEase(Ease.Linear).OnComplete(() => { StartCoroutine(IdleDelayAndGo()); });
            i %= waypointsT.Length;
            return i;
        }

        private void Hurt()
        {
            Die();
        }

        private void Die()
        {
            _ragdoll.Activate();
            _mainCollider.enabled = false;
            _rotationTweener.Kill();
            _currTweener.Kill();
            _gun.transform.parent = null;
            _gun.gameObject.AddComponent<Rigidbody>();
            _gun.gameObject.AddComponent<BoxCollider>();
            _aimIK.enabled = false;

            Destroy(_animator);
            Destroy(_rb);
            Destroy(gameObject, 5);
            Destroy(this);
        }

        private void Update()
        {
            if (_aiming)
            {
                _aimTarget.position = _targetT.position + Vector3.up * 1.5f;
                _gun.Shoot(false);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            bool _smash = false;

            switch (collision.gameObject.layer)
            {
                case 9: Hurt(); break;
                case 8: Die(); _smash = true; break;
            }

            if (_smash) {
                foreach(Collider c in Physics.OverlapSphere(collision.GetContact(0).point, 0.4f))
                {
                    if(c.gameObject.layer == 6)
                        try
                        {
                            c.GetComponent<Rigidbody>().velocity = (Vector3.forward + Vector3.up).normalized * 20;
                        }
                        catch { }
                }                
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == 8)
            {
                _aiming = true;
                _targetT = other.transform;
                _animator.SetBool("Walk", false);
                _animator.SetBool("Aim", true);
                _aimIK.enabled = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == 8)
            {
                _aiming = false;
                _animator.SetBool("Aim", false);
                _aimIK.enabled = false;
            }
        }
    }
}