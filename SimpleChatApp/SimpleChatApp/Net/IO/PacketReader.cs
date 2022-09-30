using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream ns;

        public PacketReader(NetworkStream ns)
            : base(ns)
        {
            this.ns = ns;
        }

        public string ReadMessage()
        {
            byte[] msgBuffer;
            var lenght = ReadInt32();
            msgBuffer = new byte[lenght];
            ns.Read(msgBuffer, 0, lenght);

            var msg = Encoding.ASCII.GetString(msgBuffer);

            return msg;
        }
    }
}
