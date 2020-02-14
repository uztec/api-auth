using Microsoft.Extensions.Configuration;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using UzunTec.API.Authentication.RestAPI.Authentication;

namespace UzunTec.API.Authentication.RestAPI
{
    public class APIAuthContainer : Container
    {
        private readonly IConfiguration configuration;

        public APIAuthContainer(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
        }

        public void Initialize()
        {
            //this.Register<Authenticator>(Lifestyle.Scoped);
            //this.Register<AuthenticationConfig>(this.BuildAuthConfig, Lifestyle.Scoped);


#if DEBUG
            this.Verify();
#endif
        }

        private AuthenticationConfig BuildAuthConfig()
        {
            return this.configuration.GetSection("AuthSettings").Get<AuthenticationConfig>();
        }
    }

}