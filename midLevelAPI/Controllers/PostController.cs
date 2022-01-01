using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using midLevelAPI.Models;
using midLevelAPI.Services;

namespace midLevelAPI.Controllers
{
    public class PostController : Controller
    {


        private readonly PostService _postService;

        public PostController(PostService postService)
        {
            _postService = postService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("api/addPost")]
        public IActionResult addPost([FromBody] Users user)
        {
            var response = _postService.addPost(user, user.posts[0]);
            if( response.flag == true)
            {
                return new OkObjectResult(new { status = "ok", message = response.message, result = response.user });
            }
            else
            {
                return new BadRequestObjectResult(new { status = "error", message = response.message });
            }
        }
    }
}
