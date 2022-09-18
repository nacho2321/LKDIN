using LKDIN_Server.Domain;
using System;
using System.Collections.Generic;
using System.Text;

namespace LkdinServer.Logic
{
    public class JobProfileLogic
    {
        private List<JobProfile> jobProfiles = new List<JobProfile>();

        public void CreateJobProfile(string description, string imagePath, List<string> abilities)
        {
            JobProfile newJobProfile = new JobProfile()
            {
                Abilities = abilities,
                Description = description,
                ImagePath = imagePath
            };

            jobProfiles.Add(newJobProfile);
        }
    }
}
