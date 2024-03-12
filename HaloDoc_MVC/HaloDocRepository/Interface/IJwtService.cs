using HaloDocDataAccess.ViewModels;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocRepository.Interface
{
    public interface IJwtService
    {
        string GenerateJWTAuthetication(UserInfo userinfo);
        bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler);
    }
}
