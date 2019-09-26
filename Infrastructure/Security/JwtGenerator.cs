using System.Collections.Generic;
using System.Security.Claims;
using Application.Interfaces;
using Domain;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Security
{
    public class JwtGenerator : IJwtGenerator
    {
        private readonly SymmetricSecurityKey _securityKey;
        public JwtGenerator(IConfiguration configuration)
        {
            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["TokenKey"]));
        }

        public string CreateToken(AppUser user)
        {
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.NameId,user.UserName)
            };
            var claimsIdentity = new ClaimsIdentity(claims);

            var signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}