using DG.Tweening;
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

        private bool _isMoving;
        private Tweener _currTweener, _rotationTweener;

        private int _wpIndex = 0;
        private float _baseHealth;

        private void Start()
        {
            _baseHealth = _health;
            StartMoving();   
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
            Destroy(_animator);
            Destroy(_rb);
            _ragdoll.Activate();
            _mainCollider.enabled = false;
            _rotationTweener.Kill();
            _currTweener.Kill();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if(collision.gameObject.layer == 9)
            {
                Hurt();
            }
        }
    }
}