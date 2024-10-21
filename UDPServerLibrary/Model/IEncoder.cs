namespace UDPServerLibrary.Model
{
    public interface IEncoder
    {
        string BytesToString(byte[] data);
        byte[] StringToBytes(string data);
    }
}