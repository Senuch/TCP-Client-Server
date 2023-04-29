using System;
using System.Net;
using System.Net.Sockets;
using Bindings;

namespace Server
{
    public static class ServerTcp
    {
        private static readonly Socket ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private static readonly Client[] Clients = new Client[Constants.MAX_PLAYERS];

        public static void SetupServer()
        {
            for (int i = 0; i < Clients.Length; i++)
            {
                Clients[i] = new Client();
            }
            
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, 5555));
            ServerSocket.Listen(10);
            ServerSocket.BeginAccept(AcceptCallback, null);
        }

        private static void AcceptCallback(IAsyncResult ar)
        {
            Socket socket = ServerSocket.EndAccept(ar);
            ServerSocket.BeginAccept(AcceptCallback, null);

            for (int i = 0; i < Constants.MAX_PLAYERS; i++)
            {
                if (Clients[i].Socket is null)
                {
                    Clients[i].Socket = socket;
                    Clients[i].Index = i;
                    Clients[i].Ip = socket.RemoteEndPoint.ToString();
                    Clients[i].StartClient();
                    Console.WriteLine("Connection from '{0}' received", Clients[i].Ip);
                    SendConnectionOk(i);
                    return;
                }
            }
        }

        private static void SendData(int index, byte[] data)
        {
            byte[] sizeInfo = new byte[4];
            sizeInfo[0] = (byte)data.Length;
            sizeInfo[1] = (byte)(data.Length >> 8);
            sizeInfo[2] = (byte)(data.Length >> 16);
            sizeInfo[3] = (byte)(data.Length >> 24);

            Clients[index].Socket.Send(sizeInfo);
            Clients[index].Socket.Send(data);
        }

        private static void SendConnectionOk(int index)
        {
            var buffer = new PacketBuffer();
            buffer.WriteInteger((int)ServerPackets.SConnectionOk);
            buffer.WriteString("You are connected to the server.");
            SendData(index, buffer.ToArray());
            buffer.Dispose();
        }
    }

    public class Client
    {
        public int Index;
        public string Ip;
        public Socket Socket;
        private readonly byte[] _buffer = new byte[1024];

        public void StartClient()
        {
            Socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, Socket);
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket socket = (Socket)ar.AsyncState;

            try
            {
                int received = socket.EndReceive(ar);
                if (received <= 0)
                {
                    CloseClient();
                }
                else
                {
                    byte[] dataBuffer = new byte[received];
                    Array.Copy(_buffer, dataBuffer, received);
                    // Handle Network Information
                    ServerHandleNetworkData.HandleNetworkInformation(Index, dataBuffer);
                    socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, socket);
                }
            }
            catch
            {
                CloseClient();
            }
        }

        private void CloseClient()
        {
            Console.WriteLine("Connection from {0} has been terminated.", Ip);
            Socket.Close();
        }
    }
}