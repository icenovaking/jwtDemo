using jwtAuthDemo.Models;
using jwtAuthDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace jwtAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserContext _userContext;

        public AuthController(UserContext userContext, ITokenService tokenService)
        {
            _userContext = userContext;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel loginModel)
        {
            var user = _userContext.LoginModels.FirstOrDefault(u =>(u.UserName == loginModel.UserName) && (u.Password == loginModel.Password));

            if (user is null)
                return BadRequest("invalid request data !");

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, "Manager")
            };
            var accessToken = _tokenService.GenerateAccessToken(claims);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);
            _userContext.SaveChanges();
            return Ok(new AuthenticatedResponse
            {
                Token = accessToken,
                RefreshToken = refreshToken
            });
        }
    }
}
