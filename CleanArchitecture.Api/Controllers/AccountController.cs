using CleanArchitecture.Api.Filters;
using CleanArchitecture.Api.Filters.Attributes;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Models.SystemModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [ServiceFilter(typeof(RateLimitResourceFilter))]
    [Authorize(Roles = "sysadmin, admin")]
    [Route("api/[controller]/{company}")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }
        [TypeFilter(typeof(ExecutionTimeLoggingFilterAttribute))]
        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ApiUserResponseDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [Produces("application/json")]
        public async Task<IActionResult> GetAllUsers(string company)
        {
            var users = await accountService.GetAllUsersAsync(company);
            return Ok(users);
        }
        [HttpGet("user/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(string company, Guid userId)
        {
            var user = await accountService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        [HttpGet("client/{clientId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByClientId(string company, string clientId)
        {
            var user = await accountService.GetUserByClientIdAsync(clientId);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }
        /*
        [HttpGet("status/{status}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ApiUserStatusDto>))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUsersByStatus(string company, bool status)
        {
            var users = await accountService.GetUsersByStatusAsync(company, status);
            return Ok(users);
        }
        */
        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser(string company, string org)
        {
            //if (user == null)
            //{
            //    return BadRequest("User cannot be null.");
            //}
            var createdUser = await accountService.CreateUserAsync(company);
            return CreatedAtAction(nameof(CreateUser), new { userId = createdUser.ClientId }, createdUser);
        }
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiUserResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateUser(string company, [FromBody] ApiUserUpdateDto user)
        {
            if (user == null)
            {
                return BadRequest("User data is invalid.");
            }
            var updatedUser = await accountService.UpdateUserAsync(user);
            if (updatedUser == null)
            {
                return NotFound();
            }
            return Ok(updatedUser);
        }
        [HttpDelete("delete/{clientId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteUser(string company, string clientId)
        {
            try
            {
                if (!await accountService.UserExistsAsync(clientId))
                {
                    return NotFound();
                }
                await accountService.DeleteUserAsync(clientId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
        [HttpGet("client-exists/{clientId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserExists(string company, string clientId)
        {
            if (String.IsNullOrEmpty(clientId))
            {
                return BadRequest();
            }
            try
            {
                var exists = await accountService.UserExistsAsync(clientId);
                return Ok(exists);
            }
            catch(ArgumentException ex)
            {
                return BadRequest($"Invalid user ID: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
