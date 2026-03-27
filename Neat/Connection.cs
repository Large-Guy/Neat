using System.Text.Json;
using ENet;

namespace Neat;

internal class Connection
{
    public Host? Host = null;
    
    public Dictionary<uint, Peer> Peers = [];
    
    public bool Listen(out object? e, out uint? id)
    {
        if(Host == null)
            throw new Exception("Host not created");
        
        if (Host.Service(0, out var @event) <= 0)
        {
            e = null;
            id = null;
            return false;
        }
        
        switch(@event.Type)
        {
            case EventType.Connect:
            {
                e = new Connect
                {
                    Ip = @event.Peer.IP
                };
                Peers.Add(@event.Peer.ID, @event.Peer);
                id = @event.Peer.ID;
                return true;
            }
            case EventType.Receive:
            {
                var buffer = new byte[@event.Packet.Length];
                @event.Packet.CopyTo(buffer);
                @event.Packet.Dispose();

                using var doc = JsonDocument.Parse(buffer);
                var root = doc.RootElement;
                
                var typeName = root.GetProperty("type").GetString() ?? throw new Exception("No type");
                var data = root.GetProperty("data");
                
                var type = Type.GetType(typeName) ?? throw new Exception("Unable to find type");
                
                var obj = data.Deserialize(type) ?? throw new Exception("Unable to deserialize");

                e = obj;
                id = @event.Peer.ID;
                
                return true;
            }
            case EventType.Disconnect:
            {
                Peers.Remove(@event.Peer.ID);
                e = new Disconnect
                {
                };
                id = @event.Peer.ID;
                return true;
            }
            case EventType.Timeout:
            {
                e = new Timeout
                {
                };
                id = @event.Peer.ID;
                return true;
            }
            case EventType.None:
            default:
            {
                e = null;
                id = null;
                return false;
            }
        }
    }

    public void Send(byte[] data, Peer target, bool reliable)
    {
        var packet = new Packet();
        packet.Create(data, reliable ? PacketFlags.Reliable : PacketFlags.None);
        target.Send(0, ref packet);
    }
    
    public void Broadcast(byte[] data, bool reliable = false)
    {
        if(Host == null)
            throw new Exception("Host not created");
        
        var packet = new Packet();
        packet.Create(data, reliable ? PacketFlags.Reliable : PacketFlags.None);
        
        Host?.Broadcast(0, ref packet);
    }
}