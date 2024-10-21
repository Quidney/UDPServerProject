using System.Net;
using System.Text;
using UDPServerLibrary.Model;

using Encoder = UDPServerLibrary.Utils.Encoder;
using RNG = System.Security.Cryptography.RandomNumberGenerator;

namespace UDPTesterConsole
{
    internal class Program
    {
        static List<string> Colors =
        [
            "Black",
            "White",
            "Red",
            "Green",
            "Blue",
            "Yellow",
            "Cyan",
            "Magenta"
        ];

        static UdpNode Server = null!;

        static async Task Main()
        {
            IEncoder encoder = new Encoder(Encoding.UTF8);

            Server = new(encoder);
            Server.MessageReceived += Server_MessageReceived;

            _ = Task.Run(ColorChanger);

            await Server.StartListening(23345);
        }

        private static async void ColorChanger()
        {
            int count = 0;
            while (true)
            {
                Console.WriteLine($"Iteration '{count}'");
                int index = RNG.GetInt32(Colors.Count);
                string color = Colors[index];

                if (await Server.BroadcastMessage(color))
                    count++;
            }
        }

        private static void Server_MessageReceived(IPEndPoint sender, string dataJson)
        {
            try
            {
                Package pkg = Package.Deserialize(dataJson);
                Command command = pkg.Command;
                string data = pkg.Data;

                switch (command)
                {
                    case Command.CONNECT:
                        int port = int.Parse(data);
                        Client client = new(sender.Address, port);
                        Server.AddClient(client);
                        Console.WriteLine($"Client connected: {sender.Address}:{port}");
                        break;

                    case Command.DISCONNECT:
                        Server.RemoveClient(new(sender.Address, int.Parse(data)));
                        Console.WriteLine($"Client disconnected: {sender.Address}:{data}");
                        break;
                }

            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Invalid command received ('')");
            }
        }
    }
}
