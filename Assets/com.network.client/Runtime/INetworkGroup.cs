using network.client.message;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace network.client {
    public interface INetworkGroup : IMessageSerializable {
        string Name { get; }
        ushort Id { get; }
        Dictionary<ushort, INetworkEntity> Clients { get; }

        Task ProsessAsync();
        Task AddClient(INetworkEntity networkEntity);
        Task RemoveClient(INetworkEntity networkEntity);
        Task RealeaseAsync();
    }

}
