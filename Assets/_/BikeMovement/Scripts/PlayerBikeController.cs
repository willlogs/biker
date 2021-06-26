using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PT.Bike
{
    [RequireComponent(typeof(BikeController))]
    public class PlayerBikeController : MonoBehaviour
    {
        private TouchInputManager _inputManager;
        private BikeController _bikeController;

        private void Start()
        {
            _inputManager = TouchInputManager.Instance;
            _bikeController = GetComponent<BikeController>();
        }

        private void Update()
        {
            _bikeController.Steer(_inputManager.diff, _inputManager.hasInput);

            HandleTestInput();
        }

        private void HandleTestInput()
        {

        }
    }
}