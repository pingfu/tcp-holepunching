using System;
using System.Net;
using TcpHolePunching.Messages;

namespace TcpHolePunching
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public IPEndPoint From { get; set; }
        public MessageType MessageType { get; set; }
        public byte[] Data { get; set; }
    }
}
