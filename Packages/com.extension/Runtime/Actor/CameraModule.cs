using System.Collections.Generic;
using UnityEngine;

namespace game.actors {
    public class CameraModule : MonoBehaviour {
        public Vector2 FollowPointFraming = new Vector2(0f, 0f);
        public float FollowingSharpness = 10000f;

        public float DefaultDistance = 3.5f;
        public float TPPMinDistance = 2f;
        public float MinDistance = 0f;
        public float MaxDistance = 5f;
        public float DistanceMovementSpeed = 5f;
        public float DistanceMovementSharpness = 10f;

        public bool InvertX = false;
        public bool InvertY = false;
        [Range(-90f, 90f)]
        public float DefaultVerticalAngle = 20f;
        [Range(-90f, 90f)]
        public float MinVerticalAngle = -36f;
        [Range(-90f, 90f)]
        public float MaxVerticalAngle = 46f;
        public float RotationSpeed = 1f;
        public float RotationSharpness = 10000f;
        public bool RotateWithPhysicsMover = false;

        public float ObstructionCheckRadius = 0.2f;
        public LayerMask ObstructionLayers = -1;
        public float ObstructionSharpness = 10000f;
        public List<Collider> IgnoredColliders = new List<Collider>();
        //[SerializeField] private float playerTick = 0.65f;

        private readonly RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];
        private Vector3 PlanarDirection;
        private Vector3 _currentFollowPosition;
        //private ControllType _curentControllType;
        private const int MaxObstructions = 32;
        private int _obstructionCount;
        private float TargetDistance;
        private float _currentDistance;
        private float _targetVerticalAngle;
        private bool _isFreeze;
        private bool _isPause;
        private bool _distanceIsObstructed;

        public Camera mainCamera;
        public Transform CameraFollow;

        private Vector2 lookAxis;
        private float mouseScroll;
        private Camera _camera;

        private void Start() {
            Initialize();
        }

        public void Initialize() {
            PlanarDirection = CameraFollow.forward;
            _currentFollowPosition = CameraFollow.forward;
            _camera = Camera.main;
        }

        private void Update() {
            lookAxis.x = Input.GetAxis("Mouse X");
            lookAxis.y = Input.GetAxis("Mouse Y");
        }

        private void LateUpdate() {
            if (CameraFollow == null) return;
            if (_camera == null) return;

            var lookInputVector = new Vector3(lookAxis.x, lookAxis.y, 0f);
            var scrollInput = -mouseScroll;

            if (_isPause) {
                lookInputVector = Vector3.zero;
                scrollInput = 0;
            }

            UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);
        }

        private void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput) {
            if (_isFreeze) return;
            if (InvertX) {
                rotationInput.x *= -1f;
            }
            if (InvertY) {
                rotationInput.y *= -1f;
            }

            var rotationFromInput = Quaternion.Euler(CameraFollow.up * (rotationInput.x * RotationSpeed));
            PlanarDirection = rotationFromInput * PlanarDirection;
            PlanarDirection = Vector3.Cross(CameraFollow.up, Vector3.Cross(PlanarDirection, CameraFollow.up));
            var planarRot = Quaternion.LookRotation(PlanarDirection, CameraFollow.up);

            _targetVerticalAngle -= rotationInput.y * RotationSpeed;
            _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
            var verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
            var targetRotation = Quaternion.Slerp(_camera.transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

            _camera.transform.rotation = targetRotation;

            if (_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f) {
                TargetDistance = _currentDistance;

            }

            TargetDistance += zoomInput * DistanceMovementSpeed;
            TargetDistance = Mathf.Clamp(TargetDistance, TPPMinDistance, MaxDistance);
            _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, CameraFollow.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));


            //switch (_actor.ControllType) {
            //    case ControllType.TPP:
            //        break;
            //    case ControllType.FPP:
            //        TargetDistance = 0;
            //        _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, _actor.CameraFollowPoint.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));

            //        break;
            //    case ControllType.VR:
            //        break;
            //    default:
            //        break;
            //}

            // Handle obstructions
            {
                RaycastHit closestHit = new RaycastHit() {
                    distance = Mathf.Infinity
                };
                _obstructionCount = Physics.SphereCastNonAlloc(_currentFollowPosition, ObstructionCheckRadius, -_camera.transform.forward, _obstructions, TargetDistance, ObstructionLayers, QueryTriggerInteraction.Ignore);
                for (int i = 0; i < _obstructionCount; i++) {
                    bool isIgnored = false;
                    for (int j = 0; j < IgnoredColliders.Count; j++) {
                        if (IgnoredColliders[j] == _obstructions[i].collider) {
                            isIgnored = true;
                            break;
                        }
                    }
                    for (int j = 0; j < IgnoredColliders.Count; j++) {
                        if (IgnoredColliders[j] == _obstructions[i].collider) {
                            isIgnored = true;
                            break;
                        }
                    }

                    if (!isIgnored && _obstructions[i].distance < closestHit.distance && _obstructions[i].distance > 0) {
                        closestHit = _obstructions[i];
                    }
                }


                // If obstructions detecter
                if (closestHit.distance < Mathf.Infinity) {
                    _distanceIsObstructed = true;
                    _currentDistance = Mathf.Lerp(_currentDistance, closestHit.distance, 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
                }
                // If no obstruction
                else {
                    _distanceIsObstructed = false;
                    _currentDistance = Mathf.Lerp(_currentDistance, TargetDistance, 1 - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
                }
            }

            //if (Actor.Avatar != null) {
            //    if (_curentControllType == ControllType.TPP) {
            //        if (_currentDistance <= playerTick) {
            //            if (Actor.Avatar.Root.activeSelf) {
            //                Actor.Avatar.Root.SetActive(false);
            //            }

            //        } else {
            //            if (!Actor.Avatar.Root.activeSelf) {
            //                Actor.Avatar.Root.SetActive(true);
            //            }
            //        }
            //    }
            //}


            // Find the smoothed _camera orbit position
            Vector3 targetPosition = _currentFollowPosition - targetRotation * Vector3.forward * _currentDistance;

            // Handle framing
            targetPosition += _camera.transform.right * FollowPointFraming.x;
            targetPosition += _camera.transform.up * FollowPointFraming.y;

            // Apply position
            _camera.transform.position = targetPosition;
        }

    }
}
