using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Utils
{
    public class TargetFPS : MonoBehaviour
    {
        public int fps = 60;

        private void Awake()
        {
            Application.targetFrameRate = fps;
        }
    }
}