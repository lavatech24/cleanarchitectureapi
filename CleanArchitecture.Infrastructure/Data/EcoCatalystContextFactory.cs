using CleanArchitecture.Application.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Data
{
    public static class CleanArchitectureContextFactory
    {
        public static void AddSqlServerDatabase(this IServiceCollection services, IConfiguration config)
        {
            // Register the DbContext with the connection string from configuration
            services.AddDbContext<CleanArchitectureContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection"), 
                    providerOptions => providerOptions.EnableRetryOnFailure())
                );
        }
        public async static void Initialize(CleanArchitectureContext context, IConfiguration config)
        {
            context.Database.EnsureCreated();
            var superUser = context.ApiUsers.FirstOrDefault(u => u.CompanyId == null && u.IsSuperuser);
            var adminRole = context.Roles.FirstOrDefault(r => r.RoleCode.ToLower() == "sysadmin");

            //Lava: If superUser doesn't exist create initial super account details to generate clients
            if (superUser == null)
            {
                var authService = new AuthService(null, null, config);

                var sUser = await authService.CreateUserObj(adminRole?.Id ?? 0, null, true);
                //Serilog.Log.Warning("created superuser - " + sUser.ClientId + "/" + sUser.ClientSecret);
                context.ApiUsers.Add(sUser);
                context.SaveChanges();
            }
        }
    }
}
