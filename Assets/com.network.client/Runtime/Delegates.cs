using System;

namespace network.client {
    public delegate void ClientMessageReciver(byte[] message);
    public delegate void Disconnect(DisconnectReason reason);

    public struct Callback {
        public Action onSuccess;
        public Action<string> onError;
    }
    public struct Callback<T> {
        public Action<T> onSuccess;
        public Action<string> onError;

        public static Callback<T> Create(Action<T> onSucces, Action<string> onError) {
            return new Callback<T>();
        }
    }
}
