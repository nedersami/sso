using SSO.Infrastructure.Data;
using SSO.Infrastructure.Data.Domain;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace SSO.UI.Configurations
{
	public static class MvcConfiguration
	{
		public static void AddMvcSecurity(
			this IServiceCollection services,
			IConfiguration configuration,
			Action<IdentityOptions> identityOptions)
		{
			services.AddIdentity<ApplicationUser, IdentityRole>(identityOptions)
				.AddEntityFrameworkStores<SSODbContext>()
				.AddDefaultTokenProviders();

			services
				.Configure<CookiePolicyOptions>(options =>
				{
					options.CheckConsentNeeded = context => true;
					options.MinimumSameSitePolicy = SameSiteMode.None;
				})
				.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = new PathString("/Account/Login");
					options.LogoutPath = new PathString("/Account/Logout");
					options.ReturnUrlParameter = "returnUrl";
				});
		}
	}
}
