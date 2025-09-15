using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Domain.Models.SystemModels;

[Table("Company")]
public partial class Company
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [StringLength(50)]
    [Unicode(false)]
    public string CompanyCode { get; set; } = null!;

    [StringLength(100)]
    [Unicode(false)]
    public string CompanyName { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    [InverseProperty("Company")]
    public virtual ICollection<ApiUser> ApiUsers { get; set; } = new List<ApiUser>();
}
