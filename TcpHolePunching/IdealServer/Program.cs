using System;
using System.Net;
using System.Net.Sockets;
using TcpHolePunching;

namespace IdealServer
{
    public class Program
    {
        private static NetworkPeer Incoming { get; set; }
        private static NetworkPeer Outgoing { get; set; }

        static void Main()
        {
            Console.Title = "Ideal Server - TCP Hole Punching Proof of Concept";

            Incoming = new NetworkPeer();

            Incoming.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            Incoming.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            Console.Write("Incoming: Bind to which port?: ");
            int portToBind = Int32.Parse(Console.ReadLine());
            Incoming.Bind(new IPEndPoint(IPAddress.Any, portToBind));
            Incoming.Listen();

            Console.WriteLine("Listening for clients on {0}...", Incoming.Socket.LocalEndPoint);
            Console.ReadLine();

            Outgoing = new NetworkPeer();

            Outgoing.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.NoDelay, true);
            Outgoing.Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            Console.Write("Outgoing: Bind to which port?: ");
            Outgoing.Bind(new IPEndPoint(IPAddress.Any, portToBind));
            Console.Write("Endpoint of your peer: ");

            var introducerEndpoint = Console.ReadLine().Parse();

            Console.WriteLine("Connecting to at {0}:{1}...", introducerEndpoint.Address, introducerEndpoint.Port);
            Outgoing.Connect(introducerEndpoint.Address, introducerEndpoint.Port);

            System.Windows.Forms.Application.Run();
        }
    }
}
