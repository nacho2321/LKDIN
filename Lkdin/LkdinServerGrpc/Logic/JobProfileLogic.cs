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
        private UserLogic userLogic;

        public JobProfileLogic(UserLogic _userLogic)
        {
            userLogic = _userLogic;
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
        }

        public void DeleteJobProfile(string name)
        {
            lock (jobProfiles)
            {
                foreach (var jp in jobProfiles)
                {
                    if (jp.Name == name)
                    {
                        JobProfile jobProfileToRemove = GetJobProfile(name);
                        jobProfiles.Remove(jobProfileToRemove);
                    }
                }
            }
        }

        public void DeleteImage(string user)
        {
            lock (jobProfiles)
            {
                User userToDeleteImage = userLogic.GetUserByName(user);

                foreach (var jp in jobProfiles)
                {
                    if (jp.Name == userToDeleteImage.Profile.Name)
                    {
                        jp.ImagePath = null;
                    }
                }
            }
        }
    }
}
