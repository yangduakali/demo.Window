using network.client.message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace network.client {
    public class ClientManager<T,T2,T3> : IClientManager where T : class, IClientProsessor, new() where T2 : class, INetworkEntity, new() where T3 : class, INetworkGroup, new() {
        public ClientManager() {
            _prosessor = new T();
            _prosessor.OnConnectFailed += Prossesor_OnConnectFailed;
            _prosessor.OnWelocomeMessage += Prossesor_OnWelocomeMessage;
            _prosessor.OnEnterGroupMessage += Prossesor_OnEnterGroupMessage;
            _prosessor.OnClientEnterGroupMessage += Prossesor_OnClientEnterGroupMessageAsync;
            _prosessor.OnClientExitGroupMessage += Prossesor_OnClientExitGroupMessageAsync;
            _prosessor.OnCustomMessage += Prossesor_OnCustomMessage;
            _prosessor.OnDisconnect += Prosessor_OnDisconnect;
            _prosessor.OnExitGroupMessage += Prosessor_OnExitGroupMessageAsync;
            IClientProsessor.Instance = _prosessor;
            IClientManager.Instance = this;
        }

        public string Ip { get; set; } = "127.0.0.1";
        public ushort Port { get; set; } = 8080;
        public INetworkGroup NetworkGroup { get; private set; }
        public StatusState State { get; private set; }
        public INetworkEntity LocalPlayer { get; private set; }

        public event Action<T2> OnClientEnterGroup = delegate{ };
        public event Action<ushort> OnClientExitGroup = delegate{ };
        public event Action<IMessage> OnCustomMessage = delegate{ };
        
        private Callback _connectCallback;
        private Callback<INetworkGroup> _enterGroupCallback;
        private Callback _exitGroupCallback;

        private IClientProsessor _prosessor;
        private readonly Dictionary<ushort, INetworkEntity> _tempClients = new();

        public void Connect(string identifier = "", Callback callback = default) {
            if (State == StatusState.Connecting || State == StatusState.Connected) return;
            _connectCallback = callback;
            State = StatusState.Connecting;
            _prosessor.Connect(Ip, Port,identifier);
        }
        public void Tick() {
            _prosessor.Tick();
        }
        public void Disconnect() {
            _prosessor.Disconnect();
        }
        public void EnterGroup(string nameGroup, ushort id = 0,Callback<INetworkGroup> callback = default) {
            _enterGroupCallback = callback;
            var msg = Message.Create();
            msg.Add(nameGroup);
            msg.Add(id);
            _prosessor.SendMessageEnterGroup(msg);
        }
        public void ExitGroup(Callback callback = default) {
            _exitGroupCallback = callback;
            var msg  = Message.Create();
            _prosessor.SendMessageExitGroup(msg);
        }
        public void SendCustomMessage(IMessage message) {
            _prosessor.SendCustomMessage(message);
        }
        public void SendInputMessage(IMessage message) {
            _prosessor.SendInputMessage(message);
        }

        private void Prossesor_OnConnectFailed() {
            State = StatusState.Disconnected;
            _connectCallback.onError?.Invoke("Timed out");
        }
        private void Prossesor_OnWelocomeMessage(byte[] message) {
            var msg = Message.Create(bytes: message);
            State = StatusState.Connected;
            LocalPlayer = msg.GetClass<T2>();  
            _connectCallback.onSuccess?.Invoke();
            msg.Release();
        }
        private async void Prossesor_OnEnterGroupMessage(byte[] message) {
            var msg = Message.Create(bytes : message);
            var succes = msg.GetBool();
            var serverMessage = msg.GetString();
            msg.Release();

            if (!succes) {
                _enterGroupCallback.onError?.Invoke(serverMessage);
                return;
            }
            T3 group = msg.GetClass<T3>();
            NetworkGroup = group;
            LocalPlayer.CopyFrom(group.Clients[LocalPlayer.ConnectionId]);
            group.Clients[LocalPlayer.ConnectionId] = LocalPlayer;
            _tempClients.Clear();
            foreach (var item in group.Clients) {
                _tempClients.Add(item.Key, item.Value);
            }
            group.Clients.Clear();
            await group.ProsessAsync();
            
            foreach (var item in _tempClients) {
                var client = item.Value;
                await client.EnterGroupAsync(group);
            }
            _enterGroupCallback.onSuccess?.Invoke(group);
        }
        private async void Prossesor_OnClientEnterGroupMessageAsync(byte[] message) {
            if (NetworkGroup == null) return;
            var msg = Message.Create(bytes: message);
            var newClient = msg.GetClass<T2>();
            if (newClient == null) {
                msg.Release();
                return;
            }
            await newClient.EnterGroupAsync(NetworkGroup);
            OnClientEnterGroup?.Invoke(newClient);
            msg.Release();
        }
        private async void Prossesor_OnClientExitGroupMessageAsync(byte[] message) {
            if (NetworkGroup == null) return;

            var msg = Message.Create(bytes: message);
            var id = msg.GetUShort();
            var client = NetworkGroup.Clients[id];
            await client.ExitGroupAsync(NetworkGroup);
            OnClientExitGroup?.Invoke(id);
            msg.Release();
        }
        private async void Prosessor_OnDisconnect(DisconnectReason reason) {
            var temp = NetworkGroup;
            NetworkGroup = null;
            LocalPlayer = null;
            State = StatusState.Disconnected;
            if (temp == null) return;
            INetworkEntity[] _tempClient = temp.Clients.Values.ToArray();
            for (int i = 0; i < _tempClient.Length; i++) {
                await _tempClient[i].ExitGroupAsync(temp);
            }

            await temp.RealeaseAsync();
        }
        private async void Prosessor_OnExitGroupMessageAsync(byte[] message) {
            var msg = Message.Create(bytes: message);
            var isSucces = msg.GetBool();
            var serverMessage = msg.GetString();

            if (!isSucces) {
                _exitGroupCallback.onError?.Invoke(serverMessage);
                return;
            }

            var temp = NetworkGroup;
            NetworkGroup = null;
            if (temp == null) {
                _exitGroupCallback.onError?.Invoke("Not on group");
                return;
            } 

            INetworkEntity[] _tempClient = temp.Clients.Values.ToArray();
            for (int i = 0; i < _tempClient.Length; i++) {
                await _tempClient[i].ExitGroupAsync(temp);
            }
            await temp.RealeaseAsync();
            _exitGroupCallback.onSuccess.Invoke();
        }
        private void Prossesor_OnCustomMessage(byte[] message) {
        }

    }

    public enum DisconnectReason {
        NeverConnected,
        ConnectionRejected,
        TransportError,
        TimedOut,
        Kicked,
        ServerStopped,
        Disconnected
    }
}
