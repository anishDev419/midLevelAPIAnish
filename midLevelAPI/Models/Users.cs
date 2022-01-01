using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace midLevelAPI.Models
{
    public class Users
    {
        public string username { get; set; }
        public string password { get; set; }
        public string email { get; set; }
        public string role { get; set; }
        public byte[] Photo { get; set; }
        public List<posts> posts { get; set; }
        public bool suspended { get; set; }
        public bool pSuspended { get; set; }

    }

    public class posts
    {
        public string name { get; set; }
        public string description { get; set; }
        public byte[] Photo { get; set; }
    }
}
