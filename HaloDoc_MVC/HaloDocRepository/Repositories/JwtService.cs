using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace HaloDocRepository.Repositories
{
    public class JwtService : IJwtService
    {
        #region Constructor
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IConfiguration Configuration;
        public JwtService(IConfiguration Configuration, IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.Configuration = Configuration;
        }
        #endregion
        public string GenerateJWTAuthetication(UserInfo userinfo)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Email, userinfo.Username),
            new Claim(ClaimTypes.Role, userinfo.Role),
            new Claim("FirstName", userinfo.FirstName),
            new Claim("UserId", userinfo.UserId.ToString()),
            new Claim("Username", userinfo.Username.ToString()),
            new Claim("AspNetUserId", userinfo.AspNetUserId.ToString())
        };
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires =
                DateTime.UtcNow.AddMinutes(60);

            var token = new JwtSecurityToken(
                Configuration["Jwt:Issuer"],
                Configuration["Jwt:Audience"],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
        public bool ValidateToken(string token, out JwtSecurityToken jwtSecurityTokenHandler)
        {
            jwtSecurityTokenHandler = null;

            if (token == null)
                return false;

            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(Configuration["Jwt:Key"]);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero

                }, out SecurityToken validatedToken);

                jwtSecurityTokenHandler = (JwtSecurityToken)validatedToken;

                if (jwtSecurityTokenHandler != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

    }
}
