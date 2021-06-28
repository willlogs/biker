using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Utils
{
    [RequireComponent(typeof(Rigidbody))]
    public class PhysicalObject : MonoBehaviour
    {
        protected Rigidbody _rb;

        protected virtual void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }
    }
}
