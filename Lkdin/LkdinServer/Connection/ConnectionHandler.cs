using LKDIN_Server.Domain;
using LkdinServer.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace LkdinServer.Connection
{
    class ConnectionHandler
    {
        const int maxClients = 3; // Se tiene que llamar desde archivo de configuracion
        private int connections = 0;

        private Socket socket;
        private UserLogic userLogic;

        public ConnectionHandler(UserLogic userLogic)
        {
            this.userLogic = userLogic;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            var localEndpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000); // Se tiene que llamar desde archivo de configuracion (IP & puerto)
            this.socket.Bind(localEndpoint);
        }

        public void Listen()
        {
            this.socket.Listen(maxClients);

           
            while (connections < maxClients)
            {
                var socketClient = this.socket.Accept();

                Console.WriteLine("Nueva conexión establecida");
                new Thread(() => this.Handler(socketClient)).Start();
                connections++;
            }
            Console.WriteLine("Limite de conexiones alcanzado");
        }

        public void Handler(Socket socket)
        {
            try
            {
                bool detainedClient = false;
                while (!detainedClient)
                {
                    byte[] data = new byte[256];
                    int received = socket.Receive(data);
                    if (received == 0)
                    {
                        detainedClient = true;
                        throw new SocketException();
                    }

                    string message = Encoding.UTF8.GetString(data);
                    Console.WriteLine(message);

                    string[] splittedMessage = message.Split("#"); // 0 Comando - 1 Largo - 2 Datos

                    Command order = (Command)Int32.Parse(splittedMessage[0]);
                    string recievedData = splittedMessage[1];

                    RoutingOrder(order, recievedData);
                }
            }
            catch (SocketException)
            {
                connections--;
                Console.WriteLine("Cliente desconectado");
            }

        }

        public void RoutingOrder(Command order, String data)
        {
            string[] splittedData = data.Split("-");

            switch (order)
            {
                case Command.CreateUser:
                    userLogic.CreateUser(splittedData[0], splittedData[1], Int32.Parse(splittedData[2]), splittedData[3].Split(";").ToList(), splittedData[4]);
                    break;
                case Command.CreateJobProfile:

                    break;
                case Command.SendMessage:

                    break;
                case Command.ReadMessages:

                    break;
            }
        }
    }
}
