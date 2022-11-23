﻿using LkdinServerGrpc.Domain;
using LkdinServerGrpc.Exceptions;
using LkdinConnection;
using LkdinConnection.Logic;
using LkdinConnection.Exceptions;
using LkdinServerGrpc.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace LkdinServerGrpc.Connection
{
    class ConnectionHandler
    {
        private int connections = 0;
        private int maxClients;

        //private Socket socket;
        private Sender sender;
        private TcpListener tcpListener;
        private Listener listener;
        private UserLogic userLogic;
        private JobProfileLogic jobProfileLogic;
        private MessageLogic messageLogic;
        private LogPublisher logPublisher;
        private IModel channel;

        static readonly SettingsManager settingsMngr = new SettingsManager();

        public ConnectionHandler(UserLogic userLogic, JobProfileLogic jobProfileLogic, MessageLogic messageLogic, Sender sender, Listener listener)
        {
            this.maxClients = Int32.Parse(settingsMngr.ReadSettings(ServerConfig.serverMaxClientsconfigkey));
            this.userLogic = userLogic;
            this.jobProfileLogic = jobProfileLogic;
            this.messageLogic = messageLogic;
            this.logPublisher = logPublisher;
            this.channel = channel;

            this.sender = sender;
            this.listener = listener;

            string ipServer = settingsMngr.ReadSettings(ServerConfig.serverIPconfigkey);
            int ipPort = int.Parse(settingsMngr.ReadSettings(ServerConfig.serverPortconfigkey));
            var localEndpoint = new IPEndPoint(IPAddress.Parse(ipServer), ipPort);

            this.tcpListener = new TcpListener(localEndpoint);
        }

        public async Task Listen()
        {
            tcpListener.Start(maxClients);

            while (connections < maxClients)
            {

                var tcpClientSocket = await tcpListener.AcceptTcpClientAsync().ConfigureAwait(false);
                Console.WriteLine("Nueva conexión establecida");

                var task = Task.Run(async () => await ClientHandler(tcpClientSocket).ConfigureAwait(false));
                connections++;
            }

            Console.WriteLine("Limite de conexiones alcanzado");
        }

        public async Task ClientHandler(TcpClient tcpSocket)
        {
            try
            {
                bool runServer = true;
                using (var stream = tcpSocket.GetStream())
                {
                    while (runServer)
                    {
                        string[] message = await this.listener.ReceiveData(stream); // 0 Comando - 1 Datos
                        
                        if (message[0] != null)
                        {
                            Command order = (Command)Int32.Parse(message[0]);
                            string receivedData = message[1];

                            Console.WriteLine(order + " | " + receivedData);

                            await RoutingOrder(order, receivedData, stream);
                        }
                    }
                }
            }
            catch (Exception)
            {
                connections--;
                Console.WriteLine("Cliente desconectado");
            }
        }

        public async Task RoutingOrder(Command order, string data, NetworkStream netStream)
        {
            string[] splittedData = data.Split("-");
            try
            {
                switch (order)
                {
                    case Command.CreateUser:
                        User newUser = userLogic.CreateUser(splittedData[0], Int32.Parse(splittedData[1]), splittedData[2].Split(";").ToList(), splittedData[3]);
                        await CreationResponseHandler(Command.CreateUser,"Creation: USUARIO CREADO CORRECTAMENTE" + "- Nombre: " + newUser.Name, netStream);
                        break;

                    case Command.CreateJobProfile:
                        string fileRoute = FileLogic.GetPath(splittedData[2]);
                        JobProfile newJobProfile = jobProfileLogic.CreateJobProfile(splittedData[0], splittedData[1], fileRoute, splittedData[3].Split(";").ToList());
                        await CreationResponseHandler(Command.CreateJobProfile,"Creation: PERFIL DE TRABAJO CREADO CORRECTAMENTE" + " - Nombre: " + newJobProfile.Name, netStream);
                        break;

                    case Command.SendMessage:
                        User userSender = userLogic.GetUserByName(splittedData[0]), userReceptor = userLogic.GetUserByName(splittedData[1]);

                        messageLogic.CreateMessage(userSender, userReceptor, splittedData[2]);
                        await sender.Send(Command.CreateJobProfile, "MENSAJE ENVIADO CORRECTAMENTE", netStream);
                        logPublisher.Message(channel, "Messages: SEND - Sender: " + userSender.Name + " - Receptor: " + userReceptor.Name);
                        break;

                    case Command.ReadMessages:
                        bool readMessages = splittedData[1].Contains("readMessages");
                        string messages = userLogic.ShowMessages(splittedData[0], readMessages);

                        await sender.Send(Command.ReadMessages, messages, netStream);
                        await sender.Send(Command.ReadMessages, "MENSAJES MOSTRADOS CORRECTAMENTE", netStream);
                        logPublisher.Message(channel, "Messages: SHOW - User: " + splittedData[0]);
                        break;

                    case Command.GetUsersName:
                        List<string> usersName = userLogic.GetUsersName();
                        string joinedNames = String.Join(";", usersName.ToArray());
                        await sender.Send(Command.GetUsersName, joinedNames, netStream);
                        break;

                    case Command.GetJobProfiles:
                        List<string> jobProfiles = jobProfileLogic.GetJobProfiles();
                        string joinedJobProfiles = String.Join(";", jobProfiles.ToArray());
                        await sender.Send(Command.GetJobProfiles, joinedJobProfiles, netStream);
                        break;

                    case Command.GetSpecificProfile:
                        JobProfile profile = userLogic.GetProfileByName(splittedData[0]);

                        // envio info del perfil
                        await sender.Send(Command.GetSpecificProfile, GetJobProfileMessage(profile), netStream);

                        // envio imagen del perfil
                        await sender.SendFile(profile.ImagePath, netStream); //TODO ImagePath tiene que guardar la ruta en server, no la indicada x usuario

                        break;

                    case Command.AssignJobProfile:
                        JobProfile jobProfile = jobProfileLogic.GetJobProfile(splittedData[1]);
                        userLogic.AssignJobProfile(splittedData[0], jobProfile);
                        await CreationResponseHandler(Command.AssignJobProfile, "Creation: PERFIL DE TRABAJO ASIGNADO CORRECTAMENTE - " + splittedData[1] + " to " + splittedData[0], netStream);
                        break;

                }
            }
            catch (Exception ex)
            {
                if (ex is DomainException || ex is ArgumentNullException || ex is FileException)
                {
                    await CreationResponseHandler(Command.ThrowException, "Error: " + ex.Message, netStream);
                }
            }
        }

        private async Task CreationResponseHandler(Command cmd, string response, NetworkStream netStream)
        {
            await sender.Send(cmd, response, netStream);
            logPublisher.Message(channel, response);
        }


        private string GetJobProfileMessage(JobProfile profile) 
        {
            return profile.Name + '-' + profile.Description + '-' + FileLogic.GetName(profile.ImagePath) + '-' + String.Join(";", profile.Abilities.ToArray());
        }
    }
}
