using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Services
{
    public class EmissionService : IEmissionService
    {
        private readonly IEmissionRepository emissionRepository;

        public EmissionService(IEmissionRepository emissionRepository)
        {
            this.emissionRepository = emissionRepository;
        }
        public async Task<IEnumerable<EmissionResultDto>> GetDashboardEmissionsAsync(EmissionInputDto emissionInput)
        {
            return await emissionRepository.GetDashboardEmissionsAsync(emissionInput);
        }
    }
}
