using System.Net;
using System.Net.Sockets;
using UDPServerLibrary.Model;

namespace UDPServerLibrary
{
    public class UdpMessageSender : IMessageSender, IDisposable
    {
        public IEncoder Encoder { get; }
        private readonly UdpClient Client;

        public UdpMessageSender(IEncoder encoder)
        {
            Encoder = encoder;
            Client = new UdpClient();
        }

        public async Task SendMessage(IPEndPoint destination, string message, CancellationToken token = default)
            => await SendMessage(destination, Encoder.StringToBytes(message), token);

        public async Task SendMessage(IPEndPoint destination, ReadOnlyMemory<byte> data, CancellationToken token = default)
            => await Client.SendAsync(data, destination, cancellationToken: token);

        public void Dispose()
        {
            Client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}