using LkdinServerGrpc.Logic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LkdinLogServer.Controllers
{
    [ApiController]
    [Route("logs")]
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            this._logger = logger;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return LogLogic.GetInstance().GetAll();
        }


        [HttpGet(Name = "/creations")]
        public IEnumerable<string> GetCreations()
        {
            return LogLogic.GetInstance().GetAllCreations();
        }

        [HttpGet(Name = "/messages")]
        public IEnumerable<string> GetAllMessages()
        {
            return LogLogic.GetInstance().GetAllMessages();
        }

        [HttpGet(Name = "/errors")]
        public IEnumerable<string> GetAllErrors()
        {
            return LogLogic.GetInstance().GetAllErrors();
        }
    }
}
