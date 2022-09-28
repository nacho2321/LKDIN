using LkdinConnection.Exceptions;
using LkdinConnection.Logic;
using System;
using System.IO;

namespace LkdinConnection
{
    public class FileStreamHandler
    {
        private readonly FileLogic filelogic;

        public FileStreamHandler()
        {
            filelogic = new FileLogic();
        }

        public byte[] Read(string path, long offset, int length)
        {
            if (filelogic.Exists(path))
            {
                var data = new byte[length];

                using var fs = new FileStream(path, FileMode.Open)
                {
                    Position = offset
                };

                var bytesRead = 0;
                while (bytesRead < length)
                {
                    var read = fs.Read(data, bytesRead, length - bytesRead);
                    if (read == 0)
                        throw new FileException("Error en la lectura del archivo");
                    bytesRead += read;
                }

                return data;
            }

            throw new Exception("El archivo no existe");
        }

        public void Write(string fileName, byte[] data)
        {
            FileMode mode = (filelogic.Exists(fileName)) ? FileMode.Append : FileMode.Create;
            using var fileStream = new FileStream(fileName, mode);

            fileStream.Write(data, 0, data.Length);
        }
    }
}
