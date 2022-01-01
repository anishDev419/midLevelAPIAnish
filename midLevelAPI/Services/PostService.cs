using midLevelAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace midLevelAPI.Services
{
    public class PostService
    {
        private readonly IMongoCollection<Users> _users;
        public PostService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<Users>("Users");
        }

        public FunctionResponse addPost(Users user, posts post)
        {
            var filter = Builders<Users>.Filter.Eq(x => x.username, user.username);
            var update = Builders<Users>.Update.Push(x => x.posts, post);
            _users.UpdateOne(filter, update);
            return new FunctionResponse { status = "ok", flag = true, message = "Successfully Added Post", user = user };
        }
    }
}
