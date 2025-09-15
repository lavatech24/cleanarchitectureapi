using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Domain.Models.SystemModels;

public partial class ApiUser
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    [Unicode(false)]
    public string ClientId { get; set; } = null!;

    //[StringLength(100)]
    //[Unicode(false)]
    [NotMapped]
    public string ClientSecret { get; set; } = null!;

    [StringLength(300)]
    [Unicode(false)]
    public string Salt { get; set; } = null!;

    [StringLength(500)]
    [Unicode(false)]
    public string ClientSecretHash { get; set; } = null!;

    public bool IsActive { get; set; }

    public bool IsSuperuser { get; set; }

    public int? CompanyId { get; set; }

    public int? RoleId { get; set; }

    [StringLength(300)]
    [Unicode(false)]
    public string? RefreshToken { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? RefreshTokenExpiry { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

	[InverseProperty(nameof(ApiSubscription.ApiUser))]
	public virtual ApiSubscription? ApiSubscription { get; set; } = null!;

	[ForeignKey("CompanyId")]
	[InverseProperty("ApiUsers")]
	public virtual Company? Company { get; set; }

	[ForeignKey("RoleId")]
    [InverseProperty("ApiUsers")]
    public virtual Role? Role { get; set; }

	[InverseProperty(nameof(ApiWhitelistedIP.ApiUser))]
	public virtual ICollection<ApiWhitelistedIP> ApiWhitelistedIPs { get; set; } = new List<ApiWhitelistedIP>();
}
