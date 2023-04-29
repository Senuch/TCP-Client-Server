using System;
using System.Collections.Generic;
using Bindings;

namespace Server
{
    public class ServerHandleNetworkData
    {
        private delegate void Packet_(int index, byte[] data);
        private static Dictionary<int, Packet_> Packets;

        public static void InitializeNetworkPackages()
        {
            Console.WriteLine("Initialize Netowrk Packages");
            Packets = new Dictionary<int, Packet_>
            {
                {
                    (int)ClientPackets.CThankyou, 
                    HandleThankyou
                }
            };
        }

        public static void HandleNetworkInformation(int index, byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            int packetNumber = buffer.ReadInteger();
            buffer.Dispose();

            if (Packets.TryGetValue(packetNumber, out Packet_ packet))
            {
                packet.Invoke(index, data);
            }
        }

        private static void HandleThankyou(int index, byte[] data)
        {
            PacketBuffer buffer = new PacketBuffer();
            buffer.WriteBytes(data);
            buffer.ReadInteger();
            string msg = buffer.ReadString();
            buffer.Dispose();
            
            // Add code to execute...
            Console.WriteLine(msg);
        }
    }
}