using System;
using System.Collections.Generic;
using CleanArchitecture.Application.DTOs;
using CleanArchitecture.Domain.Models.SystemModels;
using CleanArchitecture.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Data;

public partial class CleanArchitectureContext : DbContext
{
    public CleanArchitectureContext()
    {
    }

    public CleanArchitectureContext(DbContextOptions<CleanArchitectureContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EmissionResultDto> EmissionResults { get; set; }
    public virtual DbSet<ApiSubscription> ApiSubscription { get; set; }
    public virtual DbSet<ApiUser> ApiUsers { get; set; }
    public virtual DbSet<ApiWhitelistedIP> ApiWhitelistedIPs { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply configurations for ApiUser, this will see initial data if no super user exists
        modelBuilder.ApplyConfiguration(new ApiUserConfiguration());
        modelBuilder.Entity<EmissionResultDto>(entity => entity.HasNoKey());

		modelBuilder.Entity<ApiSubscription>(entity =>
		{
			entity.HasKey(e => e.Id).HasName("PK__ApiSubsc__3214EC07212F25CF");

			entity.Property(e => e.EndDate).HasComputedColumnSql("(case [SubscriptionType] when 'M' then dateadd(month,(1),[StartDate]) when 'Q' then dateadd(month,(3),[StartDate]) when 'Y' then dateadd(year,(1),[StartDate])  end)", false);
			entity.Property(e => e.StartDate).HasDefaultValueSql("(getdate())");
			entity.Property(e => e.Status).HasComputedColumnSql("(CONVERT([bit],case when [SubscriptionType]='M' AND CONVERT([date],getdate())<=(dateadd(month,(1),[StartDate])-(1)) then (1) when [SubscriptionType]='Q' AND CONVERT([date],getdate())<=(dateadd(month,(3),[StartDate])-(1)) then (1) when [SubscriptionType]='Y' AND CONVERT([date],getdate())<=(dateadd(year,(1),[StartDate])-(1)) then (1) else (0) end))", false);
			entity.Property(e => e.SubscriptionType).HasDefaultValue("M");

			entity.HasOne(d => d.ApiUser).WithOne(p => p.ApiSubscription).OnDelete(DeleteBehavior.ClientSetNull);
		});
		modelBuilder.Entity<ApiUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ApiUsers__3214EC079175382F");

            entity.Property(e => e.Id).HasDefaultValueSql("(newid())");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Company).WithMany(p => p.ApiUsers).HasConstraintName("FK_ApiUsers_Company");
			entity.HasOne(d => d.ApiSubscription).WithOne(p => p.ApiUser).HasConstraintName("FK_ApiUsers_Subscription");
			entity.HasOne(d => d.Role).WithMany(p => p.ApiUsers).HasConstraintName("FK_ApiUsers_Role");
        });
        
        modelBuilder.Entity<ApiWhitelistedIP>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_ApiWhitelistedIPs");

            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

			entity.HasOne(d => d.ApiUser).WithMany(p => p.ApiWhitelistedIPs).OnDelete(DeleteBehavior.ClientSetNull);
		});

        modelBuilder.Entity<Company>(entity =>
        {
            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.Property(e => e.DateCreated).HasDefaultValueSql("(getdate())");

            entity.HasOne(d => d.Parent).WithMany(p => p.InverseParent).HasConstraintName("FK_Roles_Roles");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
