using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GetStat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "MainAdmin")]
    public class TestController : ControllerBase
    {
        public List<string> GetTest()
        {
            return  new List<string>{"item1","item2","item3","item4","item5"};
        }
    }
}