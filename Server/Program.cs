using System;

namespace Server
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ServerHandleNetworkData.InitializeNetworkPackages();
            ServerTcp.SetupServer();
            Console.ReadLine();
        }
    }
}