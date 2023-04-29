using System;
using System.Collections.Generic;
using Bindings;

namespace Client
{
    public class ClientHandleNetworkData
    {
        private delegate void Packet_(byte[] data);
        private static Dictionary<int, Packet_> Packets;

        public static void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Netowrk Packages");
            Packets = new Dictionary<int, Packet_>
            {
                {
                    (int)ServerPackets.SConnectionOk, 
                    HandleConnectionOk
                }
            };
        }

        public static void HandleNetworkInformation(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            int packetNumber = buffer.ReadInteger();
            buffer.Dispose();

            if (Packets.TryGetValue(packetNumber, out Packet_ packet))
            {
                packet.Invoke(data);
            }
        }

        private static void HandleConnectionOk(byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            
            // Add code to execute...
            Console.WriteLine(msg);
            
            ClientTcp.ThankyouServer();
        }
    }
}