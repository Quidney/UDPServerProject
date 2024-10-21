using System.Text;
using UDPServerLibrary.Model;

namespace UDPServerLibrary.Utils
{
    public class Encoder : IEncoder
    {
        public Encoding Encoding { get; }
        public Encoder(Encoding encoding)
        {
            Encoding = encoding;
        }

        public byte[] StringToBytes(string data) => Encoding.GetBytes(data);

        public string BytesToString(byte[] data)
        {
            ArgumentNullException.ThrowIfNull(data);
            return Encoding.GetString(data);
        }
    }
}