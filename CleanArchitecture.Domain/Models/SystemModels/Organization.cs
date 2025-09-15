using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Domain.Models.SystemModels;

[Table("Organization")]
public partial class Organization
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("CompanyID")]
    public int CompanyId { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string OrgCode { get; set; } = null!;

    [StringLength(50)]
    [Unicode(false)]
    public string OrgName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [ForeignKey("CompanyId")]
    [InverseProperty("Organizations")]
    public virtual Company Company { get; set; } = null!;

	[InverseProperty("Organization")]
	public virtual ICollection<ApiUser> ApiUsers { get; set; } = new List<ApiUser>();
}
