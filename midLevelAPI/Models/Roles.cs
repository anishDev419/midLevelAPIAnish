using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace midLevelAPI.Models
{
    public class Roles
    {
        [BsonId]
        public string roleName { get; set; }
        public List<RightClass> Rights { get; set; }
    }
    public class RightClass
    {
        public string Right { get; set; }
        public bool Value { get; set; }
    }
}
