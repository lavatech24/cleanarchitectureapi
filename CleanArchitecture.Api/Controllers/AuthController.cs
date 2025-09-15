using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }
        [HttpPost("token")]
        public async Task<IActionResult> Login(LoginDto request)
        {
            var response = await authService.GetToken(request);
            return Ok(response);
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(ApiUserRefreshTokenDto request)
        {
            var result = await authService.GenerateRefreshToken(request);
            if (result != null)
            {
                return Ok(new { ClientId = result.ClientId, AccessToken = result.AccessToken, RefreshToken = result.RefreshToken });
            }
            return BadRequest(new { message = "Failed to refresh token." });
        }
        [HttpPost("new-secret")]
        public async Task<IActionResult> GenerateNewSecret(LoginDto request)
        {
            var result = await authService.GenerateNewSecret(request);
            if (result != null)
            {
                return Ok(new { ClientId = result.ClientId, ClientSecret = result.ClientSecret });
            }
            return BadRequest(new { message = "Failed to generate new secret." });
        }
    }
}
