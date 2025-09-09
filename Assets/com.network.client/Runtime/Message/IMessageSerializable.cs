namespace network.client.message {
    public interface IMessageSerializable {
        void Serialize(IMessage message);
        void Deserialize(IMessage message);
    }

   

}