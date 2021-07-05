using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.GunPlay;
using RootMotion.FinalIK;
using DG.Tweening;

namespace PT.Bike
{
    [RequireComponent(typeof(BikeController))]
    [RequireComponent(typeof(GunController))]
    public class PlayerBikeController : MonoBehaviour
    {
        #region publics
        public Transform _gunTargetT;

        public void ActivateShootingMode()
        {
            _gun.gameObject.SetActive(true);

            DOTween.To(() => _ik.solver.rightHandEffector.positionWeight, (x) => { _ik.solver.rightHandEffector.positionWeight = x; }, 0, 0.5f);
            DOTween.To(() => _ik.solver.rightHandEffector.rotationWeight, (x) => { _ik.solver.rightHandEffector.rotationWeight = x; }, 0, 0.5f);
            DOTween.To(() => _cameraAimIK.solver.IKPositionWeight, (x) => { _cameraAimIK.solver.IKPositionWeight = x; }, 1, 0.5f);
            _isInShootingMode = true;
        }

        public void DeactivateShootingMode()
        {
            _gun.gameObject.SetActive(false);
            DOTween.To(() => _ik.solver.rightHandEffector.positionWeight, (x) => { _ik.solver.rightHandEffector.positionWeight = x; }, 1, 0.5f);
            DOTween.To(() => _ik.solver.rightHandEffector.rotationWeight, (x) => { _ik.solver.rightHandEffector.rotationWeight = x; }, 1, 0.5f);
            DOTween.To(() => _cameraAimIK.solver.IKPositionWeight, (x) => { _cameraAimIK.solver.IKPositionWeight = x; }, 0, 0.5f);
            _isInShootingMode = false;
        }
        #endregion

        #region privates
        [SerializeField] private bool _isInShootingMode = false, _testMode = false;

        [SerializeField] private FullBodyBipedIK _ik;
        [SerializeField] private AimIK _cameraAimIK;
        [SerializeField] private Gun _gun;

        [SerializeField] private float _switchEach = 10f;

        private TouchInputManager _inputManager;
        private BikeController _bikeController;
        private GunController _gunController;

        private Vector3 _starterTargetOffset;

        private void Start()
        {
            _starterTargetOffset = _gunTargetT.position - transform.position;
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