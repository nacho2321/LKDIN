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
    [Route("users")]
    public class UserController : ControllerBase
    {

        private Admin.AdminClient client;
        private string grpcURL;
        static readonly SettingsManager SettingsMgr = new SettingsManager();
        public UserController()
        {


            AppContext.SetSwitch(
                  "System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);
            grpcURL = SettingsMgr.ReadSettings(ServerConfig.GrpcURL);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] UserDTO user)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.PostUserAsync(user);
            return Ok(reply.Message);
        }

        /*
        [HttpGet]
        public async Task<ActionResult> Get(UserRequest request)
        {
            using var channel = GrpcChannel.ForAddress(grpcURL);
            client = new Admin.AdminClient(channel);
            var reply = await client.GetUserAsync(request);
            return Ok(reply.Message);
        }
        */
    }
}
