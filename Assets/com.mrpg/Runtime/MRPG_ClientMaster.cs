using ByteSizeLib;
using network.client;
using network.client.message;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using ValueType = value.ValueType;
using Random = UnityEngine.Random;
namespace mrpg.client {
    public class MRPG_ClientMaster : MonoBehaviour {

        [ShowInInspector, PropertyOrder(-100)] public string Identifier { get; set; } = "Actor";
        [ShowInInspector, PropertyOrder(-100)] public string Ip { get; set; } = "127.0.0.1";
        [ShowInInspector, PropertyOrder(-100)] public int Port { get; set; } = 8080;

        [SerializeField] private ValueType prosessor;

        [SerializeField] private GameObject[] buttonScenes;
        [SerializeField] private GameObject buttonExitScene;

        [FoldoutGroup("Event")] public UnityEvent<string> OnDebug;
        [FoldoutGroup("Event")] public UnityEvent OnConnect;
        [FoldoutGroup("Event")] public UnityEvent OnDisconect;


        private IClientManager manager;
        private readonly Stopwatch stopwatch = new ();
        ByteSize sendBytes = new();
        ByteSize reciveBytes = new();
        private long highSend;
        private long highRecive;
        private INetworkGroup networkGroup;
        private void Awake() {
            manager = CreateManager(prosessor);
            manager.Ip = Ip;
            manager.Port = (ushort)Port;
            Physics.autoSimulation = false;
        }
        private void Start() {
            OnDisconect?.Invoke();
            buttonExitScene.SetActive(false);
            buttonScenes.ForEach(x => x.SetActive(false));
        }
        private void Update() {
            OnDebug?.Invoke(DebugText());
        }
        private void FixedUpdate() {
            manager.Tick();
        }
        private void OnApplicationQuit() {
            manager.Disconnect();
        }

        private IClientManager CreateManager(Type prossesorType) {
            var t = typeof(ClientManager<,,>);
            var ct = t.MakeGenericType(prossesorType, typeof(MRPG_NetworkEntity), typeof(NetworkScene<MRPG_NetworkEntity>));
            return Activator.CreateInstance(ct) as IClientManager;
        }
        private string DebugText() {
            string t = "";
            if(manager.State == StatusState.Connected) {
                t += manager.State + $"   {manager.Ip}:{manager.Port}";
            }
            else {
                t += manager.State;
            }
            t += "\n";

            if (IClientManager.Instance.LocalPlayer != null) {
                t += $"Local : {IClientManager.Instance.LocalPlayer.ConnectionId}-{IClientManager.Instance.LocalPlayer.Id}";
                t += "\n";
            }

            if (networkGroup != null) {
                t += $"Network Group : {networkGroup.Id}-{networkGroup.Name}";
                t += "\n";
            }



            if (!stopwatch.IsRunning) {
                stopwatch.Start();
            }
            else {

                if (stopwatch.ElapsedMilliseconds >= 1000) {
                    if (ClientUltis.TotalByteSend > highSend) highSend = ClientUltis.TotalByteSend;
                    if (ClientUltis.TotalByteRecive > highRecive) highRecive = ClientUltis.TotalByteRecive;

                    sendBytes = ByteSize.FromBits(ClientUltis.TotalByteSend);
                    reciveBytes = ByteSize.FromBits(ClientUltis.TotalByteRecive);

                    ClientUltis.TotalByteSend = 0;
                    ClientUltis.TotalByteRecive = 0;
                    stopwatch.Reset();
                }
            }

            t += $"Message pool: {Message.pool.Count}";
            t += "\n";
            t += $"Message instance: {Message.instanceCount}";
            t += "\n";
            t += $"Recive bytes: {reciveBytes}";
            t += "\n";
            t += $"High recive: {ByteSize.FromBits(highRecive)}";
            t += "\n";
            t += $"Send bytes: {sendBytes}";
            t += "\n";
            t += $"High send: {ByteSize.FromBits(highSend)}";
            return t;
        }

        public void Connect() {
            manager.Ip = Ip;
            manager.Port = (ushort)Port;
            manager.Connect(Identifier, new Callback { onSuccess = () => { OnConnect?.Invoke(); }, onError = (x) => { OnDisconect?.Invoke(); } });
            OnConnect?.Invoke();
            buttonScenes.ForEach(x => x.SetActive(true));
        }
        public void Disconect() {
            buttonScenes.ForEach(x => x.SetActive(false));
            buttonExitScene.SetActive(false);
            manager.Disconnect();
            OnDisconect?.Invoke();  
        }
        public void EnterGroup(string groupName) {
            buttonScenes.ForEach(x => x.SetActive(false));
            manager.EnterGroup(groupName,
                callback: new Callback<INetworkGroup> {
                    onSuccess = netScenne => {
                        print($"succes enter {netScenne.Name}-{netScenne.Id}");
                        buttonExitScene.SetActive(true);
                        networkGroup = netScenne;
                    },
                    onError = message => {
                        buttonScenes.ForEach(x => x.SetActive(true));
                        buttonExitScene.SetActive(false);
                        print(message);
                    }
                });
        }
        public void ExitGroup() {
            manager.ExitGroup(new Callback {
                onSuccess = () => {
                    buttonScenes.ForEach(x => x.SetActive(true));
                    buttonExitScene.SetActive(false);
                },
                onError = (message) => { 
                    print(message);
                }
            });
        }
    }
}
