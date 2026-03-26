using System.Runtime.CompilerServices;
using ENet;

namespace Network;

public class Net
{
    [ModuleInitializer]
    internal static void Init()
    {
        if (!Library.Initialize())
        {
            throw new Exception("Failed to initialize ENet");
        }
    }
}