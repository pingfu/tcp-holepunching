using System.Collections.Generic;
using System.Net;

namespace TcpHolePunching.Messages
{
    public class AnnounceConnectedPeers : MessageBase
    {
        public IPEndPoint InternalOwnEndPoint { get; set; }
        public IPEndPoint ExternalPeerEndPoint { get; set; }
        public List<IPEndPoint> AvailableEndPoints { get; set; }

        public AnnounceConnectedPeers()
            : base(MessageType.RequestIntroducerIntroduction)
        {
            base.MessageBytes = this.BinarySerialize();
        }
    }
}
