using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using midLevelAPI.Services;
using midLevelAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace midLevelAPI.Controllers
{
    public class RoleController : Controller
    {

        private readonly RoleService _roleService;

        public RoleController(RoleService roleService)
        {
            _roleService = roleService;
        }

        [Authorize]
        [HttpGet]
        [Route("api/getAllRoles")]
        public IList<Roles> getAllRoles()
        {
            return this._roleService.Read();
        } 

        public IActionResult Index()
        {
            return View();
        }
    }
}
