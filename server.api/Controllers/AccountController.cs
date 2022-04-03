using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace server.Controllers
{
    public class LoginCredentials
    {
      public string Email { get; set;}
      public string Password { get; set;}
    }

    [Route("[controller]")]
    public class AccountController : Controller
    {

        private static readonly SigningCredentials SigningCreds = new SigningCredentials(Startup.SecurityKey, SecurityAlgorithms.HmacSha256);
        private readonly JwtSecurityTokenHandler _tokenHandler = new JwtSecurityTokenHandler();

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginCredentials creds)
        {
            if (!ValidateLogin(creds))
            {
                return Json(new
                {
                    error = "Login failed"
                });
            }

            var principal = GetPrincipal(creds, Startup.CookieAuthScheme);

            await HttpContext.SignInAsync(Startup.CookieAuthScheme, principal);

            return Json(new
            {
                name = principal.Identity.Name,
                email = principal.FindFirstValue(ClaimTypes.Email),
                role = principal.FindFirstValue(ClaimTypes.Role)
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return StatusCode(200);
        }

        [HttpGet("context")]
        public JsonResult Context()
        {
          return Json(new
          {
              name = this.User?.Identity?.Name,
              email = this.User?.FindFirstValue(ClaimTypes.Email),
              role = this.User?.FindFirstValue(ClaimTypes.Role),
          });
        }

        [HttpPost("token")]
        public async Task<IActionResult> Token([FromBody]LoginCredentials creds)
        {
            if (!ValidateLogin(creds))
            {
                return Json(new
                {
                    error = "Login failed"
                });
            }

            var principal = GetPrincipal(creds, Startup.JWTAuthScheme);

            var token = new JwtSecurityToken(
                "soSignalR",
                "soSignalR",
                principal.Claims,
                expires: DateTime.UtcNow.AddDays(30),
                signingCredentials: SigningCreds);

            return Json(new
            {
                token = _tokenHandler.WriteToken(token),
                name = principal.Identity.Name,
                email = principal.FindFirstValue(ClaimTypes.Email),
                role = principal.FindFirstValue(ClaimTypes.Role)
            });
        }

        private bool ValidateLogin(LoginCredentials creds)
        {
            // For this test, all logins are successful!
            return true;
        }

        private ClaimsPrincipal GetPrincipal(LoginCredentials creds, string authScheme)
        {
          var claims = new List<Claim>
          {
              new Claim(ClaimTypes.Name, creds.Email),
              new Claim(ClaimTypes.Email, creds.Email),
              new Claim(ClaimTypes.Role, "User"),
          };
          return new ClaimsPrincipal(new ClaimsIdentity(claims, authScheme));
        }
    }
}