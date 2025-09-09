
using network.client.message;
using UnityEngine;

namespace network.client.component {
    [AddComponentMenu("Com/Network/Client/Client Transform")]
    public class NetworkTransform : NetworkComponent<Transform> {
        internal override void OnRecive(IMessage message) {
            component.SetPositionAndRotation(message.GetVector3(), message.GetQuaternion());
            component.localScale = message.GetVector3();
        }
    }
}
