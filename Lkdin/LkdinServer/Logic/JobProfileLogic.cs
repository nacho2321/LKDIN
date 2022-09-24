using LKDIN_Server.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LkdinServer.Logic
{
    public class JobProfileLogic
    {
        private List<JobProfile> jobProfiles = new List<JobProfile>();

        public JobProfile CreateJobProfile(string name, string description, string imagePath, List<string> abilities)
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
            return newJobProfile;
        }

        private bool Exists(string profileName)
        {
            return (this.GetJobProfile(profileName) != null);
        }

        public JobProfile GetJobProfile(string profileName)
        {
            return jobProfiles.FirstOrDefault(p => p.Name == profileName);
        }

    }
}
