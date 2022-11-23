using LkdinLogServer.Data;
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
    public class LogController : ControllerBase
    {
        private readonly ILogger<LogController> _logger;

        public LogController(ILogger<LogController> logger)
        {
            this._logger = logger;
        }

        [Route("logs")]
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return LogDataAccess.GetInstance().GetAll();
        }

        [Route("logs/creations")]
        [HttpGet]
        public IEnumerable<string> GetCreations()
        {
            return LogDataAccess.GetInstance().GetAllCreations();
        }

        [Route("logs/messages")]
        [HttpGet]
        public IEnumerable<string> GetAllMessages()
        {
            return LogDataAccess.GetInstance().GetAllMessages();
        }

        [Route("logs/errors")]
        [HttpGet]
        public IEnumerable<string> GetAllErrors()
        {
            return LogDataAccess.GetInstance().GetAllErrors();
        }
    }
}
