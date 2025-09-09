using network.client.message;
using System.Threading.Tasks;

namespace network.client {
    public interface INetworkEntity : IMessageSerializable {
        ushort ConnectionId { get; }
        ushort Id { get; }
        bool IsLocal { get ; } 
        void CopyFrom(INetworkEntity other);
        Task EnterGroupAsync(INetworkGroup group);
        Task ExitGroupAsync(INetworkGroup group);
    }

    public abstract class NetworkEntity : INetworkEntity {
        public ushort ConnectionId { get; protected set; }
        public ushort Id { get; protected set; }
        public bool IsLocal { get => IClientManager.Instance.LocalPlayer == this; }

        public virtual void Serialize(IMessage message) {
            message.Add(ConnectionId);
            message.Add(Id);
        }
        public virtual void Deserialize(IMessage message) {
            ConnectionId = message.GetUShort();
            Id = message.GetUShort();
        }

        public virtual void CopyFrom(INetworkEntity other) {
            ConnectionId = other.ConnectionId;
            Id = other.Id;
        }

        public virtual Task EnterGroupAsync(INetworkGroup group) {
            return group.AddClient(this);
        }
        public virtual Task ExitGroupAsync(INetworkGroup group) {
            return group.RemoveClient(this);
        }
    }
}
