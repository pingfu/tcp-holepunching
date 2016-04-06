using System.Net;
using Newtonsoft.Json;
using TcpHolePunching.JsonConverters;

namespace TcpHolePunching.Messages
{
    public class ResponseIntroducerIntroductionMessage : MessageBase
    {
        [JsonProperty]
        [JsonConverter(typeof(IpEndPointConverter))]
        public IPEndPoint InternalPeerEndPoint { get; set; }

        [JsonProperty]
        [JsonConverter(typeof(IpEndPointConverter))]
        public IPEndPoint ExternalPeerEndPoint { get; set; }

        public ResponseIntroducerIntroductionMessage()
            : base(MessageType.ResponseIntroducerIntroduction)
        {
            base.MessageBytes = this.BinarySerialize();
        }
    }
}
