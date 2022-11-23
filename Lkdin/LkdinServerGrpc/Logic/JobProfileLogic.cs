using LkdinConnection.Logic;
using LkdinServerGrpc.Domain;
using LkdinServerGrpc.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LkdinServerGrpc.Logic
{
    public class JobProfileLogic
    {
        private List<JobProfile> jobProfiles = new List<JobProfile>();
        private UserLogic userLogic = UserLogic.GetInstance();
        private static JobProfileLogic instance;
        private static readonly object singletonlock = new object();

        public static JobProfileLogic GetInstance()
        {
            lock (singletonlock)
            {
                if (instance == null)
                    instance = new JobProfileLogic();
            }
            return instance;
        }

        public JobProfile CreateJobProfile(string name, string description, string imagePath, List<string> abilities)
        {
            lock(jobProfiles)
            { 
                JobProfile newJobProfile = null;

                if (!Exists(name)) { 
                    newJobProfile = new JobProfile()
                    {
                        Name = name,
                        Abilities = abilities,
                        Description = description,
                        ImagePath = imagePath
                    };
                    jobProfiles.Add(newJobProfile);
                }
                else
                {
                    throw new DomainException($"Ya existe un perfil de trabajo con el nombre {name}");
                }
                return newJobProfile;
            }
        }

        private bool Exists(string profileName)
        {
            return (this.GetJobProfile(profileName) != null);
        }

        public JobProfile GetJobProfile(string profileName)
        {
            return jobProfiles.FirstOrDefault(p => p.Name == profileName);
        }

        public List<string> GetJobProfiles()
        {
            List<string> data = new List<string>();
            for (int i = 0; i < jobProfiles.Count; i++)
            {
                data.Add(jobProfiles[i].Name);
            }

            return data;
        }

        public void UpdateJobProfile(string name, string description, string imagePath, List<string> abilities)
        {
            lock (jobProfiles)
            {
                if (Exists(name))
                {
                    foreach (var jp in jobProfiles)
                    {
                        if (jp.Name == name)
                        {
                            jp.Description = description;
                            jp.ImagePath = imagePath;
                            jp.Abilities = abilities;
                        }
                    }
                }
                else
                {
                    throw new DomainException($"El perfil de trabajo {name} no existe");
                }
            }
        }

        public void DeleteJobProfile(string profile)
        {
            lock (jobProfiles)
            {
                if (Exists(profile))
                {
                    JobProfile profileToRemove = GetJobProfile(profile);
                    FileLogic.DeleteFile(profileToRemove.ImagePath);
                    jobProfiles.Remove(profileToRemove);
                }
                else
                {
                    throw new DomainException($"El perfil de trabajo {profile} no existe");
                }
            }
        }

        public void DeleteJobProfileImage(string user)
        {
            lock (jobProfiles)
            {
                if (Exists(user))
                {
                    User userToDeleteImage = userLogic.GetUserByName(user);

                    JobProfile profileToAlter = jobProfiles.Find(profile => profile.Name == userToDeleteImage.Profile.Name);
                    if (profileToAlter.Name != null)
                    {
                        FileLogic.DeleteFile(profileToAlter.ImagePath);
                        profileToAlter.ImagePath = "";
                    }
                    else
                    {
                        throw new DomainException($"El usuario {user} no tiene perfil asociado");
                    }

                }
                else
                {
                    throw new DomainException($"El usuario {user} no existe");
                }
            }
        }
    }
}
