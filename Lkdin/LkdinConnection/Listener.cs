using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

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
        
        public async Task<string[]> ReceiveData(NetworkStream networkStream)
        {
            string[] ret = new string[2];
            int headerLength = protocolCmdLength + protocolDataLength;

            // Recibe header con la orden y el largo de los datos
            byte[] header = await BytesReceiver(headerLength, networkStream);

            // transforma a string el header y separa el comando
            string headerToString = Encoding.UTF8.GetString(header);
            string order = headerToString.Substring(0, protocolCmdLength);

            if ((Command)Int32.Parse(order) != Command.SendFile)
            {
                int length = Int32.Parse(headerToString.Substring(protocolCmdLength));
                byte[] data = await BytesReceiver(length, networkStream);

                ret[0] = order;
                ret[1] = Encoding.UTF8.GetString(data);
            }
            else
            {
                // si la orden es recibir archivo, se maneja el recibo de archivo
                int fileNameSize = Int32.Parse(headerToString.Substring(protocolCmdLength));
                await ReceiveFile(fileNameSize, networkStream);
            }
            
            return ret;
        }

        public async Task ReceiveFile(int fileNameSize, NetworkStream networkStream)
        {
            string fileName = Encoding.UTF8.GetString(await BytesReceiver(fileNameSize, networkStream));
            long fileSize = BitConverter.ToInt64(await BytesReceiver(fixedFileSize, networkStream));

            await FileStreamReceiver(fileSize, fileName, networkStream); // await ?
        }

        private async Task<byte[]> BytesReceiver(int length, NetworkStream networkStream)
        {
            byte[] response = new byte[length];
            int offset = 0;
           
            while (offset < length) 
            {
                int received = await networkStream.ReadAsync(response, offset, length - offset).ConfigureAwait(false);
                if (received == 0)
                {
                    throw new Exception(); //TODO tirar exception especifica
                }
                offset += received;
            }

            return response;
        }

        private async Task FileStreamReceiver(long fileSize, string fileName, NetworkStream networkStream)
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
                    data = await BytesReceiver(lastPartSize, networkStream);
                    offset += lastPartSize;
                }
                else
                {
                    data = await BytesReceiver(Protocol.MaxPacketSize, networkStream);
                    offset += Protocol.MaxPacketSize;
                }

                fileStreamHandler.Write(fileName, data);
                currentPart++;
            }
        }

    }
}
