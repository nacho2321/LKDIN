using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace LkdinConnection
{
    public class Listener
    {
        private readonly int protocolDataLength;
        private readonly int maxPacketSize;
        private readonly int protocolCmdLength;
        private readonly int fixedFileSize;
        private readonly FileStreamHandler fileStreamHandler;

        public Listener()
        {
            this.protocolDataLength = Protocol.protocolDataLength;
            this.maxPacketSize = Protocol.MaxPacketSize;
            this.protocolCmdLength = Protocol.protocolCmdLength;
            this.fixedFileSize = Protocol.FixedFileSize;

            this.fileStreamHandler = new FileStreamHandler();
        }
        
        public string[] ReceiveData(Socket socket)
        {
            string[] ret = new string[2];
            int headerLength = protocolCmdLength + protocolDataLength;
            byte[] header = BytesReceiver(headerLength, socket);

            string headerToString = Encoding.UTF8.GetString(header);
            string order = headerToString.Substring(0, protocolCmdLength);
            if ((Command)Int32.Parse(order) != Command.SendFile)
            {
                int length = Int32.Parse(headerToString.Substring(protocolCmdLength));
                byte[] data = BytesReceiver(length, socket);

                ret[0] = order;
                ret[1] = Encoding.UTF8.GetString(data);
            }
            else
            {
                int fileNameSize = Int32.Parse(headerToString.Substring(protocolCmdLength));
                ReceiveFile(fileNameSize, socket);
            }
            
            return ret;
        }

        public void ReceiveFile(int fileNameSize, Socket socket)
        {
            string fileName = Encoding.UTF8.GetString(BytesReceiver(fileNameSize, socket));
            long fileSize = BitConverter.ToInt64(BytesReceiver(fixedFileSize, socket));

            FileStreamReceiver(fileSize, fileName, socket);
        }

        private byte[] BytesReceiver(int length, Socket socket)
        {
            byte[] response = new byte[length];
            int offset = 0;
           
            while (offset < length) 
            {
                int received = socket.Receive(response, offset, length - offset, SocketFlags.None);
                
                if (received == 0)
                {
                    throw new SocketException();
                }
                offset += received;
            }

            return response;
        }

        private void FileStreamReceiver(long fileSize, string fileName, Socket socket)
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
                    data = BytesReceiver(lastPartSize, socket);
                    offset += lastPartSize;
                }
                else
                {
                    data = BytesReceiver(Protocol.MaxPacketSize, socket);
                    offset += Protocol.MaxPacketSize;
                }

                fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }

    }
}
