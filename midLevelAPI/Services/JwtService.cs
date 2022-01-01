using midLevelAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace midLevelAPI.Services
{
    public class JwtService
    {
        private readonly IMongoCollection<Users> _users;

        public JwtService(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            this._users = database.GetCollection<Users>("Users");
        }

        public FunctionResponse checkDubUser(string username)
        {
            var filterUsername = Builders<Users>.Filter.Eq(x => x.username, username);
            var userFound = _users.Find(filterUsername).Project<Users>(Builders<Users>.Projection.Exclude("_id")).ToList().FirstOrDefault();
            if( userFound != null)
            {
                return new FunctionResponse { flag = false, status = "error", message = "Username Already Exists" };
            }
            else
            {
                return new FunctionResponse { flag = true, status = "ok" };
            }
        }

        public FunctionResponse susTemporary(string username)
        {
            try
            {
                var filterUsername = Builders<Users>.Filter.Eq(x => x.username, username);
                var userFound = _users.Find(filterUsername).Project<Users>(Builders<Users>.Projection.Exclude("_id")).ToList().FirstOrDefault();
                userFound.suspended= !userFound.suspended;
                _users.FindOneAndReplace(filterUsername, userFound);
                return new FunctionResponse { status = "ok", message = "User Suspended", user = userFound };
            }
            catch (Exception ex)
            {
                return new FunctionResponse { status = "error", message = ex.Message };
            }
        }

        public FunctionResponse susPermanent(string username)
        {
            try
            {
                var filterUsername = Builders<Users>.Filter.Eq(x => x.username, username);
                var userFound = _users.Find(filterUsername).Project<Users>(Builders<Users>.Projection.Exclude("_id")).ToList().FirstOrDefault();
                userFound.pSuspended = true;
                _users.FindOneAndReplace(filterUsername, userFound);
                return new FunctionResponse { status = "ok", message = "User Suspended", user = userFound };
            }
            catch(Exception ex)
            {
                return new FunctionResponse { status = "error", message = ex.Message };
            }
        }

        public FunctionResponse deleteUser(string username)
        {
            try
            {
                var filterUsername = Builders<Users>.Filter.Eq(x => x.username, username);
                _users.DeleteOne(filterUsername);
                return new FunctionResponse { status = "ok", message = "User Deleted" };
            }
            catch(Exception ex)
            {
                return new FunctionResponse { status = "error", message = ex.Message };
            }
        }

        public FunctionResponse registerUser(Users user)
        {
            try
            {
                var filterUsername = Builders<Users>.Filter.Eq(x => x.username, user.username);
                var userFound = _users.Find(filterUsername).Project<Users>(Builders<Users>.Projection.Exclude("_id").Include("role").Include("username").Include("email").Include("password").Include("Photo").Include("posts").Include("suspended").Include("pSuspend")).ToList().FirstOrDefault();
                if (userFound != null)
                {
                    return new FunctionResponse { flag = false, status = "error", message = "Username Already Exists" };
                }
                else
                {
                    _users.InsertOne(user);
                    return new FunctionResponse { flag = true, status = "ok", message = "New User Created", user = user };
                }

            }
            catch(Exception ex)
            {
                return new FunctionResponse { flag = false, status = "error", message = ex.Message };
            }
        }

        public IList<Users> getAllUsers()
        {
            var filter = new BsonDocument();
            var result = _users.Find<Users>(filter).Project<Users>(Builders<Users>.Projection.Exclude("_id")).ToList();
            return result;
        } 

        public FunctionResponse getUserDetails(string username)
        {
            try
            {
                var filter = Builders<Users>.Filter.Eq(x => x.username, username);
                var user = _users.Find(filter).Project<Users>(Builders<Users>.Projection.Exclude("_id").Include("role").Include("username").Include("email").Include("password").Include("Photo").Include("posts").Include("suspended").Include("pSuspend")).ToList().FirstOrDefault();
                return new FunctionResponse { status = "ok", message = "User Found", user = user, flag = true };

            }
            catch(Exception ex)
            {
                return new FunctionResponse { status = "error", message = ex.Message, flag = false };
            }
        }

        public IList<Users> Read() => _users.Find(sub => true).Project<Users>(Builders<Users>.Projection.Exclude("_id").Include("role").Include("username").Include("email").Include("password").Include("Photo").Include("posts").Include("suspended").Include("pSuspend")).ToList();

        public FunctionResponse authenticateUser(Users user)
        {
            try
            {
                var filterUsername = Builders<Users>.Filter.Eq(x => x.username, user.username);
                var userFound = _users.Find<Users>(filterUsername).Project<Users>(Builders<Users>.Projection.Exclude("_id").Include("role").Include("username").Include("email").Include("password").Include("Photo").Include("posts").Include("suspended").Include("pSuspend")).ToList().FirstOrDefault();
                user.role = userFound.role;
                user.email = userFound.email;
                user.Photo = userFound.Photo;
                user.suspended = userFound.suspended;
                if (userFound != null)
                {
                    if (userFound.password == user.password)
                    {

                        if( user.suspended == true)
                        {
                            return new FunctionResponse { flag = false, status = "error", message = "User Suspended", user = user };
                        }
                        else
                        {
                            return new FunctionResponse { flag = true, status = "ok", message = "User Found", user = user };
                        }

                    }
                    else
                    {
                        return new FunctionResponse { flag = false, status = "error", message = "Password Incorrect" };
                    }
                }
                return new FunctionResponse { flag = false, status = "error", message = "No User Found" };

            }
            catch (Exception ex)
            {
                return new FunctionResponse { flag = false, status = "error", message = ex.Message };
            }
        }
        
    }
}
