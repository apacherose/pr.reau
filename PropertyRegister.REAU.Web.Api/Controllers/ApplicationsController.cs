using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PropertyRegister.REAU.Web.Api.Controllers
{    
    public class ApplicationsController : BaseApiController
    {
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new { id = 5 });
        }
    }
}
