using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApiApp.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public AuthController(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        [HttpPost]
        public IActionResult Authenticate([FromBody]Credential credential)
        {

            //Verify credential
            if (credential.UserName == "admin" && credential.Password == "password")
            {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.Name, "admin")
                    , new Claim(ClaimTypes.Email, "admin@mywebsite.com")
                    , new Claim("Department", "HR")
                    , new Claim("Admin", "true")
                    , new Claim("Manager", "true")
                    ,new Claim("EmploymentDate","2022-02-18")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);
                return Ok(
                    new
                    {
                        access_token=CreateToken(claims,expiresAt),
                        expires_at=expiresAt
                    });
            }
            ModelState.AddModelError("UnAuthorized", "You are not authorized to access endpoint");
            return Unauthorized(ModelState);
        }

        public string CreateToken(IEnumerable<Claim> claims,DateTime expiresAt)
        {
            var secretKey = Encoding.ASCII.GetBytes(Configuration.GetValue<string>("SecretKey"));

            var jwt = new JwtSecurityToken(
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiresAt,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature));
            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
    public class Credential
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }
}
