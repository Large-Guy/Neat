using System.Text.Json;
using ENet;

namespace Neat;

public class Server
{
    private Connection _connection = new Connection();
    
    private static JsonSerializerOptions _options = new JsonSerializerOptions { IncludeFields = true };
    
    public Server(ushort port, int maxClients)
    {
        var address = new Address();
        address.Port = port;
        _connection.Host = new Host();
        _connection.Host.Create(address, maxClients);
    }
    
    ~Server()
    {
        foreach (var peer in _connection.Peers.Values)
        {
            peer.Disconnect(0);
        }
        _connection.Host?.Dispose();
    }
    
    public void Disconnect(uint id)
    {
        _connection.Peers[id].Disconnect(0);
    }
    
    public void DisconnectAll()
    {
        foreach (var peer in _connection.Peers.Values)
        {
            peer.Disconnect(0);
        }
    }

    public List<uint> GetClients()
    {
        return _connection.Peers.Keys.ToList();
    }
    
    public bool Listen(out object? e)
    {
        return _connection.Listen(out e, out _);
    }

    public bool Listen(out object? e, out uint? id)
    {
        return _connection.Listen(out e, out id);
    }

    public void Send(object data, uint target, bool reliable = false)
    {
        var wrapper = new
        {
            type = data.GetType().AssemblyQualifiedName,
            data = data
        };
        var bin = JsonSerializer.SerializeToUtf8Bytes(wrapper, _options);
        if (!_connection.Peers.TryGetValue(target, out var peer))
            throw new Exception("Peer not found");
        _connection.Send(bin, peer, reliable);
    }

    public void Broadcast(object data, bool reliable = false)
    {
        var wrapper = new
        {
            type = data.GetType().AssemblyQualifiedName,
            data = data
        };
        var bin = JsonSerializer.SerializeToUtf8Bytes(wrapper, _options);
        _connection.Broadcast(bin);
    }
}