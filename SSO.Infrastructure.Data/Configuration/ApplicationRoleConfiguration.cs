using SSO.Infrastructure.Data.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SSO.Infrastructure.Data.Configuration
{
	public class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
	{
		private string ID { get; set; }
		private string TenantID { get; set; }

		public ApplicationRoleConfiguration(string id, string tenantRoleId)
		{
			ID = id;
			TenantID = tenantRoleId;
		}

		public void Configure(EntityTypeBuilder<ApplicationRole> builder)
		{
			var roleTimestamp = "62851b80-41b8-4da5-829c-656cfb0d336d";
			builder.HasData(new ApplicationRole { Id = ID, ConcurrencyStamp = roleTimestamp, Name = "Administrador", NormalizedName = "ADMINISTRADOR", IsTenantRole = false });
		}
	}
}
