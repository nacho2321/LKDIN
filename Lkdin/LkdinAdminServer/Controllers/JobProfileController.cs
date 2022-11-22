using Grpc.Net.Client;
using LkdinConnection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LkdinAdminServer.Controllers
{
    [ApiController]
    [Route("jobProfiles")]
    public class JobProfileController : ControllerBase
    {

        private Admin.AdminClient client;
        private string grpcURL;
        static readonly SettingsManager SettingsMgr = new SettingsManager();
        public JobProfileController()
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcURL = SettingsMgr.ReadSettings(ServerConfig.GrpcURL);
        }
        
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] JobProfileDTO profile)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostJobProfileAsync(profile);
            return Ok(reply.Message);
        }
        
        [HttpDelete("{profileName}")]
        public async Task<ActionResult> Delete(ProfileName profileName)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.DeleteJobProfileAsync(profileName);
            return Ok(reply.Message);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] JobProfileDTO profile)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.UpdateJobProfileAsync(profile);
            return Ok(reply.Message);
        }
    }
}
