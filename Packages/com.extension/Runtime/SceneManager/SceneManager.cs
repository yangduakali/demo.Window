using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace scenemanager {
    public static class SceneManager {
        public static int LoadedSceneCount => LoadedScene.Count;
        public static Dictionary<string, AsyncOperationHandle<SceneInstance>> LoadedScene = new();
        private static readonly Queue<SceneProsess> sceneProsessesPool = new();

        public static SceneProsess Load(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Additive, Action onComplite = null) {
            var prosess = GetProsess();
            prosess.AddLoadProsess(sceneName, loadSceneMode, onComplite);
            return prosess;
        }
        public static SceneProsess Load(this SceneProsess prosess, string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Additive, Action onComplite = null) {
            prosess.AddLoadProsess(sceneName, loadSceneMode, onComplite);
            return prosess;
        }
        public static SceneProsess Unload(string sceneName, Action onComplite = null) {
            var prosess = GetProsess();
            prosess.AddUnloadProsess(sceneName, onComplite);
            return prosess;
        }
        public static SceneProsess Unload(this SceneProsess prosess, string sceneName, Action onComplite = null) {
            prosess.AddUnloadProsess(sceneName, onComplite);
            return prosess;
        }
        public static SceneProsess Complite(this SceneProsess sceneProsess, Action onComplite) {
            sceneProsess.onComplite = onComplite;
            return sceneProsess;
        }
        public static SceneProsess CompliteAsTask(this SceneProsess sceneProsess, Task onComplite) {
            sceneProsess.onCompliteTask = onComplite;
            return sceneProsess;
        }
        public static async void Run(this SceneProsess sceneProsess) {
            await Task.WhenAll(sceneProsess.Handles.ConvertAll(x => x.Invoke().Task));

            if (sceneProsess.onCompliteTask != null) {
                await sceneProsess.onCompliteTask;
            }

            sceneProsess.onComplite?.Invoke();
            ReturnProsses(sceneProsess);
        }
        public static async Task RunAsync(this SceneProsess sceneProsess) {
            await Task.WhenAll(sceneProsess.Handles.ConvertAll(x => x.Invoke().Task));
            if (sceneProsess.onCompliteTask != null) {
                await sceneProsess.onCompliteTask;
            }
            sceneProsess.onComplite?.Invoke();
            ReturnProsses(sceneProsess);
        }
        private static void AddLoadProsess(this SceneProsess prosess, string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Additive, Action onComplite = null) {
            prosess.Handles.Add(() => {
                var handle = Addressables.LoadSceneAsync(sceneName, loadSceneMode);
                handle.Completed += opr => {
                    onComplite?.Invoke();
                    LoadedScene.Add(sceneName, handle);
                };
                return handle;
            });
        }
        private static void AddUnloadProsess(this SceneProsess prosess, string sceneName, Action onComplite = null) {
            prosess.Handles.Add(() => {
                var handle = Addressables.UnloadSceneAsync(LoadedScene[sceneName]);
                handle.Completed += opr => {
                    onComplite?.Invoke();
                    LoadedScene.Remove(sceneName);
                };
                return handle;
            });
        }
        private static SceneProsess GetProsess() {
            if (sceneProsessesPool.Count == 0) {
                var newProsess = new SceneProsess();
                sceneProsessesPool.Enqueue(newProsess);
            }

            return sceneProsessesPool.Dequeue();
        }
        private static void ReturnProsses(SceneProsess sceneProsess) {
            sceneProsess.Reset();
            sceneProsessesPool.Enqueue(sceneProsess);
        }
    }


    public class SceneProsess {
        public SceneProsess() {
            Handles = new();
        }

        internal List<Func<AsyncOperationHandle>> Handles;
        internal Action onComplite;

        public Task onCompliteTask;

        internal void Reset() {
            Handles.Clear();
            onComplite = null;
        }
    }
}
