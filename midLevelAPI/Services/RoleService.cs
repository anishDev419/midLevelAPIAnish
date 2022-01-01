using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using midLevelAPI.Models;

namespace midLevelAPI.Services
{
    public class RoleService
    {
        private readonly IMongoCollection<Roles> _roles;
       
        public RoleService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _roles = database.GetCollection<Roles>("Roles");
        }

        public IList<Roles> Read() => _roles.Find(sub => true).ToList();

    }
}
