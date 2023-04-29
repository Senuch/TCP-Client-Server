using System;

namespace Client
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            ClientHandleNetworkData.InitializeNetworkPackages();
            ClientTcp.ConnectToServer();
            Console.ReadLine();
        }
    }
}