using System;
using System.Linq;
using System.Net;
using System.Threading;
using TcpHolePunching;
using TcpHolePunching.Messages;

namespace Introducer
{
    public class Program
    {
        private static readonly ManualResetEvent QuitEvent = new ManualResetEvent(false);

        private static NetworkIntroducer Introducer { get; set; }

        private static void Main()
        {
            Console.Title = "Introducer - TCP Hole Punching Proof of Concept";

            Introducer = new NetworkIntroducer();
            Introducer.OnConnectionAccepted += IntroducerOnConnectionAccepted;
            Introducer.OnMessageSent += IntroducerOnMessageSent;
            Introducer.OnMessageReceived += IntroducerOnMessageReceived;
            Introducer.Listen(new IPEndPoint(IPAddress.Any, 1618));

            Console.WriteLine("Listening for clients on {0}...", Introducer.Socket.LocalEndPoint);


            Console.CancelKeyPress += (sender, eArgs) =>
            {
                QuitEvent.Set();
                eArgs.Cancel = true;
            };

            // allow other threads to operate

            QuitEvent.WaitOne();
        }

        private static void IntroducerOnConnectionAccepted(object sender, ConnectionAcceptedEventArgs e)
        {
        }

        private static void IntroducerOnMessageSent(object sender, MessageSentEventArgs e)
        {
        }

        private static void IntroducerOnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.MessageType)
            {
                case MessageType.RequestIntroducerRegistration:
                    {
                        var message = e.Data.BinaryDeserialize<RequestIntroducerRegistrationMessage>();

                        // A client wants to register - note their internal endpoint
                        var internalEndPoint = message.InternalClientEndPoint;

                        // note the external endpoint too
                        var externalEndPoint = e.From;

                        Introducer.Registrants.Add(new Registrant
                                                       {
                                                           InternalEndPoint = internalEndPoint,
                                                           ExternalEndPoint = externalEndPoint
                                                       });

                        Introducer.Send(e.From, new ResponseIntroducerRegistrationMessage
                                                    {
                                                        RegisteredEndPoint = externalEndPoint
                                                    });
                    }
                    break;
                case MessageType.RequestIntroducerIntroduction:
                    {
                        var message = e.Data.BinaryDeserialize<RequestIntroducerIntroductionMessage>();

                        // A client, A, wants to be introduced to another peer, B
                        var bExternalEndPoint = message.ExternalPeerEndPoint;

                        // Get this peer's registration
                        var b =
                            Introducer.Registrants.First(
                                registrant => registrant.ExternalEndPoint.Equals(message.ExternalPeerEndPoint));

                        var a = new Registrant
                                    {InternalEndPoint = message.InternalOwnEndPoint, ExternalEndPoint = e.From};

                        Introducer.Send(a.ExternalEndPoint, new ResponseIntroducerIntroductionMessage
                                                                {
                                                                    InternalPeerEndPoint = b.InternalEndPoint,
                                                                    ExternalPeerEndPoint = b.ExternalEndPoint,
                                                                });

                        Introducer.Send(b.ExternalEndPoint, new ResponseIntroducerIntroductionMessage
                                                                {
                                                                    InternalPeerEndPoint = a.InternalEndPoint,
                                                                    ExternalPeerEndPoint = a.ExternalEndPoint,
                                                                });
                    }
                    break;
            }
        }
    }
}
