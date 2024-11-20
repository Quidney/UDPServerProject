using System.Net;

namespace UDPServerLibrary.Model
{
    internal interface IListener
    {
        event Action<IPEndPoint, byte[]>? MessageReceived;
        event Action<IPEndPoint, string>? MessageReceivedBase64String;
        Task StartListening(int port);
        Task StopListening();
    }
}