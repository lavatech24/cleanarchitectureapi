using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Domain.Models.SystemModels;

[Table("ApiSubscriptions")]

[Index("ApiUserId", Name = "UQ__ApiSubsc__8508824B92DE3CA6", IsUnique = true)]
public partial class ApiSubscription
{
    [Key]
    public int Id { get; set; }

    public Guid ApiUserId { get; set; }

    [StringLength(10)]
    public string SubscriptionType { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime StartDate { get; set; }

	[Column(TypeName = "datetime")]
	public DateTime? EndDate { get; set; }

	public bool? Status { get; set; }

	[Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    public int? CreatedBy { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    public int? UpdatedBy { get; set; }

    [ForeignKey("ApiUserId")]
    [InverseProperty("ApiSubscription")]
    public virtual ApiUser ApiUser { get; set; } = null!;
}
