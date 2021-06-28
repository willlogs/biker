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
        [SerializeField] private bool _isInShootingMode = false;

        [SerializeField] private FullBodyBipedIK _ik;
        [SerializeField] private Gun _gun;

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
        }

        private void Update()
        {
            if (_isInShootingMode)
            {
                _gunController.ApplyMovement(_inputManager.diff, _inputManager.hasInput);
            }
            else
            {
                _bikeController.Steer(_inputManager.diff, _inputManager.hasInput);                
            }
        }
        #endregion privates
    }
}