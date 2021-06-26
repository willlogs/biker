using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    public class TouchInputManager : MonoBehaviour
    {
        #region statics

        public static TouchInputManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject().AddComponent<TouchInputManager>();
                }

                return instance;
            }
        }

        private static TouchInputManager instance;

        #endregion

        #region publics

        public Vector3 diff;
        public bool hasInput = false;

        #endregion

        #region privates
        [SerializeField] private float _multiplier = 1;

        private Vector3 _lastInput;
        private bool _hasLastInput;

        private void Start()
        {
            _hasLastInput = false;
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                if (_hasLastInput)
                {
                    hasInput = true;
                    Vector3 currMouseVP = GetMouseVP();
                    diff = (currMouseVP - _lastInput) * _multiplier;
                    _lastInput = currMouseVP;
                }
                else
                {
                    _hasLastInput = true;
                    _lastInput = GetMouseVP();
                }
            }
            else if(_hasLastInput)
            {
                hasInput = false;
                diff = Vector3.zero;
                _hasLastInput = false;
            }
        }

        private static Vector3 GetMouseVP()
        {
            return Camera.main.ScreenToViewportPoint(Input.mousePosition);
        }

        #endregion
    }
}