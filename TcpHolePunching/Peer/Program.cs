using System;
using System.Net;
using System.Threading;
using TcpHolePunching;
using TcpHolePunching.Messages;

namespace Peer
{
    public class Program
    {
        static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static NetworkPeer IntroducerSocket { get; set; }
        private static NetworkPeer ListenSocket { get; set; }
        private static NetworkPeer ConnectSocketInternal { get; set; }
        private static NetworkPeer ConnectSocketExternal { get; set; }

        private static int PORT = 53472;

        static void Main(string[] args)
        {
            Console.Title = "Peer - TCP Hole Punching Proof of Concept";

            ListenSocket = new NetworkPeer();
            ListenSocket.OnConnectionAccepted += (s, e1) => Console.WriteLine("ListenSocket.OnConnectionAccepted");
            ListenSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));
            ListenSocket.Listen();
            Console.WriteLine("Listening for clients on {0}...", ListenSocket.Socket.LocalEndPoint);

            IntroducerSocket = new NetworkPeer();
            IntroducerSocket.OnConnectionAccepted += Peer_OnConnectionAccepted;
            IntroducerSocket.OnConnectionSuccessful += PeerOnConnectionSuccessful;
            IntroducerSocket.OnMessageSent += PeerOnMessageSent;
            IntroducerSocket.OnMessageReceived += Peer_OnMessageReceived;

            IntroducerSocket.Bind(new IPEndPoint(IPAddress.Any, PORT));

            Console.Write("Endpoint of the introducer (try 178.62.120.34:1618): ");

            var input = Console.ReadLine();
            input = (String.IsNullOrEmpty(input)) ? "178.62.120.34:1618" : input;
            var introducerEndpoint = input.Parse();

            Console.WriteLine("Connecting to the Introducer at {0}:{1}...", introducerEndpoint.Address, introducerEndpoint.Port);
            IntroducerSocket.Connect(introducerEndpoint.Address, introducerEndpoint.Port);



            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            // allow other threads to operate

            QuitEvent.WaitOne();
        }

        static void Peer_OnConnectionAccepted(object sender, ConnectionAcceptedEventArgs e)
        {
            Console.WriteLine();
        }

        static void PeerOnConnectionSuccessful(object sender, ConnectionAcceptedEventArgs e)
        {
            Console.WriteLine();
            Console.WriteLine("Requesting to register with the Introducer...");
            IntroducerSocket.Send(new RequestIntroducerRegistrationMessage { InternalClientEndPoint = (IPEndPoint) e.Socket.LocalEndPoint} );
        }

        static void PeerOnMessageSent(object sender, MessageSentEventArgs e)
        {
        }

        static void Peer_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.MessageType)
            {
                case MessageType.ResponseIntroducerRegistration:
                    {
                        var message = new ResponseIntroducerRegistrationMessage();
                        message.ReadPayload(e.MessageReader);

                        Console.WriteLine("Introducer: You have been registered as \"{0}\".", message.RegisteredEndPoint);

                        Console.Write("Endpoint of your peer: ");

                        var peerEndPoint = Console.ReadLine().Parse();

                        Console.WriteLine("Requesting an introduction to {0}:{1}...", peerEndPoint.Address, peerEndPoint.Port);
                        IntroducerSocket.Send(new RequestIntroducerIntroductionMessage() { InternalOwnEndPoint = (IPEndPoint) IntroducerSocket.Socket.LocalEndPoint, ExternalPeerEndPoint = peerEndPoint} );
                    }
                    break;
                case MessageType.ResponseIntroducerIntroduction:
                    {
                        var message = new ResponseIntroducerIntroductionMessage();
                        message.ReadPayload(e.MessageReader);

                        Console.WriteLine("Introducer: Your peer's internal endpoint is \"{0}\".", message.InternalPeerEndPoint);
                        Console.WriteLine("Introducer: Your peer's external endpoint is \"{0}\".", message.ExternalPeerEndPoint);

                        ConnectSocketInternal = new NetworkPeer();
                        ConnectSocketInternal.Bind(new IPEndPoint(IPAddress.Any, PORT));
                        Console.WriteLine("Connecting to your peer's internal endpoint...");
                        ConnectSocketInternal.OnConnectionSuccessful += (s, e1) => Console.WriteLine("ConnectSocketInternal.OnConnectionSuccessful");
                        ConnectSocketInternal.Connect(message.InternalPeerEndPoint.Address, message.InternalPeerEndPoint.Port);

                        ConnectSocketExternal = new NetworkPeer();
                        ConnectSocketExternal.Bind(new IPEndPoint(IPAddress.Any, PORT));
                        Console.WriteLine("Connecting to your peer's external endpoint...");
                        ConnectSocketExternal.OnConnectionSuccessful += (s, e1) => Console.WriteLine("ConnectSocketExternal.OnConnectionSuccessful");
                        ConnectSocketExternal.Connect(message.ExternalPeerEndPoint.Address, message.ExternalPeerEndPoint.Port);
                    }
                    break;
            }
        }
    }
}
