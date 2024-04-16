using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using HaloDocRepository.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HaloDocWeb.Controllers.Admin
{
    [AttributeUsage(AttributeTargets.All)]
    public class CheckAccess : Attribute, IAuthorizationFilter
    {
        private readonly List<string> _role;
        public CheckAccess(string role)
        {
            _role = role.Split(',').ToList();
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwtService>();
            if (jwtservice == null)
            {
                filterContext.Result = new RedirectResult("../Admin/IndexPlatformLogin");
                return;
            }
            var request = filterContext.HttpContext.Request;
            var toket = request.Cookies["jwt"];
            if (toket == null || !jwtservice.ValidateToken(toket, out JwtSecurityToken jwtSecurityTokenHandler))
            {
                filterContext.Result = new RedirectResult("../Admin/IndexPlatformLogin");
                return;
            }
            var roles = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == ClaimTypes.Role);

            if (roles == null)
            {
                filterContext.Result = new RedirectResult("../Admin/IndexPlatformLogin");
                return;
            }

            var flag = false;
            foreach (var role in _role)
            {
                if (string.IsNullOrWhiteSpace(role) || roles.Value != role)
                {
                    flag = false;
                }
                else
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                filterContext.Result = new RedirectResult("../Home/AuthError");
            }

        }

    }
}