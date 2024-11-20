using System.Net;

namespace UDPServerLibrary.Model
{
    internal interface IMessageSender
    {
        IEncoder Encoder { get; }
        Task SendMessage(IPEndPoint destination, string message, CancellationToken token = default);
    }
}