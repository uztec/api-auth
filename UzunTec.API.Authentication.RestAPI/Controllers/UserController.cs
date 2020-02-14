using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UzunTec.API.Authentication.Engine;

namespace UzunTec.API.Authentication.RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Authenticator authenticator;

        public UserController(Authenticator authenticator)
        {
            this.authenticator = authenticator;
        }

        [HttpGet("{user}")]
        public ActionResult<TokenData> Get(string user)
        {
            if (user == "admin")
            {
                Dictionary<string, string> claims = new Dictionary<string, string>
                {
                    { "Access", "full" },
                };

                List<string> roles = new List<string>
                {
                    "WritePermission",
                };

                TokenData token = this.authenticator.GenerateToken(user, claims, roles);
                return this.Ok(token);
            }
            else
            {
                return this.Ok(this.authenticator.GenerateToken(user)); // No Claims and no Roles
            }
        }

    }
}
