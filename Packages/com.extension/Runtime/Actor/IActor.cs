using actor.module;
using System.Threading.Tasks;
using UnityEngine;

namespace actor {
    public interface IActor {
        GameObject Root { get; set; }
        Transform CameraFollow { get; }
        Animator Animator { get; }
        void Initialize();
        void RegisterModule(ActorModule module);
        void SetPosition(Vector3 target);
        void SetRotaiton(Quaternion target);
        void Freeze(bool value);
        Task Destroy();
    }

    public interface ISetpositionModule {
        void SetPosition(Vector3 target);
    }

    public interface ISetRotationModule {
        void SetRotation(Quaternion target);
    }

    public interface IFreezable {
        void Freeze(bool value);
    }
}
