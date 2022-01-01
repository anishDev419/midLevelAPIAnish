using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace midLevelAPI.Models
{
    public class UserWithFile
    {
        public string user { get; set; }
        public IFormFile file { get; set; }
    }
}
