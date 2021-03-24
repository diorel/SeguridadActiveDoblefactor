using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using BanorteXXISecurity.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BanorteXXISecurity.Business
{
    public class GenerateToken
    {
        public BodyToken CreateToken(string Correo, int ExpritationToken)
        {
            BodyToken TokenCreado = new BodyToken();

            string llaveSecreta = "AXXI77700QRKNEJRNEJJ";

            var claims = new[]
            {
               new Claim(JwtRegisteredClaimNames.UniqueName, Correo),
               new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var Key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(llaveSecreta));
            var creds = new SigningCredentials(Key, SecurityAlgorithms.HmacSha256);
            //var expiracion = DateTime.UtcNow.AddMinutes(5);
            var expiracion = DateTime.Now.AddMinutes(ExpritationToken);

            JwtSecurityToken token = new JwtSecurityToken(
                     issuer: "banorte.com",
                     audience: "banorte.com",
                     claims: claims,
                     expires: expiracion,
                     signingCredentials: creds);

            TokenCreado.TokenCreated = new JwtSecurityTokenHandler().WriteToken(token);
            TokenCreado.TimeToken = expiracion.ToString();

            return TokenCreado;
        }

    }
}
