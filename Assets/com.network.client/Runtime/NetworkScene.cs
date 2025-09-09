using network.client.message;
using scenemanager;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace network.client {
    public class NetworkScene<T> : INetworkGroup where T : class, INetworkEntity, new() {
        public string Name { get; set; }
        public ushort Id { get; set; }
        public bool IsInstance { get; }

        public Dictionary<ushort, INetworkEntity> Clients { get; protected set; } = new();
        public Scene Scene { get; protected set; }

        public async Task ProsessAsync() {
            await scenemanager.SceneManager.Load(Name).RunAsync();
            var oriScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(Name);
            Scene = oriScene;
            //var copyScene = UnityEngine.SceneManagement.SceneManager.CreateScene($"{Name}-{Id}",new CreateSceneParameters{localPhysicsMode = LocalPhysicsMode.Physics3D});
            //var oriRootCount = oriScene.rootCount;
            //for (int i = 0; i < oriRootCount; i++) {
            //    var root = oriScene.GetRootGameObjects()[i];
            //    UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(root, copyScene);
            //}
            //Scene = copyScene;
            //await scenemanager.SceneManager.Unload(Name).RunAsync();
        }
        public Task AddClient(INetworkEntity networkEntity) {
            Clients.Add(networkEntity.ConnectionId, networkEntity);
            return Task.CompletedTask;
        }
        public Task RemoveClient(INetworkEntity networkEntity) {
            Clients.Remove(networkEntity.ConnectionId);
            return Task.CompletedTask;
        }
        public async Task RealeaseAsync() {
            var complite = false;
            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(Scene).completed += opr => {
                complite = true;
            };
            while (!complite) {
                await Task.Yield();
            }
        }
        public virtual void Deserialize(IMessage message) {
            Name = message.GetString();
            Id = message.GetUShort();
            var clientCount = message.GetUShort();
            for (int i = 0; i < clientCount; i++) {
                var client = message.GetClass<T>();
                Clients.Add(client.ConnectionId, client);
            }
        }
        public virtual void Serialize(IMessage message) { }
    }

}
