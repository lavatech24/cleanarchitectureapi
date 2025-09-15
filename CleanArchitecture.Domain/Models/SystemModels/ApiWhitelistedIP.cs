using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Domain.Models.SystemModels;

public class ApiWhitelistedIP
{
    [Key]
    public int Id { get; set; }
    public Guid ApiUserId { get; set; }
    [StringLength(50)]
    [Unicode(false)]
    public string IPAddress { get; set; } = null!;
    public bool IsActive { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

	[ForeignKey("ApiUserId")]
	[InverseProperty(nameof(ApiUser.ApiWhitelistedIPs))]
	public virtual ApiUser ApiUser { get; set; } = null!;
}