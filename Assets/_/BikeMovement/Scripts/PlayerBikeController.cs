using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PT.GunPlay;
using RootMotion.FinalIK;
using DG.Tweening;
using UnityEngine.UI;
using PT.Utils;

namespace PT.Bike
{
    [RequireComponent(typeof(BikeController))]
    [RequireComponent(typeof(GunController))]
    public class PlayerBikeController : MonoBehaviour
    {
        #region publics
        public TraceFollower traceFollower;

        public void StopEverything()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }

        public void FollowTarget(Transform target)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            _followingTarget = true;
            _target = new GameObject().transform;
            _target.position = transform.position;
            _target.rotation = transform.rotation;
            _target.parent = target;
        }

        public void ContinueMoving()
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().velocity = transform.forward * 10;
            _followingTarget = false;
            _bikeController._autoAccelerate = true;
        }

        public void ActivateShootingMode(Transform target = null, bool lookAtIt = false)
        {
            /*_cameraCopyPosTween.Follow();
            Camera.main.DOFieldOfView(100, 0.5f);*/

            _gunIndex = Random.Range(0, _guns.Length);

            if(target != null)
            {
                _gunAimTargetT.DOMove(target.position, 0.5f).SetUpdate(true).OnComplete(() => {
                    _aimUI.rectTransform.anchoredPosition = Vector2.zero;
                    _gunController.ApplyMovement(Vector2.one * 0.1f, false);
                });
            }
            else
            {
                _gunAimTargetT.DOMove(transform.position + transform.forward * 10 + transform.up * 5, 0.5f).SetUpdate(true);
            }

            _guns[_gunIndex].gameObject.SetActive(true);
            _gunController.SetGun(_guns[_gunIndex]);
            _ik.solver.rightHandEffector.positionWeight = 0;
            _ik.solver.rightHandEffector.rotationWeight = 0;
            _aimIK.solver.IKPositionWeight = 1;
            _aimIK.enabled = true;
            _isInShootingMode = true;

            _aimUI.gameObject.SetActive(true);

            if (lookAtIt)
            {
                Camera.main.transform.forward = (target.position - Camera.main.transform.position).normalized;
            }
        }

        public void DeactivateShootingMode()
        {
            /*_cameraCopyPosTween.StopFollow();
            Camera.main.fieldOfView = 88;
            _cameraCopyPosTween.transform.position = _mainViewT.position;
            _cameraCopyPosTween.transform.rotation = _mainViewT.rotation;*/

            DOTween.To(() => _cameraIK.solver.IKPositionWeight, (x) => { _cameraIK.solver.IKPositionWeight = x; }, 0f, 0.5f).SetUpdate(true).OnComplete(() => { _cameraIK.enabled = false; });

            foreach(Gun g in _guns)
            {
                g.gameObject.SetActive(false);
            }

            _ik.solver.rightHandEffector.positionWeight = 1;
            _ik.solver.rightHandEffector.rotationWeight = 1;

            _aimIK.solver.IKPositionWeight = 0;
            _aimIK.enabled = false;
            _isInShootingMode = false;

            _aimUI.gameObject.SetActive(false);

            _followingTarget = false;
        }
        #endregion

        #region privates
        [SerializeField] private bool _isInShootingMode = false, _testMode = false;

        [SerializeField] private FullBodyBipedIK _ik;
        [SerializeField] private AimIK _aimIK, _cameraIK;
        [SerializeField] private Transform _gunAimTargetT;
        [SerializeField] private Gun[] _guns;
        [SerializeField] private Image _aimUI;

        [SerializeField] private float _switchEach = 10f;

        [SerializeField] private int _gunIndex = 0;

        [SerializeField] private TweenCopyPosition _cameraCopyPosTween;
        [SerializeField] private Transform _mainViewT;

        private TouchInputManager _inputManager;
        private BikeController _bikeController;
        private GunController _gunController;

        private bool _followingTarget = false;
        private Transform _target;

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

        private void FixedUpdate()
        {
            if (_isInShootingMode)
            {
                _gunController.ApplyMovement(_inputManager.diff, _inputManager.hasInput);

                if (_followingTarget)
                {
                    transform.position = _target.position;
                    transform.rotation = _target.rotation;
                }
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