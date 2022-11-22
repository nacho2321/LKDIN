using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServerGrpc.Logic
{
    public class LogLogic
    {
        private List<string> logs = new List<string>();

        public void AddLog(string log)
        {
            lock (logs)
            {
                logs.Add(log);
            }
        }

        public List<string> GetAll()
        {
            lock (logs)
            {
                return logs;
            }
        }

        public List<string> GetAllCreations()
        {
            lock (logs)
            {
                List<string> logCreations = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs.Contains("Creation"))
                    {
                        logCreations.Add(logs[i]);
                    }
                }

                return logCreations;
            }
        }

        public List<string> GetAllMessages()
        {
            lock (logs)
            {
                List<string> logMessages = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs.Contains("Message"))
                    {
                        logMessages.Add(logs[i]);
                    }
                }

                return logMessages;
            }
        }

        public List<string> GetAllErrors()
        {
            lock (logs)
            {
                List<string> logMessages = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs.Contains("ERROR"))
                    {
                        logMessages.Add(logs[i]);
                    }
                }

                return logMessages;
            }
        }

    }
}
