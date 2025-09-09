using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace assetmanager
{
    public static class AssetManager 
    {
        public static Task InitializeAsync() {
            return Addressables.DownloadDependenciesAsync("Default Local").Task;
        }

        public static Task<GameObject> InstantiateAsync(string key) {
            return Addressables.InstantiateAsync(key).Task;
        }
    }
}
