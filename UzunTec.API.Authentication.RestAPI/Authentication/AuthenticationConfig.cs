using Microsoft.IdentityModel.Tokens;

namespace UzunTec.API.Authentication.RestAPI.Authentication
{
    public class AuthenticationConfig
    {
        public string JtiCode { get; set; }
        public string SignatureKey { get; set; }
        public string SignatureAlgorithm { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
        public int TokenExpireTimeInSeconds { get; set; }

        public AuthenticationConfig()
        {
            // Commoms: RsaSha256Signature or HmacSha256Signature
            this.SignatureAlgorithm = SecurityAlgorithms.HmacSha256Signature;
        }
    }
}