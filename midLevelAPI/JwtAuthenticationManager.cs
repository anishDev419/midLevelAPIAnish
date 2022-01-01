using Microsoft.IdentityModel.Tokens;
using midLevelAPI.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace midLevelAPI
{
    public class JwtAuthenticationManager: IJwtAuthenticationManager
    {
        private readonly IMongoCollection<Users> _users;
        private readonly string _key;
        public JwtAuthenticationManager(string key)
        {
            this._key = key;
        }

        public JwtAuthenticationManager(IDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _users = database.GetCollection<Users>("Users");
        }


        public string Authenticate(string usernameF, string passwordF)
        {

            //if (userFound == null)
            //{
            //    return null;
            //}

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, usernameF)
                }
                ),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = new SigningCredentials( new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);

        }
    }
}
