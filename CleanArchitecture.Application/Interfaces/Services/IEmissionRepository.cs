using CleanArchitecture.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Interfaces.Services;

public interface IEmissionRepository
{
    Task<IEnumerable<EmissionResultDto>> GetDashboardEmissionsAsync(EmissionInputDto emissionInput);
}
