using network.client.message;
using System;

namespace network.client {
    public interface IClientProsessor {

        event Action OnConnectFailed;
        event ClientMessageReciver OnWelocomeMessage;
        event ClientMessageReciver OnEnterGroupMessage;
        event ClientMessageReciver OnClientEnterGroupMessage;
        event ClientMessageReciver OnClientExitGroupMessage;
        event ClientMessageReciver OnCustomMessage;
        event ClientMessageReciver OnNetworkComponentMessage;
        event ClientMessageReciver OnExitGroupMessage;
        event Disconnect OnDisconnect;

        void Connect(string ip, ushort port, string identifier = "");
        void Disconnect();
        void SendCustomMessage(IMessage message);
        void SendInputMessage(IMessage message);
        void SendMessageEnterGroup(IMessage message);
        void SendMessageExitGroup(IMessage message);
        void Tick();

        static IClientProsessor Instance { get; internal set; }
    }

}
