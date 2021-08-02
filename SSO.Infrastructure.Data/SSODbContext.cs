using SSO.Infrastructure.Data.Configuration;
using SSO.Infrastructure.Data.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

namespace SSO.Infrastructure.Data
{
	public partial class SSODbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{
		public SSODbContext(DbContextOptions<SSODbContext> options)
			: base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			var roleId = "71d62b68-89ab-4e2b-b8f2-e5b4bb0f9577";
			var tenantRoleId = "7439c92b-3759-4195-8481-4f6e5df61930";
			builder.ApplyConfiguration(new ApplicationRoleConfiguration(roleId, tenantRoleId));

			var userId = "347c6db3-fd25-4375-9b68-035bb2aa2cf3";
			builder.ApplyConfiguration(new ApplicationUserConfiguration(userId));

			builder.ApplyConfiguration(new IdentityUserRoleConfiguration(userId, roleId));
		}
	}
}
