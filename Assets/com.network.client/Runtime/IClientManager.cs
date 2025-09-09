using network.client.message;

namespace network.client {
    public interface IClientManager {
        string Ip { get; set; }
        ushort Port { get; set; }
        INetworkEntity LocalPlayer { get; }
        StatusState State { get; }

        void Connect(string identifier = "", Callback callback = default);
        void Tick();
        void Disconnect();
        void EnterGroup(string nameGroup, ushort id = 0, Callback<INetworkGroup> callback = default);
        void ExitGroup(Callback callback = default);
        void SendCustomMessage(IMessage message);
        void SendInputMessage(IMessage message);

        static IClientManager Instance;
    }

    public enum StatusState {
        Disconnected,
        Connecting,
        Connected,
    }
}
