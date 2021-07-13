using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PT.Utils
{
    public class CustomSlider : MonoBehaviour
    {
        [SerializeField] private Image _filler;

        public void SetValue(float value)
        {
            _filler.fillAmount = value;
        }
    }
}