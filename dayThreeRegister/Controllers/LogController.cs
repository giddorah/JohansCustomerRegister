using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace dayThreeRegister.Controllers
{
    [Route("api/log")]
    public class LogController : Controller
    {
        [HttpGet, Route("checklog")]
        public IEnumerable<string> CheckLog()
        {
            var location = Assembly.GetEntryAssembly().Location;
            var directory = System.IO.Path.GetDirectoryName(location);
            string dataLocation = directory + "/log/nlog-own.log";

            //var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var log = System.IO.File.ReadAllLines(dataLocation);
            return log;
        }
    }
}
