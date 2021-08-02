using Microsoft.AspNetCore.Identity;

namespace SSO.Infrastructure.Data.Domain
{
	public class ApplicationUser : IdentityUser
	{
		public string Name { get; set; }

		public int? TenantId { get; set; }
	}
}
