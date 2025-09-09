using assetmanager;
using game.actors;
using network.client;
using network.client.component;
using network.client.message;
using System.Threading.Tasks;

namespace mrpg.client {
    public class MRPG_NetworkEntity : NetworkEntity {
        public MRPG_Player Player { get; set; }
        public string StringData { get; private set; }

        private NetworkTransform _networkTransform;
        private NetworkAnimator _networkAnimator;

        public override void CopyFrom(INetworkEntity other) {
            base.CopyFrom(other);
            StringData = ((MRPG_NetworkEntity)other).StringData;
        }
        public override async Task EnterGroupAsync(INetworkGroup group) {

            var playerObject = await AssetManager.InstantiateAsync(StringData);
            Player = await ActorUltis.CreatePlayerAsync<MRPG_Player>(playerObject);
            Player.NetworkEntity = this;

            if (IsLocal) {
                Player.AddModule<InputSenderModule>(10);
            }

            Player.Initialize();

            if (IsLocal) {
                Player.AddCameraPlayer();
            }
            Player.Animator.applyRootMotion = false;

            _networkTransform = Player.Root.AddComponent<NetworkTransform>()
                .Initialize($"player{ConnectionId}") as NetworkTransform;
            _networkAnimator = Player.Root.AddComponent<NetworkAnimator>()
                .Initialize($"player{ConnectionId}", Player.Animator) as NetworkAnimator;

            await base.EnterGroupAsync(group);
        }
        public override Task ExitGroupAsync(INetworkGroup group) {
            _networkTransform.Release();
            _networkAnimator.Release();
            Player.Destroy();
            return base.ExitGroupAsync(group);
        }
        public override void Serialize(IMessage message) {
            base.Serialize(message);
            message.Add(StringData);
        }
        public override void Deserialize(IMessage message) {
            base.Deserialize(message);
            StringData = message.GetString();
        }
    }
}
