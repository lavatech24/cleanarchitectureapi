using CleanArchitecture.Domain.Models.SystemModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Configurations
{
    public class ApiUserConfiguration : IEntityTypeConfiguration<ApiUser>
    {
        public void Configure(EntityTypeBuilder<ApiUser> builder)
        {
            //builder.HasData(builder
            //    .OwnsOne(u => u.Client, client =>
            //    {
            //        client.Property(c => c.ClientId).IsRequired().HasMaxLength(100);
            //        client.Property(c => c.ClientSecret).IsRequired().HasMaxLength(100);
            //    }));
        }
    }
}
