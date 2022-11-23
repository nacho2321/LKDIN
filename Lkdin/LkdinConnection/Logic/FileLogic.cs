using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LkdinConnection.Logic
{
    public static class FileLogic
    {
        public static bool Exists(string path)
        {
            return File.Exists(path);
        }

        public static string GetName(string path)
        {
            return (Exists(path)) ? new FileInfo(path).Name : "";
        }

        public static string GetPath(string fileName)
        {
            return Path.GetFullPath(fileName);
        }

        public static long GetFileSize(string path)
        {
            return (Exists(path)) ? new FileInfo(path).Length : 0;
        }

        public static void DeleteFile(string path)
        {
            if(Exists(path))
                File.Delete(GetPath(path));
        }

    }
}
