using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthorizationService.Utils
{
    public class JwtHelper
    {
        private readonly IConfiguration _config;

        public JwtHelper(IConfiguration config)
        {
            _config = config;
        }

        public virtual string GetToken(Dictionary<string, string> claimsDict)
        {
            string secret = _config.GetValue<string>("Jwt:Secret");
            string issuer = _config.GetValue<string>("Jwt:Issuer");

            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new List<Claim>();
            foreach (var claimPair in claimsDict)
            {
                claims.Add(new Claim(claimPair.Key, claimPair.Value));
            }

            var token = new JwtSecurityToken(
                issuer: issuer,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
