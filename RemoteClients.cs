using System;
using System.Collections.Generic;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Newtonsoft.Json.Linq;
using ServerApp;
using System.Timers;
using Timer = System.Threading.Timer;

namespace superHot_Server
{
    public enum GameState
    {
        Playing,
        Updating
    }
    public static class RemoteClients
    {
        private static Connection _firstPlayer;
        private static Connection _secondPlayer;
        private static GameState _currentGameState = GameState.Updating;

        public static void OnConnect(Connection conn)
        {
            conn.AppendIncomingPacketHandler<string>("GetGameState", OnGetGameState);

            if (_firstPlayer == null)
            {
                _firstPlayer = conn;
            }
            else
            {
                _secondPlayer = conn;

                _firstPlayer.AppendIncomingPacketHandler<string>("Shoot", OnShoot1);
                _secondPlayer.AppendIncomingPacketHandler<string>("Shoot", OnShoot2);

                _firstPlayer.AppendIncomingPacketHandler<string>("Move", OnMove1);
                _secondPlayer.AppendIncomingPacketHandler<string>("Move", OnMove2);

                _firstPlayer.AppendIncomingPacketHandler<string>("Rotate", OnRotate1);
                _secondPlayer.AppendIncomingPacketHandler<string>("Rotate", OnRotate2);

                _firstPlayer.AppendIncomingPacketHandler<string>("Quit", OnQuit1);
                _secondPlayer.AppendIncomingPacketHandler<string>("Quit", OnQuit2);

            }

            Console.WriteLine("OnConnect");
        }

        private static void OnQuit2(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _firstPlayer?.SendObject("Quit", incomingobject);
        }

        private static void OnQuit1(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _secondPlayer?.SendObject("Quit", incomingobject);
        }

        private static void OnRotate2(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _firstPlayer?.SendObject("Rotate", incomingobject);
        }

        private static void OnRotate1(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _secondPlayer?.SendObject("Rotate", incomingobject);
        }

        private static void OnMove2(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _firstPlayer?.SendObject("Move", incomingobject);
        }

        private static void OnMove1(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _secondPlayer?.SendObject("Move", incomingobject);
        }

        private static void OnShoot1(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _secondPlayer?.SendObject("Shoot", incomingobject);
        }

        private static void OnShoot2(PacketHeader packetheader, Connection connection, string incomingobject)
        {
            _firstPlayer?.SendObject("Shoot", incomingobject);
        }

        public static void KickEveryOne(string reason)
        {
            _firstPlayer.SendObject("Kick", reason);
            _secondPlayer.SendObject("Kick", reason);
        }

        public static void SetGameState(GameState gameState)
        {
            _currentGameState = gameState;
        }

        private static void OnGetGameState(PacketHeader header, Connection connection, string incomingObject)
        {
            connection.SendObject(header.PacketType + "Res", new JObject { { "gamestate", _currentGameState.ToString() } }.ToString());
        }

        private static void OnConnectionBreak(Connection conn)
        {
            if (_firstPlayer == conn)
            {
                _firstPlayer = null;
                _secondPlayer?.SendObject("Disconnect", "");
            }

            if (_secondPlayer == conn)
            {
                _secondPlayer = null;
                _firstPlayer?.SendObject("Disconnect", "");
            }
            Console.WriteLine("Lost Connection With Client On " + conn.ConnectionInfo.RemoteEndPoint);
        }
    }
}