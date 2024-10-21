using System.Text.Json;

namespace UDPTesterConsole
{
    public class Package(Command command, string data)
    {
        public Command Command { get; private set; } = command;
        public string Data { get; private set; } = data;

        public string Serialize() => JsonSerializer.Serialize(this);
        public static Package Deserialize(string json) => JsonSerializer.Deserialize<Package>(json) ?? throw new InvalidDataException("Given json is invalid.");
    }

    public enum Command
    {
        CONNECT,
        DISCONNECT,
        COLOR
    }
}
