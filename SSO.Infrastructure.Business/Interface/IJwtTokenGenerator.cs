using SSO.Infrastructure.Data.Domain;

namespace SSO.Infrastructure.Business.Interface
{
	public interface IJwtTokenGenerator
	{
		string GenerateAccessToken(ApplicationUser user, ApplicationRole role);
	}
}
