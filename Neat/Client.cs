using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using ENet;

namespace Network;

public class Client
{
    private Connection _connection = new Connection();
    private Peer? _server = null;
    
    public Client(string ip, ushort port)
    {
        _connection.Host = new Host();
        _connection.Host.Create(null, 1, 2, 0, 0);
        var address = new Address();
        if(!ip.IsWhiteSpace())
            address.SetHost(ip);
        else
            address.SetHost("localhost");
        address.Port = port;
        _server = _connection.Host.Connect(address);
        
        _connection.Host.Flush();
    }

    ~Client()
    {
        Disconnect();
    }

    public void Disconnect()
    {
        _server?.Disconnect(0);
        _connection.Listen(out _);
    }
    
    public bool Listen(out object? e)
    {
        return _connection.Listen(out e);
    }

    public void Send(object data, bool reliable = false)
    {
        var wrapper = new
        {
            type = data.GetType().AssemblyQualifiedName,
            data = data
        };
        var bin = JsonSerializer.SerializeToUtf8Bytes(wrapper);
        _connection.Send(bin, _server ?? throw new Exception("Peer not created"), reliable);
    }
}