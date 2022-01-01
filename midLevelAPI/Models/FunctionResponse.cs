using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace midLevelAPI.Models
{
    public class FunctionResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public bool flag { get; set; }
        public Users user { get; set; }
    }
}
