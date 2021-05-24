using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using Newtonsoft.Json.Linq;

namespace superHot_Server
{
    class Program
    {
        private const int ServerPort = 11771;
        static void Main(string[] args)
        {
            NetworkComms.AppendGlobalConnectionEstablishHandler(RemoteClients.OnConnect);
            try
            {
                var listeners = Connection.StartListening(ConnectionType.TCP, new IPEndPoint(IPAddress.Any, ServerPort));
                if (listeners.Count != 0)
                {
                    Console.WriteLine("Server Started On Port " + ServerPort + ", Listeners: " + listeners.Count);
                }
            }
            catch
            {
                Console.WriteLine("Unable to start server on " + ServerPort);
                return;
            }

            string command = "";
            do
            {
                command = Console.ReadLine();
                if (command == "clear")
                {
                    Console.Clear();
                }
                else if (command == "kick")
                {
                    RemoteClients.KickEveryOne("reason");
                }
                else if (command == "set play")
                {
                    RemoteClients.SetGameState(GameState.Playing);
                }
                else if (command == "set update")
                {
                    RemoteClients.SetGameState(GameState.Updating);
                }
            } while (command != "exit");
            Console.ReadKey();
        }
    }
}
