using network.client.message;
using network.client.ultis;
using System.Collections.Generic;
using UnityEngine;

namespace network.client.component {
    public abstract class NetworkComponent<T> : MonoBehaviour where T : Component {

        public string Identifier;
        public T component;
        public bool initializeAtStart;
        protected bool hasInitialize;
        private ushort componentId;

        internal static Dictionary<ushort,Dictionary<string,NetworkComponent<T>>> instanceComponent = new();

        private void Start() {
            if (initializeAtStart) Initialize(Identifier, component);
        }

        public virtual NetworkComponent<T> Initialize(string identifier, T component = null) {
            Identifier = identifier;
            this.component = component == null ? gameObject.GetComponent<T>() : component;
            if (string.IsNullOrWhiteSpace(identifier)) {
                this.Identifier = gameObject.name;
            }
            if (this.component == null) {
                gameObject.SetActive(false);
                return this;
            }
            componentId = Helper.GetComponentId<T>();
            hasInitialize = true;

            if (instanceComponent.TryGetValue(componentId, out var compStack)) {
                compStack.Add(this.Identifier, this);
            }
            else {
                instanceComponent.Add(componentId, new());
                instanceComponent[componentId].Add(this.Identifier, this);
            }

            NetworkComponentReciver<T>.Initialize();
            return this;
        }
        public virtual void Release() {
            if (instanceComponent.TryGetValue(componentId, out var compStack)) {
                if (compStack.TryGetValue(Identifier, out var target)) {
                    instanceComponent[componentId].Remove(this.Identifier);
                }
                else {
                    instanceComponent.Remove(componentId);
                }
            }

            NetworkComponentReciver<T>.Realease();
        }

        internal abstract void OnRecive(IMessage message);

        public static implicit operator T(NetworkComponent<T> component) {
            return component.component;
        }
    }
}
