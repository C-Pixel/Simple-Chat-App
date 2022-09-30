using ChatServer;
using ChatServer.Net.IO;
using System;
using System.Net;
using System.Net.Sockets;

public class Program
{
    static List<Client> users;
    static TcpListener listener;
    static void Main(string[] args)
    {
        users = new List<Client>();
        listener = new TcpListener(IPAddress.Parse("192.168.0.23"), 9898);
        listener.Start();

        while (true)
        {
            var client = new Client(listener.AcceptTcpClient());
            users.Add(client);

            BroadcastConnection();
        }

        static void BroadcastConnection()
        {
            foreach (var user in users)
            {
                foreach (var usr in users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.UID.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
    }

    public static void BroadcastMessage(string message)
    {
        foreach (var user in users)
        {
            var msgPacket = new PacketBuilder();
            msgPacket.WriteOpCode(5);
            msgPacket.WriteMessage(message);
            user.ClientSocket.Client.Send(msgPacket.GetPacketBytes());
        }
    }

    public static void BroadcastDisconnect(string uid)
    {
        var disconnectedUser = users.Where(x => x.UID.ToString() == uid).FirstOrDefault();
        users.Remove(disconnectedUser);

        foreach (var user in users)
        {
            var broadcastPacket = new PacketBuilder();
            broadcastPacket.WriteOpCode(10);
            broadcastPacket.WriteMessage(uid);
            user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
        }

        BroadcastMessage($"[{disconnectedUser.Username}] Disconnected!");
    }
}

