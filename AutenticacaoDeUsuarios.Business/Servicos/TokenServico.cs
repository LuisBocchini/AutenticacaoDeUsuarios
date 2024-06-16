using AutenticacaoDeUsuarios.Data.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutenticacaoDeUsuarios.Business.Servicos
{
    public static class TokenServico
    {
        private static IConfigurationBuilder Builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false);
        private static IConfiguration oConfig = Builder.Build();
        private static string chave = oConfig["ChaveJWT"];
        public static string? GerarToken(Usuario? usuario, int minutosExpiracao = 60)
        {
            if (usuario == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(chave);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("Email", !string.IsNullOrEmpty(usuario.Email) ? usuario.Email: string.Empty),
                }),
                Expires = DateTime.UtcNow.AddMinutes(minutosExpiracao),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public static ClaimsPrincipal ObterClaimsToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(chave)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
        public static string GerarRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public static DateTime? ObterDataExpiracaoToken(string? token)
        {
            if (token == null)
                return null;

            var claimsUsuario = TokenServico.ObterClaimsToken(token);
            var expiracao = long.Parse(claimsUsuario?.Claims.FirstOrDefault(c => c.Type == "exp")?.Value);
            var dataExpiracao = DateTimeOffset.FromUnixTimeSeconds(expiracao).LocalDateTime;
            return dataExpiracao;
        }

    }
}
