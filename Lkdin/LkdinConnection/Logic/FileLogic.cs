using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LkdinConnection.Logic
{
    public class FileLogic
    {
        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public string GetName(string path)
        {
            return (Exists(path)) ? new FileInfo(path).Name : "";
        }

        public string GetPath(string fileName)
        {
            return Path.GetFullPath(fileName);
        }

        public long GetFileSize(string path)
        {
            return (Exists(path)) ? new FileInfo(path).Length : 0;
        }

    }
}
