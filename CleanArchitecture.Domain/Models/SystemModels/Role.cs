using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Domain.Models.SystemModels;

public partial class Role
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string RoleCode { get; set; } = null!;

    [StringLength(100)]
    public string RoleName { get; set; } = null!;

    public bool IsVisible { get; set; }

    public bool IsActive { get; set; }

    public int? ParentId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DateCreated { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? DateModified { get; set; }

    public int? ModifiedBy { get; set; }

    [InverseProperty("Parent")]
    public virtual ICollection<Role> InverseParent { get; set; } = new List<Role>();

    [ForeignKey("ParentId")]
    [InverseProperty("InverseParent")]
    public virtual Role? Parent { get; set; }

    [InverseProperty("Role")]
    public virtual ICollection<ApiUser> ApiUsers { get; set; } = new List<ApiUser>();
}
