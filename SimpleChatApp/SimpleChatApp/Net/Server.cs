using ChatClient.Net.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.Net
{
    public class Server
    {
        TcpClient client;
        public PacketReader PacketReader;

        public event Action connectedEvent;
        public event Action msgReceivedEvent;
        public event Action userDisconnectEvent;

        public Server()
        {
            client = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if (!client.Connected)
            {
                client.Connect("192.168.0.23", 9898);
                PacketReader = new PacketReader(client.GetStream());

                if (!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    client.Client.Send(connectPacket.GetPacketBytes());
                }

                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var opcode = PacketReader.ReadByte();
                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;

                        case 5:
                            msgReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("ah yes...");
                            break;
                    }
                }
            });
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteMessage(message);
            messagePacket.WriteOpCode(5);
            client.Client.Send(messagePacket.GetPacketBytes());
        }
    }
}
