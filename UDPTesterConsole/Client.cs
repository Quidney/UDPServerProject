using System.Net;

namespace UDPTesterConsole
{
    public class Client(IPAddress ip, int port)
    {
        public IPAddress IP { get; set; } = ip;
        public int Port { get; set; } = port;
    }
}
