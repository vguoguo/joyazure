using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreCodeCamp.Controllers
{
    [ApiController]
    [Route("feedbackRequest")]
    public class HomeController : Controller
    {
       /*   [HttpGet]
          public IActionResult Index()
          {
              var myData = new
              {
                  Host = @"sftp.myhost.gr",
                  UserName = "my_username",
                  Password = "my_password",
                  SourceDir = "/export/zip/mypath/",
                  FileName = "my_file.zip"
              };

              //Tranform it to Json object
              string jsonData = JsonConvert.SerializeObject(myData);
              return Ok(jsonData);
             // return Problem(jsonData, statusCode: (int?)HttpStatusCode.BadRequest);
          }*/
          
        [HttpGet]
        public IActionResult url(int id)
        {
            if (id == 1)
            {
                return Ok("https://teams.microsoft.com/l/app/380c7983-f2bd-40bf-9a1e-a5a68fd98702?source=app-details-dialog");
            }
            else {
                return Ok("https://www.youtube.com/watch?v=ppMQHx_9liw");
            }
        }
    }
}
