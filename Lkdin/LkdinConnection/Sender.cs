using LkdinConnection.Exceptions;
using LkdinConnection.Logic;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LkdinConnection
{
    public class Sender
    {
        private readonly int protocolDataLength;
        private readonly int maxPacketSize;

        private readonly FileLogic fileLogic;
        private readonly FileStreamHandler fileStreamHandler;
        
        public Sender()
        {
            this.protocolDataLength = Protocol.protocolDataLength;
            this.maxPacketSize = Protocol.MaxPacketSize;

            this.fileLogic = new FileLogic();
            this.fileStreamHandler = new FileStreamHandler();
        }

        public async Task Send(Command order, TcpClient tcpSocket)
        {
            await Send(order, "", tcpSocket);
        }

        public async Task Send(Command order, string message, TcpClient tcpSocket)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            int dataLength = data.Length;

            byte[] headerData = this.HeaderEncoder(order, dataLength);

            await BytesSender (headerData, tcpSocket);    // Primero envia header con orden y largo de datos
            await BytesSender (data, tcpSocket);          // Luego envia los datos (el mensaje)
        }

        public async Task SendFile(string path, TcpClient tcpSocket)
        {
            if (fileLogic.Exists(path))
            {
                string fileName = this.fileLogic.GetName(path);
                byte[] headerData = this.HeaderEncoder(Command.SendFile, fileName.Length);
                byte[] convertedfileName = Encoding.UTF8.GetBytes(fileName);

                long fileSize = this.fileLogic.GetFileSize(path);
                byte[] convertedfileSize = BitConverter.GetBytes(fileSize);

                await BytesSender(headerData, tcpSocket);          // Envia header con orden y largo del nombre del archivo
                await BytesSender(convertedfileName, tcpSocket);   // Envia nombre del archivo
                await BytesSender(convertedfileSize, tcpSocket);   // Envia tamaño del archivo
                await FileStreamSenderAsync(fileSize, path, tcpSocket); // Envia archivo
            }
            else
            {
                throw new FileException("El archivo no existe");
            }
        }

        private byte[] HeaderEncoder(Command order, int messageLength)
        {
            int dataLengthBytes = messageLength.ToString().Length;
            int zerosToAdd = protocolDataLength - dataLengthBytes;

            string lengthOfMessage = "";

            for (int i = 0; i < zerosToAdd; i++)
            {
                lengthOfMessage += "0";
            }

            lengthOfMessage += messageLength.ToString();
            string header = (int)order + lengthOfMessage;

            return Encoding.UTF8.GetBytes(header);
        }

        private async Task BytesSender(byte[] data, TcpClient tcpSocket)
        {
            int size = data.Length;
            await using (var networkStream = tcpSocket.GetStream())
            {
                await networkStream.WriteAsync(data, 0, size).ConfigureAwait(false);

            }
        }

        private async Task FileStreamSenderAsync(long fileSize, string path, TcpClient tcpSocket)
        {
            long fileParts = Protocol.FileParts(fileSize);
            long offset = 0;
            long currentPart = 1;

            while (fileSize > offset)
            {
                byte[] data;
                if (currentPart == fileParts)
                {
                    var lastPartSize = (int)(fileSize - offset);
                    data = fileStreamHandler.Read(path, offset, lastPartSize);
                    offset += lastPartSize;
                }
                else
                {
                    data = fileStreamHandler.Read(path, offset, maxPacketSize);
                    offset += maxPacketSize;
                }

                await BytesSender(data, tcpSocket);

                currentPart++;
            }
        }
    }
}

public enum Command
{
    CreateUser,
    CreateJobProfile,
    SendMessage,
    ReadMessages,
    GetUsersName,
    GetJobProfiles,
    AssignJobProfile,
    SendFile,
    GetSpecificProfile,
    ThrowException
}
