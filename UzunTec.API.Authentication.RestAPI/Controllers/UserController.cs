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
            if (user == "renato")
            {
                return this.Ok(this.authenticator.GenerateToken(user, new Dictionary<string, string> { { "teste", "teste" } }, new List<string> { "Admin" }));
            }
            else
            {
                return this.Ok(this.authenticator.GenerateToken(user));
            }
        }

    }
}
