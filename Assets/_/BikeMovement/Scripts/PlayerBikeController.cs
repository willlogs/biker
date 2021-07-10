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
        public TraceFollower traceFollower;

        public void ActivateShootingMode(Transform target = null)
        {
            if(target != null)
            {
                _gunAimTargetT.DOMove(target.position, 0.5f).SetUpdate(true);
            }
            else
            {
                _gunAimTargetT.DOMove(transform.position + transform.forward * 10 + transform.up * 5, 0.5f).SetUpdate(true);
            }

            _cameraIK.enabled = true;
            _cameraIK.solver.IKPositionWeight = 0;
            DOTween.To(() => _cameraIK.solver.IKPositionWeight, (x) => { _cameraIK.solver.IKPositionWeight = x; }, 0.3f, 0.5f).SetUpdate(true);

            _gun.gameObject.SetActive(true);
            _ik.solver.rightHandEffector.positionWeight = 0;
            _ik.solver.rightHandEffector.rotationWeight = 0;
            _isInShootingMode = true;
        }

        public void DeactivateShootingMode()
        {            
            DOTween.To(() => _cameraIK.solver.IKPositionWeight, (x) => { _cameraIK.solver.IKPositionWeight = x; }, 0f, 0.5f).SetUpdate(true).OnComplete(() => { _cameraIK.enabled = false; });

            _gun.gameObject.SetActive(false);
            _ik.solver.rightHandEffector.positionWeight = 1;
            _ik.solver.rightHandEffector.rotationWeight = 1;
            _isInShootingMode = false;
        }
        #endregion

        #region privates
        [SerializeField] private bool _isInShootingMode = false, _testMode = false;

        [SerializeField] private FullBodyBipedIK _ik;
        [SerializeField] private AimIK _aimIK, _cameraIK;
        [SerializeField] private Transform _gunAimTargetT;
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