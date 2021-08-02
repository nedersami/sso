using SSO.Infrastructure.Data.Domain;
using System;

namespace SSO.Infrastructure.Business.Model
{
	[Serializable]
	public class IdentityUserModel : ApplicationUser 
	{
		public ApplicationRole Role { get; set; }
		public string Token { get; set; }
	}
}
