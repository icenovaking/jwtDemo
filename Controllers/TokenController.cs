using jwtAuthDemo.Models;
using jwtAuthDemo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace jwtAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserContext _userContext;
        private readonly ITokenService _tokenService;
        public TokenController(UserContext userContext, ITokenService tokenService)
        {
            _userContext = userContext;
            _tokenService = tokenService;
        }
        [HttpPost]
        [Route("refresh")]
        public IActionResult Refresh(TokenApiModel tokenApiModel)
        {
            if (tokenApiModel is null)
                return BadRequest("Invalid client request");
            string accessToken = tokenApiModel.AccessToken;
            string refreshToken = tokenApiModel.RefreshToken;
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            var username = principal.Identity.Name; //this is mapped to the Name claim by default
            var user = _userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            if (user is null)
                return NotFound("User Not Found");
            if (user.RefreshToken != refreshToken)
                return BadRequest("Invalid RefreshToken");
            if (user.RefreshTokenExpiryTime <= DateTime.Now)
                return BadRequest("RefreshToken Expire");

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            _userContext.SaveChanges();
            return Ok(new AuthenticatedResponse()
            {
                Token = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpPost, Authorize]
        [Route("revoke")]
        public IActionResult Revoke()
        {
            var username = User.Identity.Name;
            var user = _userContext.LoginModels.SingleOrDefault(u => u.UserName == username);
            if (user == null) return BadRequest();
            user.RefreshToken = null;
            _userContext.SaveChanges();
            return NoContent();
        }
    }
}
