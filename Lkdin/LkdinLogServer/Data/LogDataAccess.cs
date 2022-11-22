using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LkdinLogServer.Data
{
    public class LogDataAccess
    {
        private List<string> logs;
        private object padlock;
        private static LogDataAccess instance;

        private static object singletonPadlock = new object();
        public static LogDataAccess GetInstance()
        {

            lock (singletonPadlock)
            { // bloqueante 
                if (instance == null)
                {
                    instance = new LogDataAccess();
                }
            }
            return instance;
        }

        private LogDataAccess()
        {
            logs = new List<string>();
            padlock = new object();
        }

        public string[] GetLogs()
        {
            lock (padlock)
            {
                return logs.ToArray();
            }
        }
    }
}
