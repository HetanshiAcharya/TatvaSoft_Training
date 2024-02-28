using HaloDocDataAccess.DataModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HaloDocDataAccess.DataModels;
using HaloDocDataAccess.DataContext;
using HaloDocDataAccess.ViewModels;
using HaloDocRepository.Interface;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;

namespace HaloDocRepository.Repositories
{
    public class AuthService : IAuthService
    {
        private readonly HaloDocDbContext _context;

        public AuthService(HaloDocDbContext context)
        {
            _context = context;
        }
        //Generate Password Hash function
        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        public bool PatientAuthentication(PatientLoginDetails patientLogin)
        {
            string hashPassword = GenerateSHA256(patientLogin.Password);
            return _context.AspNetUsers.Any(Au => Au.Email == patientLogin.Email && Au.PasswordHash == hashPassword);
        }

        public bool PatientForgotPass(PatientForgotPassword model)
        {
            PatientLoginDetails patientResetPassword = new PatientLoginDetails();
            return _context.AspNetUsers.Any(x => x.Email == model.Email);
        }
        public void ResetPassword(PatientResetPassword model)
        {
            if (model.Password == model.ConfirmPassword)
            {
                AspNetUser user1 = _context.AspNetUsers.FirstOrDefault(rq => rq.Email == model.Email);
                string hashPassword = GenerateSHA256(model.Password);
                user1.PasswordHash = hashPassword;
                _context.AspNetUsers.Update(user1);
                _context.SaveChanges();

            }
        }
    }
}
