using actor.module;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace actor {
    public class Actor : MonoBehaviour, IActor {
        public Transform CameraFollow { get; private set; }
        public GameObject Root { get; set; }
        public Animator Animator { get; private set; }

        private readonly List<ActorModule> actorModules = new();
        private ISetRotationModule _ISetRotationModule;
        private ISetpositionModule _ISetpositionModule;
        private readonly List<IFreezable> _IFreezable = new();

        public void Initialize() {
            Root = gameObject;
            actorModules.OrderBy(x => x.order);
            GameObject newTrans = new("Camera Follow");
            CameraFollow = newTrans.transform;
            var allTrans = Root.GetComponentsInChildren<Transform>();
            foreach (Transform trens in allTrans) {
                if (trens.name.ToLower().Contains("neck")) {
                    CameraFollow.SetPositionAndRotation(trens.position, Root.transform.rotation);
                    break;
                }
            }

            CameraFollow.SetParent(Root.transform);
            Animator = GetComponent<Animator>();

            for (int i = 0; i < actorModules.Count; i++) {
                SetupModule(actorModules[i]);
                actorModules[i].Initialize(this);
            }
        }

        public void RegisterModule(ActorModule actorModule) {
            if (actorModules.Contains(actorModule)) return;
            actorModules.Add(actorModule);
        }

        public void SetPosition(Vector3 target) {
            _ISetpositionModule.SetPosition(target);
        }

        public void SetRotaiton(Quaternion target) {
            _ISetRotationModule.SetRotation(target);
        }

        protected virtual void SetupModule(ActorModule actorModule) {
            if (actorModule is ISetpositionModule setPosition) _ISetpositionModule = setPosition;
            if (actorModule is ISetRotationModule setRotation) _ISetRotationModule = setRotation;
            if (actorModule is IFreezable Freezable) _IFreezable.Add(Freezable);
        }

        public void Freeze(bool value) {
            for (int i = 0; i < _IFreezable.Count; i++) {
                _IFreezable[i].Freeze(value);
            }
        }

        public Task Destroy() {
            if (Root == null) return Task.CompletedTask;
            Destroy(Root);
            return Task.CompletedTask;
        }
    }
}
