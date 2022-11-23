using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LkdinLogServer.Data
{
    public class LogDataAccess
    {
        private List<string> logs = new List<string>();

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

        public void AddLog(string log)
        {
            lock (padlock)
            {
                logs.Add(log);
            }
        }

        public List<string> GetAll()
        {
            lock (padlock)
            {
                return logs;
            }
        }

        public List<string> GetAllCreations()
        {
            lock (padlock)
            {
                List<string> logCreations = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs[i].Contains("Creation"))
                    {
                        logCreations.Add(logs[i]);
                    }
                }

                return logCreations;
            }
        }

        public List<string> GetAllMessages()
        {
            lock (padlock)
            {
                List<string> logMessages = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs[i].Contains("Message"))
                    {
                        logMessages.Add(logs[i]);
                    }
                }

                return logMessages;
            }
        }

        public List<string> GetAllErrors()
        {
            lock (padlock)
            {
                List<string> logMessages = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs[i].Contains("Error"))
                    {
                        logMessages.Add(logs[i]);
                    }
                }

                return logMessages;
            }
        }

    }
}
