using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using HaloDocRepository.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace HalloDoc.Controllers.Admin
{
    [AttributeUsage(AttributeTargets.All)]
    public class CheckPhysicianAccess : Attribute, IAuthorizationFilter
    {
        private readonly string _role;
        public CheckPhysicianAccess(string role)
        {
            _role = role;
        }
        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            var jwtservice = filterContext.HttpContext.RequestServices.GetService<IJwtService>();
            if (jwtservice == null)
            {
                filterContext.Result = new RedirectResult("../Admin/Index");
                return;
            }
            var request = filterContext.HttpContext.Request;
            var toket = request.Cookies["jwt"];
            if (toket == null || !jwtservice.ValidateToken(toket, out JwtSecurityToken jwtSecurityTokenHandler))
            {
                filterContext.Result = new RedirectResult("../Admin/Index");
                return;
            }
            var roles = jwtSecurityTokenHandler.Claims.FirstOrDefault(claiim => claiim.Type == ClaimTypes.Role);

            if (roles == null)
            {
                filterContext.Result = new RedirectResult("../Admin/Index");
                return;
            }

            if (string.IsNullOrWhiteSpace(_role) || roles.Value != _role)
            {
                filterContext.Result = new RedirectResult("../Admin/AuthError");

            }

        }

    }
}