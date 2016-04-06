using System.Net;
using Newtonsoft.Json;
using TcpHolePunching.JsonConverters;

namespace TcpHolePunching.Messages
{
    public class ResponseIntroducerRegistrationMessage : MessageBase
    {
        [JsonProperty]
        [JsonConverter(typeof(IpEndPointConverter))]
        public IPEndPoint RegisteredEndPoint { get; set; }

        public ResponseIntroducerRegistrationMessage()
            : base(MessageType.ResponseIntroducerRegistration)
        {
            base.MessageBytes = this.BinarySerialize();
        }
    }
}
