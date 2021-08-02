using Microsoft.AspNetCore.Identity;

namespace SSO.Infrastructure.Data.Domain
{
    public partial class ApplicationRole : IdentityRole
    {
        public bool IsTenantRole { get; set; }
    }
}