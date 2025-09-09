using actor.module;
using System;
using UnityEngine;

namespace actor {
    public class LocomotionModule : ActorModule {

        [SerializeField] private float gravity = -9.81f;
        [SerializeField] private float smoothTurn = 0.1f;

        public event Action OnJumpingStart; // event buat send ke client

        private IActorInput _actorInput;
        private Animator _animator;
        private CharacterController characterController;

        private int _hash_InputX = Animator.StringToHash("Input X");
        private int _hash_InputY = Animator.StringToHash("Input Y");
        private int _hash_InputM = Animator.StringToHash("Input M");
        private int _hash_JumpRequest = Animator.StringToHash("Jump Request");

        private Vector3 _velocityRootMotion;
        private bool _isSprint;
        private bool canJump;

        private bool _isJumping = false;
        private float _ref_smoothTurn;


        public override void Initialize(IActor actor) {
            base.Initialize(actor);
            _actorInput = actor.Root.GetComponent<IActorInput>();
            _animator = actor.Animator;
            //_actorInput.OnSprintPress += OnSprintPress;
            //_actorInput.OnJumpPress += OnJumpPress;
            canJump = true;
            characterController = actor.Root.AddComponent<CharacterController>();
            characterController.center = new Vector3(0, 0.93f, 0);
            characterController.radius = 0.32f;
            characterController.height = 1.7f;
        }

        private void OnJumpPress() {
            if (!canJump) return;
            _animator.SetTrigger(_hash_JumpRequest);
            canJump = false;
            OnJumpingStart?.Invoke();
        }

        private void Update() {
            var _cameraRotation = _actorInput.CameraRotation();
            var moveInputVector = Vector3.ClampMagnitude(new Vector3(_actorInput.MoveAxis().x, 0, _actorInput.MoveAxis().y), 1f);

            if (moveInputVector.magnitude > 0.1f) {
                var targetAngle = Mathf.Atan2(moveInputVector.x, moveInputVector.z) * Mathf.Rad2Deg + _cameraRotation.eulerAngles.y;
                var angle = Mathf.SmoothDampAngle(Actor.Root.transform.eulerAngles.y, targetAngle, ref _ref_smoothTurn, smoothTurn);
                Actor.Root.transform.rotation = Quaternion.Euler(0, angle, 0);
            }

            float magnitude = _actorInput.MoveAxis().normalized.magnitude;

            if (_isSprint) magnitude *= 2;
            _animator.SetFloat(_hash_InputM, magnitude, 0.1f, Time.deltaTime);
            _isSprint = false;
        }

        private void FixedUpdate() {
            if (!_isJumping) _velocityRootMotion.y += gravity * Time.fixedDeltaTime;
            characterController.Move(_velocityRootMotion);
            _velocityRootMotion = Vector3.zero;
        }

        private void OnAnimatorMove() {
            _velocityRootMotion += _animator.deltaPosition;
        }

        private void OnSprintPress() {
            _isSprint = true;
        }

        public void JumpStart() {
            _isJumping = true;
        }

        public void EndJump() {
            _isJumping = false;
            canJump = true;
        }

    }
}
