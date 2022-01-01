using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using midLevelAPI.Services;
using midLevelAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using System.IO;

namespace midLevelAPI.Controllers
{
    public class JwtController : Controller
    {
        private readonly IJwtAuthenticationManager jwtAuthenticationManager;
        private readonly JwtService _jwtService;

        public JwtController(IJwtAuthenticationManager jwtAuthenticationManager, JwtService jwtService)
        {
            this.jwtAuthenticationManager = jwtAuthenticationManager;
            this._jwtService = jwtService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("api/getAllUsers")]
        public IList<Users> getAllRoles()
        {
            return this._jwtService.getAllUsers();
        }

        [HttpGet]
        [Route("api/getUserDetails")]
        public IActionResult getUserDetails([FromQuery] string username)
        {
            var responseDub = _jwtService.checkDubUser(username);
            if( responseDub.flag == false)
            {
                var userResponse = _jwtService.getUserDetails(username);
                if( userResponse.flag == true)
                {
                    return new OkObjectResult(new { status = "ok", message = userResponse.message, result = userResponse.user });
                }
                else
                {
                    return new BadRequestObjectResult(new { status = "error", message = userResponse.message });
                }
            }
            else
            {
                return new BadRequestObjectResult(new { status = "error", message = "No User Found" });
            }
        }

        [Authorize]
        [HttpGet]
        [Route("api/deleteUser")]
        public IActionResult deleteUser([FromQuery] string username)
        {
             var responseDub = _jwtService.checkDubUser(username);
            if(responseDub.flag == false)
            {
                var deleteRes = _jwtService.deleteUser(username);
                if( deleteRes.status == "ok")
                {
                    return new OkObjectResult(new { status = "ok", message = "User Deleted" });
                }
                else
                {
                    return new BadRequestObjectResult(new { status = "error", message = deleteRes.message });
                }
            }
            else
            {
                return new BadRequestObjectResult(new { status = "error", message = "No Users Detected" });
            }
        }

        [Authorize]
        [HttpPost]
        [Route("api/addModerator")]
        public IActionResult AddModerator([FromBody] Users user)
        {
            var response = _jwtService.checkDubUser(user.username);
            if( response.flag == true)
            {
                var createResponse = _jwtService.registerUser(user);
                if( createResponse.flag == true)
                {
                    return new OkObjectResult(new { status = "ok", message = createResponse.message, result = user });
                }
                else
                {
                    return new BadRequestObjectResult(new { status = "error", message = createResponse.message });
                }
            }
            else
            {
                return new BadRequestObjectResult(new { status = "error", result = response.message });
            }
        }

        [HttpGet]
        [Route("api/susTemporary")]
        public IActionResult susTemporary([FromQuery] string username)
        {
            var response = _jwtService.checkDubUser(username);
            if (response.flag == false)
            {
                var responseReplace = _jwtService.susTemporary(username);
                if (responseReplace.status == "ok")
                {
                    return new OkObjectResult(new { status = "ok", message = responseReplace.message, result = responseReplace.user });
                }
                else
                {
                    return new BadRequestObjectResult(new { status = "error", message = responseReplace.message });
                }
            }
            else
            {
                return new BadRequestObjectResult(new { status = "error", message = response.message });
            }
        }

        //[Authorize]
        [HttpGet]
        [Route("api/susPermanent")]
        public IActionResult susPermanent([FromQuery] string username)
        {
            var response = _jwtService.checkDubUser(username);
            if( response.flag == false)
            {
                var responseReplace = _jwtService.susPermanent(username);
                if( responseReplace.status == "ok")
                {
                    return new OkObjectResult(new { status = "ok", message = responseReplace.message, result = responseReplace.user });
                }
                else
                {
                    return new BadRequestObjectResult(new { status = "error", message = responseReplace.message });
                }
            }
            else
            {
                return new BadRequestObjectResult(new { status = "error", message = response.message });
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/login")]
        public IActionResult Authenticate([FromBody] Users userCred )
        {
            var response = _jwtService.authenticateUser(userCred);
            //response.user.Photo = this.GetImage(Convert.ToBase64String(response.user.Photo));
            if(response.flag == false)
            {
                return new BadRequestObjectResult(new { status = "error", result = response.message });
            }
            var token = jwtAuthenticationManager.Authenticate(userCred.username, userCred.password);
            if(token == null) { return Unauthorized(); }
            return new OkObjectResult( new { status = "ok", result = token, message = userCred } );
        }

        public byte[] GetImage(string sBase64String)
        {
            byte[] bytes = null;
            if (!string.IsNullOrEmpty(sBase64String))
            {
                bytes = Convert.FromBase64String(sBase64String);
            }
            return bytes;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/registerFile")]
        public void RegisterFile()
        {
            string str = Request.Form["data"].ToString();
            JsonSerializerSettings jss = new JsonSerializerSettings();
            //jss.NullValueHandling = NullValueHandling.Ignore;
            Users user = JsonConvert.DeserializeObject<Users>(str, jss);
            //Treatment treatment = JsonConvert.DeserializeObject<Treatment>(str, jss);
            //Users user = JsonConvert.DeserializeObject<Users>(fileObj.user);


                using (var ms = new MemoryStream())
                {
                    foreach( var file in Request.Form.Files)
                    {
                        file.CopyTo(ms);
                        var fileBytes = ms.ToArray();

                        user.Photo = fileBytes;
                        var response = _jwtService.registerUser(user);

                    }
                }
            
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/register")]
        public IActionResult Register([FromBody] Users user)
        {
            var response = _jwtService.registerUser(user);
            if( response.status == "error")
            {
                return new BadRequestObjectResult(new { status = "error", result = response.message });
            }
            else
            {
                return new OkObjectResult(new { status = "Ok", result = response.message });
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("api/checkDupUser")]
        public IActionResult checkDubUser([FromQuery] string username)
        {
            var response = _jwtService.checkDubUser(username);
            if( response.flag == false)
            {
                return new BadRequestObjectResult(new { status = response.status, result = response.message });
            }
            else
            {
                return new OkObjectResult(new { status = response.status });
            }
        }

    }
}
