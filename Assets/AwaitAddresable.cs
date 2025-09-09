using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AwaitAddresable : MonoBehaviour {
    [SerializeField] GameObject master;
    private async void Awake() {
        await Addressables.DownloadDependenciesAsync("Default Local").Task;
        master.SetActive(true);
    }

}
