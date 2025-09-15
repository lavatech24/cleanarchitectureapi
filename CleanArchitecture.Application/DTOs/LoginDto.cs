using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs
{
    public class LoginDto
    {
        public string ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
