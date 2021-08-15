using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PT.Utils
{
    public class EndZone : MonoBehaviour
    {
        public LayerMask layerMask;

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.layer == 8)
            {
                TimeManager.Instance.DoWithDelay(1f, () =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                });
            }
        }
    }
}