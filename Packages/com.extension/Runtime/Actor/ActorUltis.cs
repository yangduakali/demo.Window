using actor;
using actor.module;
using System;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace game.actors {

    public static class ActorUltis {

        public static void CreatePlayer<T>(GameObject playerObject, Action<IPlayer> onComplite)
            where T : IPlayer {
            var player = playerObject.AddComponent(typeof(T)) as IPlayer;
            player.Root = playerObject;
            onComplite?.Invoke(player);
        }
        public static Task<T> CreatePlayerAsync<T>(GameObject playerObject)
            where T : IPlayer {
            var player = playerObject.AddComponent(typeof(T)) as IPlayer;
            player.Root = playerObject;
            return Task.FromResult((T)player);
        }

        public static void CreatePlayer(GameObject playerObject, Action<IPlayer> onComplite){
            var player = playerObject.AddComponent(typeof(Player)) as IPlayer;
            player.Root = playerObject;
            onComplite?.Invoke(player);
        }


        public static Task<IPlayer> CreatePlayerAsync(GameObject playerObject) {
            var player = playerObject.AddComponent(typeof(Player)) as IPlayer;
            player.Root = playerObject;
            return Task.FromResult(player);
        }

        public static T AddModule<T>(this IPlayer player, int order) where T : ActorModule {
            var module = player.Root.AddComponent(typeof(T)) as ActorModule;
            module.order = order;
            return module as T;
        }

        public static void AddCameraPlayer(this IPlayer player, Camera camera = null) {
            var nGo = new GameObject("Camera Module");
            var cam = nGo.AddComponent<CameraModule>();
            cam.mainCamera = camera == null ? Camera.main : camera;
            cam.CameraFollow = player.CameraFollow;
        }

    }
}
