using Network;

namespace TestServer;

class Program
{
    static void Main(string[] args)
    {
        var server = new Server(1234, 10);

        while (true)
        {
            while(server.Listen(out var @event))
            {
                switch (@event)
                {
                    case Connect connect:
                    {
                        Console.WriteLine($"Client {connect.Id} connected from {connect.Ip}");
                        server.Send($"Hello Client {connect.Id}", connect.Id);
                        server.Broadcast("Hey guys, we got a new client here!");
                        break;
                    }
                    case Disconnect disconnect:
                    {
                        Console.WriteLine($"Client {disconnect.Id} disconnected");
                        break;
                    }
                    case string str:
                    {
                        Console.WriteLine(str);
                        server.Broadcast(str);
                        break;
                    }
                }
            }
            
        }
    }
}