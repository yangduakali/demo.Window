using System;
using UnityEngine;

namespace actor.module {
    public class UnityInputOldModule : ActorModule, IActorInput {
        //public event Action OnJumpPress;
        //public event Action OnSprintPress;

        private Transform cameraTransform;

        protected override void Awake() {
            base.Awake();
            cameraTransform = Camera.main.transform;
        }

        public Quaternion CameraRotation() {
            return cameraTransform.rotation;
        }

        public Vector2 LookAxis() {
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        public Vector2 MoveAxis() {
            return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }


    }
}
