using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Repositories
{
    public class EmissionRepository : IEmissionRepository
    {
        private readonly CleanArchitectureContext dbContext;

        public EmissionRepository(CleanArchitectureContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IEnumerable<EmissionResultDto>> GetDashboardEmissionsAsync(EmissionInputDto emissionInput)
        {
            //var inputData = await dbContext.ProgramOptions.Include().Where
            var result =  await dbContext.EmissionResults.FromSqlRaw("EXEC spGetGhgComparisonByLci @p0, @p1, @p2",
				parameters: new[] { emissionInput.ProgramId.ToString(), emissionInput.ForecastId.ToString(), emissionInput.Year.ToString() }).ToListAsync(); //Global Warming-100yrs
			//parameters: new[] { "4", "11", "2024"}).ToListAsync(); 

			return result.Where(w => w.ImpactCategoryId == emissionInput.ImpactCategoryId && w.Emissions > 0);

        }
    }
}
