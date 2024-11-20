using System.Net;
using UDPServerLibrary;
using UDPServerLibrary.Model;

namespace UDPTesterConsole
{
    public class UdpNode
    {
        public event Action<IPEndPoint, byte[]>? MessageReceived
        {
            add => Listener.MessageReceived += value;
            remove => Listener.MessageReceived -= value;
        }

        List<Client> Clients;

        readonly UdpListener Listener;
        readonly UdpMessageSender Sender;

        public void AddClient(Client client) => Clients.Add(client);
        public void RemoveClient(Client client) => Clients.Remove(client);

        public UdpNode(IEncoder encoder)
        {
            Clients = [];
            Listener = new UdpListener(encoder);
            Sender = new UdpMessageSender(encoder);
        }
        public async Task<bool> BroadcastMessage(string message)
        {
            if (Clients.Count == 0)
            {
                Console.WriteLine($"Cannot broadcast '{message}' because there are no clients.");
                return false;
            }

            Console.WriteLine($"Broadcasting '{message}' to '{Clients.Count}' clients.");

            foreach (Client client in Clients)
            {
                IPEndPoint ipEP = new(client.IP, client.Port);
                await SendMessage(ipEP, message);
            }
            return true;
        }
        public async Task SendMessage(IPEndPoint ipEp, Command cmd, string data) => await SendMessage(ipEp, new Package(cmd, data));
        public async Task SendMessage(IPEndPoint ipEp, Package pkg) => await SendMessage(ipEp, pkg.Serialize());
        public async Task SendMessage(IPEndPoint ipEp, string packageJson) => await Sender.SendMessage(ipEp, packageJson);
        public async Task StartListening(int port) => await Listener.StartListening(port);

        public async Task StopListening() => await Listener.StopListening();
    }
}
