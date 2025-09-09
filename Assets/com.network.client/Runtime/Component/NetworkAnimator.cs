
using network.client.message;
using UnityEngine;

namespace network.client.component {
    [AddComponentMenu("Com/Network/Client/Client Animator")]
    public class NetworkAnimator : NetworkComponent<Animator> {
        private AnimatorControllerParameter[] parameters;
        private int parametersCount;

        public override NetworkComponent<Animator> Initialize(string identifier, Animator component = null) {
            parameters = component.parameters;
            parametersCount = component.parameterCount;
            return base.Initialize(identifier, component);
        }

        internal override void OnRecive(IMessage message) {
            for (int i = 0; i < parametersCount; i++) {
                var parameter = parameters[i];
                switch (parameter.type) {
                    case AnimatorControllerParameterType.Float:
                        component.SetFloat(parameter.name, message.GetFloat());
                        break;
                    case AnimatorControllerParameterType.Int:
                        component.SetInteger(parameter.name, message.GetInt());

                        break;
                    case AnimatorControllerParameterType.Bool:
                        component.SetBool(parameter.name, message.GetBool());
                        break;
                    case AnimatorControllerParameterType.Trigger:
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
