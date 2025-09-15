using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.DTOs
{
    public class TokenResponseDto
    {
        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        //public int? Expiry{ get; set; }
        public string RefreshToken { get; set; }
        //public DateTime? RefreshTokenExpiry { get; set; }
    }
}
