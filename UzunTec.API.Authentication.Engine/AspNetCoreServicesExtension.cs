using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UzunTec.API.Authentication.Engine
{
    public static class AspNetCoreServicesExtension
    {
        public static void AddAuthenticationEngine(this IServiceCollection services, IConfigurationSection authConfigurationSection)
        {
            AuthenticationConfig authenticationConfig = authConfigurationSection.Get<AuthenticationConfig>();
            Authenticator authenticator = new Authenticator(authenticationConfig);
            services.AddSingleton<AuthenticationConfig>(authenticationConfig);
            services.AddSingleton<Authenticator>(authenticator);

            services.AddAuthentication(delegate (AuthenticationOptions authOptions)
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(delegate (JwtBearerOptions jwtOptions)
            {
                authenticator.SetBearerTokenOptions(jwtOptions, authenticationConfig);
            });
        }
    }
}
