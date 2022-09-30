using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer.Net.IO
{
    public class PacketBuilder
    {
        MemoryStream ms;

        public PacketBuilder()
        {
            ms = new MemoryStream();
        }

        public void WriteOpCode(byte opcode)
        {
            ms.WriteByte(opcode);
        }

        public void WriteMessage(string msg)
        {
            var msgLenght = msg.Length;
            ms.Write(BitConverter.GetBytes(msgLenght));
            ms.Write(Encoding.ASCII.GetBytes(msg));
        }

        public byte[] GetPacketBytes()
        {
            return ms.ToArray();
        }
    }
}
