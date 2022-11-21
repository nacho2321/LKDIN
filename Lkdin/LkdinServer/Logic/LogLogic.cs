using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServer.Logic
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

        public List<string> GetAllDeleted()
        {
            lock (logs)
            {
                List<string> logsDeleted = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs.Contains("Deleted"))
                    {
                        logsDeleted.Add(logs[i]);
                    }
                }

                return logsDeleted;
            }
        }

        public List<string> GetAllUpdates()
        {
            lock (logs)
            {
                List<string> logUpdates = new List<string>();
                for (int i = 0; i < logs.Count; i++)
                {
                    if (logs.Contains("Update"))
                    {
                        logUpdates.Add(logs[i]);
                    }
                }

                return logUpdates;
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


    }
}
