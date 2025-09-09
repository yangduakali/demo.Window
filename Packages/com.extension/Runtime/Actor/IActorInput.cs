using System;
using UnityEngine;

namespace actor {
    public interface IActorInput {

        Vector2 MoveAxis();
        Vector2 LookAxis();
        Quaternion CameraRotation();

        //event Action OnJumpPress;
        //event Action OnSprintPress;
    }
}
