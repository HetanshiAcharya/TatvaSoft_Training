using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Globalization;
using System.Net.Http;
using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics.Metrics;
using System.IO;

namespace HaloDocRepository.Repositories
{
    public class ProviderService : IProviderService
    {
        private readonly HaloDocDbContext _context;
        private readonly IConfiguration _config;
        private readonly EmailConfiguration _emailConfig;

        public ProviderService(HaloDocDbContext context, IConfiguration config, EmailConfiguration emailConfig)
        {
            _context = context;
            _config = config;
            _emailConfig = emailConfig;
        }
            #region Find_Location_Physician
            public List<PhysicianLocation> FindPhysicianLocation()
            {
                List<PhysicianLocation> pl =  _context.PhysicianLocations
                                        .OrderByDescending(x => x.PhysicianName)
                            .Select(r => new PhysicianLocation
                            {
                                LocationId = r.LocationId,
                                Longitude = r.Longitude,
                                Latitute = r.Latitute,
                                PhysicianName = r.PhysicianName

                            }).ToList();
                return pl;

            }
            #endregion
        }
    }
