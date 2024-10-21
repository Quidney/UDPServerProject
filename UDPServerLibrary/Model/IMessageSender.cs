using System.Net;

namespace UDPServerLibrary.Model
{
    internal interface IMessageSender
    {
        Task SendMessage(IPEndPoint destination, string message);
    }
}