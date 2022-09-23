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

        private int connections = 0;
        private int maxClients;

        private Socket socket;
        private Sender sender;
        private Listener listener;
        private UserLogic userLogic;
        private JobProfileLogic jobProfileLogic;
        private MessageLogic messageLogic;
        
        static readonly SettingsManager settingsMngr = new SettingsManager();

        public ConnectionHandler(UserLogic userLogic, JobProfileLogic jobProfileLogic, MessageLogic messageLogic, Sender sender, Listener listener)
        {
            this.maxClients = Int32.Parse(settingsMngr.ReadSettings(ServerConfig.serverMaxClientsconfigkey));
            this.userLogic = userLogic;
            this.jobProfileLogic = jobProfileLogic;
            this.messageLogic = messageLogic;
            this.sender = sender;
            this.listener = listener;
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            string ipServer = settingsMngr.ReadSettings(ServerConfig.serverIPconfigkey);
            int ipPort = int.Parse(settingsMngr.ReadSettings(ServerConfig.serverPortconfigkey));
            var localEndpoint = new IPEndPoint(IPAddress.Parse(ipServer), ipPort);

            this.socket.Bind(localEndpoint);
        }

        public void Listen()
        {
            this.socket.Listen(maxClients);

            while (connections < maxClients)
            {
                var socketClient = this.socket.Accept();

                Console.WriteLine("Nueva conexión establecida");
                new Thread(() => this.ClientHandler(socketClient)).Start();
                connections++;
            }
            Console.WriteLine("Limite de conexiones alcanzado");
        }

        public void ClientHandler(Socket socket)
        {
            try
            {
                bool runServer = true;
                while (runServer)
                {
                    string[] message = this.listener.RecieveData(socket); // 0 Comando - 1 Datos
                    Command order = (Command)Int32.Parse(message[0]);
                    string recievedData = message[1];
                    
                    Console.WriteLine(order + " | " + recievedData);

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
                        User newUser = userLogic.CreateUser(splittedData[0], Int32.Parse(splittedData[1]), splittedData[2].Split(";").ToList(), splittedData[3]);
                        Command cmd = (newUser != null) ? Command.CreateUser : Command.ThrowException;
                        string messageToReturn = (newUser != null) ? "USUARIO CREADO CORRECTAMENTE" : "YA EXISTE EL USUARIO";

                        sender.Send(cmd, messageToReturn, socket);
                        break;

                    case Command.CreateJobProfile:
                        jobProfileLogic.CreateJobProfile(splittedData[0], splittedData[1], splittedData[2].Split(";").ToList());

                        sender.Send(Command.CreateJobProfile, "PERFIL DE TRABAJO CREADO CORRECTAMENTE", socket);
                        break;

                    case Command.SendMessage:
                        User userSender = userLogic.GetUserByName(splittedData[0]);
                        User userReceptor = userLogic.GetUserByName(splittedData[1]);
                        messageLogic.CreateMessage(userSender, userReceptor, splittedData[2]);

                        sender.Send(Command.CreateJobProfile, "MENSAJE ENVIADO CORRECTAMENTE", socket);
                        break;

                    case Command.ReadMessages:
                        string messages = "";
                        if (splittedData[1].Contains("readMessages"))
                        {
                            messages = userLogic.ShowMessages(splittedData[0], true);
                        }
                        else
                        {
                            messages = userLogic.ShowMessages(splittedData[0], false);
                        }

                        sender.Send(Command.ReadMessages, messages, socket);
                        sender.Send(Command.ReadMessages, "MENSAJES MOSTRADOS CORRECTAMENTE", socket);
                        break;

                    case Command.GetUsersName:
                        List<string> usersName = userLogic.GetUsersName();
                        string joinedNames = String.Join("; ", usersName.ToArray());
                        sender.Send(Command.GetUsersName, joinedNames, socket);

                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex is DomainException || ex is ArgumentNullException)
                {
                    sender.Send(Command.ThrowException, ex.Message, socket);
                }
            }
        }
    }
}
