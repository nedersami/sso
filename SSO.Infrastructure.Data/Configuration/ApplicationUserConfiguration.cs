using SSO.Infrastructure.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace SSO.Infrastructure.Data.Configuration
{
	public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
	{
		private string ID { get; set; }

		public ApplicationUserConfiguration(string id)
		{
			ID = id;
		}

		public void Configure(EntityTypeBuilder<ApplicationUser> builder)
		{
			var userTimestamp = "5a1bd3b8-6d09-4344-9415-70c6227fa3a1";
			var passwordHash = "AQAAAAEAACcQAAAAEBhoVAoaMXFn6gswQzivFUiIJuHdfpbzhnmwtweANQFtrNUEtedYPX3IzUyCQWiTHA=="; //"!qaz2WSX" hashed

			var adminUser = new ApplicationUser { Id = ID, ConcurrencyStamp = userTimestamp, UserName = "admin@fornax.com.br", NormalizedUserName = "ADMIN", Email = "admin@fornax.com.br", NormalizedEmail = "ADMIN@FORNAX.COM.BR", Name = "Administrador", EmailConfirmed = true };
			adminUser.PasswordHash = passwordHash;

			builder.HasData(adminUser);
		}
	}
}
