using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Utils
{
    public class DestroyAfter : MonoBehaviour
    {
        public bool callOnStart;
        public float lifeTime = 3f;

        public void DestroyAfterLifeTime()
        {
            Destroy(gameObject, lifeTime);
        }

        private void Start()
        {
            if (callOnStart)
            {
                DestroyAfterLifeTime();
            }
        }
    }
}