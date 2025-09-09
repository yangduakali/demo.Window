
using network.client.message;
using System.Collections.Generic;
using UnityEngine;

namespace network.client.component {
    internal static class NetworkComponentReciver<T> where T : Component {

        public static void Initialize() {
            IClientProsessor.Instance.OnNetworkComponentMessage += Instance_OnNetworkComponentMessage;
        }
        public static void Realease() {
            IClientProsessor.Instance.OnNetworkComponentMessage -= Instance_OnNetworkComponentMessage;
        }

        private static void Instance_OnNetworkComponentMessage(byte[] message) {
            var msg = Message.Create(bytes: message);
            var componentId =msg.GetUShort();

            if (NetworkComponent<T>.instanceComponent.TryGetValue(componentId, out var stack)) {
                var identifier = msg.GetString();
                if (stack.TryGetValue(identifier, out var netCompt)) {
                    netCompt.OnRecive(msg);
                    msg.Release();
                }
                else {
                    msg.Release();
                }
            }
            else {
                msg.Release();
            }


        }


    }
}
