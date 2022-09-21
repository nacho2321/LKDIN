using LKDIN_Server.Domain;
using LkdinConnection;
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
        private Sender sender;
        private UserLogic userLogic;

        public ConnectionHandler(UserLogic userLogic, Sender sender)
        {
            this.userLogic = userLogic;
            this.sender = sender;
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

                    RoutingOrder(order, recievedData, socket);
                }
            }
            catch (SocketException)
            {
                connections--;
                Console.WriteLine("Cliente desconectado");
            }

        }

        public void RoutingOrder(Command order, string data, Socket socket)
        {
            string[] splittedData = data.Split("-");
            try
            {
                switch (order)
                {
                    case Command.CreateUser:
                        userLogic.CreateUser(splittedData[0], Int32.Parse(splittedData[1]), splittedData[2].Split(";").ToList(), splittedData[3]);
                        sender.SendBytes(Command.CreateUser, "USUARIO CREADO CORRECTAMENTE", socket);
                        break;
                    case Command.CreateJobProfile:

                        break;
                    case Command.SendMessage:

                        break;
                    case Command.ReadMessages:

                        break;
                    case Command.GetUsers:
                        userLogic.GetUsers();
                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex is DomainException || ex is ArgumentNullException)
                {
                    sender.SendBytes(Command.ThrowException, ex.Message, socket);
                }
            }
        }
    }
}
