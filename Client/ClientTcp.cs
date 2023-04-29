using System;
using System.Net.Sockets;
using Bindings;

namespace Client
{
    public static class ClientTcp
    {
        private static readonly Socket ClientSocket =
            new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        public static void ConnectToServer()
        {
            Console.WriteLine("Connecting to server...!");
            ClientSocket.BeginConnect("127.0.0.1", 5555, ConnectCallback, ClientSocket);
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            ClientSocket.EndConnect(ar);

            while (true)
            {
                OnReceive();
            }
        }

        private static void OnReceive()
        {
            byte[] sizeInfo = new byte[4];

            try
            {
                int totalRead;
                var currentRead = totalRead = ClientSocket.Receive(sizeInfo);
                if (totalRead <= 0)
                {
                    Console.WriteLine("You are not connected to the server.");
                }
                else
                {
                    while (totalRead < sizeInfo.Length && currentRead > 0)
                    {
                        currentRead = ClientSocket.Receive(sizeInfo, totalRead, sizeInfo.Length - totalRead,
                            SocketFlags.None);
                        totalRead += currentRead;
                    }

                    int messageSize = 0;
                    messageSize |= sizeInfo[0];
                    messageSize |= (sizeInfo[1] << 8);
                    messageSize |= (sizeInfo[2] << 16);
                    messageSize |= (sizeInfo[3] << 24);

                    byte[] data = new byte[messageSize];
                    totalRead = 0;
                    currentRead = totalRead =
                        ClientSocket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);
                    while (totalRead < messageSize && currentRead > 0)
                    {
                        currentRead = ClientSocket.Receive(data, totalRead, data.Length - totalRead, SocketFlags.None);
                        totalRead += currentRead;
                    }
                    
                    // Handle Network Information
                    ClientHandleNetworkData.HandleNetworkInformation(data);
                }
            }
            catch
            {
                Console.WriteLine("You are not connected to the server.");
            }
        }

        private static void SendData(byte[] data)
        {
            ClientSocket.Send(data);
        }

        public static void ThankyouServer()
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteInteger((int)ClientPackets.CThankyou);
            buffer.WriteString("Thank you for connection");
            SendData(buffer.ToArray());
            buffer.Dispose();
        }
    }
}