using System.Net;
using System.Net.Sockets;
using UDPServerLibrary.Model;

namespace UDPServerLibrary
{
    public class UdpMessageSender : IMessageSender, IDisposable
    {
        private UdpClient Client;
        private readonly IEncoder Encoder;

        public UdpMessageSender(IEncoder encoder)
        {
            Encoder = encoder;
            Client = new UdpClient();
        }

        public async Task SendMessage(IPEndPoint destination, string message)
        {
            ReadOnlyMemory<byte> data = Encoder.StringToBytes(message);
            await Client.SendAsync(data, destination);
        }

        public void Dispose() => Client?.Dispose();
    }
}