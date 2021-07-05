using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.GunPlay;
using RootMotion.FinalIK;

namespace PT.Bike
{
    [RequireComponent(typeof(BikeController))]
    [RequireComponent(typeof(GunController))]
    public class PlayerBikeController : MonoBehaviour
    {
        #region publics
        public void ActivateShootingMode()
        {
            _gun.gameObject.SetActive(true);
            _ik.solver.rightHandEffector.positionWeight = 0;
            _ik.solver.rightHandEffector.rotationWeight = 0;
            _isInShootingMode = true;
        }

        public void DeactivateShootingMode()
        {
            _gun.gameObject.SetActive(false);
            _ik.solver.rightHandEffector.positionWeight = 1;
            _ik.solver.rightHandEffector.rotationWeight = 1;
            _isInShootingMode = false;
        }
        #endregion

        #region privates
        [SerializeField] private bool _isInShootingMode = false, _testMode = false;

        [SerializeField] private FullBodyBipedIK _ik;
        [SerializeField] private Gun _gun;

        [SerializeField] private float _switchEach = 10f;

        private TouchInputManager _inputManager;
        private BikeController _bikeController;
        private GunController _gunController;

        private void Start()
        {
            _inputManager = TouchInputManager.Instance;
            _bikeController = GetComponent<BikeController>();
            _gunController = GetComponent<GunController>();

            if (_isInShootingMode)
                ActivateShootingMode();
            else
                DeactivateShootingMode();

            if(_testMode)
                StartCoroutine(SwitchModes());
        }

        private void Update()
        {
            if (_isInShootingMode)
            {
                _gunController.ApplyMovement(_inputManager.diff, _inputManager.hasInput);
                _bikeController.Steer(Vector3.zero, true);
            }
            else
            {
                _bikeController.Steer(_inputManager.diff, _inputManager.hasInput);                
            }
        }

        private void ToggleMode()
        {
            if (_isInShootingMode)
            {
                DeactivateShootingMode();
            }
            else
            {
                ActivateShootingMode();
            }
        }

        private IEnumerator SwitchModes()
        {
            while (true)
            {
                yield return new WaitForSeconds(_switchEach);
                ToggleMode();
            }
        }
        #endregion privates
    }
}