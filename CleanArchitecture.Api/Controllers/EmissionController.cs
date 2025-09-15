using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitecture.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmissionController : ControllerBase
    {
        private readonly IEmissionService emissionService;

        public EmissionController(IEmissionService emissionService)
        {
            this.emissionService = emissionService;
        }
        [HttpPost]
        public async Task<IActionResult> GetEmissions([FromBody] EmissionInputDto emissionInput)
        {
            var result = await emissionService.GetDashboardEmissionsAsync(emissionInput);
            return Ok(result);
        }
    }
}
