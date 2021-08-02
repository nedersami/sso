using SSO.Infrastructure.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSO.Infrastructure.Data.Configuration
{
	public class IdentityUserRoleConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
	{
		private string UserId { get; set; }
		private string RoleId { get; set; }

		public IdentityUserRoleConfiguration(string userId, string roleId)
		{
			UserId = userId;
			RoleId = roleId;
		}

		public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
		{
			builder.HasData(new IdentityUserRole<string> { UserId = UserId, RoleId = RoleId });
		}
	}
}
