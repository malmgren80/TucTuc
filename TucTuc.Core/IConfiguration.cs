namespace TucTuc
{
    public interface IConfiguration
    {
        ITransport Transport { get; }
        ISerializer Serializer { get; }
    }
}