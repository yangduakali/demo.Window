using actor.module;
using network.client;
using network.client.message;
using UnityEngine;

namespace mrpg.client {
    public class InputSenderModule : ActorModule {

        private Vector2 moveInput;
        private Vector2 lookInput;
        private Quaternion cameraRotation;

        private void Update() {
            moveInput.x = Input.GetAxis("Horizontal");
            moveInput.y = Input.GetAxis("Vertical");
            lookInput.x = Input.GetAxis("Mouse X");
            lookInput.y = Input.GetAxis("Mouse Y");

            if (Camera.main == null) return;
            cameraRotation = Camera.main.transform.rotation;
        }

        private void FixedUpdate() {
            var msg = Message.Create(SendMode.Unreliable);
            msg.Add(moveInput);
            msg.Add(lookInput);
            msg.Add(cameraRotation);
            IClientManager.Instance.SendInputMessage(msg);
        }

    }
}
