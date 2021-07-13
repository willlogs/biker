using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PT.Utils
{
    public class OnCollisionEvent : MonoBehaviour
    {
        public LayerMask layerMask;

        public UnityEvent<Collision> OnEnter, OnExit;

        private void OnCollisionEnter(Collision collision)
        {
            int layerTest = layerMask.value & (1 << collision.gameObject.layer);
            if (layerTest > 0){
                OnEnter?.Invoke(collision);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            int layerTest = layerMask.value & (1 << collision.gameObject.layer);
            if (layerTest > 0)
            {
                OnExit?.Invoke(collision);
            }
        }
    }
}