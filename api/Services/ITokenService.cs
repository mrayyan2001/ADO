using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Services
{
    public interface ITokenService
    {
        public string CreateToken(int userId, string email, string role);
    }
}