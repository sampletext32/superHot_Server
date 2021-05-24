using System;
using System.Collections.Generic;
using System.Linq;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Newtonsoft.Json.Linq;

namespace ServerApp
{
    public class RemoteClient
    {
        public Connection Connection { get; private set; }
        public DateTime LastRequestTime { get; private set; }

        private void OnInnerDisconnect(Connection conn)
        {
            Console.WriteLine("RC Inner Disconnected Call");
        }

        private void OnMessage(PacketHeader header, Connection conn, string incomingData)
        {
            Console.WriteLine(incomingData);
        }

        public void Kick(string reason)
        {
            Connection.SendObject("Kick", reason);
        }

        public void SetConnection(Connection connection)
        {
            Connection = connection;
            connection.AppendShutdownHandler(OnInnerDisconnect);
            connection.AppendIncomingPacketHandler<string>("Message", OnMessage);
        }

        public RemoteClient()
        {
            LastRequestTime = DateTime.Now;
        }
    }
}
