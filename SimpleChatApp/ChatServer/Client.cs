using ChatServer.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Client
    {
        PacketReader packetReader;

        public Client(TcpClient client)
        {
            this.ClientSocket = client;
            this.UID = Guid.NewGuid();
            packetReader = new PacketReader(ClientSocket.GetStream());

            var opCode = packetReader.ReadByte();
            Username = packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has conneceted with the username: {Username}");

            Task.Run(() => Process());
        }

        public string Username { get; set; }
        public Guid UID { get; set; }
        public TcpClient ClientSocket { get; set; }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = packetReader.ReadByte();
                    switch (opcode)
                    {
                        case 5:
                            var msg = packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}]: Message received: {msg}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {msg}");
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine($"[{Username.ToString()}]: Disconnected!");
                    Program.BroadcastDisconnect(UID.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
