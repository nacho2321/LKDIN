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
        private LogLogic logLogic;

        public LogController(ILogger<LogController> logger, LogLogic logLogic)
        {
            this._logger = logger;
            this.logLogic = logLogic;
        }

        [HttpGet]
        public IEnumerable<string> Get()
        {
            return logLogic.GetAll();
        }
        /*
        [HttpGet]
        public IEnumerable<string> GetCreations()
        {
            return logLogic.GetAllCreations();
        }

        [HttpGet]
        public IEnumerable<string> GetAllMessages()
        {
            return logLogic.GetAllMessages();
        }

        [HttpGet]
        public IEnumerable<string> GetAllErrors()
        {
            return logLogic.GetAllErrors();
        }*/
    }
}
