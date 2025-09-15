using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs
{
    public class EmissionResultDto
    {
        public string Process { get; set; }
        public string MfoName { get; set; }
        public string LciName { get; set; }
        public int ImpactCategoryId { get; set; }
        public string ImpactCategoryUnit { get; set; }
        public double Emissions { get; set; }
    }
}
