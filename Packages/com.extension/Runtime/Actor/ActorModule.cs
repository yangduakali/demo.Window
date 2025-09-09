using UnityEngine;

namespace actor.module {

    [RequireComponent(typeof(IActor))]
    public abstract class ActorModule : MonoBehaviour {

        public int order;
        protected IActor Actor { get; private set; }
        protected bool isActive;

        protected virtual void Awake() {
            enabled = false;
            Actor = GetComponent<IActor>();
            Actor.RegisterModule(this);
        }

        public virtual void Initialize(IActor actor) {
            enabled = true;
        }
    }
}
