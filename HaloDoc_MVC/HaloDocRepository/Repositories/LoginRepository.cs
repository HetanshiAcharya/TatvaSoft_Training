using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HaloDocRepository.Repositories
{
        public class LoginRepository : ILoginRepository
        {
            #region Constructor
            private readonly IHttpContextAccessor httpContextAccessor;
            private readonly HaloDocDbContext _context;
            public LoginRepository(HaloDocDbContext context, IHttpContextAccessor httpContextAccessor)
            {
                this.httpContextAccessor = httpContextAccessor;
                _context = context;
            }
            #endregion

            #region Constructor
            public async Task<UserInfo> CheckAccessLogin(AspNetUser aspNetUser)
            {
                var user = await _context.AspNetUsers.FirstOrDefaultAsync(u => u.Email == aspNetUser.Email && u.PasswordHash == aspNetUser.PasswordHash);
                UserInfo admin = new UserInfo();
                if (user != null)
                {
                    var data = _context.AspNetUserRoles.FirstOrDefault(E => E.UserId == user.Id);
                    var datarole = _context.AspNetRoles.FirstOrDefault(e => e.Id == data.RoleId);
                    admin.Username = user.UserName;
                    admin.FirstName = admin.FirstName ?? string.Empty;
                    admin.LastName = admin.LastName ?? string.Empty;
                    admin.Role = datarole.Name;
                    if (admin.Role == "Admin")
                    {
                        var admindata = _context.Admins.FirstOrDefault(u => u.AspNetUserId == user.Id);
                        admin.UserId = admindata.AdminId;
                    }
                    else if (admin.Role == "Patient")
                    {
                        var admindata = _context.Users.FirstOrDefault(u => u.AspNetUserId == user.Id);
                        admin.UserId = admindata.UserId;
                    }
                    else
                    {
                        var admindata = _context.Physicians.FirstOrDefault(u => u.AspNetUserId == user.Id);
                        admin.UserId = admindata.PhysicianId;
                    }
                    return admin;
                }
                else
                {
                    return null;
                }
            }
            #endregion
        }
    }

