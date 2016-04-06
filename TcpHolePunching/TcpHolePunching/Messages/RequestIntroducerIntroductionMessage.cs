using System.Net;
using Newtonsoft.Json;
using TcpHolePunching.JsonConverters;

namespace TcpHolePunching.Messages
{
    public class RequestIntroducerIntroductionMessage : MessageBase
    {
        [JsonProperty]
        [JsonConverter(typeof(IpEndPointConverter))]
        public IPEndPoint InternalOwnEndPoint { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(IpEndPointConverter))]
        public IPEndPoint ExternalPeerEndPoint { get; set; }

        public RequestIntroducerIntroductionMessage()
            : base(MessageType.RequestIntroducerIntroduction)
        {
            base.MessageBytes = this.BinarySerialize();
        }
    }
}
