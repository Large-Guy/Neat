using Neat;

namespace TestClient;

class Program
{
    static void Main(string[] args)
    {
        var client = new Client("", 1234);
        while (true)
        {
            while (client.Listen(out var @event))
            {
                switch (@event)
                {
                    case Connect connect:
                    {
                        Console.WriteLine($"Connected to {connect.Ip}");
                        break;
                    }

                    case Disconnect:
                    {
                        Console.WriteLine("Disconnected from server");
                        break;
                    }

                    case string str:
                    {
                        Console.WriteLine(str);
                        break;
                    }
                }
            }

            var message = Console.ReadLine() ?? "";
            if (message == "exit")
                break;
            client.Send(message);
        }
        client.Disconnect();
    }
}