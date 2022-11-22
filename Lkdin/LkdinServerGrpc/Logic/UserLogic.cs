﻿using LkdinServerGrpc.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LkdinServerGrpc.Logic
{
    public class UserLogic
    {
        private List<User> users = new List<User>();
        private static UserLogic instance;

        public static UserLogic GetInstance()
        {
            lock (instance)
            {
                if (instance == null)
                    instance = new UserLogic();
            }
            return instance;
        }

        public User CreateUser(string name, int age, List<string> professions, string country)
        {
            lock (users)
            {
                User newUser = null;

                if (!Exists(name))
                {
                    newUser = new User()
                    {
                        Name = name,
                        Age = age,
                        Professions = professions,
                        Country = country,
                        Profile = new JobProfile(),
                        Inbox = new List<Message>()
                    };

                    users.Add(newUser);
                }

                return newUser;
            }
        }

        internal void AddMessage(Message newMessage, User receptor)
        {
            receptor.Inbox.Add(newMessage);
        }

        public User GetUserByName(string name)
        {
            User userToReturn = null;

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == name)
                {
                    userToReturn = users[i]; 
                }
            }

            return userToReturn;
        }

        private bool Exists(string name)
        {
            return (GetUserByName(name) != null);
        }

        public void AssignJobProfile(string name, JobProfile jobProfile)
        {
            User userToAssign = GetUserByName(name);

            userToAssign.Profile = jobProfile;
        }

        public JobProfile GetProfileByName(string user)
        {
            return users.FirstOrDefault(x => x.Name == user).Profile;
        }

        public List<string> GetUsersName()
        {
            List<string> usersName = new List<string>();
            for (int i = 0; i < users.Count; i++)
            {
                usersName.Add(users[i].Name);
            }

            return usersName;
        }

        public string ShowMessages(string user, bool readMessages)
        {
            List<Message> messages = new List<Message>();
            string filteredMessages = "";

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Name == user) 
                {
                    messages = users[i].Inbox;
                }
            }

            for (int i = 0; i < messages.Count; i++)
            {
                if (!messages[i].Read && !readMessages)
                {
                    filteredMessages += "DESTINATARIO: " + messages[i].Sender.Name + "\n" + messages[i].Content;
                    messages[i].ReadMessage();
                }
                else if (messages[i].Read && readMessages)
                {
                    filteredMessages += "DESTINATARIO: " + messages[i].Sender.Name + "\n" + messages[i].Content + "\n" +"\n";
                }
            }

            return filteredMessages;
        }

        public void UpdateUser(string user, int age, List<string> professions, string country)
        {
            lock (users)
            {
                foreach (var u in users)
                {
                    if (u.Name == user)
                    {
                        u.Professions = professions;
                        u.Country = country;
                        u.Age = age;
                    }
                }
            }
        }

        public void DeleteUser(string user)
        {
            lock (users)
            {
                foreach (var u in users)
                {
                    if (u.Name == user)
                    {
                        User userToRemove = GetUserByName(user);
                        users.Remove(userToRemove);
                    }
                }
            }
        }

    }
}