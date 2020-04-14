using AutoMapper.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace InventoryBooks.Helper
{
    public class TokenManager
    {
      
        public static string CreateToken(string userId, string firstName, string emailAddress, string roles)
        {
            var key = Encoding.ASCII.GetBytes("OfED+KgbZxtu4e4+JSQWdtSgTnuNixKy1nMVAEww8QL3IN3idcNgbNDSSaV4491Fo3sq2aGSCtYvekzs7JwXJnNAyvDSJjfK");
            var sharedKey = new SymmetricSecurityKey(key);

            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, emailAddress),
                new Claim(ClaimTypes.Name,firstName),
                new Claim(ClaimTypes.PrimarySid, userId),
                new Claim(ClaimTypes.Role, roles),
                new Claim(ClaimTypes.System, "Web")
            };
            var signinCredentials = new SigningCredentials(sharedKey, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescription = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.Now,
                Expires = DateTime.Now.AddMinutes(30),
                SigningCredentials = signinCredentials
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescription);
            string tokenString = tokenHandler.WriteToken(token);
            return tokenString;

        }

    }
}