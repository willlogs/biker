using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PT.Utils
{
    public class LevelManager : MonoBehaviour
    {
        private static LevelManager _instance;

        public static LevelManager Instance{
            get
            {
                return _instance;
            }
        }

        private void Start()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void Next()
        {

        }

        public void Restart()
        {

        }
    }
}