using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.AI
{
    public class RagdollManager : MonoBehaviour
    {
        public bool active = false, resetRotations = false;

        public void Activate()
        {
            foreach(Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
            }

            foreach (Collider cd in GetComponentsInChildren<Collider>())
            {
                cd.enabled = true;
                if (resetRotations && cd.name.Contains("Neck"))
                {
                    cd.transform.rotation = Quaternion.identity;
                }
            }
        }

        public void Deactivate()
        {
            foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = true;
            }

            foreach (Collider cd in GetComponentsInChildren<Collider>())
            {
                cd.enabled = false;
            }
        }

        private void Start()
        {
            if (active)
                Activate();
            else
                Deactivate();
        }
    }
}