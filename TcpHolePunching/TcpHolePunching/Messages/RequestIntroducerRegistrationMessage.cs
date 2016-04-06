using System.Net;
using MsgPack;
using Newtonsoft.Json;
using TcpHolePunching.JsonConverters;

namespace TcpHolePunching.Messages
{
    public class RequestIntroducerRegistrationMessage : MessageBase, IPackable, IUnpackable
    {
        [JsonProperty]
        [JsonConverter(typeof(IpEndPointConverter))]
        public IPEndPoint InternalClientEndPoint { get; set; }

        public RequestIntroducerRegistrationMessage()
            : base(MessageType.RequestIntroducerRegistration)
        {
            MessageBytes = this.BinarySerialize();
        }

        public void PackToMessage(Packer packer, PackingOptions options)
        {
            throw new System.NotImplementedException();
        }

        public void UnpackFromMessage(Unpacker unpacker)
        {
            throw new System.NotImplementedException();
        }
    }
}
