using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Utils
{
    public class TweenCopyPosition : MonoBehaviour
    {
        public bool follow = false;
        public Transform target;

        public float duration = 0.5f;

        public void Follow()
        {
            follow = true;
        }

        public void StopFollow()
        {
            follow = false;

            _tweener.Kill();
            _rotTweener.Kill();
        }

        private Tweener _tweener, _rotTweener;
        private bool _hasTweener = false;

        private void Update()
        {
            if (follow)
            {
                if (!_hasTweener)
                    _hasTweener = true;
                else
                {
                    _tweener.Kill();
                    _rotTweener.Kill();
                }

                _tweener = transform.DOMove(target.position, duration);
                _rotTweener = transform.DORotateQuaternion(target.rotation, duration);
            }
        }
    }
}