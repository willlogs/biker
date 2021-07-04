using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Utils
{
    public class TimeManager : MonoBehaviour
    {
        private static TimeManager _instance;

        public static TimeManager Instance{
            get
            {
                if(_instance == null)
                {
                    _instance = new GameObject().AddComponent<TimeManager>();
                }

                return _instance;
            }
        }

        private void Awake()
        {
            _instance = this;
        }

        private bool _isSlow = false;

        public void SlowDown(float duration)
        {
            if (!_isSlow)
            {
                _isSlow = true;
                if (duration == 0)
                {
                    SetSpeed(0.3f);
                }
                else
                {
                    SetSpeedTweener(duration, 0.3f);
                }
            }
        }

        public void GoNormal(float duration = 0)
        {
            if (_isSlow)
            {
                _isSlow = false;
                if (duration == 0)
                {
                    SetSpeed(1f);
                }
                else
                {
                    SetSpeedTweener(duration, 1f);
                }
            }
        }

        private Tweener _scaleTweener, _fixedDTTweener;

        private void SetSpeedTweener(float duration, float ts)
        {
            try
            {
                _scaleTweener.Kill();
                _fixedDTTweener.Kill();
            }
            catch { }

            _scaleTweener = DOTween.To(() => Time.timeScale, (x) => { Time.timeScale = x; }, ts, duration).SetUpdate(true);
            _fixedDTTweener = DOTween.To(() => Time.fixedDeltaTime, (x) => { Time.fixedDeltaTime = x; }, ts * 0.01f, duration).SetUpdate(true);
        }

        private void SetSpeed(float ts)
        {
            Time.timeScale = ts;
            Time.fixedDeltaTime = 0.01f * ts;
        }
    }
}