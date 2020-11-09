using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace UzunTec.API.Authentication.Engine
{
    public class Authenticator
    {
        private readonly AuthenticationConfig config;
        private readonly Lazy<SecurityKey> lazyKey;
        private SecurityKey Key { get { return this.lazyKey.Value; } }

        public Authenticator(AuthenticationConfig config)
        {
            this.config = config;
            this.lazyKey = new Lazy<SecurityKey>(this.GenerateSecurityKey);
        }
        public TokenData GenerateToken(string userCode)
        {
            return this.GenerateToken(userCode, null);
        }

        public TokenData GenerateToken(string userCode, IReadOnlyDictionary<string, string> claims)
        {
            return this.GenerateToken(userCode, null, null);
        }
        public TokenData GenerateToken(string userCode, IReadOnlyDictionary<string, string> claims, IEnumerable<string> roles)
        {
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = this.GetSigningCredentials(),
                Issuer = this.config.Issuer,
                Audience = this.config.Audience,
                Subject = this.GenerateClaimsIdentity(userCode, claims, roles),
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddSeconds(this.config.TokenExpireTimeInSeconds),
            };

            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            return new TokenData(tokenHandler.WriteToken(token), this.config.TokenExpireTimeInSeconds, "Bearer");
        }


        public void SetBearerTokenOptions(JwtBearerOptions jwtOptions)
        {
            jwtOptions.RequireHttpsMetadata = false;
            jwtOptions.SaveToken = true;

            jwtOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = this.Key,
                ValidateIssuer = !String.IsNullOrEmpty(this.config.Issuer),
                ValidIssuer = this.config.Issuer,
                ValidateAudience = !String.IsNullOrEmpty(this.config.Audience),
                ValidAudience = this.config.Audience,
                ValidateLifetime = true,
            };
        }

        private ClaimsIdentity GenerateClaimsIdentity(string userCode, IReadOnlyDictionary<string, string> claims, IEnumerable<string> roles)
        {
            ClaimsIdentity identity = new ClaimsIdentity();
            identity.AddClaims(new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, this.config.JtiCode),
                new Claim(JwtRegisteredClaimNames.UniqueName, userCode),
                new Claim(ClaimTypes.NameIdentifier, userCode)
            });

            foreach (string role in roles ?? new string[0])
            {
                identity.AddClaim(new Claim(ClaimsIdentity.DefaultRoleClaimType, role));
            }

            foreach (string claimCode in claims?.Keys ?? new string[0])
            {
                identity.AddClaim(new Claim(claimCode, claims[claimCode]));
            }
            return identity;
        }

        private SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(this.Key, this.config.SignatureAlgorithm);
        }

        private SecurityKey GenerateSecurityKey()
        {
            if (this.config.SignatureAlgorithm == SecurityAlgorithms.RsaSha256Signature)
            {
                using (RSACryptoServiceProvider provider = new RSACryptoServiceProvider(2048))
                {
                    return new RsaSecurityKey(provider.ExportParameters(true));
                }
            }
            else
            {
                byte[] keyData = Encoding.ASCII.GetBytes(this.config.SignatureKey);
                return new SymmetricSecurityKey(keyData);
            }
        }
    }
}
