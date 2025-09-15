using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs;

public class EmissionInputDto
{
    public int ProgramId { get; set; }
    public int ForecastId { get; set; }
    public int Year { get; set; }
    public int ImpactCategoryId { get; set; }
}
