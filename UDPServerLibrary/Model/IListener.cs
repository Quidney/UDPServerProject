using System.Net;

namespace UDPServerLibrary.Model
{
    internal interface IListener
    {
        event Action<IPEndPoint, string>? MessageReceived;
        Task StartListening(int port);
        Task StopListening();
    }
}