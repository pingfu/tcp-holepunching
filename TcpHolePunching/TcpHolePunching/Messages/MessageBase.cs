namespace TcpHolePunching.Messages
{
    public abstract class MessageBase
    {
        public MessageType MessageType { get; set; }
        public byte[] MessageBytes { get; set; }

        protected MessageBase(MessageType messageType)
        {
            MessageType = messageType;
        }
    }

    public enum MessageType
    {
        Internal,
        RequestIntroducerRegistration,
        ResponseIntroducerRegistration,
        RequestIntroducerIntroduction,
        ResponseIntroducerIntroduction,
    }
}
