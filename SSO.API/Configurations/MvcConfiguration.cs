using SSO.Infrastructure.Data;
using SSO.Infrastructure.Data.Domain;
using SSO.Infrastructure.Business.Model;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSO.API.Configurations
{
	public static class MvcConfiguration
	{
		public static void AddMvcSecurity(
			this IServiceCollection services,
			IConfiguration configuration)
		{
			services.AddIdentity<ApplicationUser, IdentityRole>(options =>
			{
				options.Password.RequireDigit = true;
				options.Password.RequireLowercase = true;
				options.Password.RequireNonAlphanumeric = true;
				options.Password.RequireUppercase = true;
				options.Password.RequiredLength = 8;
				options.Password.RequiredUniqueChars = 1;

				options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
				options.Lockout.MaxFailedAccessAttempts = 5;
				options.Lockout.AllowedForNewUsers = true;

				options.User.AllowedUserNameCharacters =
				"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedEmail = true;
			})
				.AddEntityFrameworkStores<SSODbContext>()
				.AddDefaultTokenProviders();

			var settingConfiguration = new SettingConfigurationModel();
			new ConfigureFromConfigurationOptions<SettingConfigurationModel>(
				configuration.GetSection("SettingConfiguration"))
					.Configure(settingConfiguration);

			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			}).
			AddJwtBearer(bearerOptions =>
			{
				bearerOptions.RequireHttpsMetadata = false;
				bearerOptions.SaveToken = true;
				bearerOptions.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(settingConfiguration.SecretKey)),
					ValidateIssuer = false,
					ValidateAudience = false
				};
			});

			services.AddAuthorization(auth =>
			{
				auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
					.AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
					.RequireAuthenticatedUser().Build());
			});

			services.AddMvc();
		}
	}
}
